using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Common.Misc;
using UCA.Content.Projectiles.Rogue.BlazingProj;

namespace UCA.Content.Items.Weapons.Rogue
{
    public class BlazingHammer: BaseHammerItem
    {
        public override int ShootProjID => ModContent.ProjectileType<BlazingHammerProj>();
        public override void ExSD()
        {
            Item.width = Item.height = 66;
            Item.damage = 75;
            //这里的ut有意为之
            Item.useTime = 8;
            Item.useAnimation = 8;
            Item.shootSpeed = 18f;
            Item.rare = ItemRarityID.Yellow;
            Item.value = UCAShopValue.RarityYellowBuyPrice;
        }
        //实际合成材料可随意，我个人推荐为花后
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<PunishmentHammer>().
                AddIngredient(ItemID.PaladinsHammer).
                AddIngredient<UnholyCore>(10).
                AddIngredient<LifeAlloy>(5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
