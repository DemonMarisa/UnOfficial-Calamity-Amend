using CalamityMod;
using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;
using UCA.Core.GlobalInstance.Players;

namespace UCA.Core.Utilities
{
    public static partial class UCAUtilities
    {
        public static void RemoveCooldown(this Player player, string id)
        {
            CalamityPlayer calamityPlayer = player.Calamity();
            calamityPlayer.cooldowns.Remove(id);
        }

        public static UCAPlayer UCA(this Player player)
        {
            return player.GetModPlayer<UCAPlayer>();
        }
    }
}
