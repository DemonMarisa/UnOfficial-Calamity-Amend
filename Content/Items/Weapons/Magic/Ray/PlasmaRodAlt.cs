using CalamityMod;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Common.Misc;
using UCA.Content.Projectiles.HeldProj.Magic.PlasmaRodHeld;
using UCA.Core.BaseClass;
using UCA.Core.Keybinds;
using LAP.Core.MiscDate;
using UCA.Core.Utilities;
using LAP.Core.Utilities;

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

            Item.UCA().UseWeaponSkill = true;
            Item.UCA().DrawSmallIcon = true;
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

        public override void WeaponSkill(Player player)
        {
            if (!player.HasProj<PlasmaRodHeldProj>() && !player.HasProj<PlasmaRodHeldProjBlast>() && !player.HasProj<PlasmaRodSkillProj>())
            {
                if (player.CheckMana(player.ActiveItem(), (int)(50 * player.manaCost), true, false))
                {
                    if (PlasmaRodFilp == 1)
                    {
                        PlasmaRodFilp = -1;
                    }
                    else
                    {
                        PlasmaRodFilp = 1;
                    }
                    float kb = player.GetWeaponKnockback(Item);
                    int Damage = player.GetWeaponDamage(Item);
                    int Index = Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<PlasmaRodSkillProj>(), Damage * 10, kb, player.whoAmI, PlasmaRodFilp);
                    LAPUtilities.SendProjSync(Index);
                }
            }
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
