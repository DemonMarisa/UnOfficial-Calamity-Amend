using CalamityMod;
using CalamityMod.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Common.Misc;
using UCA.Content.Paths;
using UCA.Content.Projectiles.HeldProj.Magic;
using UCA.Core.BaseClass;
using UCA.Core.Keybinds;
using UCA.Core.MiscDate;
using UCA.Core.Utilities;

namespace UCA.Content.ItemOverride.Magic
{
    public class PlasmaRodOverride : BaseWeaponOverride
    {
        public static int PlasmaRodFilp = 1;
        public override bool InstancePerEntity => true;
        public override int OverrideType { get => ModContent.ItemType<PlasmaRod>(); set => ModContent.ItemType<PlasmaRod>(); }
        public override void SetStaticDefaults()
        {
            TextureAssets.Item[ModContent.ItemType<PlasmaRod>()] = ModContent.Request<Texture2D>($"{ItemOverridePaths.MagicWeaponsPath}" + "PlasmaRodOverride");
            Item.staff[ModContent.ItemType<PlasmaRod>()] = true;
        }

        public override void SetDefaults(Item item)
        {
            item.damage = 8;
            item.DamageType = DamageClass.Magic;
            item.mana = 10;
            item.width = 44;
            item.height = 44;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.Shoot;
            item.noMelee = true;
            item.knockBack = 3.25f;
            item.value = UCAShopValue.RarityBlueBuyPrice;
            item.rare = ItemRarityID.Blue;
            item.UseSound = null;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<PlasmaRodHeldProj>();
            item.shootSpeed = 6f;

            item.noUseGraphic = true;
            item.channel = true;

            item.UCA().UseWeaponSkill = true;
            item.UCA().DrawSmallIcon = true;
        }

        public override bool AltFunctionUse(Item item, Player player)
        {
            return true;
        }

        public override bool CanUseItem(Item item, Player player)
        {
            return !player.HasProj<PlasmaRodHeldProj>() && !player.HasProj<PlasmaRodHeldProjBlast>() && !player.HasProj<PlasmaRodSkillProj>();
        }

        public override void CustomShoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
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
        }

        public override void WeaponSkill(Item item, Player player)
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
                    float kb = player.GetWeaponKnockback(item);
                    int Damage = player.GetWeaponDamage(item);
                    Projectile.NewProjectileDirect(player.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<PlasmaRodSkillProj>(), Damage * 10, kb, player.whoAmI, PlasmaRodFilp);
                }
            }
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            string newKey = UCAKeybind.WeaponSkillHotKey.TooltipHotkeyString();
            string totalStatBuffs = UCA.Instance.GetLocalization("CalMagicWeaponsChange.PlasmaRod").Format(newKey);

            TooltipLine line = new(Mod, "UCAWeaponsChange", totalStatBuffs);

            tooltips.Add(line);
        }

        public override void AddRecipes()
        {
            Recipe.Create(ModContent.ItemType<PlasmaRod>()).
                AddIngredient(ItemID.Amethyst).
                AddIngredient(ItemID.Glass,  2).
                AddRecipeGroup(VanillaRecipeGroups.Wood, 12).
                AddTile(TileID.WorkBenches).
                Register();
        }

    }
}
