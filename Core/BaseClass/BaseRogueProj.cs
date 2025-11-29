using CalamityMod;
using Terraria;
using Terraria.ModLoader;
using UCA.Assets;

namespace UCA.Core.BaseClass
{
    public abstract class RogueProjClass : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => UCATextureRegister.InvisibleTexturePath;
        public Player Owner => Main.player[Projectile.owner];
        public bool StealthType => Projectile.Calamity().stealthStrike;
        public override void SetDefaults()
        {
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
            ExSD();
        }
        public virtual void ExSD() { }
    }
}
