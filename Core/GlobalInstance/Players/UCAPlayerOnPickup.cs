using CalamityMod;
using Terraria;
using Terraria.ModLoader;
using UCA.Content.Items.Weapons.Rogue.Hammer;

namespace UCA.Core.GlobalInstance.Players
{
    public partial class UCAPlayer : ModPlayer
    {
        public override bool OnPickup(Item item)
        {
            if (StopGodHammerShimemrGuide(item))
                return true;
            return base.OnPickup(item);
        }
        /// <summary>
        /// 梦魇锤投掷微光转为弑神锤的引导
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool StopGodHammerShimemrGuide(Item item)
        {
            if (item.type == ModContent.ItemType<DivineHammer>() && DownedBossSystem.downedDoG)
            {
                CanDisableGuideForGodsHammer = true;
                return true;
            }
            if (item.type == ModContent.ItemType<ThunderHammer>() && DownedBossSystem.downedCalamitas && DownedBossSystem.downedExoMechs)
            {
                CanDisableGuideForGrandHammer = true;
                return true;
            }
            return false;
        }
    }
}
