using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using LAP.Core.AnimationHandle;
using LAP.Core.Enums;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Assets.Sounds;
using UCA.Content.Items.Weapons.Magic.Ray;
using UCA.Content.Particiles;
using UCA.Content.Paths;
using UCA.Content.Projectiles.HeldProj.Magic.ElementRayHeld;
using UCA.Content.Projectiles.Magic.Ray;
using UCA.Content.UCACooldowns;
using UCA.Core.GlobalInstance.Players;

namespace UCA.Content.Projectiles.HeldProj.Magic.ShadowBoltStaffHeld
{
    public class ShadowBoltStaffSkillHeldProj : ModProjectile, ILocalizedModType
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<ShadowBoltStaffAlt>();
        public override string Texture => $"{ProjPath.HeldProjPath}" + "Magic/ShadowBoltStaffHeld/ShadowBoltStaffHeldProj";
        public Player Owner => Main.player[Projectile.owner];
        public AnimationHelper animationHelper = new AnimationHelper(3);
        public BasePartInfo ShadowOrb;
        public float Opacity = 1f;
        public Vector2 ProjCenterOffset = Vector2.Zero;
        public float BeginRot = 0;
        public override void SetDefaults()
        {
            Projectile.width = 66;
            Projectile.height = 64;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.netImportant = true;
        }
        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }
        public override void AI()
        {
            if (Projectile.LAP().FirstFrame)
            {
                SoundEngine.PlaySound(SoundsMenu.MAGNOLIASPRelease, Projectile.Center);
                SoundEngine.PlaySound(SoundsMenu.MagicStaffCharge, Projectile.Center);
                Texture2D texture2d = UCATextureRegister.ShadowBoltStaffOrb.Value;
                ShadowOrb = new BasePartInfo(texture2d, Vector2.Zero, Vector2.Zero, 0, texture2d.Size() / 2);
                BeginRot = Owner.GetToMouseVector2(Owner.Center).ToRotation();
                animationHelper.MaxAniProgress[AnimationState.Begin] = 30;
                animationHelper.MaxAniProgress[AnimationState.End] = 100;
            }
            if (!Owner.active || Owner.dead)
                Projectile.Kill();
            // 基础信息
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.ChangeDir(Owner.LocalMouseWorld().X > Owner.Center.X ? 1 : -1);
            Owner.heldProj = Projectile.whoAmI;
            BeginRot = LAPUtilities.GetVector2(Owner.Center, Owner.LocalMouseWorld()).ToRotation() + MathHelper.ToRadians(-0 * Owner.direction);
            Projectile.velocity = Projectile.rotation.ToRotationVector2();
            Projectile.Center = Owner.Center + ProjCenterOffset.RotatedBy(BeginRot);

            ProjCenterOffset = new Vector2(-9, -3 * Owner.direction);
            // 设置玩家手持效果
            float baseRotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, baseRotation);
            Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, baseRotation);

            UpdateOrb();
            HandleAni();
        }
        #region 处理动画
        public void HandleAni()
        {
            if (!animationHelper.HasFinish[AnimationState.Begin])
            {
                animationHelper.UpDateAni(AnimationState.Begin, 10);
                HandleBeginAni();
            }
            else if (!animationHelper.HasFinish[AnimationState.End])
            {
                Projectile.extraUpdates = 10;
                animationHelper.UpDateAni(AnimationState.End);
                HandleEndAni();
            }
            else
            {
                Projectile.Kill();
            }
        }
        #region 处理开始动画
        public void HandleBeginAni()
        {
            int MaxAni = animationHelper.MaxAniProgress[AnimationState.Begin];
            int CurAni = animationHelper.AniProgress[AnimationState.Begin];
            // 使用缓动函数让动画更自然
            float easedProgress = EasingHelper.EaseOutCubic(CurAni / (float)MaxAni);
            // 设置起始与结束角度
            float startAngleOffset = MathHelper.ToRadians(-45);
            float endAngleOffset = MathHelper.ToRadians(135);
            // 计算基础旋转角度
            float baseRotation = MathHelper.Lerp(startAngleOffset, endAngleOffset, easedProgress);
            // 根据玩家方向进行镜像处理
            if (Owner.direction == -1)// 水平镜像
                baseRotation = baseRotation * Owner.direction;
            Projectile.rotation = baseRotation + BeginRot;
            Opacity = MathHelper.Lerp(1f, 0f, easedProgress);
            Vector2 offset = new Vector2(50, 0);
            if (CurAni < MaxAni / 2)
            {
                float beginrot = Main.rand.NextFloat(0, MathHelper.TwoPi);
                float rotSpeed = Main.rand.NextBool() ? 0.07f : -0.07f;
                int length = Main.rand.Next(250, 500);
                int LifeTime = Main.rand.Next(30, 60);
                new ProjAbsorbGlowBall(Owner.Center, Color.DarkViolet, LifeTime, 0.1f, beginrot, rotSpeed, Projectile.whoAmI, length, offset).Spawn();
            }
            if (CurAni == 1)
            {
                int LifeTime = 60;
                new FollowProjCrossGlow(Owner.Center, Color.DarkViolet, LifeTime, 0.8f, Projectile.whoAmI, offset).Spawn();
                new FollowProjCrossGlow(Owner.Center, Color.Violet, LifeTime, 0.4f, Projectile.whoAmI, offset).Spawn();
            }

            Color Firecolor = LAPUtilities.LerpColor(Color.Black, Color.DarkViolet);
            new Fire(Projectile.Center + offset.RotatedBy(Projectile.rotation), Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.2f, 1.2f) * 4, Firecolor, 90, Main.rand.NextFloat(MathHelper.TwoPi), 1f, 0.3f).SpawnToPriorityNonPreMult();
        }
        #endregion
        #region 处理结束动画
        public void HandleEndAni()
        {
            int MaxAni = animationHelper.MaxAniProgress[AnimationState.End];
            int CurAni = animationHelper.AniProgress[AnimationState.End];
            // 使用缓动函数让动画更自然
            float easedProgress = EasingHelper.EaseInCubic(CurAni / (float)MaxAni);
            // 设置起始与结束角度
            float startAngleOffset = MathHelper.ToRadians(135);
            float endAngleOffset = MathHelper.ToRadians(-135);
            // 计算基础旋转角度
            float baseRotation = MathHelper.Lerp(startAngleOffset, endAngleOffset, easedProgress);
            // 根据玩家方向进行镜像处理
            if (Owner.direction == -1)// 水平镜像
                baseRotation = baseRotation * Owner.direction;
            Projectile.rotation = baseRotation + BeginRot;

            Vector2 offset = new Vector2(50, 0);
            Color Firecolor = LAPUtilities.LerpColor(Color.Black, Color.DarkViolet);
            new Fire(Projectile.Center + offset.RotatedBy(Projectile.rotation), Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.2f, 1.2f) * 4, Firecolor, 90, Main.rand.NextFloat(MathHelper.TwoPi), 1f, 0.3f).SpawnToPriorityNonPreMult();
        }
        #endregion
        #endregion
        public void UpdateOrb()
        {
            Vector2 TargetPos = new Vector2(52, 7 * Owner.direction).RotatedBy(Projectile.rotation);
            ShadowOrb.Position = Vector2.Lerp(ShadowOrb.Position, TargetPos, 0.4f);
        }
        public override void OnKill(int timeLeft)
        {
            Vector2 offset = new Vector2(50, 0);
            Vector2 firpos = Projectile.Center + offset.RotatedBy(Projectile.rotation);
            for (int i = 0; i < 100; i++)
            {
                Color Firecolor = LAPUtilities.LerpColor(Color.Black, Color.DarkViolet);
                new Fire(firpos, Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.6f, 1.2f) * 12, Firecolor, 90, Main.rand.NextFloat(MathHelper.TwoPi), 1f, 0.3f).SpawnToPriorityNonPreMult();
            }
            for (int i = 0; i < 8; i++)
            {
                Color color = LAPUtilities.LerpColor(Color.Violet, Color.DarkViolet);
                new NoiseShockRing(firpos, Vector2.Zero, color, 60, 1f, 1f + i * 0.05f, -1, Vector2.Zero).Spawn();
            }
            new CrossGlow(firpos, Vector2.Zero, Color.Violet, 60, 1f, 0.7f, true).Spawn();
            new CrossGlow(firpos, Vector2.Zero, Color.DarkViolet, 60, 1f, 0.7f, true).Spawn();
            for (int i = 0; i < 8; i++)
            {
                float X = Main.rand.Next(300, 500);
                Vector2 SpawnPos = new Vector2(X, 0).RotatedByRandom(MathHelper.TwoPi);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + SpawnPos, Vector2.Zero, ModContent.ProjectileType<ShadowPlayer>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 8 * i);
            }
            SoundEngine.PlaySound(SoundsMenu.ShadowBoltStaffSkillrelease, Projectile.Center);
            Owner.AddCooldown(ShadowBotlStaffDodge.ID, 1800);
            Owner.AddCooldown(ShadowBotlStaffCount.ID, 1800);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            LAPUtilities.ReSetToBeginShader(BlendState.NonPremultiplied);
            Main.graphics.GraphicsDevice.Textures[1] = UCATextureRegister.Noise.Value;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
            LAPUtilities.FastApplyEdgeMeltsShader(Opacity, ModContent.Request<Texture2D>(Texture).Size(), Color.DarkViolet, 0.01f, 0);
            DrawBaseStaff();
            DrawOrb();
            LAPUtilities.ReSetToEndShader();
            return false;
        }
        public void DrawBaseStaff()
        {
            Texture2D DrawTexture = UCATextureRegister.ShadowBoltStaffLong.Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            float drawRotation = Projectile.rotation + (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0f + MathHelper.PiOver4 * (Projectile.spriteDirection + 1));
            Vector2 rotationPoint = DrawTexture.Size() / 2f;
            SpriteEffects flipSprite = Projectile.spriteDirection * Main.player[Projectile.owner].gravDir == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.spriteBatch.Draw(DrawTexture, drawPosition, null, Color.White, drawRotation - MathHelper.PiOver4, rotationPoint, Projectile.scale * Main.player[Projectile.owner].gravDir, flipSprite, 0f);
        }
        public void DrawOrb()
        {
            Texture2D DrawTexture = ShadowOrb.Texture;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition + ShadowOrb.Position;
            Vector2 rotationPoint = DrawTexture.Size() / 2f;
            SpriteEffects flipSprite = Projectile.spriteDirection * Main.player[Projectile.owner].gravDir == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            // spriteBatch会自动把textures0设置为当前使用的材质，所以需要你手动改一下
            Main.spriteBatch.Draw(DrawTexture, drawPosition, null, Color.White, 0, rotationPoint, Projectile.scale * Main.player[Projectile.owner].gravDir, flipSprite, 0f);
        }
    }
}
