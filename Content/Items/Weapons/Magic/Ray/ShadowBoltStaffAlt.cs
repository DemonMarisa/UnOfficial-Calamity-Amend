using CalamityMod;
using CalamityMod.Items.Materials;
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
using UCA.Content.Projectiles.HeldProj.Magic.ShadowBoltStaffHeld;
using UCA.Content.UCACooldowns;
using UCA.Core.BaseClass;
using UCA.Core.Enums;
using UCA.Core.Keybinds;
using UCA.Core.Utilities;

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
            Item.value = UCAShopValue.RarityBlueBuyPrice;
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

            Item.LAP().WeaponSkillManaCost = 200;
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
            if (player.altFunctionUse == 2)
            {
                Projectile.NewProjectile(source, position, Vector2.Zero, ModContent.ProjectileType<ShadowBoltStaffSpecialHeldProj>(), damage, knockback, player.whoAmI);
            }
            else
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            return false;
        }
        public override void WeaponSkill(Player player)
        {
            if (!player.HasProj<ShadowBoltStaffSkillHeldProj>() && !player.HasProj<ShadowBoltStaffHeldProj>() && !player.HasProj<ShadowBoltStaffSpecialHeldProj>())
            {
                if (player.HasCooldown(ShadowBotlStaffCount.ID))
                    return;

                if (player.CheckMana(player.ActiveItem(), Item.LAP().WeaponSkillRealManaCost, true, false))
                {
                    float kb = player.GetWeaponKnockback(Item);
                    int Damage = player.GetWeaponDamage(Item);
                    Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<ShadowBoltStaffSkillHeldProj>(), Damage, kb, player.whoAmI);
                }
            }
        }

        public override void UpdateHoldItem(Player player)
        {
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.ShadowbeamStaff).
                AddIngredient<ArmoredShell>(3).
                AddIngredient<RuinousSoul>(2).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
