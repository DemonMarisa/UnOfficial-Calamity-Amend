using LAP.Assets.Sounds;
using LAP.Assets.TextureRegister;
using LAP.Content.Particles;
using LAP.Core.AnimationHandle;
using LAP.Core.Enums;
using LAP.Core.Graphics.DeepGlow;
using LAP.Core.Graphics.VFX;
using LAP.Core.Presets.Content;
using LAP.Core.SpecificEffectManagers;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Content.Items.Weapons.Magic.Ray;
using UCA.Content.VFXs;

namespace UCA.Content.Projectiles.HeldProj.Magic.VividClarityHeld
{
    public class VividClarityGreatSword : ModProjectile
    {
        public override string Texture => GetInstance<VividClarityAlt>().Texture;
        public Player Owner => Main.player[Projectile.owner];
        public VFXInstance VividGreadSword;
        public VFXInstance Trail;
        public ref float VGSScale => ref VividGreadSword.AiFloat[0];
        public ref float VGSDrawScale => ref VividGreadSword.AiFloat[1];
        public float GFXYOffset;
        public int GFXYOffsetProgress;
        public AniHelper AniHelper = new AniHelper(10);
        public float ToMouseRotation;
        public bool CanUpdateToMouseRotation;
        public int SwordLength = 900;
        public bool BeginFadeOut;
        public int FadeOutTime;
        public bool CanSlash1;
        public bool CanSlash2;
        public bool CanSlash3;
        public bool BeginHit;
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (!BeginHit)
                return false;
            float _ = float.NaN;
            bool c = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Owner.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * SwordLength * Projectile.scale * 0.9f, 128f, ref _);
            return c;
        }
        public override void AI()
        {
            Init();
            UpdateRotation();
            UpdateGeneral();
            UpdateAni();
        }
        public void Init()
        {
            if (Projectile.LAP().FirstFrame)
            {
                SoundEngine.PlaySound(LAPSoundsMenu.MagicCharge_ER with { Volume = 1f, Pitch = Main.rand.NextFloat(-0.2f, 0.2f), MaxInstances = -1 }, Projectile.Center);
                SoundEngine.PlaySound(LAPSoundsMenu.TerraMagicaRelease with { Volume = 0.2f, Pitch = Main.rand.NextFloat(-0.2f, 0.2f), MaxInstances = -1 }, Projectile.Center);
                Trail = null;
                ToMouseRotation = Projectile.Center.AngleTo(Owner.LocalMouseWorld());
                VividGreadSword = VividGreatSword.Spawn(Projectile.whoAmI);
                AniHelper.MaxAniProgress[0] = 25;
                AniHelper.MaxAniProgress[1] = 15;
                AniHelper.MaxAniProgress[2] = 200;
                AniHelper.MaxAniProgress[3] = 25;
                AniHelper.MaxAniProgress[4] = 200;
                AniHelper.MaxAniProgress[5] = 20;
                AniHelper.MaxAniProgress[6] = 200;
                AniHelper.MaxAniProgress[7] = 20;
                Projectile.rotation = ToMouseRotation;
                Projectile.Center = Owner.MountedCenter;
                CanUpdateToMouseRotation = true;
            }
        }
        public void UpdateGeneral()
        {
            Projectile.SetHeldProj(Owner, false, CanUpdateToMouseRotation);
            Owner.SetArmRot(LAPUtilities.GetVector2(Owner.Center, Projectile.Center).ToRotation());
            // 蓄力后坐力
            float progress = GFXYOffsetProgress / (float)MiscAniNum.Frame25;
            GFXYOffset = MathHelper.Lerp(0f, -25f, MathF.Pow(MathF.Sin(progress * MathHelper.Pi), 3f));
            if (GFXYOffsetProgress > 0)
                GFXYOffsetProgress--;
            if (VividGreadSword.OldPos.Count > 80)
                VividGreadSword.OldPos.RemoveAt(0);
            if (VividGreadSword.OldRot.Count > 80)
                VividGreadSword.OldRot.RemoveAt(0);
        }
        public void UpdateRotation()
        {
            if (CanUpdateToMouseRotation)
                ToMouseRotation = Utils.AngleLerp(ToMouseRotation, Owner.Center.AngleTo(Owner.LocalMouseWorld()), 0.2f);
        }
        public void UpdateAni()
        {
            if (BeginFadeOut)
            {
                Projectile.extraUpdates = 0;
                DirectFadeOut();
            }
            // 蓄力
            if (!AniHelper.HasFinish[0])
            {
                AniHelper.UpDateAni(0, 240);
                HandleBeginAni();
                if (AniHelper.BreakTime[0] > 180 && !CanSlash1)
                    BeginFadeOut = true;
                else if (Owner.LAP().MouseLeft && !BeginFadeOut)
                    CanSlash1 = true;
                if (CanSlash1 && AniHelper.BreakTime[0] > 95)
                    AniHelper.HasFinish[0] = true;
            } // 准备第一次挥砍
            else if (!AniHelper.HasFinish[1] && CanSlash1)
            {
                AniHelper.UpDateAni(1, 0);
                UpdateReadySwing();
            } // 第一次挥砍
            else if (!AniHelper.HasFinish[2] && CanSlash1)
            {
                AniHelper.UpDateAni(2);
                BeginFirstSwing();
                if (Owner.LAP().MouseLeft)
                    CanSlash2 = true;
            } // 准备第二次
            else if (!AniHelper.HasFinish[3] && CanSlash1)
            {
                AniHelper.UpDateAni(3, 60);
                ReadySecondSwing();

                if (Owner.LAP().MouseLeft && !BeginFadeOut)
                    CanSlash2 = true;

                if (AniHelper.BreakTime[3] > 10 && !CanSlash2)
                    BeginFadeOut = true;

                if (CanSlash2 && AniHelper.BreakTime[3] > 15)
                    AniHelper.HasFinish[3] = true;
            }
            else if (!AniHelper.HasFinish[4] && CanSlash1 && CanSlash2)
            { // 第二次
                AniHelper.UpDateAni(4);
                BeginSecondSwing();
                if (Owner.LAP().MouseLeft)
                    CanSlash3 = true;
            }
            else if (!AniHelper.HasFinish[5] && CanSlash1 && CanSlash2)
            {
                AniHelper.UpDateAni(5, 90);
                ReadyThirdSwing();

                if (Owner.LAP().MouseLeft && !BeginFadeOut)
                    CanSlash3 = true;

                if (AniHelper.BreakTime[5] > 45 && !CanSlash3)
                    BeginFadeOut = true;

                if (CanSlash3 && AniHelper.BreakTime[5] > 15)
                    AniHelper.HasFinish[5] = true;
            }
            else if (!AniHelper.HasFinish[6] && CanSlash1 && CanSlash2 && CanSlash3)
            {
                AniHelper.UpDateAni(6, 0);
                BeginThirdSwing();
            }
            else if (!AniHelper.HasFinish[7] && CanSlash1 && CanSlash2 && CanSlash3)
            {
                AniHelper.UpDateAni(7, 45);
                FadeOut();
            }
            else if (!BeginFadeOut)
                Projectile.Kill();
        }
        public void HandleBeginAni()
        {
            float progress = AniHelper.GetProgress(0);
            float easedProgress = EasingHelper.EaseOutCubic(progress);
            float UpdateRot = AniHelper.UpDateAngle(90, -145, Owner.direction, easedProgress);
            Vector2 EndPos = new Vector2(-300, -100);
            Vector2 EndAdd = new Vector2(100, -30);
            Vector2 BeginAdd = new Vector2(0, -30);
            Vector2 PositionAdd = BezierEaseHelper.BezierCurve(Vector2.Zero, Vector2.Zero + BeginAdd, EndPos + EndAdd, EndPos, easedProgress);
            PositionAdd.Y *= Owner.direction;
            if (AniHelper.BreakTime[0] < 90)
                Projectile.Opacity = MathHelper.Lerp(0f, 1f, progress);
            Projectile.Center = Vector2.Lerp(Projectile.Center, Owner.MountedCenter + PositionAdd.RotatedBy(ToMouseRotation), 0.4f);
            Projectile.rotation = UpdateRot + ToMouseRotation;
            Projectile.spriteDirection = Owner.direction;
            if (AniHelper.BreakTime[0] == 10)
            {
                SoundEngine.PlaySound(LAPSoundsMenu.CarianGreatswordCharage with { Volume = 1f, Pitch = Main.rand.NextFloat(-0.2f, 0.2f), MaxInstances = -1 }, Projectile.Center);
                VGSScale = 0.3f;
                VividGreadSword.AiBool[0] = true;
                GFXYOffsetProgress = MiscAniNum.Frame10;
            }
            else if (AniHelper.BreakTime[0] == 50)
            {
                SoundEngine.PlaySound(LAPSoundsMenu.CarianGreatswordCharage with { Volume = 1f, Pitch = Main.rand.NextFloat(-0.2f, 0.2f), MaxInstances = -1 }, Projectile.Center);
                VGSScale = 0.6f;
                VividGreadSword.AiBool[0] = true;
                GFXYOffsetProgress = MiscAniNum.Frame10;
            }
            else if (AniHelper.BreakTime[0] == 90)
            {
                SoundEngine.PlaySound(LAPSoundsMenu.CarianGreatswordCharage with { Volume = 1f, Pitch = Main.rand.NextFloat(-0.2f, 0.2f), MaxInstances = -1 }, Projectile.Center);
                VGSScale = 1f;
                VividGreadSword.AiBool[0] = true;
                VividGreadSword.AiBool[1] = true;
                GFXYOffsetProgress = MiscAniNum.Frame10;
            }
        }
        public void UpdateReadySwing()
        {
            // 准备时无法调整角度
            CanUpdateToMouseRotation = false;

            float progress = AniHelper.GetProgress(1);
            float easedProgress = EasingHelper.EaseOutCubic(progress);
            Vector2 VectorAdd = new Vector2(300, 0);
            // 进行旋转
            float baseRotation = AniHelper.UpDateAngle(-160, -170, Owner.direction, easedProgress);
            // 确定椭圆的点
            Vector2 TargetPos = VectorAdd.BetterRotatedBy(baseRotation, Vector2.Zero, 1, 1f);
            // 移动向指定地点
            Projectile.Center = Vector2.Lerp(Projectile.Center, Owner.MountedCenter + TargetPos.RotatedBy(ToMouseRotation), easedProgress);
            Projectile.rotation = Utils.AngleLerp(Projectile.rotation, TargetPos.ToRotation() + ToMouseRotation, 0.16f);

            BeginHit = true;
        }
        public void BeginFirstSwing()
        {
            Projectile.extraUpdates = 10;
            float progress = AniHelper.GetProgress(2);
            float easedProgress = BezierEaseHelper.BezierSmooth(new Vector2(0.9f, 0), new Vector2(1f, 1f),progress);
            Vector2 VectorAdd = new Vector2(300, 0);
            // 进行旋转
            float baseRotation = AniHelper.UpDateAngle(-170, 145, Owner.direction, easedProgress);
            // 确定椭圆的点
            Vector2 TargetPos = VectorAdd.BetterRotatedBy(baseRotation, Vector2.Zero, 1, 1f);
            // 移动向指定地点
            Projectile.Center = Owner.MountedCenter + TargetPos.RotatedBy(ToMouseRotation);
            Projectile.rotation = TargetPos.ToRotation() + ToMouseRotation;
            if (AniHelper.BreakTime[2] > 0)
            {
                Projectile.extraUpdates = 0;
                Projectile.numUpdates = -1;

                BeginHit = false;
            }
            Trail ??= SlashTrail.Spawn(Color.White, 25, 80, 200);
            Trail.OldPos.Add(Projectile.Center + Projectile.GetOwnerStepFromEu() + TargetPos.RotatedBy(ToMouseRotation) + Projectile.rotation.ToRotationVector2() * 120);
            Trail.OldRot.Add(Projectile.rotation + MathHelper.PiOver2);
            Trail.Oldfloat.Add(300);

            SpawnDust(progress, TargetPos, false, false);
            if (AniHelper.AniProgress[2] == 1)
            {
                SoundEngine.PlaySound(LAPSoundsMenu.CarianGreatswordUse with { Volume = 1f, Pitch = Main.rand.NextFloat(-0.2f, 0.2f), MaxInstances = -1 }, Projectile.Center);
                Projectile.LAP().OnceHitEffect = true;
            }
        }
        public void ReadySecondSwing()
        {
            // 开始矫正角度
            CanUpdateToMouseRotation = true;
            float easedProgress = EasingHelper.EaseOutCubic(AniHelper.GetProgress(3));

            float baseRotation = MathHelper.ToRadians(145 * Owner.direction);
            Vector2 VectorAdd = new Vector2(360, 0);
            // 确定椭圆的点
            Vector2 TargetPos = VectorAdd.BetterRotatedBy(baseRotation, Vector2.Zero, 1f, 0.4f);

            Projectile.Center = Vector2.Lerp(Projectile.Center, Owner.MountedCenter + TargetPos.RotatedBy(ToMouseRotation), easedProgress);
            Projectile.rotation = Utils.AngleLerp(Projectile.rotation, TargetPos.ToRotation() + ToMouseRotation, 0.16f);

            // 为下一次椭圆挥砍做缩放IK
            float ToTargetPointLength = new Vector2(350 * 2.5f, 0).BetterRotatedBy(MathHelper.ToRadians(145 * Owner.direction), Vector2.Zero, 1.3f, 0.6f).Length();
            Projectile.scale = MathHelper.Lerp(Projectile.scale, ToTargetPointLength / 970f, 0.15f);
            VGSDrawScale = Projectile.scale;

            if (AniHelper.BreakTime[3] > 5)
            {
                CanUpdateToMouseRotation = false;
                Trail = null;

                BeginHit = true;
            }
        }
        public void BeginSecondSwing()
        {
            CanUpdateToMouseRotation = false;

            Projectile.extraUpdates = 8;

            float progress = AniHelper.GetProgress(4);
            float easedProgress = BezierEaseHelper.BezierSmooth(new Vector2(0.9f, 0), new Vector2(1f, 1f), progress);
            float baseRotation = AniHelper.UpDateAngle(145, 535, Owner.direction, easedProgress);
            Vector2 VectorAdd = new Vector2(360, 0);
            // 确定椭圆的点
            Vector2 TargetPos = VectorAdd.BetterRotatedBy(baseRotation, Vector2.Zero, 1f, 0.4f);

            Projectile.Center = Owner.MountedCenter + TargetPos.RotatedBy(ToMouseRotation);
            Projectile.rotation = TargetPos.ToRotation() + ToMouseRotation;

            float ToTargetPointLength = new Vector2(360 * 2.5f, 0).BetterRotatedBy(baseRotation, Vector2.Zero, 1.3f, 0.6f).Length();
            Projectile.scale = ToTargetPointLength / 980f;
            VGSDrawScale = Projectile.scale;

            if (AniHelper.BreakTime[4] > 0)
            {
                Projectile.extraUpdates = 0;
                Projectile.numUpdates = -1;

                BeginHit = false;
            }

            Trail ??= SlashTrail.Spawn(Color.White, 25, 120, 200);
            Trail.OldPos.Add(Projectile.Center + Projectile.GetOwnerStepFromEu() + TargetPos.RotatedBy(ToMouseRotation) * 1.5f);
            Trail.OldRot.Add(Projectile.rotation + MathHelper.PiOver2);
            Trail.Oldfloat.Add(ToTargetPointLength * 0.3f);

            SpawnDust(progress, TargetPos, true, false);

            if (AniHelper.AniProgress[4] == 1)
            {
                SoundEngine.PlaySound(LAPSoundsMenu.CarianGreatswordUse with { Volume = 1f, Pitch = Main.rand.NextFloat(-0.2f, 0.2f), MaxInstances = -1 }, Projectile.Center);
                Projectile.LAP().OnceHitEffect = true;
            }
        }
        public void ReadyThirdSwing()
        {
            float easedProgress = EasingHelper.EaseOutCubic(AniHelper.GetProgress(5));
            // 开始矫正角度
            CanUpdateToMouseRotation = true; 
            float baseRotation = MathHelper.ToRadians(190 * Owner.direction);
            Vector2 VectorAdd = new Vector2(300, 0);
            // 确定椭圆的点
            Vector2 TargetPos = VectorAdd.BetterRotatedBy(baseRotation, Vector2.Zero, 1f, 0.6f);
            Projectile.scale = MathHelper.Lerp(Projectile.scale, 1f, 0.15f);
            VGSDrawScale = Projectile.scale;
            if (AniHelper.BreakTime[5] > 5)
            {
                CanUpdateToMouseRotation = false;
                // IK平滑
                Vector2 NextVectorAdd = new Vector2(258, 0);
                // 确定椭圆的点
                Vector2 NextTargetPos = NextVectorAdd.BetterRotatedBy(MathHelper.ToRadians(190 * Owner.direction));
                Projectile.Center = Vector2.Lerp(Projectile.Center, Owner.MountedCenter + NextTargetPos.RotatedBy(ToMouseRotation), easedProgress);
                Projectile.rotation = Utils.AngleLerp(Projectile.rotation, NextTargetPos.ToRotation() + ToMouseRotation, 0.16f);

                BeginHit = true;
            }
            else
            {
                // IK平滑
                Vector2 NextVectorAdd = new Vector2(258, 0);
                // 确定椭圆的点
                Vector2 NextTargetPos = NextVectorAdd.BetterRotatedBy(MathHelper.ToRadians(190 * Owner.direction));
                Projectile.Center = Vector2.Lerp(Projectile.Center, Owner.MountedCenter + NextTargetPos.RotatedBy(ToMouseRotation), easedProgress);
                Projectile.rotation = Utils.AngleLerp(Projectile.rotation, NextTargetPos.ToRotation() + ToMouseRotation, 0.16f);
            }
            Trail = null;
        }
        public void BeginThirdSwing()
        {
            Projectile.extraUpdates = 8;

            float progress = AniHelper.GetProgress(6);
            float easedProgress = BezierEaseHelper.BezierSmooth(new Vector2(0.9f, 0), new Vector2(1f, 1f), progress);
            float baseRotation = AniHelper.UpDateAngle(190, -115, Owner.direction, easedProgress);
            Vector2 VectorAdd = new Vector2(258, 0);
            // 确定椭圆的点
            Vector2 TargetPos = VectorAdd.BetterRotatedBy(baseRotation);

            Projectile.Center = Owner.MountedCenter + TargetPos.RotatedBy(ToMouseRotation);
            Projectile.rotation = TargetPos.ToRotation() + ToMouseRotation;

            if (AniHelper.BreakTime[6] > 0)
            {
                Projectile.extraUpdates = 0;
                Projectile.numUpdates = -1;

                BeginHit = false;
            }
            Trail ??= SlashTrail.Spawn(Color.White, 25, 80, 200);
            Trail.OldPos.Add(Projectile.Center + Projectile.GetOwnerStepFromEu() + TargetPos.RotatedBy(ToMouseRotation) + Projectile.rotation.ToRotationVector2() * 180);
            Trail.OldRot.Add(Projectile.rotation + MathHelper.PiOver2);
            Trail.Oldfloat.Add(300);

            SpawnDust(progress, TargetPos, false, true);

            if (AniHelper.AniProgress[6] == 1)
            {
                SoundEngine.PlaySound(LAPSoundsMenu.CarianGreatswordUse with { Volume = 1f, Pitch = Main.rand.NextFloat(-0.2f, 0.2f), MaxInstances = -1 }, Projectile.Center);
                Projectile.LAP().OnceHitEffect = true;
            }
        }
        public void SpawnDust(float progress, Vector2 TargetPos, bool Oval, bool filpVel)
        {
            int SpawnCount = (int)MathF.Ceiling(2 * progress);
            for (int i = 0; i < SpawnCount; i++)
            {
                Vector2 Pos = Vector2.Lerp(Projectile.Center, Projectile.Center + Projectile.GetOwnerStepFromEu() + TargetPos.RotatedBy(ToMouseRotation) + Projectile.rotation.ToRotationVector2() * 240, Main.rand.NextFloat(0.5f, 1.4f));
               if (Oval)
                    Pos = Vector2.Lerp(Projectile.Center, Projectile.Center + Projectile.GetOwnerStepFromEu() + TargetPos.RotatedBy(ToMouseRotation) * 1.5f, Main.rand.NextFloat(0.5f, 1.4f));
                new CampSmoke(Pos, Owner.velocity * Main.rand.NextFloat(0f, 1.5f), Color.White, 90, Main.rand.NextFloat(MathHelper.TwoPi), 0.6f, Main.rand.NextFloat(0.4f, 0.6f)).Spawn();
                ParticlePreset.NewTMGlowBall(Pos, Vector2.Zero, Color.White, 60, 0.2f, 6f);
                ParticlePreset.NewTOFL(Pos, Vector2.Zero, Color.White, 60, 0.2f, 6f);
            }
            Vector2 SpawnPos = Vector2.Lerp(Projectile.Center, Projectile.Center + Projectile.GetOwnerStepFromEu() + TargetPos.RotatedBy(ToMouseRotation) + Projectile.rotation.ToRotationVector2() * 240, Main.rand.NextFloat(0.5f, 1.4f));
            if (Oval)
                SpawnPos = Vector2.Lerp(Projectile.Center, Projectile.Center + Projectile.GetOwnerStepFromEu() + TargetPos.RotatedBy(ToMouseRotation) * 1.5f, Main.rand.NextFloat(0.5f, 1.4f));
            Vector2 firVel = Vector2.UnitX.RotatedBy(Projectile.rotation + MathHelper.PiOver2) * 18 * Owner.direction;
            Color DrawColor = Color.White;
            new TrailGlowBall(SpawnPos, firVel * Main.rand.NextFloat(0f, 1f) * (filpVel ? -1 : 1), DrawColor * 0.5f, Main.rand.Next(60, 90), 0.3f, true).Spawn();
            if (Main.rand.NextBool(4 - SpawnCount))
            {
                ParticlePreset.NewDustGlow(SpawnPos, firVel * 0.3f * (filpVel ? -1 : 1), 0, Color.White, 45, 0.4f, 0);
            }
        }
        public void FadeOut()
        {
            CanUpdateToMouseRotation = false;

            float progress = AniHelper.GetProgress(7);
            float easedProgress = BezierEaseHelper.BezierSmooth(new Vector2(0f, 0), new Vector2(0f, 1f), progress);
            float baseRotation = AniHelper.UpDateAngle(-115, -125, Owner.direction, easedProgress);
            Vector2 VectorAdd = new Vector2(258, 0);
            // 确定椭圆的点
            Vector2 TargetPos = VectorAdd.BetterRotatedBy(baseRotation);
            // IK平滑
            Projectile.Center = Vector2.Lerp(Projectile.Center, Owner.MountedCenter + TargetPos.RotatedBy(ToMouseRotation), progress);
            Projectile.rotation = Utils.AngleLerp(Projectile.rotation, TargetPos.ToRotation() + ToMouseRotation, 0.16f);

            BeginFadeOut = true;
        }
        public void DirectFadeOut()
        {
            FadeOutTime++;
            Projectile.Opacity = MathHelper.Lerp(1f, 0f, EasingHelper.EaseInCubic(FadeOutTime / 45f));
            VividGreadSword.Opacity = Projectile.Opacity;
            VividGreadSword.AiFloat[2] = Projectile.Opacity;
            if (FadeOutTime > 45)
                Projectile.Kill();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.LAP().OnceHitEffect)
            {
                ScreenShakeSystem.AddScreenShake_Sin(Projectile.Center, 30, 90, MathHelper.PiOver2);
                ScreenShakeSystem.AddScreenShakes(Projectile.Center, 25, 15, 0);
                Projectile.LAP().OnceHitEffect = false;
            }
        }
        public override void OnKill(int timeLeft)
        {
            Owner.SetItemAnimation(0);
            Owner.SetItemTime(0);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            LAPUtilities.ReSetToBeginShader(BlendState.Additive, SamplerState.PointClamp);

            Vector4 cut = new Vector4(0.5f, 1f, 0f, 1f);
            LAPUtilities.ApplyUVRot(cut, 1f);
            DrawControlCircle(0.35f, 20);
            LAPUtilities.ApplyUVRot(cut, -1f);
            DrawControlCircle(0.25f, -15);

            LAPUtilities.ReSetToBeginShader(BlendState.NonPremultiplied);

            Projectile.GetProjDrawInfo_Staff(out Texture2D texture, out Vector2 drawPosition, out float drawRotation, out Vector2 rotationPoint, out SpriteEffects flipSprite);
            LAPUtilities.FastApplyEdgeMeltsShader(1 - Projectile.Opacity, texture.Size(), Color.White);
            LAPUtilities.SetTexture(LAPTextureRegister.Noise.Value,SamplerState.PointClamp, 1);

            Main.spriteBatch.Draw(texture, drawPosition + new Vector2(GFXYOffset, 0).RotatedBy(Projectile.rotation), null, lightColor, drawRotation, rotationPoint, Projectile.scale, flipSprite, 0);

            LAPUtilities.ReSetToBeginShader(BlendState.Additive, SamplerState.PointClamp);

            Vector4 Frontcut = new Vector4(0f, 0.5f, 0f, 1f);
            LAPUtilities.ApplyUVRot(Frontcut, 1f);
            DrawControlCircle(0.35f, 20);
            LAPUtilities.ApplyUVRot(Frontcut, -1f);
            DrawControlCircle(0.25f, -15);

            LAPUtilities.ReSetToEndShader();

            DeepGlow.SubmitCustomGlow(() =>
            {

                LAPUtilities.ReSetToBeginShader(BlendState.Additive, SamplerState.PointClamp);

                Vector4 cut = new Vector4(0f, 1f, 0f, 1f);
                LAPUtilities.ApplyUVRot(cut, 1f);
                DrawControlCircle(0.35f, 20);
                LAPUtilities.ApplyUVRot(cut, -1f);
                DrawControlCircle(0.25f, -15);

                Projectile.GetProjDrawInfo_Staff(out Texture2D texture, out Vector2 drawPosition, out float drawRotation, out Vector2 rotationPoint, out SpriteEffects flipSprite);
                LAPUtilities.FastApplyEdgeMeltsShader(1 - Projectile.Opacity, texture.Size(), Color.White);
                LAPUtilities.SetTexture(LAPTextureRegister.Noise.Value, SamplerState.PointClamp, 1);

                Main.spriteBatch.Draw(texture, drawPosition + new Vector2(GFXYOffset, 0).RotatedBy(Projectile.rotation), null, Color.Transparent, drawRotation, rotationPoint, Projectile.scale, flipSprite, 0);

                LAPUtilities.ReSetToEndShader(BlendState.Additive);
            });
            return false;
        }
        public void DrawControlCircle(float scale = 1f, float YOffset = 24f)
        {
            Vector2 offset = new Vector2(YOffset, 0).RotatedBy(Projectile.rotation) + new Vector2(0, GFXYOffset).RotatedBy(Projectile.rotation - MathHelper.PiOver2);
            Texture2D circle = UCATextureRegister.TechCircle.Value;
            LAPUtilities.Draw(circle, Projectile.Center - Main.screenPosition + offset, null, Color.White * Projectile.Opacity, Projectile.rotation, circle.Size() / 2, new Vector2(0.25f, 1f) * scale, 0);
        }
    }
}
