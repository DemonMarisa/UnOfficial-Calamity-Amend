using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using UCA.Core.MetaBallsSystem;

namespace UCA.Core.SystemsLoader
{
    public static class UCAMetaBall
    {
        public static int MetaBallType<T>() where T : BaseMetaBall => ModContent.GetInstance<T>()?.Type ?? 0;
    }
}
