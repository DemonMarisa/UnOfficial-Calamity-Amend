using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Localization;

namespace UCA.Core.Utilities
{
    public static partial class UCAUtilities
    {
        public static LocalizedText GetText(string key)
        {
            return Language.GetOrRegister("Mods.UCA." + key);
        }
        public static string GetTextValue(string key)
        {
            return Language.GetTextValue("Mods.UCA." + key);
        }
    }
}
