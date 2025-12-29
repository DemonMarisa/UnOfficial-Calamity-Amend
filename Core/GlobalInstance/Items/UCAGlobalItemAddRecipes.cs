using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Content.Items.Weapons.Magic.Ray;

namespace UCA.Core.GlobalInstance.Items
{
    public partial class UCAGlobalItem : GlobalItem
    {
        public override void AddRecipes()
        {
            AddTerraRecipes();
        }

        public static void AddTerraRecipes()
        {
            Recipe.Create(ModContent.ItemType<Photosynthesis>()).
                AddIngredient(ModContent.ItemType<ValkyrieRay>()).
                AddIngredient(ModContent.ItemType<CarnageRay>()).
                AddIngredient(ModContent.ItemType<LivingShard>(), 12).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
