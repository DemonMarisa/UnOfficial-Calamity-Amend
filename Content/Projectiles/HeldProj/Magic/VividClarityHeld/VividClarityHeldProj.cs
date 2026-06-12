using LAP.Assets.TextureRegister;
using LAP.Core.AnimationHandle;
using LAP.Core.BaseClass.Projectiles;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using UCA.Content.Items.Weapons.Magic.Ray;
using UCA.Content.Projectiles.Magic.Ray;

namespace UCA.Content.Projectiles.HeldProj.Magic.VividClarityHeld
{
    public class VividClarityHeldProj : BaseHeldProj
    {
        public override string Texture => GetInstance<VividClarityAlt>().Texture;
        public override LocalizedText DisplayName => LAPUtilities.GetItemName<VividClarityAlt>();
        public Vector2 RotVector => new Vector2(0, 10).BetterRotatedBy(Owner.GetPlayerToMouseVector2().ToRotation());
        public override Vector2 PositionOffset => RotVector * Owner.direction;
        public AniHelper AniHelper = new AniHelper(3);
        public int NeedFireLeft;
        public bool DrawOnce = false;
        public override void SetDefaults()
        {
            Projectile.width = 142;
            Projectile.height = 142;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.netImportant = true;
        }
        public override void Initialize()
        {
            Projectile.Opacity = 0f;
            RotAmount = 0.2f;
        }
        public override void ExAI()
        {
            if (Owner.LAP().MouseLeft && UseDelay <= 0 && Owner.CheckMana(Owner.ActiveItem(), (int)(Owner.HeldItem.mana * Owner.manaCost), true, false))
            {
                UseDelay = 90;
                NeedFireLeft = 9;
            }
            UpdateOpacity();
            CheckFire();
        }
        public void UpdateOpacity()
        {
            if (Owner.LAP().MouseLeft)
                Projectile.Opacity = MathHelper.Lerp(Projectile.Opacity, 1f, 0.12f);
            else
                Projectile.Opacity = MathHelper.Lerp(Projectile.Opacity, 0f, 0.12f);
        }
        public void CheckFire()
        {
            if (UseDelay % 6 == 0 && NeedFireLeft > 0)
            {
                Vector2 firePos = -Projectile.velocity.RotateRandom(MathHelper.PiOver4) * Main.rand.Next(450, 600);
                Vector2 Spawn = Projectile.Center + firePos;
                Vector2 firvel = LAPUtilities.GetVector2(Spawn, Owner.LAP().SyncedMouseWorld) * 12;

                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Spawn, firvel, ProjectileType<VividBeam>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                NeedFireLeft--;
            }
        }
        public override void SetPlayerVisuals()
        {
            Projectile.SetHeldProj(Owner);
            float baseRotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            float directionVerticality = MathF.Abs(Projectile.velocity.X) * 0.8f;
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, baseRotation + Owner.direction * directionVerticality * 1.5f + 0.5f * Owner.direction);
            Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, baseRotation + Owner.direction * directionVerticality * 1.2f + 0.2f * Owner.direction);
        }
        public override void AimToMouse()
        {
            Projectile.spriteDirection = Owner.direction;
            Projectile.Center = PositionOffset + Owner.Center;
            Projectile.timeLeft = 2;
            Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            Vector2 target = LAPUtilities.GetVector2(Projectile.Center, Owner.LocalMouseWorld());
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, target, RotAmount);
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            LAPUtilities.ReSetToBeginShader(BlendState.AlphaBlend);

            Projectile.GetProjDrawInfo_Staff(out Texture2D texture, out Vector2 drawPosition, out float drawRotation, out Vector2 rotationPoint, out SpriteEffects flipSprite);

            Main.graphics.GraphicsDevice.Textures[1] = LAPTextureRegister.Noise.Value;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;

            LAPUtilities.FastApplyEdgeMeltsShader(1 - Projectile.Opacity, texture.Size(), Color.GhostWhite, 0.01f, 0);

            Main.spriteBatch.Draw(texture, drawPosition + DrawPosOffset, null, lightColor, drawRotation + DrawRotOffset, rotationPoint, Projectile.scale, flipSprite, 0);

            LAPUtilities.ReSetToEndShader();
            return false;
        }
    }
}
