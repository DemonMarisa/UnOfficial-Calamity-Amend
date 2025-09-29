using System.ComponentModel;
using Terraria.Localization;
using Terraria.ModLoader.Config;

namespace UCA.Content.Configs
{
    [BackgroundColor(105, 105, 105, 216)]
    public class UCAConfig : ModConfig
    {
        public static UCAConfig Instance;
        public override ConfigScope Mode => ConfigScope.ClientSide;
        public override bool AcceptClientChanges(ModConfig pendingConfig, int whoAmI, ref NetworkText message) => true;

        [BackgroundColor(211, 211, 211, 192)]
        [Range(500, 10000)]
        [Increment(1)]
        [DefaultValue(2000)]
        public int ParticleLimit { get; set; }
    }
}
