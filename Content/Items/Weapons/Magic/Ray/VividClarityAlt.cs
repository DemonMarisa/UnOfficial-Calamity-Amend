using LAP.Core.Keybind;
using LAP.Core.LAPSource;
using LAP.Core.MiscDate;
using LAP.Core.SystemsLoader;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Assets.Sounds;
using UCA.Content.Paths;
using UCA.Content.Projectiles.HeldProj.Magic.ElementRayHeld;
using UCA.Content.Projectiles.HeldProj.Magic.VividClarityHeld;
using UCA.Core.BaseClass;
using UCA.Core.Enum;
using UCA.Core.Utilities;

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
            Item.useTime = 54;
            Item.useAnimation = 54;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 7.5f;
            Item.value = Item.buyPrice(5, 0, 0, 0);
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.shoot = ProjectileType<VividClarityHeldProj>();
            Item.rare = ItemRarityID.Purple;
            Item.shootSpeed = 12f;
            Item.LAP().SkillShoot = ProjectileType<VividClarityGreatSword>();
            Item.LAP().WeaponSkillFocusCost = 100;
            Item.LAP().WeaponSkillManaCost = 200;

            Item.channel = true;
            Item.noUseGraphic = true;
        }
        public override void ModifyManaCost(Player player, ref float reduce, ref float mult)
        {
        }
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
        public override bool CanUseItem(Player player)
        {
            if (player.UCA().VividClarityStates == VividClarityState.Support)
            {
                if (player.HasProj<VividClaritySupportMinion>())
                {
                    return false;
                }    
            }
            return !player.HasProj<VividClarityHeldProj>();
        }
        public override bool PrePayMana(Player player, int manaCost)
        {
            return false;
        }
        public override bool PrePayFocus(Player player, int focusCost)
        {
            return false;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.HasProj<VividClarityHeldProj>() && player.HasProj<VividClarityDefenseLeft>() && player.HasProj<VividClarityHeldParry>())
                return false;
            if (player.UCA().VividClarityStates == VividClarityState.Attack)
            {
                if (player.altFunctionUse == 2)
                {
                    if (player.CheckMana(Item.LAP().WeaponSkillRealManaCost - (int)(Item.mana * player.manaCost), true))
                        Projectile.NewProjectile(source, position, velocity, ProjectileType<VividClarityGreatSword>(), damage * 100, knockback, player.whoAmI);
                }
                else
                    Projectile.NewProjectile(source, position, velocity, ProjectileType<VividClarityHeldProj>(), damage, knockback, player.whoAmI);
            }
            else if (player.UCA().VividClarityStates == VividClarityState.Defense)
            {
                if (!player.HasProj<VividClarityDefenseLeft>() && !player.HasProj<VividClarityHeldParry>())
                {
                    if (player.altFunctionUse == 2)
                    {
                        if (player.CheckFocus(Item.LAP().WeaponSkillRealFocusCost, true))
                            Projectile.NewProjectile(source, position, velocity, ProjectileType<VividClarityHeldParry>(), damage, knockback, player.whoAmI);
                    }
                    else
                        Projectile.NewProjectile(source, position, velocity, ProjectileType<VividClarityDefenseLeft>(), damage, knockback, player.whoAmI);
                }
            }
            else if (player.UCA().VividClarityStates == VividClarityState.Support)
            {
                if (!player.HasProj<VividClaritySupportMinion>())
                {
                    if (player.altFunctionUse == 2)
                    {
                        if (player.CheckFocus(Item.LAP().WeaponSkillRealFocusCost, true))
                            Projectile.NewProjectile(source, position, velocity, ProjectileType<VividClaritySupportMinion>(), damage, knockback, player.whoAmI);
                        player.SetItemTime(0);
                        player.SetItemAnimation(0);
                    }
                    else
                        Projectile.NewProjectile(source, position, velocity, ProjectileType<VividClaritySupportLeft>(), damage, knockback, player.whoAmI);
                }
            }
            return false;
        }

        public override void WeaponSkill(Player player, EntitySource_ItemUse_WeaponSkill source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (!player.HasProj<VividClaritySkillHeldProj>())
            {
                Projectile.NewProjectile(source, position, velocity, ProjectileType<VividClaritySkillHeldProj>(), damage, knockback, player.whoAmI);
            }
        }
        public override void UpdateHoldItem(Player player)
        {
            if (player.UCA().VividClarityStates == VividClarityState.Attack)
            {
                Item.LAP().WeaponSkillFocusCost = 100;
            }
            else if (player.UCA().VividClarityStates == VividClarityState.Defense)
            {
                Item.LAP().WeaponSkillFocusCost = 10;
            }
            else if (player.UCA().VividClarityStates == VividClarityState.Support)
            {
                Item.LAP().WeaponSkillFocusCost = 100;
            }
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            LAPUtilities.IntegrateHotkey(tooltips, LAPKeybind.WeaponSkillHotKey);
            Player player = Main.LocalPlayer;
            string MiscTooltip;
            if (player.UCA().VividClarityStates == VividClarityState.Attack)
            {
                MiscTooltip = LocalizedPath.VividClarity_AssaultTooltip;
                LAPUtilities.FindAndReplace(tooltips, "[UCAVividClarityReplaceKey]", MiscTooltip);
            }
            if (player.UCA().VividClarityStates == VividClarityState.Defense)
            {
                MiscTooltip = LocalizedPath.VividClarity_DefenseTooltip;
                LAPUtilities.FindAndReplace(tooltips, "[UCAVividClarityReplaceKey]", MiscTooltip);
            }
            if (player.UCA().VividClarityStates == VividClarityState.Support)
            {
                MiscTooltip = LocalizedPath.VividClarity_SupportTooltip;
                LAPUtilities.FindAndReplace(tooltips, "[UCAVividClarityReplaceKey]", MiscTooltip);
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
