using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace UCA.Core.BaseClass
{
    public abstract class BaseMagicProj : ModProjectile,ILocalizedModType
    {
        public new string LocalizationCategory => "MagicProjectiles";
    }
}
