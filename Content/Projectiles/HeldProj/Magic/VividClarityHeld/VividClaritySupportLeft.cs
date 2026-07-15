using LAP.Assets.TextureRegister;
using LAP.Core.AnimationHandle;
using LAP.Core.BaseClass.Projectiles;
using LAP.Core.Graphics.DeepGlow;
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
    public class VividClaritySupportLeft : BaseHeldProj
    {
        public override string Texture => GetInstance<VividClarityAlt>().Texture;
        public override LocalizedText DisplayName => LAPUtilities.GetItemName<VividClarityAlt>();
        public Vector2 RotVector => new Vector2(0, 10).BetterRotatedBy(Owner.GetPlayerToMouseVector2().ToRotation());
        public override Vector2 PositionOffset => RotVector * Owner.direction;
        public AniHelper AniHelper = new AniHelper(3);
        public int NeedFireRemain;
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
                UseDelay = 35;
                NeedFireRemain = 5;
            }
            UpdateOpacity();
            CheckFire();
        }
        public void UpdateOpacity()
        {
            if (Owner.LAP().MouseLeft || UseDelay > 15)
                Projectile.Opacity = MathHelper.Lerp(Projectile.Opacity, 1f, 0.12f);
            else
                Projectile.Opacity = MathHelper.Lerp(Projectile.Opacity, 0f, 0.12f);
        }
        public void CheckFire()
        {
            if (UseDelay % 3 == 0 && NeedFireRemain > 0)
            {
                float f = Main.rand.NextFloat() * MathHelper.TwoPi;
                float spreadX = 80f;
                float spreadY = 120f;
                Vector2 source = Owner.Center + f.ToRotationVector2() * MathHelper.Lerp(spreadX, spreadY, Main.rand.NextFloat());
                Vector2 firvel = LAPUtilities.GetVector2(source, Owner.LAP().SyncedMouseWorld) * 12;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), source, firvel, ProjectileType<VividBeam_Weak>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 1f);
                NeedFireRemain--;
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
        public override void OnKill(int timeLeft)
        {
            Owner.SetItemAnimation(0);
            Owner.SetItemTime(0);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            LAPUtilities.ReSetToBeginShader(BlendState.AlphaBlend);

            Projectile.GetProjDrawInfo_Staff(out Texture2D texture, out Vector2 drawPosition, out float drawRotation, out Vector2 rotationPoint, out SpriteEffects flipSprite);

            LAPUtilities.SetTexture(LAPTextureRegister.Noise.Value, SamplerState.PointWrap, 1);

            LAPUtilities.FastApplyEdgeMeltsShader(1 - Projectile.Opacity, texture.Size(), Color.GhostWhite, 0.01f, 0);

            Main.spriteBatch.Draw(texture, drawPosition + DrawPosOffset, null, lightColor, drawRotation + DrawRotOffset, rotationPoint, Projectile.scale, flipSprite, 0);

            LAPUtilities.ReSetToEndShader();

            DeepGlow.SubmitCustomGlow(() =>
            {
                LAPUtilities.ReSetToBeginShader(BlendState.AlphaBlend);

                Projectile.GetProjDrawInfo_Staff(out Texture2D texture, out Vector2 drawPosition, out float drawRotation, out Vector2 rotationPoint, out SpriteEffects flipSprite);

                LAPUtilities.SetTexture(LAPTextureRegister.Noise.Value, SamplerState.PointWrap, 1);

                LAPUtilities.FastApplyEdgeMeltsShader(1 - Projectile.Opacity, texture.Size(), Color.GhostWhite, 0.01f, 0);

                Main.spriteBatch.Draw(texture, drawPosition + DrawPosOffset, null, Color.Transparent, drawRotation + DrawRotOffset, rotationPoint, Projectile.scale, flipSprite, 0);

                LAPUtilities.ReSetToEndShader();
            });
            return false;
        }
    }
}
