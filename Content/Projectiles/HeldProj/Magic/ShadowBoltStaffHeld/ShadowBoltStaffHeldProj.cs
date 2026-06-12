using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Localization;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Assets.Sounds;
using UCA.Content.Particiles;
using UCA.Content.Projectiles.Magic.Ray;
using LAP.Core.AnimationHandle;
using LAP.Core.Enums;
using UCA.Content.Items.Weapons.Magic.Ray;
using LAP.Core.Utilities;
using LAP.Core.BaseClass.Projectiles;

namespace UCA.Content.Projectiles.HeldProj.Magic.ShadowBoltStaffHeld
{
    public class ShadowBoltStaffHeldProj : BaseHeldProj
    {
        public override LocalizedText DisplayName => LAPUtilities.GetItemName<ShadowBoltStaffAlt>();
        public Vector2 RotVector => new Vector2(12 * Owner.direction, 7).BetterRotatedBy(Owner.GetPlayerToMouseVector2().ToRotation(), default, 0.5f, 1f);
        public override Vector2 PositionOffset => RotVector * Owner.direction;
        public float Opacity = 1f;
        public AniHelper aniHelper = new AniHelper(3);
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
        public override bool PreAI()
        {
            if (Projectile.LAP().FirstFrame)
            {
                Texture2D texture2d = UCATextureRegister.ShadowBoltStaffOrb.Value;
                ShadowOrb = new BasePartInfo(texture2d, Vector2.Zero, Vector2.Zero, 0, texture2d.Size() / 2);
                aniHelper.MaxAniProgress[AniState.Begin] = 15;
            }
            return true;
        }
        public override void ExAI()
        {
            RotAmount = 0.25f;
            if (Owner.LAP().MouseLeft && !Owner.LAP().MouseRight)
            {
                if (UseDelay <= 0 && Owner.CheckMana(Owner.ActiveItem(), (int)(Owner.HeldItem.mana * Owner.manaCost), true, false))
                {
                    FirePorj();
                    UseDelay = Owner.HeldItem.useTime;
                }
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
        public override void PostAI()
        {
            base.PostAI();
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
                if (aniHelper.AniProgress[AniState.Begin] < aniHelper.MaxAniProgress[AniState.Begin])
                    aniHelper.AniProgress[AniState.Begin]++;
            }
            else
            {
                if (aniHelper.AniProgress[AniState.Begin] > 0)
                    aniHelper.AniProgress[AniState.Begin]--;
            }
            int MaxAni = aniHelper.MaxAniProgress[AniState.Begin];
            int CurAni = aniHelper.AniProgress[AniState.Begin];
            float easedProgress = (CurAni / (float)MaxAni);
            Opacity = MathHelper.Lerp(0.7f, 0f, easedProgress);
            if (!Owner.LAP().MouseLeft && Owner.LAP().MouseRight && UseDelay == 0)
            {
                Main.mouseRight = false;
                Owner.itemTime = 0;
                Owner.itemAnimation = 0;
                Projectile.Kill();
            }
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
            DrawBaseStaff(lightColor);
            DrawOrb(lightColor);
            return false;
        }
        public void DrawBaseStaff(Color lightColor)
        {
            Texture2D DrawTexture = UCATextureRegister.ShadowBoltStaffLong.Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            float drawRotation = Projectile.rotation + (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0f + MathHelper.PiOver4 * (Projectile.spriteDirection + 1));
            Vector2 rotationPoint = DrawTexture.Size() / 2f;
            SpriteEffects flipSprite = Projectile.spriteDirection * Main.player[Projectile.owner].gravDir == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.spriteBatch.Draw(DrawTexture, drawPosition, null, lightColor, drawRotation - MathHelper.PiOver4, rotationPoint, Projectile.scale * Main.player[Projectile.owner].gravDir, flipSprite, 0f);
        }
        public void DrawOrb(Color lightColor)
        {
            Texture2D DrawTexture = ShadowOrb.Texture;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition + ShadowOrb.Position;
            Vector2 rotationPoint = DrawTexture.Size() / 2f;
            SpriteEffects flipSprite = Projectile.spriteDirection * Main.player[Projectile.owner].gravDir == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            // spriteBatch会自动把textures0设置为当前使用的材质，所以需要你手动改一下
            Main.spriteBatch.Draw(DrawTexture, drawPosition, null, lightColor, 0, rotationPoint, Projectile.scale * Main.player[Projectile.owner].gravDir, flipSprite, 0f);
        }
    }
}
