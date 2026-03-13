using LAP.Core.LAPSource;
using LAP.Core.SystemsLoader;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Common.Misc;
using UCA.Content.Projectiles.HeldProj.Magic.CarnageRayHeld;
using UCA.Core.BaseClass;

namespace UCA.Content.Items.Weapons.Magic.Ray
{
    public class CarnageRay : BaseMagicWeapon
    {
        public static int SkillCost = 200;
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.damage = 45;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 10;
            Item.width = 46;
            Item.height = 46;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3.25f;
            Item.value = UCAShopValue.RarityOrangeBuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<CarnageRayHeldProj>();
            Item.shootSpeed = 6f;

            Item.noUseGraphic = true;
            Item.channel = true;
            Item.LAP().UseWeaponSkill = true;
            Item.LAP().UseCustomWeaponSkill = true;
            Item.LAP().DrawUCASmallIcon = true;
            Item.LAP().WeaponSkillManaCost = 200;
            Item.LAP().WeaponSkillFocusCost = 50;
            Item.LAP().WeaponSkillRealManaCost = 200;
            Item.LAP().WeaponSkillRealFocusCost = 50;
            Item.LAP().SkillShoot = ProjectileType<CarnageRaySkillProj>();
        }
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
        public override bool CanUseItem(Player player)
        {
            return !player.HasProj<CarnageRayHeldProj>() && !player.HasProj<CarnageRayHeldProjMelee>() && !player.HasProj<CarnageRaySkillProj>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                if (player.ownedProjectileCounts[ModContent.ProjectileType<CarnageRayHeldProjMelee>()] < 1)
                    Projectile.NewProjectileDirect(source, position, velocity, ModContent.ProjectileType<CarnageRayHeldProjMelee>(), damage, knockback, player.whoAmI);
            }
            else
            {
                if (player.ownedProjectileCounts[ModContent.ProjectileType<CarnageRayHeldProj>()] < 1)
                    Projectile.NewProjectileDirect(source, position, velocity, ModContent.ProjectileType<CarnageRayHeldProj>(), damage, knockback, player.whoAmI);
            }
            return false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
        }

        public override void WeaponSkill(Player player, EntitySource_ItemUse_WeaponSkill source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (!player.HasProj<CarnageRaySkillProj>() && !player.HasProj<CarnageRayHeldProjMelee>() && !player.HasProj<CarnageRayHeldProj>())
            {
                Projectile.NewProjectile(source, position, Vector2.Zero, type, damage, knockback, player.whoAmI);
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.CrimsonRod).
                AddIngredient(ItemID.MagicMissile).
                AddIngredient<PlasmaRodAlt>().
                AddIngredient(ItemID.ThunderStaff).
                AddTile(TileID.DemonAltar).
                Register();
        }
    }
}
