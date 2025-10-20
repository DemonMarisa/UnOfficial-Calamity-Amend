using CalamityMod;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Runtime.InteropServices;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Content.DrawNodes;
using UCA.Content.Particiles;
using UCA.Content.Paths;
using UCA.Content.Projectiles.HeldProj.Magic.ElementRayHeld;
using UCA.Content.Projectiles.Magic;
using UCA.Content.Projectiles.Magic.Ray;
using UCA.Content.Projectiles.Misc.Test;
using UCA.Content.UCACooldowns;
using UCA.Core.Graphics;
using UCA.Core.Graphics.DrawNode;
using UCA.Core.MetaBallsSystem;
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

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            /*
            for (int i = 0; i < 11; i++)
            {
                float rotAdd = MathHelper.ToRadians(3);
                Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(-15) + rotAdd * i) * Main.rand.NextFloat(1f, 2f) * 0.5f, ModContent.ProjectileType<NebulaEnegry>(), damage, knockback, player.whoAmI);
            }
            */
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<SolarFireBall>(), damage, knockback, player.whoAmI);
            
            player.UCA().ElementalRayStates = ElementalRayState.Solar;
            /*
            Vector2 inputUV = new Vector2(0, 0.5f);
            Main.NewText("输入的UV : " + inputUV);
            Main.NewText("输出的UV : " + Solar2(inputUV));
            */
            return false;
        }

        public Vector2 Solar(Vector2 input)
        {
            // 1. 获取纹理坐标 (UV) 并将其中心点移至 (0, 0)
            // UV 坐标范围从 (0, 0) 到 (1, 1)，但是length的中心点是(
            Vector2 uv = input;
            Vector2 centeredUV = uv - new Vector2(0.5f , 0.5f);
            // 2. 笛卡尔坐标 -> 极坐标
            // r = sqrt(x^2 + y^2)
            float radius = centeredUV.Length();
            // θ = atan2(y, x)
            float angle = centeredUV.ToRotation();
            // 4. 极坐标 -> 笛卡尔坐标
            float x = (float)(Math.Cos(angle) * radius);
            float y = (float)(Math.Sin(angle) * radius);
            Vector2 distortedUV = new Vector2(x, y);
            // 5. 将中心点移回 (0.5, 0.5)
            distortedUV += new Vector2(0.5f, 0.5f);
            return distortedUV;
        }
        public Vector2 Solar2(Vector2 input)
        {
            Vector2 uv = input;
            uv = uv - new Vector2(0.5f, 0.5f); //坐标中心移到物体中心
            float theta = uv.ToRotation(); //获取夹角，值域为-π到π
            theta = theta / 3.1415926f * 0.5f + 0.5f; //将夹角值域转为-1到1
            float r = uv.Length(); //获取半径，加上了时间偏移，得到向圆心收缩的动态效果
            uv = new Vector2(theta, r);
            return uv;
        }
        public override bool? UseItem(Player player)
        {
            // new Flame(Main.MouseWorld, Vector2.Zero, Color.White, 64, 0, 1, 1f).Spawn();
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
