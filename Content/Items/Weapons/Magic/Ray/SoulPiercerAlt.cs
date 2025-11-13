using CalamityMod;
using LAP.Core.Enums;
using LAP.Core.MiscDate;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Common.Misc;
using UCA.Content.Projectiles.HeldProj.Magic.PlasmaRodHeld;
using UCA.Content.Projectiles.HeldProj.Magic.ShadowBoltStaffHeld;
using UCA.Content.Projectiles.HeldProj.Magic.SoulPiercerHeld;
using UCA.Core.BaseClass;
using UCA.Core.Keybinds;

namespace UCA.Content.Items.Weapons.Magic.Ray
{
    public class SoulPiercerAlt : BaseMagicWeapon
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.damage = 620;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 10;
            Item.width = 76;
            Item.height = 76;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3.25f;
            Item.value = UCAShopValue.RarityBlueBuyPrice;
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<SoulPiercerHeldProj>();
            Item.shootSpeed = 6f;

            Item.noUseGraphic = true;
            Item.channel = true;
       
            Item.LAP().UseWeaponSkill = true;
            Item.LAP().DrawUCASmallIcon = true;

            Item.LAP().WeaponTier = AllWeaponTier.PostDOG;
            Item.LAP().UseCICalStatInflation = true; 
        }
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
        public override bool CanUseItem(Player player)
        {
            return !player.HasProj<SoulPiercerHeldProj>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                Projectile.NewProjectile(source, position, Vector2.Zero, ModContent.ProjectileType<SoulPiercerSpecialHeldProj>(), (int)(damage * 1.5f), knockback, player.whoAmI);
            }
            else
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            return false;
        }
        public override void WeaponSkill(Player player)
        {
            if (!player.HasProj<SoulPiercerHeldProj>() && !player.HasProj<SoulPiercerSpecialHeldProj>() && !player.HasProj<SoulPiercerSkillHeldProj>())
            {
                if (player.CheckMana(player.ActiveItem(), (int)(300 * player.manaCost), true, false))
                {
                    float kb = player.GetWeaponKnockback(Item);
                    int Damage = player.GetWeaponDamage(Item);
                    Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<SoulPiercerSkillHeldProj>(), Damage * 10, kb, player.whoAmI);
                }
            }
        }

        public override void UpdateHoldItem(Player player)
        {
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.IntegrateHotkey(UCAKeybind.WeaponSkillHotKey);
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
