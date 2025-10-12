using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Content.Items.Weapons.Magic.Ray;
using UCA.Content.MetaBalls;
using UCA.Content.Particiles;
using UCA.Content.Paths;
using UCA.Content.Projectiles.Magic.Ray;
using UCA.Content.UCACooldowns;
using UCA.Core.AnimationHandle;
using UCA.Core.Enums;
using UCA.Core.SpecificEffectManagers;
using UCA.Core.Utilities;

namespace UCA.Content.Projectiles.HeldProj.Magic
{
    public class CarnageRaySkillProj : ModProjectile, ILocalizedModType
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<CarnageRay>();
        public Player Owner => Main.player[Projectile.owner];
        public override string Texture => $"{ProjPath.HeldProjPath}" + "Magic/CarnageRayHeldProj";

        public float Opacity = 1f; // 1f是完全透明

        public AnimationHelper animationHelper;

        public float BeginRot;

        public int LengthOffset = 25;

        public int HitBoxLength = 0;

        public bool Canhit = false;
        public int RotFilp = 1;
        public override void SetDefaults()
        {
            Projectile.width = 5;
            Projectile.height = 5;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 45;
        }
        public int Dir = 0;
        public override void OnSpawn(IEntitySource source)
        {
            animationHelper = new AnimationHelper(10);

            animationHelper.MaxAniProgress[(int)AnimationState.Begin] = 100;
            animationHelper.MaxAniProgress[(int)AnimationState.Middle] = 40;
            animationHelper.MaxAniProgress[(int)AnimationState.End] = 40;

            Dir = Owner.LocalMouseWorld().X > Owner.Center.X ? 1 : -1;

            BeginRot = Owner.GetPlayerToMouseVector2().ToRotation();

            Owner.AddCooldown(CarnageBoost.ID, 600);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (!Canhit)
                return null;
            // Otherwise, perform an AABB line collision check to check the whole beam.
            float _ = float.NaN;
            bool c = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * HitBoxLength * 0.95f, 24f, ref _);
            return c;
        }

        public override void AI()
        {
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;

            Owner.ChangeDir(Dir);
            Owner.heldProj = Projectile.whoAmI;
            Projectile.Center = Owner.Center;

            // 基础信息
            Projectile.velocity = Projectile.rotation.ToRotationVector2();
            Projectile.timeLeft = 2;

            if (Projectile.owner == Main.myPlayer)
                AllAI();

            Projectile.netUpdate = true;

            if (!animationHelper.HasFinish[(int)AnimationState.Middle])
            {
                int Lenth = LengthOffset + 50;
                int Spawn = 9;
                float Scale = 0.2f;

                // 进行一定的补正，因为这里最初面向的没有椭圆挥砍，所以先这样打补丁了
                float BA = BeginRot + MathHelper.PiOver2;
                float Progress = BA / MathHelper.Pi;
                float AngleOffset = MathHelper.Lerp(MathHelper.PiOver2, -MathHelper.PiOver2, Progress);

                Vector2 SpawnLength = Vector2.UnitX.BetterRotatedBy(Projectile.rotation + AngleOffset, default, 2, 1) * Lenth * Main.rand.NextFloat(0.9f, 1.1f);
                Vector2 BaseOffset = Vector2.UnitX.BetterRotatedBy(Projectile.rotation + AngleOffset, default, 2, 1) * 50;

                SpawnLength = SpawnLength.RotatedBy(BeginRot);
                BaseOffset = BaseOffset.RotatedBy(BeginRot);

                HitBoxLength = (int)(SpawnLength.Length() + BaseOffset.Length());
                for (int i = 1; i < Spawn; i++)
                {
                    Vector2 FinalLength = SpawnLength * ((float)i / Spawn);

                    CarnageMetaBall.SpawnParticle(Projectile.Center + FinalLength + BaseOffset,
                        Projectile.velocity.RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloat(0f, 1.2f) * -6f * Owner.direction, Scale, Projectile.rotation + MathHelper.PiOver2, true);

                    Scale *= 0.97f;
                }
                for (int i = 1; i < 6; i++)
                {
                    Vector2 FinalLength = SpawnLength * ((float)i / 6);

                    new LilyLiquid(Projectile.Center + FinalLength + BaseOffset,
                        Projectile.velocity.RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloat(0f, 1.2f) * -6f * Owner.direction, Color.Red, 64, 0, 1, 1.5f).Spawn();

                    new LilyLiquid(Projectile.Center + FinalLength + BaseOffset,
                        Projectile.velocity.RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloat(0f, 1.2f) * -6f * Owner.direction, Color.Black, 64, 0, 1, 1.5f).Spawn();
                }
            }
        }
        public void AllAI()
        {
            float baseRotation = Projectile.velocity.ToRotation();
            float directionVerticality = MathF.Abs(Projectile.velocity.X);
            float offset = MathHelper.Pi;

            if (Owner.direction == -1)
                offset = 0;

            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, baseRotation + Owner.direction * directionVerticality * 1.5f + offset);
            Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, baseRotation + Owner.direction * directionVerticality * 1.2f + offset);

            if (!animationHelper.HasFinish[(int)AnimationState.Begin])
            {
                Dir = Owner.LocalMouseWorld().X > Owner.Center.X ? 1 : -1;

                BeginRot = Utils.AngleTowards(BeginRot, Owner.GetPlayerToMouseVector2().ToRotation(), 0.05f);

                if (animationHelper.AniProgress[(int)AnimationState.Begin] == 1)
                    SoundEngine.PlaySound(SoundsMenu.CarnageCharge, Projectile.Center);

                Projectile.extraUpdates = 2;

                animationHelper.AniProgress[(int)AnimationState.Begin]++;

                HandleBeginAni();

                if (animationHelper.AniProgress[(int)AnimationState.Begin] == animationHelper.MaxAniProgress[(int)AnimationState.Begin])
                {
                    RotFilp = -1;
                    animationHelper.HasFinish[(int)AnimationState.Begin] = true;
                }
            }
            else if (!animationHelper.HasFinish[(int)AnimationState.Middle])
            {
                if (animationHelper.AniProgress[(int)AnimationState.Middle] == 1)
                {
                    Projectile.UCA().OnceHitEffect = true;
                    SoundEngine.PlaySound(SoundsMenu.CarnageSwingBeign, Projectile.Center);
                }

                Projectile.extraUpdates = 10;
                animationHelper.AniProgress[(int)AnimationState.Middle]++;

                HandleMiddleAni();

                // 提前几帧在速度未消失前进入下一个动画
                if (animationHelper.AniProgress[(int)AnimationState.Middle] == animationHelper.MaxAniProgress[(int)AnimationState.Middle])
                {
                    animationHelper.HasFinish[(int)AnimationState.Middle] = true;
                }
            }
            else if (!animationHelper.HasFinish[(int)AnimationState.End])
            {
                Projectile.extraUpdates = 0;

                animationHelper.AniProgress[(int)AnimationState.End]++;

                HandleEndAni();

                if (animationHelper.AniProgress[(int)AnimationState.End] == animationHelper.MaxAniProgress[(int)AnimationState.End] - 10)
                    animationHelper.HasFinish[(int)AnimationState.End] = true;
            }
            else
            {
                Projectile.Kill();
            }
        }
        #region 处理准备动画
        public void HandleBeginAni()
        {
            int MaxAni = animationHelper.MaxAniProgress[(int)AnimationState.Begin];
            int CurAni = animationHelper.AniProgress[(int)AnimationState.Begin];

            if (CurAni > 1)
                Canhit = true;

            Opacity = MathHelper.Lerp(0.8f, 0, EasingHelper.EaseOutCubic(CurAni / (float)MaxAni));

            float BeginAngle = BeginRot + MathHelper.Pi;
            float EndAngle = BeginRot - MathHelper.Pi * 0.8f;

            if (Owner.direction == 1)
                Projectile.rotation = MathHelper.Lerp(BeginAngle, EndAngle, EasingHelper.EaseOutCubic(CurAni / (float)MaxAni));
            else
            {
                // 特判的角度处理
                BeginAngle -= MathHelper.PiOver4;
                EndAngle -= MathHelper.PiOver4;
                Projectile.rotation = MathHelper.Lerp(EndAngle, BeginAngle, EasingHelper.EaseOutCubic(CurAni / (float)MaxAni));
            }

            LengthOffset = (int)MathHelper.Lerp(LengthOffset, 45, EasingHelper.EaseInCubic(CurAni / (float)MaxAni));
            
            if (CurAni % 10 == 0)
            {
                RandomSpawnProj();
            }
            
        }
        #endregion
        #region 处理中间动画
        float angularVelocity; // 旋转角速度
        public void HandleMiddleAni()
        {
            int MaxAni = animationHelper.MaxAniProgress[(int)AnimationState.Middle];
            int CurAni = animationHelper.AniProgress[(int)AnimationState.Middle];

            if (Owner.direction == 1)
            {
                float BeginAngle = BeginRot + MathHelper.Pi * 0.9f;
                float EndAngle = BeginRot - MathHelper.Pi * 0.8f;
                Projectile.rotation = MathHelper.Lerp(EndAngle, BeginAngle, EasingHelper.EaseInCubic(CurAni / (float)MaxAni));
            }
            else
            {
                float BeginAngle = BeginRot + MathHelper.Pi;
                float EndAngle = BeginRot - MathHelper.Pi * 0.7f;
                // 特判的角度处理
                BeginAngle -= MathHelper.PiOver4;
                EndAngle -= MathHelper.PiOver4;
                Projectile.rotation = MathHelper.Lerp(BeginAngle, EndAngle, EasingHelper.EaseInCubic(CurAni / (float)MaxAni));
            }

            if (CurAni == MaxAni - 5)
            {
                angularVelocity = 0.025f;
            }

            LengthOffset = (int)MathHelper.Lerp(15, 55, EasingHelper.EaseInCubic(CurAni / (float)MaxAni));

            if (CurAni % 5 == 0)
            {
                SpawnProj();
            }
        }

        public void SpawnProj()
        {
            SoundEngine.PlaySound(SoundsMenu.CarnageBallSpawn, Projectile.Center);
            int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.rotation) * Main.rand.Next(LengthOffset, HitBoxLength),
                Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.PiOver2 * Owner.direction) * 12f, ModContent.ProjectileType<CarnageBall>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner, 1);
            Main.projectile[p].tileCollide = false;
        }

        public void RandomSpawnProj()
        {
            SoundEngine.PlaySound(SoundsMenu.CarnageBallSpawn, Projectile.Center);
            int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.rotation) * Main.rand.Next(LengthOffset, HitBoxLength),
                Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedByRandom(MathHelper.TwoPi) * 4f, ModContent.ProjectileType<CarnageBall>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner, 1);
            Main.projectile[p].tileCollide = false;
        }
        #endregion
        #region 处理结束动画
        public void HandleEndAni()
        {
            int MaxAni = animationHelper.MaxAniProgress[(int)AnimationState.End];
            int CurAni = animationHelper.AniProgress[(int)AnimationState.End];

            Projectile.rotation += angularVelocity * (float)Math.Sin(MathHelper.TwoPi * (CurAni / (float)MaxAni)) * 0.25f * -Owner.direction;

            Opacity = MathHelper.Lerp(0, 1, CurAni / (float)MaxAni);
        }
        #endregion
        public override void OnKill(int timeLeft)
        {
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.graphics.GraphicsDevice.Textures[0] = ModContent.Request<Texture2D>(Texture).Value;
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

            Main.graphics.GraphicsDevice.Textures[1] = UCATextureRegister.Noise.Value;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;

            UCAUtilities.FastApplyEdgeMeltsShader(Opacity, ModContent.Request<Texture2D>(Texture).Size(), Color.Red, 0.01f, 0);
            // 绘制位置，在这里进行偏移，碰撞箱使用自定义碰撞箱
            Vector2 drawPosition = Projectile.Center - Main.screenPosition + Vector2.UnitX.RotatedBy(Projectile.rotation) * LengthOffset;

            float drawRotation = Projectile.rotation + (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0f) + MathHelper.PiOver2;

            Vector2 rotationPoint = ModContent.Request<Texture2D>(Texture).Value.Size() / 2f;

            SpriteEffects flipSprite = Projectile.spriteDirection * Main.player[Projectile.owner].gravDir == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            // spriteBatch会自动把textures0设置为当前使用的材质，所以需要你手动改一下
            Main.spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, drawPosition, null, Color.White, drawRotation - MathHelper.PiOver4, rotationPoint, Projectile.scale * Main.player[Projectile.owner].gravDir, flipSprite, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage *= 5f;

            modifiers.SetCrit();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Vector2 ToMouseVector = target.Center - Owner.Center;
            ToMouseVector = ToMouseVector.SafeNormalize(Vector2.UnitX);
            for (int i = 0; i < 10; i++)
            {
                Vector2 shootVel = ToMouseVector.RotatedByRandom(MathHelper.PiOver4 * 0.7f) * Main.rand.NextFloat(0.2f, 1.2f) * 24f;

                if (shootVel.ToRotation() > 0)
                    shootVel.Y *= 0.15f;

                Color color = Main.rand.NextBool(3) ? Color.Black : Color.DarkRed;
                new BloodDrop(target.Center, shootVel, color, Main.rand.Next(60, 90), 0, 1, 0.1f).Spawn();
            }

            for (int i = 0; i < 10; i++)
            {
                Vector2 SpawnVector = ToMouseVector.RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(0f, 1.2f) * 36f;
                CarnageMetaBall.SpawnParticle(target.Center,
                    SpawnVector, 
                    1.5f, SpawnVector.ToRotation());

            }

            if (Projectile.UCA().OnceHitEffect)
                ScreenShakeSystem.AddScreenShakes(Projectile.Center, 35 * RotFilp * - Owner.direction, 15, Projectile.rotation + MathHelper.PiOver2, 0.5f, true , 1000);

            SoundEngine.PlaySound(SoundsMenu.CarnageSkillMeleeHit, Projectile.Center);

            Owner.AddCooldown(CarnageBoost.ID, 1200);

            target.AddBuff(ModContent.BuffType<BurningBlood>(), 600);
        }
    }
}
