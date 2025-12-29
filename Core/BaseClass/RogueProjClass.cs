using Terraria;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Core.Utilities;

namespace UCA.Core.BaseClass
{
    public abstract class RogueProjClass : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => UCATextureRegister.InvisibleTexturePath;
        public Player Owner => Main.player[Projectile.owner];
        public bool Stealth => GetStealthType;
        public override void SetDefaults()
        {
            Projectile.DamageType = RangedDamageType;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
            ExSD();
        }
        public virtual void ExSD() { }
        internal bool GetStealthType
        {
            get
            {
                if (ModLoader.HasMod("CalamityMod"))
                {
                    Mod fuckCalamity = ModLoader.GetMod("CalamityMod");
                    return (bool)fuckCalamity.Call("GetStealthProjectile", Projectile);
                }
                else
                    return Projectile.UCA().SetStealthStrike;
            }
        }
        internal DamageClass RangedDamageType
        {
            get
            {
                if (ModLoader.HasMod("CalamityMod"))
                {
                    Mod fuckCalamity = ModLoader.GetMod("CalamityMod");
                    if (fuckCalamity.TryFind("RogueDamageClass", out DamageClass d))
                        return d;
                }
                return DamageClass.Ranged;
            }
        }
    }
}
