using CalamityMod;
using CalamityMod.CalPlayer;
using Terraria;
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

        public static float ApplyPlayerDefAndDR(this Player player, int Damage, bool ApplyDRRot)
        {
            float InComingDamage = Damage;
            InComingDamage -= player.statDefense;
            if (ApplyDRRot)
            {
                float realDR = player.endurance / 1f + player.endurance;
                InComingDamage *= 1 - realDR;
            }
            else
            {
                InComingDamage *= 1 - player.endurance;
            }
            return InComingDamage;
        }
    }
}
