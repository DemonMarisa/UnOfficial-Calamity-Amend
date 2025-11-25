using Terraria.ModLoader;
using UCA.Content.Items.Weapons.Rogue.Hammer;

namespace UCA.Core.List
{
    public class UCAList : ModSystem
    {
        public static int[] RogueHammer = 
            [
            ModContent.ItemType<PunishmentHammer>(),
            ModContent.ItemType<BlazingHammer>(),
            ModContent.ItemType<NightmareHammer>(),
            ModContent.ItemType<DivineHammer>(),
            //ModContent.ItemType<ThunderHammer>(),
            ];
    }
}
