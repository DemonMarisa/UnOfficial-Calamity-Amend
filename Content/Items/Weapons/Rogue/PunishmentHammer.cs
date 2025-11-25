using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Common.Misc;
using UCA.Content.Projectiles.Rogue.PunishmentProj;
using UCA.Core.BaseClass;

namespace UCA.Content.Items.Weapons.Rogue
{
    public class PunishmentHammer: BaseHammerItem
    {
        public override int ShootProjID => ModContent.ProjectileType<PunishmentHammerProj>();
        public override void ExSD()
        {
            Item.width = Item.height = 58;
            Item.damage = 50;
            Item.useTime = 13;
            Item.useAnimation = 13;
            Item.shootSpeed = 18f;
            Item.rare = ItemRarityID.LightRed;
            Item.value = UCAShopValue.RarityLightRedBuyPrice;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Pwnhammer).
                AddIngredient(ItemID.SoulofNight, 10).
                AddIngredient(ItemID.DarkShard, 2).
                AddIngredient(ItemID.Diamond, 5).
                AddIngredient(ItemID.Amethyst, 5).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
