using LAP.Common.CalamityModCross;
using LAP.Common.Utilities;
using LAP.Core.Enums;
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
using UCA.Content.Projectiles.HeldProj.Magic.ShadowBoltStaffHeld;
using UCA.Content.UCACooldowns;
using UCA.Core.BaseClass;

namespace UCA.Content.Items.Weapons.Magic.Ray
{
    public class ShadowBoltStaffAlt : BaseMagicWeapon
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.damage = 560;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 10;
            Item.width = 44;
            Item.height = 44;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3.25f;
            Item.value = UCAShopValue.RarityTurquoiseBuyPrice;
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<ShadowBoltStaffHeldProj>();
            Item.shootSpeed = 6f;

            Item.noUseGraphic = true;
            Item.channel = true;

            Item.LAP().UseWeaponSkill = true;
            Item.LAP().DrawUCASmallIcon = true;

            Item.LAP().UseCICalStatInflation = true;
            Item.LAP().UseCustomWeaponSkill = true;
            Item.LAP().WeaponTier = AllWeaponTier.PostPolterghast;

            Item.LAP().WeaponSkillFocusCost = 250;

            Item.LAP().SkillShoot = ProjectileType<ShadowBoltStaffSkillHeldProj>();
            Item.LAP().SkillShootSpeed = 0;
        }
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
        public override bool CanUseItem(Player player)
        {
            return !player.HasProj<ShadowBoltStaffHeldProj>();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2 && player.CheckMana((int)(200 * player.manaCost), true))
            {
                Projectile.NewProjectile(source, position, Vector2.Zero, ModContent.ProjectileType<ShadowBoltStaffSpecialHeldProj>(), damage, knockback, player.whoAmI);
            }
            else
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            return false;
        }
        public override bool CanUseWeaponSkill(Player player) => !player.HasCD<ShadowBotlStaffCount>();
        public override void WeaponSkill(Player player, EntitySource_ItemUse_WeaponSkill source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (!player.HasProj<ShadowBoltStaffSkillHeldProj>() && !player.HasProj<ShadowBoltStaffHeldProj>() && !player.HasProj<ShadowBoltStaffSpecialHeldProj>())
            {
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            }
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
            int manacost = (int)(200 * Main.LocalPlayer.manaCost);
            tooltips.FindAndReplace("[ManaCost]" , manacost.ToString());
        }
        public override void UpdateHoldItem(Player player)
        {
        }

        public override void AddRecipes()
        {
            if (ModCrossUtils.HasCalamityMod())
            {
                CreateRecipe().
                    AddIngredient(ItemID.ShadowbeamStaff).
                    AddIngredient(CalMaterialsID.ArmoredShellID, 3).
                    AddIngredient(CalMaterialsID.RuinousSoulID, 2).
                    AddTile(TileID.LunarCraftingStation).
                    Register();
            }
            else
            {
                CreateRecipe().
                    AddIngredient(ItemID.ShadowbeamStaff).
                    AddIngredient(ItemID.LunarBar, 12).
                    AddTile(TileID.LunarCraftingStation).
                    Register();
            }
        }
    }
}
