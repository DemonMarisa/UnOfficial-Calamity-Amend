using CalamityMod.Items.Weapons.Magic;
using LAP.Core.Utilities;
using Terraria;
using Terraria.ModLoader;
using UCA.Content.Items.Weapons.Magic.Ray;

namespace UCA.Content.UCARecipeGroups
{
    public class UCARecipeGroup : ModSystem
    {
        public string PlasmaRod = "UCA:PlasmaRod";
        public static RecipeGroup PlasmaRodGroup;
        public override void AddRecipeGroups()
        {
            LAPUtilities.CreatRecipeGroup(ref PlasmaRodGroup, ModContent.ItemType<PlasmaRodAlt>(), PlasmaRod, ModContent.ItemType<PlasmaRodAlt>(), ModContent.ItemType<PlasmaRod>());
        }
        public override void Unload()
        {
            PlasmaRodGroup = null;
        }
    }
}
