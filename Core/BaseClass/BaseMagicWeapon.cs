using CalamityMod;
using LAP.Core.BaseClass;
using LAP.Core.Keybind;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace UCA.Core.BaseClass
{
    public abstract class BaseMagicWeapon : BaseSkillWeapon, ILocalizedModType
    {
        public new string LocalizationCategory => "MagicWeapons";
    }
}
