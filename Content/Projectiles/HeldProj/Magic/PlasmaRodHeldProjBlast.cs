using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Content.Particiles;
using UCA.Content.Paths;
using UCA.Content.Projectiles.Magic.Ray;
using UCA.Content.UCACooldowns;
using UCA.Core.AnimationHandle;
using UCA.Core.Enums;
using UCA.Core.Utilities;

namespace UCA.Content.Projectiles.HeldProj.Magic
{
    public class PlasmaRodHeldProjBlast : ModProjectile, ILocalizedModType
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<PlasmaRod>();
        public override string Texture => $"{ItemOverridePaths.MagicWeaponsPath}" + "PlasmaRodOverride";

        public AnimationHelper animationHelper;

        public int OwnerDir = 0;
        public Player Owner => Main.player[Projectile.owner];

        public float BeginRot = 0;
        public bool UseEnergySword => Projectile.ai[0] != 0;
        public bool Filp => Projectile.ai[1] == 1;
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

        #region 注册数据
        public override void OnSpawn(IEntitySource source)
        {
            animationHelper = new AnimationHelper(3);

            animationHelper.MaxAniProgress[AnimationState.Begin] = 30;
            animationHelper.MaxAniProgress[AnimationState.End] = 10;

            OwnerDir = Owner.LocalMouseWorld().X > Owner.Center.X ? 1 : -1;

            BeginRot = Owner.GetPlayerToMouseVector2().ToRotation();
        }
        #endregion

        public override void AI()
        {
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;

            Owner.ChangeDir(OwnerDir);
            Owner.heldProj = Projectile.whoAmI;
            Projectile.Center = Owner.Center;

            // 基础信息
            Projectile.velocity = Projectile.rotation.ToRotationVector2();
            Projectile.timeLeft = 2;

            if (Projectile.owner == Main.myPlayer)
            {
                AllAI();
            }

            Projectile.netUpdate = true;
        }
        public void AllAI()
        {
            float baseRotation = Projectile.velocity.ToRotation();
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, baseRotation - MathHelper.PiOver2);

            if (!animationHelper.HasFinish[AnimationState.Begin])
            {
                Projectile.extraUpdates = 0;

                animationHelper.AniProgress[AnimationState.Begin]++;

                HandleBeginAni();

                if (animationHelper.AniProgress[AnimationState.Begin] == animationHelper.MaxAniProgress[AnimationState.Begin])
                    animationHelper.HasFinish[AnimationState.Begin] = true;
            }
            else if (!animationHelper.HasFinish[AnimationState.End])
            {
                animationHelper.AniProgress[AnimationState.End]++;

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

            if (CurAni == 8)
            {
                FireProj();
            }

            // 使用缓动函数让动画更自然
            float easedProgress = EasingHelper.EaseOutBack(CurAni / (float)MaxAni);

            float startAngleOffset = MathHelper.ToRadians(185) * Projectile.ai[1];
            float endAngleOffset = -MathHelper.ToRadians(125) * Projectile.ai[1];

            // 计算基础旋转角度
            float baseRotation = MathHelper.Lerp(startAngleOffset, endAngleOffset, easedProgress);

            float FinalOffset = MathHelper.ToRadians(125f);

            if (!Filp)
                FinalOffset = 0;

            baseRotation = baseRotation + FinalOffset;

            // 根据玩家方向进行镜像处理
            if (Owner.direction == -1)// 水平镜像
                baseRotation =  baseRotation * Owner.direction;

            // 最终加上使用时的角度
            Projectile.rotation = baseRotation + BeginRot;
        }

        public void FireProj()
        {
            SoundEngine.PlaySound(SoundsMenu.NightRayHit, Projectile.Center);
            Vector2 FireOffset = new Vector2(54, 0).RotatedBy(BeginRot);
            for (int i = 0; i < 35; i++)
            {
                float offset = MathHelper.TwoPi / 35;
                Color RandomColor = Color.Lerp(Color.DarkViolet, Color.LightPink, Main.rand.NextFloat(0, 1));
                new MediumGlowBall(Projectile.Center + FireOffset, Projectile.velocity.RotatedBy(offset * i), RandomColor, 60, 0, 1, 0.2f, Main.rand.NextFloat(2f, 2.2f)).Spawn();
            }
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + FireOffset, BeginRot.ToRotationVector2() * 2f, ModContent.ProjectileType<PlasmaPrimarySpark>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            // 绘制位置，在这里进行偏移，碰撞箱使用自定义碰撞箱
            Vector2 drawPosition = Projectile.Center - Main.screenPosition + Vector2.UnitX.RotatedBy(Projectile.rotation) * 0;

            float drawRotation = Projectile.rotation + (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0f) + MathHelper.PiOver2;

            Vector2 rotationPoint = new Vector2(0, texture.Size().Y);

            SpriteEffects flipSprite = Projectile.spriteDirection * Main.player[Projectile.owner].gravDir == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            // spriteBatch会自动把textures0设置为当前使用的材质，所以需要你手动改一下
            Main.spriteBatch.Draw(texture, drawPosition, null, Color.White, drawRotation - MathHelper.PiOver4, rotationPoint, Projectile.scale * Main.player[Projectile.owner].gravDir, flipSprite, 0f);
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

        }

        public override void OnKill(int timeLeft)
        {
            if (Main.mouseRight)
            {
                if (Projectile.ai[1] == 1)
                {
                    Projectile.ai[1] = -1;
                }
                else
                {
                    Projectile.ai[1] = 1;
                }
                animationHelper = new AnimationHelper();
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, Type, Projectile.damage, Projectile.knockBack, Projectile.owner, 0, Projectile.ai[1]);
            }
        }
    }
}
