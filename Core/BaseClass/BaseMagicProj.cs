using Terraria.ModLoader;

namespace UCA.Core.BaseClass
{
    public abstract class BaseMagicProj : ModProjectile,ILocalizedModType
    {
        public new string LocalizationCategory => "MagicProjectiles";
    }
}
