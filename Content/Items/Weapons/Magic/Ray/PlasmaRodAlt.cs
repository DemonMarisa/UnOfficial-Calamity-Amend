using LAP.Core.LAPSource;
using LAP.Core.MiscDate;
using LAP.Core.SystemsLoader;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Common.Misc;
using UCA.Content.Projectiles.HeldProj.Magic.PlasmaRodHeld;
using UCA.Core.BaseClass;
using static System.Net.Mime.MediaTypeNames;

namespace UCA.Content.Items.Weapons.Magic.Ray
{
    public class PlasmaRodAlt : BaseMagicWeapon
    {
        public static int PlasmaRodFilp = 1;
        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 8;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 10;
            Item.width = 44;
            Item.height = 44;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3.25f;
            Item.value = UCAShopValue.RarityBlueBuyPrice;
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<PlasmaRodHeldProj>();
            Item.shootSpeed = 6f;

            Item.noUseGraphic = true;
            Item.channel = true;

            Item.LAP().UseWeaponSkill = true;
            Item.LAP().UseCustomWeaponSkill = true;
            Item.LAP().DrawUCASmallIcon = true;
            Item.LAP().WeaponSkillFocusCost = 10;
            Item.LAP().SkillShoot = ProjectileType<PlasmaRodSkillProj>();
            Item.LAP().SkillShootSpeed = 10;
        }
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
        public override bool CanUseItem(Player player)
        {
            return !player.HasProj<PlasmaRodHeldProj>() && !player.HasProj<PlasmaRodHeldProjBlast>() && !player.HasProj<PlasmaRodSkillProj>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                if (PlasmaRodFilp == 1)
                {
                    PlasmaRodFilp = -1;
                }
                else
                {
                    PlasmaRodFilp = 1;
                }
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<PlasmaRodHeldProjBlast>(), damage * 5, knockback, player.whoAmI, 0, PlasmaRodFilp);
            }
            else
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            return false;
        }

        public override void WeaponSkill(Player player, EntitySource_ItemUse_WeaponSkill source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (!player.HasProj<PlasmaRodHeldProj>() && !player.HasProj<PlasmaRodHeldProjBlast>() && !player.HasProj<PlasmaRodSkillProj>())
            {
                if (PlasmaRodFilp == 1)
                    PlasmaRodFilp = -1;
                else
                    PlasmaRodFilp = 1;
                Projectile.NewProjectile(source, position, velocity, type, damage * 10, knockback, player.whoAmI, PlasmaRodFilp);
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Amethyst).
                AddIngredient(ItemID.Glass, 2).
                AddRecipeGroup(VanillaRecipeGroups.Wood, 12).
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
