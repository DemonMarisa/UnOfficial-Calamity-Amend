using LAP.Assets.Effects;
using LAP.Assets.Sounds;
using LAP.Assets.TextureRegister;
using LAP.Common.Blance;
using LAP.Content.Configs;
using LAP.Content.Particles;
using LAP.Core.AnimationHandle;
using LAP.Core.Enums;
using LAP.Core.Graphics.PixelatedRender;
using LAP.Core.Graphics.Primitives.Trail;
using LAP.Core.Keybind;
using LAP.Core.LAPSource;
using LAP.Core.SpecificEffectManagers;
using LAP.Core.SystemsLoader;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.Localization;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Content.Items.Weapons.Melee.GreatSword;
using UCA.Content.Projectiles.Melee.NormalProj;

namespace UCA.Content.Projectiles.HeldProj.Melee.StormRuler
{
    public class StormRulerHeld_KingofStorm : ModProjectile, ILocalizedModType, IPixelatedRenderer
    {
        public DrawLayer LayerToRenderTo => DrawLayer.BeforeDusts;
        public BlendState BlendState => BlendState.Additive;
        public override LocalizedText DisplayName => LAPUtilities.GetItemName<StormRulerAlt>();
        public override string Texture => UCATextureRegister.StormRulerAlt.Path;
        public Player Owner => Projectile.Owner();
        public Vector2 IdleOffset => new Vector2(12, 4 * Owner.direction);
        public ref float FatherHeldPosX => ref Projectile.ai[0];
        public ref float FatherHeldPosY => ref Projectile.ai[1];
        public ref float FatherTargetRot => ref Projectile.ai[2];
        public float FatherProjRotOffset => MathHelper.ToRadians(225f);
        public int UseTime => Owner.ApplyWeaponAttackSpeed(Owner.HeldItem, Owner.HeldItem.useTime * 25, 250);
        public int HitCooldown;
        public int BeginDir;
        public AnimationHelper animationHelper = new AnimationHelper(5);
        public List<Vector2> OldAimPos = [];
        public List<Vector2> OldAimPos2 = [];
        public Vector2 BeginMouseWorld;
        public Vector2 HeldPos;
        public float TargetRot;
        public float ProjRotOffset;
        public float SlashOpacity = 1f;
        public float SlashOpacity2 = 1f;
        public float Heigh = 1f;
        public int SwordLength = 360;
        public bool HasFocus;
        public override void SetStaticDefaults()
        {
            Projectile.AddHeldProj();
        }
        public override void SetDefaults()
        {
            Projectile.width = 5;
            Projectile.height = 5;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.noEnchantmentVisuals = true;
            Projectile.netImportant = true;
            Projectile.extraUpdates = 10;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
                return true;
            if (Projectile.LAP().FirstFrame)
                return false;
            float _ = float.NaN;
            Vector2 beamBeginPos = Owner.Center;
            Vector2 beamEndPos = Projectile.Center + OldAimPos[^1];
            if (animationHelper.HasFinish[AnimationState.Middle] && OldAimPos2.Count > 1)
                beamEndPos = Projectile.Center + OldAimPos2[^1];
            bool c = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), beamBeginPos, beamEndPos, 64f, ref _);
            return c;
        }
        public override void AI()
        {
            Owner.SetUseFocus(2);
            Init();
            UpdateGeneral();
            SetArmRot();
            UpdateAnimation();
        }
        public void Init()
        {
            if (!Projectile.LAP().FirstFrame)
                return;
            HasFocus = Owner.CheckFocus(Owner.ActiveItem().LAP().WeaponSkillRealFocusCost * 2);
            SoundEngine.PlaySound(LAPSoundsMenu.SwingAttack with { Volume = 1f, Pitch = Main.rand.NextFloat(-0.4f, -0.2f), MaxInstances = -1 }, Projectile.Center);
            SoundEngine.PlaySound(LAPSoundsMenu.SPSwing with { Volume = 1f, Pitch = Main.rand.NextFloat(-0.4f, -0.2f), MaxInstances = -1 }, Projectile.Center);
            SoundEngine.PlaySound(LAPSoundsMenu.StormRulerAttack with { Volume = 1f, Pitch = Main.rand.NextFloat(-0.2f, 0.2f), MaxInstances = -1 }, Projectile.Center);
            BeginMouseWorld = Owner.LocalMouseWorld();
            BeginDir = Owner.LocalMouseWorld().X > Owner.Center.X ? 1 : -1;
            TargetRot = FatherTargetRot;
            Projectile.rotation = TargetRot;
            HeldPos = new Vector2(FatherHeldPosX, FatherHeldPosY);
            ProjRotOffset = FatherProjRotOffset;
            animationHelper.MaxAniProgress[AnimationState.Begin] = (int)(UseTime * 0.1f);
            animationHelper.MaxAniProgress[AnimationState.Middle] = (int)(UseTime * 0.9f);
            animationHelper.MaxAniProgress[AnimationState.End] = (int)(UseTime * 0.1f);
            animationHelper.MaxAniProgress[3] = (int)(UseTime * 0.9f);
        }
        #region 常规更新
        public void UpdateGeneral()
        {
            Projectile.SetHeldProj(Owner);
            Owner.ChangeDir(BeginDir);
            Projectile.Center = Owner.GetArmRoot() + HeldPos;
            Projectile.timeLeft = 2;
            Projectile.direction = Owner.direction;
            Projectile.spriteDirection = Owner.direction;
            if (HitCooldown > 0)
                HitCooldown--;
            Projectile.velocity = Projectile.rotation.ToRotationVector2();
        }
        public void SetArmRot()
        {
            Vector2 target = Projectile.Center;
            float rot = LAPUtilities.GetVector2(Owner.GetArmRoot(), target).ToRotation();
            Owner.SetArmRot(rot);
        }
        #endregion
        #region 更新动画
        public void UpdateAnimation()
        {
            if (!animationHelper.HasFinish[AnimationState.Begin])
            {
                animationHelper.UpDateAni(AnimationState.Begin);
                HandleBeginAni();
            }
            else if (!animationHelper.HasFinish[AnimationState.Middle])
            {
                animationHelper.UpDateAni(AnimationState.Middle);
                HandleMiddleAni();
            }
            else if (!animationHelper.HasFinish[AnimationState.End])
            {
                animationHelper.UpDateAni(AnimationState.End);
                HandleEndAni();
            }
            else if (!animationHelper.HasFinish[3])
            {
                animationHelper.UpDateAni(3);
                HandleFinalAni();
            }
            else
            {
                Projectile.Kill();
            }
        }
        public void HandleBeginAni()
        {
            float easedProgress = EasingHelper.EaseInCubic(animationHelper.GetProgress(AnimationState.Begin));
            float baseRotation = animationHelper.UpDateAngle(-115, 150, Owner.direction, easedProgress);
            HeldPos = IdleOffset.RotatedBy(baseRotation).RotatedBy(TargetRot);
            float ProjRotation = animationHelper.UpDateAngle(-155, 145, Owner.direction, easedProgress);
            ProjRotOffset = ProjRotation + TargetRot;

            float easedProgress2 = animationHelper.GetProgress(AnimationState.Begin);
            float baseSlashRotation = animationHelper.UpDateAngle(-145, 150, Owner.direction, easedProgress2);
            Matrix Slashtransform = Matrix.CreateRotationZ(baseSlashRotation) * Matrix.CreateScale(1.2f, Heigh, 1f);
            Vector2 SlashTargetPos = Vector2.Transform(Vector2.UnitX, Slashtransform) * 1.25f;
            Vector2 TargetPos = SlashTargetPos.RotatedBy(TargetRot) * SwordLength;
            OldAimPos.Add(TargetPos);

            Vector2 beginSpawnPos = Owner.Center;
            Vector2 EndSpawnPos = Owner.Center + TargetPos * 1.1f;
            Vector2 SpawnPos = Vector2.Lerp(beginSpawnPos, EndSpawnPos, Main.rand.NextFloat());
            Vector2 firVel = Vector2.UnitX.RotatedBy(baseSlashRotation + TargetRot + MathHelper.PiOver2) * 9 * Owner.direction;
            Color DrawColor = Color.White;
            new TrailGlowBall(SpawnPos, firVel, DrawColor * 0.5f, Main.rand.Next(60, 90), 0.3f, true).Spawn();

            for (int i = 0; i < 2; i++)
            {
                Vector2 Pos = Vector2.Lerp(Projectile.Center, Projectile.Center + TargetPos, Main.rand.NextFloat(0.5f, 1.2f));
                new CampSmoke(Pos, Owner.velocity * Main.rand.NextFloat(0f, 1.5f), Color.White, 90, Main.rand.NextFloat(MathHelper.TwoPi), 0.6f, Main.rand.NextFloat(0.4f, 0.6f)).Spawn();
            }
            Vector2 Pos2 = Vector2.Lerp(Projectile.Center, Projectile.Center + TargetPos, Main.rand.NextFloat(0.35f, 1.2f));
            new SmallGlowBall(Pos2, Vector2.Zero, Color.White, Main.rand.Next(90, 180), 0.1f, 3f).Spawn();
        }
        public void HandleMiddleAni()
        {
            float easedProgress = EasingHelper.EaseOutCubic(animationHelper.GetProgress(AnimationState.Middle));
            float baseRotation = animationHelper.UpDateAngle(150, 170, Owner.direction, easedProgress);
            HeldPos = IdleOffset.RotatedBy(baseRotation).RotatedBy(TargetRot);
            float ProjRotation = animationHelper.UpDateAngle(145, 160, Owner.direction, easedProgress);
            ProjRotOffset = ProjRotation + TargetRot;
            if (easedProgress < 0.2f)
            {
                float easedProgress2 = EasingHelper.EaseOutCubic(animationHelper.GetProgress(AnimationState.Middle));
                float baseSlashRotation = animationHelper.UpDateAngle(150, 155, Owner.direction, easedProgress2);
                Matrix Slashtransform = Matrix.CreateRotationZ(baseSlashRotation) * Matrix.CreateScale(1.2f, Heigh, 1f);
                Vector2 SlashTargetPos = Vector2.Transform(Vector2.UnitX, Slashtransform) * 1.25f;
                if (Projectile.FinalExtraUpdate())
                    OldAimPos.Add(SlashTargetPos.RotatedBy(TargetRot) * SwordLength);
                Projectile.scale = SlashTargetPos.Length();
            }
            if (easedProgress == 1f)
                Projectile.ResetLocalNPCHitImmunity();

            BeginDir = Owner.LocalMouseWorld().X > Owner.Center.X ? 1 : -1;
            TargetRot = TargetRot.AngleTowards(Owner.GetPlayerToMouseVector2().ToRotation(), 0.01f);
            SlashOpacity = 1f - animationHelper.GetProgress(AnimationState.Middle);
        }
        public void HandleEndAni()
        {
            if (animationHelper.AniProgress[2] == 2)
            {
                SoundEngine.PlaySound(LAPSoundsMenu.SwingAttack with { Volume = 1f, Pitch = Main.rand.NextFloat(-0.4f, -0.2f), MaxInstances = -1 }, Projectile.Center);
                SoundEngine.PlaySound(LAPSoundsMenu.SPSwing with { Volume = 1f, Pitch = Main.rand.NextFloat(-0.4f, -0.2f), MaxInstances = -1 }, Projectile.Center);
                SoundEngine.PlaySound(LAPSoundsMenu.StormRulerAttack with { Volume = 1f, Pitch = Main.rand.NextFloat(-0.2f, 0.2f), MaxInstances = -1 }, Projectile.Center);
            }
            float easedProgress = EasingHelper.EaseOutCubic(animationHelper.GetProgress(AnimationState.End));
            float baseRotation = animationHelper.UpDateAngle(170, 480, Owner.direction, easedProgress);
            HeldPos = IdleOffset.RotatedBy(baseRotation).RotatedBy(TargetRot);
            float ProjRotation = animationHelper.UpDateAngle(160, 510, Owner.direction, easedProgress);
            ProjRotOffset = ProjRotation + TargetRot;

            float easedProgress2 = animationHelper.GetProgress(AnimationState.End);
            float baseSlashRotation = animationHelper.UpDateAngle(-145, 150, Owner.direction, easedProgress2);
            Matrix Slashtransform = Matrix.CreateRotationZ(baseSlashRotation) * Matrix.CreateScale(1.2f, Heigh * 0.5f, 1f);
            Vector2 SlashTargetPos = Vector2.Transform(Vector2.UnitX, Slashtransform) * 3.5f;
            Vector2 TargetPos = SlashTargetPos.RotatedBy(TargetRot) * SwordLength;
            OldAimPos2.Add(TargetPos);

            Vector2 beginSpawnPos = Owner.Center;
            Vector2 EndSpawnPos = Owner.Center + TargetPos * 0.65f;
            Vector2 SpawnPos = Vector2.Lerp(beginSpawnPos, EndSpawnPos, Main.rand.NextFloat());
            Vector2 firVel = Vector2.UnitX.RotatedBy(baseSlashRotation + TargetRot + MathHelper.PiOver2) * 9 *  Owner.direction;
            Color DrawColor = Color.White;
            new TrailGlowBall(SpawnPos, firVel, DrawColor * 0.5f, Main.rand.Next(60, 90), 0.3f, true).Spawn();

            for (int i = 0; i < 3; i++)
            {
                Vector2 Pos = Vector2.Lerp(Projectile.Center, Projectile.Center + TargetPos * 0.55f, Main.rand.NextFloat(0.5f, 1f));
                new CampSmoke(Pos, Owner.velocity * Main.rand.NextFloat(0f, 1.5f), Color.White, 90, Main.rand.NextFloat(MathHelper.TwoPi), 0.6f, Main.rand.NextFloat(0.4f, 0.6f)).Spawn();
            }
            Vector2 Pos2 = Vector2.Lerp(Projectile.Center, Projectile.Center + TargetPos * 0.55f, Main.rand.NextFloat(0.35f, 1f));
            new SmallGlowBall(Pos2, Vector2.Zero, Color.White, Main.rand.Next(90, 180), 0.1f, 3f).Spawn();
        }
        public void HandleFinalAni()
        {
            float easedProgress = EasingHelper.EaseOutCubic(animationHelper.GetProgress(3));
            float baseRotation = animationHelper.UpDateAngle(480, 500, Owner.direction, easedProgress);
            HeldPos = IdleOffset.RotatedBy(baseRotation).RotatedBy(TargetRot);
            float ProjRotation = animationHelper.UpDateAngle(510, 520, Owner.direction, easedProgress);
            ProjRotOffset = ProjRotation + TargetRot;

            BeginDir = Owner.LocalMouseWorld().X > Owner.Center.X ? 1 : -1;
            TargetRot = TargetRot.AngleTowards(Owner.GetPlayerToMouseVector2().ToRotation(), 0.01f);
            SlashOpacity2 = 1f - animationHelper.GetProgress(3);
        }
        #endregion        
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (HasFocus)
                modifiers.SourceDamage *= UCABlanceRule.StormRulerKingofStormDamageMult;
            else
                modifiers.SourceDamage *= UCABlanceRule.StormRulerKingofStormNoFocusDamageMult;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (HitCooldown == 0)
            {
                Projectile p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ProjectileType<StormBlast>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                p.LAP().isWeaponSkillProj = true;
                HitCooldown = 5;
            }
            ScreenShakeSystem.AddScreenShakes(Projectile.Center, -25 * -Owner.direction, 45, MathHelper.TwoPi, 0.5f, true, 1000);
        }
        public override void OnKill(int timeLeft)
        {
            if (LAPKeybind.WeaponSkillHotKey.Current)
            {
                Item item = ItemLoader.GetItem(ItemType<StormRulerAlt>()).Item;
                EntitySource_ItemUse_WeaponSkill source = new(Owner, item);
                if (!Owner.HasProj<StormRulerHeldSkillProj>())
                {
                    Projectile p = Projectile.NewProjectileDirect(source, Projectile.Center, Projectile.velocity, ProjectileType<StormRulerHeldSkillProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    p.LAP().isWeaponSkillProj = Projectile.LAP().isWeaponSkillProj;
                } 
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            PixelatedRenderManger.BeginDrawProj = true;
            DrawBlade(lightColor);
            return false;
        }
        public void DrawBlade(Color lightColor)
        {
            Texture2D texture = UCATextureRegister.StormRulerAlt.Value;
            Projectile.GetProjDrawInfo_Melee(texture, out Vector2 _, out float drawRotation, out Vector2 rotationPoint, out SpriteEffects flipSprite);
            Vector2 drawPosition = Projectile.Center - Main.screenPosition + Vector2.UnitX.RotatedBy(Projectile.rotation + ProjRotOffset) * -32 * Projectile.direction;
            drawRotation += Projectile.direction == -1 ? MathHelper.Pi : 0;
            Main.spriteBatch.Draw(texture, drawPosition, null, lightColor, drawRotation + ProjRotOffset, rotationPoint, Projectile.scale * 1.25f, flipSprite, 0f);
        }
        public void RenderPixelated(SpriteBatch spriteBatch)
        {
            LAPContent.ReSetToBeginShader_Pixel(BlendState.Additive);

            Texture2D texture = LAPTextureRegister.StandardGradient.Value;
            Effect effect = LAPShaderRegister.AlphaFade.Value;
            effect.Parameters["uFadeoutLeftLength"].SetValue(0.1f);
            effect.Parameters["uFadeinRigtLength"].SetValue(0.2f);
            effect.Parameters["UVMult"].SetValue(new Vector2(1f, 1f));
            effect.CurrentTechnique.Passes[0].Apply();
            DrawSlash(texture, Color.White * 0.5f, 0.95f);
            DrawSlash(texture, Color.White * 0.6f, 0.7f);
            DrawSlash(texture, Color.White * 0.4f, 0.3f);
            DrawSlash(texture, Color.White * 0.3f, 0f);
            if (!LAPConfig.Instance.PerformanceMode)
            {
                Texture2D texture2 = UCATextureRegister.Aura_01.Value;
                Effect effect2 = LAPShaderRegister.AlphaFade_ACut_OColor.Value;
                effect2.Parameters["uFadeoutLeftLength"].SetValue(0.1f);
                effect2.Parameters["uFadeinRigtLength"].SetValue(0.2f);
                effect2.Parameters["UVOffset"].SetValue(new Vector2(Main.GlobalTimeWrappedHourly * -0.5f, 0));
                effect2.Parameters["UVMult"].SetValue(new Vector2(3f, 3f));
                effect2.Parameters["OverlayColor"].SetValue(Color.White.ToVector4());
                effect2.CurrentTechnique.Passes[0].Apply();
                DrawSlash(texture2, Color.White, 0.4f);
                texture2 = UCATextureRegister.Slash.Value;
                DrawSlash(texture2, Color.White, 0.4f);
            }

            effect.Parameters["uFadeoutLeftLength"].SetValue(0.1f);
            effect.Parameters["uFadeinRigtLength"].SetValue(0.2f);
            effect.Parameters["UVMult"].SetValue(new Vector2(1f, 1f));
            effect.CurrentTechnique.Passes[0].Apply();
            DrawSlash2(texture, Color.White * 0.5f, 0.95f);
            DrawSlash2(texture, Color.White * 0.6f, 0.7f);
            DrawSlash2(texture, Color.White * 0.4f, 0.3f);
            DrawSlash2(texture, Color.White * 0.5f, 0f);
            if (!LAPConfig.Instance.PerformanceMode)
            {
                Texture2D texture2 = UCATextureRegister.Aura_01.Value;
                Effect effect2 = LAPShaderRegister.AlphaFade_ACut_OColor.Value;
                effect2.Parameters["uFadeoutLeftLength"].SetValue(0.1f);
                effect2.Parameters["uFadeinRigtLength"].SetValue(0.2f);
                effect2.Parameters["UVOffset"].SetValue(new Vector2(Main.GlobalTimeWrappedHourly * -0.5f, 0));
                effect2.Parameters["UVMult"].SetValue(new Vector2(3f, 3f));
                effect2.Parameters["OverlayColor"].SetValue(Color.White.ToVector4());
                effect2.CurrentTechnique.Passes[0].Apply();
                DrawSlash2(texture2, Color.White, 0.4f);
                texture2 = UCATextureRegister.Slash.Value;
                DrawSlash2(texture2, Color.White, 0.4f);
            }
            LAPContent.ReSetToEndShader_Pixel();
        }
        public void DrawSlash(Texture2D texture, Color drawcolor, float mult = 0.8f)
        {
            if (OldAimPos.Count < 3)
                return;
            List<VertexPositionColorTexture2D> Vertexlist = new List<VertexPositionColorTexture2D>();
            for (int i = 0; i < OldAimPos.Count; i++)
            {
                float progress = (float)i / OldAimPos.Count;
                Vector2 DrawPos_Head = OldAimPos[i] + Owner.Center - Main.screenPosition;
                Vector2 DrawPos_Source = OldAimPos[i] * mult + Owner.Center - Main.screenPosition;
                Vertexlist.Add(new VertexPositionColorTexture2D(DrawPos_Head, drawcolor * SlashOpacity, new Vector3(progress, 0, 0)));
                Vertexlist.Add(new VertexPositionColorTexture2D(DrawPos_Source, drawcolor * SlashOpacity, new Vector3(progress, 1, 0)));
            }
            Main.graphics.GraphicsDevice.Textures[0] = texture;
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, Vertexlist.ToArray(), 0, Vertexlist.Count - 2);
        }
        public void DrawSlash2(Texture2D texture, Color drawcolor, float mult = 0.8f)
        {
            if (OldAimPos2.Count < 3)
                return;
            List<VertexPositionColorTexture2D> Vertexlist = new List<VertexPositionColorTexture2D>();
            for (int i = 0; i < OldAimPos2.Count; i++)
            {
                float progress = (float)i / OldAimPos2.Count;
                Vector2 DrawPos_Head = OldAimPos2[i] * 0.5f + Owner.Center - Main.screenPosition;
                Vector2 DrawPos_Source = OldAimPos2[i] * mult * 0.5f + Owner.Center - Main.screenPosition;
                Vertexlist.Add(new VertexPositionColorTexture2D(DrawPos_Head, drawcolor * SlashOpacity2, new Vector3(progress, 0, 0)));
                Vertexlist.Add(new VertexPositionColorTexture2D(DrawPos_Source, drawcolor * SlashOpacity2, new Vector3(progress, 1, 0)));
            }
            Main.graphics.GraphicsDevice.Textures[0] = texture;
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, Vertexlist.ToArray(), 0, Vertexlist.Count - 2);
        }
    }
}
