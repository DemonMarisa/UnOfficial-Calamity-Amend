using CalamityMod;
using CalamityMod.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Content.Particiles;
using UCA.Content.Paths;
using UCA.Content.Projectiles.Magic.Ray;
using UCA.Core.BaseClass;
using UCA.Core.Utilities;

namespace UCA.Content.Projectiles.HeldProj.Magic
{
    public class PlasmaRodHeldProj : BaseHeldProj
    {
        public override string Texture => $"{ItemOverridePaths.MagicWeaponsPath}" + "PlasmaRodOverride";
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<PlasmaRod>();

        public override Vector2 RotPoint => Owner.direction == -1 ? new Vector2(TextureAssets.Projectile[Type].Size().X, TextureAssets.Projectile[Type].Size().Y) : new Vector2(0, TextureAssets.Projectile[Type].Size().Y);

        public Vector2 RotVector => new Vector2(0 * Owner.direction, 0).BetterRotatedBy(Owner.GetPlayerToMouseVector2().ToRotation());

        public override Vector2 Posffset => Vector2.Zero;
         
        public override float RotAmount => 0.3f;

        public override float RotOffset => MathHelper.PiOver4;

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

        public override bool StillInUse()
        {
            return Active && !UCAUtilities.JustPressRightClick();
        }

        public override void HoldoutAI()
        {
            if (UseDelay <= 0 && Owner.CheckMana(Owner.ActiveItem(), (int)(Owner.HeldItem.mana * Owner.manaCost), true, false))
            {
                SoundEngine.PlaySound(SoundsMenu.PlasmaRodAttack, Projectile.Center);
                FirePorj();
                UseDelay = Owner.HeldItem.useTime;
            }
        }

        public override void PostAI()
        {
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Owner.GetPlayerToMouseVector2().ToRotation() - MathHelper.PiOver2);
        }

        public void FirePorj()
        {
            SoundEngine.PlaySound(SoundID.Item91, Projectile.Center);
            Vector2 FireOffset = new Vector2(54, 0).RotatedBy(Projectile.rotation);
            for (int i = 0; i < 35; i++)
            {
                float offset = MathHelper.TwoPi / 35;
                Color RandomColor = Color.Lerp(Color.DarkViolet, Color.LightPink, Main.rand.NextFloat(0, 1));
                new MediumGlowBall(Projectile.Center + FireOffset, Projectile.velocity.RotatedBy(offset * i), RandomColor, 60, 0, 1, 0.2f, Main.rand.NextFloat(2f, 2.2f)).Spawn();
            }
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + FireOffset, Projectile.velocity * 2f, ModContent.ProjectileType<PlasmaSpark>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            Projectile.velocity -= Projectile.velocity.RotatedBy(Projectile.spriteDirection * MathHelper.PiOver2) * 0.1f;
        }

        public override void OnKill(int timeLeft)
        {
            Main.mouseRight = false;
            Owner.itemTime = 0;
            Owner.itemAnimation = 0;
        }
    }
}
