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
