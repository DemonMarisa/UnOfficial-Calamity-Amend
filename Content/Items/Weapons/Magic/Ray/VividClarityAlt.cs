using LAP.Core.LAPSource;
using LAP.Core.MiscDate;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Content.Projectiles.HeldProj.Magic.VividClarityHeld;
using UCA.Core.BaseClass;

namespace UCA.Content.Items.Weapons.Magic.Ray
{
    public class VividClarityAlt : BaseMagicWeapon
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 142;
            Item.height = 142;
            Item.damage = 300;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 40;
            Item.useTime = 6;
            Item.useAnimation = 54;
            Item.reuseDelay = 25;
            Item.useLimitPerAnimation = 9;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 7.5f;
            //Item.value = CIShopValue.RarityPriceCatalystViolet;
            //Item.UseSound = UseSound;
            Item.autoReuse = true;
            Item.shoot = ProjectileType<VividClarityHeldProj>();
            //Item.rare = RarityType<CatalystViolet>();
            Item.shootSpeed = 12f;
            Item.LAP().SkillShoot = ProjectileType<VividClarityGreatSword>();

            Item.channel = true;
            Item.noUseGraphic = true;
        }
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
        public override bool CanUseItem(Player player)
        {
            return !player.HasProj<VividClarityHeldProj>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                Projectile.NewProjectile(source, position, velocity, ProjectileType<VividClarityGreatSword>(), damage, knockback, player.whoAmI);
            }
            else
                Projectile.NewProjectile(source, position, velocity, ProjectileType<VividClarityHeldProj>(), damage, knockback, player.whoAmI);
            return false;
        }

        public override void WeaponSkill(Player player, EntitySource_ItemUse_WeaponSkill source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (!player.HasProj<VividClarityHeldProj>())
            {
                Projectile.NewProjectile(source, position, velocity, ProjectileType<VividClarityGreatSword>(), damage * 100, knockback, player.whoAmI);
            }
        }

        public override void AddRecipes()
        {
            //CreateRecipe().
            //    AddIngredient(ItemID.Amethyst).
            //    AddIngredient(ItemID.Glass, 2).
            //    AddRecipeGroup(VanillaRecipeGroups.Wood, 12).
            //    AddTile(TileID.WorkBenches).
            //    Register();
        }
    }
}
