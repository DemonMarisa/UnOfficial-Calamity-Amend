using CalamityMod;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Common.Misc;
using UCA.Content.Projectiles.HeldProj.Magic.PlasmaRodHeld;
using UCA.Content.Projectiles.HeldProj.Magic.ShadowBoltStaffHeld;
using UCA.Core.BaseClass;
using LAP.Core.MiscDate;
using UCA.Core.Utilities;
using LAP.Core.Utilities;

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
            Item.damage = 1200;
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

            Item.UCA().UseWeaponSkill = true;
            Item.UCA().DrawSmallIcon = true;
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
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<ShadowBoltStaffHeldProj>(), damage * 5, knockback, player.whoAmI);
            }
            else
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            return false;
        }
        public override void WeaponSkill(Player player)
        {
            if (!player.HasProj<PlasmaRodHeldProj>() && !player.HasProj<PlasmaRodHeldProjBlast>() && !player.HasProj<PlasmaRodSkillProj>())
            {
                if (player.CheckMana(player.ActiveItem(), (int)(50 * player.manaCost), true, false))
                {
                    float kb = player.GetWeaponKnockback(Item);
                    int Damage = player.GetWeaponDamage(Item);
                    int Index = Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<PlasmaRodSkillProj>(), Damage * 10, kb, player.whoAmI);
                    LAPUtilities.SendProjSync(Index);
                }
            }
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            // tooltips.IntegrateHotkey(UCAKeybind.WeaponSkillHotKey);
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
