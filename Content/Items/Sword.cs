using LAP.Core.SystemsLoader;
using LAP.Core.UISystem;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Assets.Sounds;
using UCA.Content.GUI.ERayUI;
using UCA.Content.MetaBalls;
using UCA.Content.Projectiles.HealPRoj;
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

            Item.useTime = 6;
            Item.useAnimation = 54;
            Item.reuseDelay = 25;
            Item.useLimitPerAnimation = 9;
        }
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo projSource, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            Vector2 playerPos = player.RotatedRelativePoint(player.MountedCenter, true);
            float speed = Item.shootSpeed;
            float xPos = Main.mouseX + Main.screenPosition.X - playerPos.X;
            float yPos = Main.mouseY + Main.screenPosition.Y - playerPos.Y;
            float f = Main.rand.NextFloat() * MathHelper.TwoPi;

            float sourceVariationLow = 90f;
            float sourceVariationHigh = 180f;
            Vector2 source = playerPos + f.ToRotationVector2() * MathHelper.Lerp(sourceVariationLow, sourceVariationHigh, Main.rand.NextFloat());

            for (int i = 0; i < 50; i++)
            {
                source = playerPos + f.ToRotationVector2() * MathHelper.Lerp(sourceVariationLow, sourceVariationHigh, Main.rand.NextFloat());
                if (Collision.CanHit(playerPos, 0, 0, source + (source - playerPos).SafeNormalize(Vector2.UnitX) * 8f, 0, 0))
                {
                    break;
                }
                f = Main.rand.NextFloat() * MathHelper.TwoPi;
            }
            Vector2 velocityReal = Main.MouseWorld - source;
            Vector2 velocityVariation = new Vector2(xPos, yPos).SafeNormalize(Vector2.UnitY) * speed;
            velocityReal = velocityReal.SafeNormalize(velocityVariation) * speed;

            Projectile.NewProjectile(projSource, source, velocityReal, ProjectileType<VividBeam>(), damage, knockback, player.whoAmI);
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
            return base.UseItem(player);
        }   

		public override void AddRecipes()
		{
		}
	}
}
