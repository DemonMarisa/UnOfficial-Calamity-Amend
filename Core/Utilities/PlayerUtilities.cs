using CalamityMod;
using CalamityMod.CalPlayer;
using CalamityMod.NPCs.TownNPCs;
using Microsoft.Xna.Framework;
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
        public static void HealDirect(this Player player, int amt)
        {
            player.statLife += amt;
            if (Main.myPlayer == player.whoAmI)
                player.HealEffect(amt);

            if (player.statLife > player.statLifeMax2)
                player.statLife = player.statLifeMax2;
        }
    }
}
