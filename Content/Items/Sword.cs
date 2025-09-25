using CalamityMod;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Content.Particiles;
using UCA.Content.UCACooldowns;
using UCA.Core.Utilities;

namespace UCA.Content.Items
{ 
	// This is a basic item template.
	// Please see tModLoader's ExampleMod for every other example:
	// https://github.com/tModLoader/tModLoader/tree/stable/ExampleMod
	public class Sword : ModItem
	{
		// The Display Name and Tooltip of this item can be edited in the 'Localization/en-US_Mods.UCA.hjson' file.
		public override void SetDefaults()
		{
			Item.damage = 50;
			Item.DamageType = DamageClass.Melee;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 6;
			Item.value = Item.buyPrice(silver: 1);
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			velocity *= 4;
            for (int i = 0; i < 35; i++)
			{
                new LilyLiquid(Main.MouseWorld, velocity.RotatedByRandom(MathHelper.PiOver4 * 0.6f) * Main.rand.NextFloat(0.15f, 1.2f), Color.DarkRed, 64, 0, 1, 2).Spawn();
            }
            for (int i = 0; i < 25; i++)
            {
                new LilyLiquid(Main.MouseWorld, velocity.RotatedByRandom(MathHelper.PiOver4 * 0.6f) * Main.rand.NextFloat(0.15f, 1.2f), Color.Black, 64, 0, 1, 2).Spawn();
            }
            return false;
        }

        public override bool? UseItem(Player player)
        {
			// new LilyLiquid(Main.MouseWorld, Vector2.Zero, Color.White, 64, 0, 1, 1f).Spawn();
            return base.UseItem(player);
        }   

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.DirtBlock, 10);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
	}
}
