using LAP.Core.BaseClass.Legacys;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.ModLoader;
using UCA.Assets.Sounds;
using UCA.Content.Items.Weapons.Magic.Ray;
using UCA.Content.Projectiles.Magic.Ray;

namespace UCA.Content.Projectiles.HeldProj.Magic.SoulPiercerHeld
{
    public class SoulPiercerHeldProj : BaseHeldProj
    {
        public override LocalizedText DisplayName => LAPUtilities.GetItemName<SoulPiercerAlt>();
        public Vector2 RotVector => new Vector2(12 * Owner.direction, 7).BetterRotatedBy(Owner.GetPlayerToMouseVector2().ToRotation(), default, 0.5f, 1f);
        public override Vector2 RotPoint => TextureAssets.Projectile[Type].Size() / 2;
        public override Vector2 Posffset => new Vector2(RotVector.X, RotVector.Y) * Owner.direction;
        public override float RotAmount => 0.25f;
        public override float RotOffset => MathHelper.PiOver4;
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
            SoundEngine.PlaySound(SoundsMenu.MagicStaffFire with { Volume = 0.3f, Pitch = Main.rand.NextFloat(0.2f, 0.5f) });
            Vector2 FireOffset = new Vector2(48, 0).RotatedBy(Projectile.rotation);
            if (Projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + FireOffset, Projectile.velocity, ModContent.ProjectileType<CosmicLaser>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            Projectile.velocity -= Projectile.velocity.RotatedBy(Projectile.spriteDirection * MathHelper.PiOver2) * 0.1f * Owner.direction;
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
        }
        public override void OnKill(int timeLeft)
        {
            Main.mouseRight = false;
            Owner.itemTime = 0;
            Owner.itemAnimation = 0;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D DrawTexture = TextureAssets.Projectile[Type].Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            float drawRotation = Projectile.rotation + (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0f + MathHelper.PiOver4 * (Projectile.spriteDirection + 1));
            Vector2 rotationPoint = DrawTexture.Size() / 2f;
            SpriteEffects flipSprite = Projectile.spriteDirection * Main.player[Projectile.owner].gravDir == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.spriteBatch.Draw(DrawTexture, drawPosition, null, Color.White, drawRotation - MathHelper.PiOver4, rotationPoint, Projectile.scale * Main.player[Projectile.owner].gravDir, flipSprite, 0f);
            return false;
        }
    }
}
