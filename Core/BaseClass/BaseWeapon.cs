using LAP.Core.BaseClass;
using Terraria.ModLoader;

namespace UCA.Core.BaseClass
{
    public abstract class BaseMagicWeapon : BaseSkillWeapon, ILocalizedModType
    {
        public new string LocalizationCategory => "MagicWeapons";
    }
    public abstract class BaseMeleeWeapon : BaseSkillWeapon, ILocalizedModType
    {
        public new string LocalizationCategory => "MeleeWeapons";
    }
}
