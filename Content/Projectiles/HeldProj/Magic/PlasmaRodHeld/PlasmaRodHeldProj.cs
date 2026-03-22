using LAP.Core.BaseClass.Projectiles;
using LAP.Core.IDSets;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using UCA.Assets.Sounds;
using UCA.Content.Items.Weapons.Magic.Ray;
using UCA.Content.Particiles;
using UCA.Content.Projectiles.Magic.Ray;

namespace UCA.Content.Projectiles.HeldProj.Magic.PlasmaRodHeld
{
    public class PlasmaRodHeldProj : BaseHeldProj
    {
        public override string Texture => GetInstance<PlasmaRodAlt>().Texture;
        public override LocalizedText DisplayName => LAPUtilities.GetItemName<PlasmaRodAlt>();
        public Vector2 RotVector => new Vector2(0 * Owner.direction, 0).BetterRotatedBy(Owner.GetPlayerToMouseVector2().ToRotation());
        public override Vector2 PositionOffset => RotVector * Owner.direction;

        public int UseAni = 0;

        public int UseCount = 0;
        public override void SetDefaults()
        {
            Projectile.width = 44;
            Projectile.height = 44;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void ExAI()
        {
            RotAmount = 0.25f;
            if (Owner.LAP().MouseLeft && !Owner.LAP().MouseRight)
            {
                if (UseDelay <= 0 && Owner.CheckMana(Owner.ActiveItem(), (int)(Owner.HeldItem.mana * Owner.manaCost), true, false))
                {
                    SoundEngine.PlaySound(SoundsMenu.PlasmaRodAttack, Projectile.Center);
                    SoundEngine.PlaySound(SoundID.Item91, Projectile.Center);
                    Vector2 FireOffset = new Vector2(54, 0).RotatedBy(Projectile.rotation);
                    for (int i = 0; i < 35; i++)
                    {
                        float offset = MathHelper.TwoPi / 35;
                        Color RandomColor = Color.Lerp(Color.DarkViolet, Color.LightPink, Main.rand.NextFloat(0, 1));
                        new MediumGlowBall(Projectile.Center + FireOffset, Projectile.velocity.RotatedBy(offset * i), RandomColor, 60, 0, 1, 0.2f, Main.rand.NextFloat(2f, 2.2f)).Spawn();
                    }
                    FirePorj();
                    Projectile.velocity -= Projectile.velocity.RotatedBy(Projectile.spriteDirection * MathHelper.PiOver2) * 0.1f;
                    UseDelay = Owner.HeldItem.useTime;
                }
            }
        }

        public override void PostAI()
        {
            base.PostAI();
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Owner.GetPlayerToMouseVector2().ToRotation() - MathHelper.PiOver2);
        }

        public void FirePorj()
        {
            Vector2 FireOffset = new Vector2(54, 0).RotatedBy(Projectile.rotation);
            if (Projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + FireOffset, Projectile.velocity * 2f, ModContent.ProjectileType<PlasmaSpark>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }

        public override void OnKill(int timeLeft)
        {
            Main.mouseRight = false;
            Owner.itemTime = 0;
            Owner.itemAnimation = 0;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetProjDrawInfo_Melee(out Texture2D texture, out Vector2 drawPosition, out float drawRotation, out Vector2 rotationPoint, out SpriteEffects flipSprite);
            Main.spriteBatch.Draw(texture, drawPosition + DrawPosOffset, null, lightColor, drawRotation + DrawRotOffset, rotationPoint, Projectile.scale, flipSprite, 0);
            return false;
        }
    }
}
