using CalamityMod;
using CalamityMod.Items.Weapons.Magic;
using Humanizer;
using LAP.Core.Keybind;
using LAP.Core.SystemsLoader;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Common.Misc;
using UCA.Content.Projectiles.HeldProj.Magic.CarnageRayHeld;
using UCA.Content.Projectiles.HeldProj.Magic.NightRatHeld;
using UCA.Content.UCACooldowns;
using UCA.Content.UCARecipeGroups;
using UCA.Core.BaseClass;
using UCA.Core.Keybinds;
using UCA.Core.Utilities;

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
            Item.value = UCAShopValue.RarityLightRedBuyPrice;
            Item.rare = ItemRarityID.LightRed;
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

        public override void WeaponSkill(Player player)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<CarnageRaySkillProj>()] < 1 && 
                player.ownedProjectileCounts[ModContent.ProjectileType<CarnageRayHeldProjMelee>()] < 1 && 
                player.ownedProjectileCounts[ModContent.ProjectileType<CarnageRayHeldProj>()] < 1)
            {
                if (player.CheckFocus(Item.LAP().WeaponSkillRealFocusCost, false) && player.CheckMana(player.ActiveItem(), Item.LAP().WeaponSkillRealManaCost, false))
                {
                    player.CheckFocus(Item.LAP().WeaponSkillRealFocusCost, true);
                    player.CheckMana(player.ActiveItem(), Item.LAP().WeaponSkillRealManaCost, true);
                    float kb = player.GetWeaponKnockback(Item);
                    int Damage = player.GetWeaponDamage(Item);
                    Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<CarnageRaySkillProj>(), Damage, kb, player.whoAmI);
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.CrimsonRod).
                AddIngredient(ItemID.MagicMissile).
                AddRecipeGroup(UCARecipeGroup.PlasmaRodGroup).
                AddIngredient(ItemID.ThunderStaff).
                AddTile(TileID.DemonAltar).
                Register();
        }
    }
}
