using CalamityMod;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
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

        public override bool? UseItem(Player player)
        {
			Vector2 SpawnPos = Main.MouseWorld + Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(100, 200);
            Vector2 SpawnPosToMouseWorld = (Main.MouseWorld - SpawnPos).SafeNormalize(Vector2.UnitX);
			float rot = SpawnPosToMouseWorld.ToRotation() + 1;
            new Line(SpawnPos, Vector2.Zero, new Color(255, 255, 255, 0), Main.rand.Next(60, 90), rot, 1, 0.15f, true, Main.MouseWorld).Spawn();
            CalamityPlayer calamityPlayer = player.Calamity();
            calamityPlayer.cooldowns.Remove(NightBoost.ID);
            player.UCA().NightShieldHP = 0;
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
