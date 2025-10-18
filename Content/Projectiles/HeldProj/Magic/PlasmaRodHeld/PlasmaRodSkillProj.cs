using CalamityMod;
using CalamityMod.Graphics.Primitives;
using CalamityMod.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Content.Particiles;
using UCA.Content.Paths;
using UCA.Content.Projectiles.Magic.Ray;
using UCA.Content.Projectiles.Misc;
using UCA.Core.AnimationHandle;
using UCA.Core.Enums;
using UCA.Core.SpecificEffectManagers;
using UCA.Core.Utilities;

namespace UCA.Content.Projectiles.HeldProj.Magic.PlasmaRodHeld
{
    public class PlasmaRodSkillProj : ModProjectile, ILocalizedModType, IPixelatedPrimitiveRenderer
    {
        public PixelationPrimitiveLayer layer = PixelationPrimitiveLayer.AfterProjectiles;
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<PlasmaRod>();
        public override string Texture => $"{ItemOverridePaths.MagicWeaponsPath}" + "PlasmaRodOverride";

        public AnimationHelper animationHelper = new AnimationHelper(3);

        public int OwnerDir = 0;
        public Player Owner => Main.player[Projectile.owner];

        public float BeginRot = 0;

        public int SwordLength = 180;

        public float Opacity = 1f;

        public bool CanHit = false;
        public int Filp => (int)Projectile.ai[0];
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
            Projectile.netImportant = true;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (target.friendly)
                return false;
            else if (CanHit)
                return true;

            return false;
        }
        #region 注册数据
        public override void OnSpawn(IEntitySource source)
        {
        }
        #endregion
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(animationHelper.MaxAniProgress[AnimationState.Begin]);
            writer.Write(animationHelper.MaxAniProgress[AnimationState.End]);
            writer.Write(BeginRot);
            writer.Write(CanHit);
            writer.Write(SwordLength);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            animationHelper.MaxAniProgress[AnimationState.Begin] = reader.ReadInt32();
            animationHelper.MaxAniProgress[AnimationState.End] = reader.ReadInt32();
            BeginRot = reader.ReadSingle();
            CanHit = reader.ReadBoolean();
            SwordLength = reader.ReadInt32();
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            // Otherwise, perform an AABB line collision check to check the whole beam.
            float _ = float.NaN;
            bool c = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * SwordLength * Projectile.scale, 12f, ref _);
            return c;
        }

        public override void AI()
        {
            Projectile.netUpdate = true;
            if (Projectile.UCA().FirstFrame)
            {
                animationHelper.MaxAniProgress[AnimationState.Begin] = 45;
                animationHelper.MaxAniProgress[AnimationState.End] = 10;

                OwnerDir = Owner.LocalMouseWorld().X > Owner.Center.X ? 1 : -1;

                BeginRot = Owner.GetPlayerToMouseVector2().ToRotation();
            }
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;

            Owner.ChangeDir(OwnerDir);
            Owner.heldProj = Projectile.whoAmI;
            Projectile.Center = Owner.Center;

            // 基础信息
            Projectile.velocity = Projectile.rotation.ToRotationVector2();
            Projectile.timeLeft = 2;
            
            AllAI();
            Projectile.netSpam = 0;
            Projectile.netUpdate = true;
        }
        public void AllAI()
        {
            float baseRotation = Projectile.velocity.ToRotation();
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, baseRotation - MathHelper.PiOver2);

            // 处理动画
            if (!animationHelper.HasFinish[AnimationState.Begin])
            {
                Projectile.extraUpdates = 1;

                animationHelper.AniProgress[AnimationState.Begin]++;

                HandleBeginAni();

                if (animationHelper.AniProgress[AnimationState.Begin] == animationHelper.MaxAniProgress[AnimationState.Begin])
                    animationHelper.HasFinish[AnimationState.Begin] = true;
            }
            else if (!animationHelper.HasFinish[AnimationState.End])
            {
                animationHelper.AniProgress[AnimationState.End]++;

                Opacity = MathHelper.Lerp(0f, 1f, animationHelper.AniProgress[AnimationState.End] / (float)animationHelper.MaxAniProgress[AnimationState.End]);

                if (animationHelper.AniProgress[AnimationState.End] == animationHelper.MaxAniProgress[AnimationState.End])
                    animationHelper.HasFinish[AnimationState.End] = true;
            }
            else
            {
                Projectile.Kill();
            }
        }

        public void HandleBeginAni()
        {
            int MaxAni = animationHelper.MaxAniProgress[AnimationState.Begin];
            int CurAni = animationHelper.AniProgress[AnimationState.Begin];
            Opacity = MathHelper.Lerp(1f, 0f, CurAni / 25f);
            // 播放音效与开始判定
            if (CurAni == 25)
                SoundEngine.PlaySound(SoundsMenu.CarnageRightUse, Projectile.Center);
            if (CurAni > 10)
            {
                BeginRot = BeginRot.AngleTowards(Owner.GetPlayerToMouseVector2().ToRotation(), 0.05f);
                CanHit = true;
            }
            // 使用缓动函数让动画更自然
            float easedProgress = EasingHelper.EaseInBack(CurAni / (float)MaxAni);
            // 设置起始与结束角度
            float startAngleOffset = -MathHelper.ToRadians(125) * Filp;
            float endAngleOffset = MathHelper.ToRadians(125) * Filp;
            // 计算基础旋转角度
            float baseRotation = MathHelper.Lerp(startAngleOffset, endAngleOffset, easedProgress);
            // 根据玩家方向进行镜像处理
            if (Owner.direction == -1)// 水平镜像
                baseRotation = baseRotation * Owner.direction;
            // 确定椭圆的点
            Vector2 TargetPos = Vector2.UnitX.BetterRotatedBy(baseRotation, default, 1, 0.65f);
            // 让射弹中心指向对应的点
            // 这样会让运动曲线出现一些问题，不过速度快就问题不大
            // 最终加上使用时的角度
            Projectile.rotation = TargetPos.ToRotation() + BeginRot;
            // 根据长度来计算缩放
            float BaseLength = SwordLength;
            float curProjCenterToPointLength = TargetPos.Length() * BaseLength;
            Projectile.scale = curProjCenterToPointLength / BaseLength;
            // 生成粒子
            for (int i = 2; i < 6; i++)
            {
                Vector2 BeginPos = Vector2.Lerp(Projectile.Center, Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.rotation) * Main.rand.Next(60, 160) * Projectile.scale, i / 4f);
                new Fire(BeginPos, Projectile.velocity.RotatedBy(MathHelper.PiOver2), Color.Purple, 64, Main.rand.NextFloat(MathHelper.TwoPi), 1f, 0.2f).Spawn();
                new Fire(BeginPos, Projectile.velocity.RotatedBy(MathHelper.PiOver2), Color.Violet, 64, Main.rand.NextFloat(MathHelper.TwoPi), 1f, 0.1f).Spawn();
            }
        }

        public void RenderPixelatedPrimitives(SpriteBatch spriteBatch, PixelationPrimitiveLayer layer)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullCounterClockwise, null);

            Texture2D texture = UCATextureRegister.BladeAura.Value;

            Vector2 drawPosition = Projectile.Center - Main.screenPosition;

            float drawRotation = Projectile.rotation + (Owner.direction == -1 ? MathHelper.Pi : 0f);

            SpriteEffects flipSprite = Owner.direction * Main.player[Projectile.owner].gravDir == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Main.spriteBatch.Draw(UCATextureRegister.BladeAura.Value, drawPosition / 2, null, Color.Violet * (1 - Opacity), drawRotation + MathHelper.PiOver2 * Owner.direction,
                new Vector2(texture.Size().X / 2, texture.Size().Y + 200), Projectile.scale * Main.player[Projectile.owner].gravDir * 0.07f, flipSprite, default);

            Main.graphics.GraphicsDevice.Textures[1] = UCATextureRegister.Noise.Value;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;

            UCAUtilities.FastApplyEdgeMeltsShader(Opacity, ModContent.Request<Texture2D>(Texture).Size(), Color.Violet, 0.01f, 0);

            Main.spriteBatch.Draw(UCATextureRegister.BladeAura.Value, drawPosition / 2, null, Color.Violet, drawRotation + MathHelper.PiOver2 * Owner.direction,
               new Vector2(texture.Size().X / 2, texture.Size().Y + 200), Projectile.scale * Main.player[Projectile.owner].gravDir * 0.07f, flipSprite, default);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            // 绘制位置，在这里进行偏移，碰撞箱使用自定义碰撞箱
            Vector2 drawPosition = Projectile.Center - Main.screenPosition + Vector2.UnitX.RotatedBy(Projectile.rotation) * 0;

            float drawRotation = Projectile.rotation + (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0f) + MathHelper.PiOver2;

            Vector2 rotationPoint = new(0, texture.Size().Y);

            SpriteEffects flipSprite = Projectile.spriteDirection * Main.player[Projectile.owner].gravDir == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            // spriteBatch会自动把textures0设置为当前使用的材质，所以需要你手动改一下
            Main.spriteBatch.Draw(texture, drawPosition, null, Color.White, drawRotation - MathHelper.PiOver4, rotationPoint, Projectile.scale * Main.player[Projectile.owner].gravDir, flipSprite, 0f);
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<UseForOnHitNPCProj>(), 0, 0, Projectile.owner, Type);

            target.AddBuff(BuffID.ShadowFlame, 300);

            SoundEngine.PlaySound(SoundsMenu.PlasmaRodSwingHit, Projectile.Center);

            if (Projectile.UCA().OnceHitEffect)
                ScreenShakeSystem.AddScreenShakes(Projectile.Center, 4, 5, Projectile.rotation + MathHelper.PiOver2, 0.2f, true, 1000);
        }

        public override void OnKill(int timeLeft)
        {
            Main.mouseRight = false;
            Owner.itemTime = 0;
            Owner.itemAnimation = 0;
        }
    }
}
