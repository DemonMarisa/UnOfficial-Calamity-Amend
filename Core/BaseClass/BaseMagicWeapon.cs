using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace UCA.Core.BaseClass
{
    public abstract class BaseMagicWeapon : ModItem,ILocalizedModType
    {
        public new string LocalizationCategory => "MagicWeapons";
    }
}
