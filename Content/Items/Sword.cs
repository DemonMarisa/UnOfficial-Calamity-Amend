using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Content.Projectiles.Magic.Ray;
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
			Item.shoot = ProjectileID.WoodenArrowFriendly;
			Item.shootSpeed = 12;
		}
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            /*
            for (int i = 0; i < 11; i++)
            {
                float rotAdd = MathHelper.ToRadians(3);
                Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(-15) + rotAdd * i) * Main.rand.NextFloat(1f, 2f) * 0.5f, ModContent.ProjectileType<NebulaEnegry>(), damage, knockback, player.whoAmI);
            }
            */
            Projectile.NewProjectile(source, position, velocity * 0.25f, ModContent.ProjectileType<CarnageBall>(), damage, knockback, player.whoAmI);
            return false;
        }
        public override bool? UseItem(Player player)
        {
            // new Flame(Main.MouseWorld, Vector2.Zero, Color.White, 64, 0, 1, 1f).Spawn();
            return base.UseItem(player);
        }   

		public override void AddRecipes()
		{
		}
	}
}
