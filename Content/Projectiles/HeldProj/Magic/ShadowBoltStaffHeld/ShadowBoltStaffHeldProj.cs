using CalamityMod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Assets.Sounds;
using UCA.Content.Particiles;
using UCA.Content.Projectiles.Magic.Ray;
using LAP.Core.AnimationHandle;
using LAP.Core.BaseClass;
using UCA.Core.Enums;
using UCA.Content.Items.Weapons.Magic.Ray;
using LAP.Core.Utilities;

namespace UCA.Content.Projectiles.HeldProj.Magic.ShadowBoltStaffHeld
{
    public class ShadowBoltStaffHeldProj : BaseHeldProj
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<ShadowBoltStaffAlt>();
        public Vector2 RotVector => new Vector2(12 * Owner.direction, 7).BetterRotatedBy(Owner.GetPlayerToMouseVector2().ToRotation(), default, 0.5f, 1f);
        public override Vector2 RotPoint => TextureAssets.Projectile[Type].Size() / 2;

        public override Vector2 Posffset => new Vector2(RotVector.X, RotVector.Y) * Owner.direction;

        public override float RotAmount => 0.25f;

        public override float RotOffset => MathHelper.PiOver4;
        public float Opacity = 1f;
        public AnimationHelper animationHelper = new AnimationHelper(3);
        public BasePartInfo ShadowOrb;
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
        public override bool StillInUse()
        {
            return !Owner.noItems && !LAPUtilities.JustPressRightClick() && !Owner.CCed && Owner.LAP().MouseLeft;
        }
        public override bool PreAI()
        {
            if (Projectile.LAP().FirstFrame)
            {
                Texture2D texture2d = UCATextureRegister.ShadowBoltStaffOrb.Value;
                ShadowOrb = new BasePartInfo(texture2d, Vector2.Zero, Vector2.Zero, 0, texture2d.Size() / 2);
                animationHelper.MaxAniProgress[AnimationState.Begin] = 15;
            }
            return true;
        }
        public override void ExtraHoldoutAI()
        {
            if (UseDelay <= 0 && Owner.CheckMana(Owner.ActiveItem(), (int)(Owner.HeldItem.mana * Owner.manaCost), true, false))
            {
                FirePorj();
                UseDelay = Owner.HeldItem.useTime;
            }
        }
        public void FirePorj()
        {
            SoundEngine.PlaySound(SoundsMenu.PlasmaRodAttack with { Pitch = 0f}, Projectile.Center);
            Vector2 FireOffset = new Vector2(54, 0).RotatedBy(Projectile.rotation);
            for (int i = 0; i < 35; i++)
            {
                float offset = MathHelper.TwoPi / 35;
                Color RandomColor = Color.Lerp(Color.DarkViolet, Color.LightPink, Main.rand.NextFloat(0, 1));
                new MediumGlowBall(Projectile.Center + FireOffset, Projectile.velocity.RotatedBy(offset * i), RandomColor, 60, 0, 1, 0.2f, Main.rand.NextFloat(2f, 2.2f)).Spawn();
            }
            if (Projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + FireOffset, Projectile.velocity * 12f, ModContent.ProjectileType<ShadowBeam>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            Projectile.velocity -= Projectile.velocity.RotatedBy(Projectile.spriteDirection * MathHelper.PiOver2) * 0.1f;
        }
        public override bool CanDel()
        {
            return UseDelay <= 0 && !LAPUtilities.JustPressLeftClick();
        }
        public override void PostAI()
        {
            // 设置玩家手持效果
            float baseRotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            float directionVerticality = MathF.Abs(Projectile.velocity.X);
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, baseRotation + Owner.direction * directionVerticality * 1.5f);
            Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, baseRotation + Owner.direction * directionVerticality * 1.2f);
            UpdateOrb();
            if (Projectile.owner != Main.myPlayer)
                return;
            if ((Main.mouseLeft || Active) && !LAPUtilities.JustPressRightClick())
            {
                if (animationHelper.AniProgress[AnimationState.Begin] < animationHelper.MaxAniProgress[AnimationState.Begin])
                    animationHelper.AniProgress[AnimationState.Begin]++;
            }
            else
            {
                if (animationHelper.AniProgress[AnimationState.Begin] > 0)
                    animationHelper.AniProgress[AnimationState.Begin]--;
            }
            int MaxAni = animationHelper.MaxAniProgress[AnimationState.Begin];
            int CurAni = animationHelper.AniProgress[AnimationState.Begin];
            float easedProgress = (CurAni / (float)MaxAni);
            Opacity = MathHelper.Lerp(0.7f, 0f, easedProgress);
        }
        public void UpdateOrb()
        {
            Vector2 TargetPos = new Vector2(52, 7 * Owner.direction).RotatedBy(Projectile.rotation);
            ShadowOrb.Position = Vector2.Lerp(ShadowOrb.Position, TargetPos, 0.4f);
        }
        public override void OnKill(int timeLeft)
        {
            Main.mouseRight = false;
            Owner.itemTime = 0;
            Owner.itemAnimation = 0;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            LAPUtilities.ReSetToBeginShader(BlendState.NonPremultiplied);
            Main.graphics.GraphicsDevice.Textures[1] = UCATextureRegister.Noise.Value;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
            LAPUtilities.FastApplyEdgeMeltsShader(Opacity, ModContent.Request<Texture2D>(Texture).Size(), Color.DarkViolet, 0.01f, 0);
            DrawBaseStaff();
            LAPUtilities.ReSetToEndShader();
            DrawOrb();
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
