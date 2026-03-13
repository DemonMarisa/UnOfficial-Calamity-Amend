using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Content.MetaBalls;
using UCA.Content.Projectiles.HeldProj.Melee.StormRuler;
using UCA.Content.Projectiles.Magic.Ray;
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
            Projectile.NewProjectile(source, position, velocity * 0.5f, ModContent.ProjectileType<StormRulerHeldSkillProj>(), damage, knockback, player.whoAmI);
            return false;
        }
        public static void GenUnDeathSign(Vector2 firePos, float speedMult = 1)
        {
            for (int i = 0; i < 145f; i++)
            {
                float offsetAngle = MathHelper.TwoPi * i / 145f;
                float unitOffsetX = (float)Math.Pow(Math.Cos(offsetAngle), 5D) * 1.5f;
                float unitOffsetY = (float)Math.Pow(Math.Sin(offsetAngle), 5D);
                Vector2 puffDustVelocity = new Vector2(unitOffsetX, unitOffsetY) * 7f * speedMult;
                CosmicMetaBall.SpawnCircleParticle(firePos, puffDustVelocity, 0.13f, 90);
            }
        }
        public static void GenStar()
        {
        }
        public override bool? UseItem(Player player)
        {
            var UCAPlayer = player.UCA();
            return base.UseItem(player);
        }   

		public override void AddRecipes()
		{
		}
	}
}
