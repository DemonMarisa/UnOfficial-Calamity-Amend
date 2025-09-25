using CalamityMod;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using UCA.Common.Misc;
using UCA.Content.Projectiles.HeldProj.Magic;
using UCA.Content.UCACooldowns;
using UCA.Core.BaseClass;
using UCA.Core.GlobalInstance.Players;
using UCA.Core.Keybinds;
using UCA.Core.Utilities;
using static System.Net.Mime.MediaTypeNames;

namespace UCA.Content.Items.Magic.Ray
{
    public class CarnageRay : BaseMagicWeapon, ILocalizedModType
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return true;
        }

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
            Item.UCA().UseWeaponSkill = true;
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
            tooltips.IntegrateHotkey(UCAKeybind.WeaponSkillHotKey);
        }

        public override void HoldItem(Player player)
        {
            
            if (UCAKeybind.WeaponSkillHotKey.JustPressed && !Main.blockMouse && !Main.mouseText)
            {
                if (player.ownedProjectileCounts[ModContent.ProjectileType<CarnageRaySkillProj>()] < 1 && player.ownedProjectileCounts[ModContent.ProjectileType<CarnageRayHeldProjMelee>()] < 1 && player.ownedProjectileCounts[ModContent.ProjectileType<CarnageRayHeldProj>()] < 1)
                {
                    if (player.CheckMana(player.ActiveItem(), (int)(200 * player.manaCost), false, false))
                    {
                        player.PickAmmo(Item, out int toshoot, out float speed, out int Damage, out float kb, out AmmoID.None);
                        Projectile.NewProjectileDirect(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<CarnageRaySkillProj>(), Damage, kb, player.whoAmI);
                    }
                }
            }
            
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.CrimsonRod).
                AddIngredient(ItemID.MagicMissile).
                AddIngredient(ItemID.WandofSparking).
                AddIngredient(ItemID.ThunderStaff).
                AddTile(TileID.DemonAltar).
                Register();
        }
    }
}
