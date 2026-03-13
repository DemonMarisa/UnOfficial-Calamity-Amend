using Terraria.ModLoader;

namespace UCA.Core.BaseClass
{
    public abstract class BaseMagicProj : ModProjectile,ILocalizedModType
    {
        public new string LocalizationCategory => "MagicProjectiles";
    }
    public abstract class BaseMeleeProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "MeleeProjectiles";
    }
}
