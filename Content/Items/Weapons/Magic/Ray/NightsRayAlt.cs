using CalamityMod;
using CalamityMod.Items.Weapons.Magic;
using LAP.Core.Enums;
using LAP.Core.Keybind;
using LAP.Core.SystemsLoader;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Common.Misc;
using UCA.Content.Paths;
using UCA.Content.Projectiles.HeldProj.Magic.NightRatHeld;
using UCA.Content.UCACooldowns;
using UCA.Content.UCARecipeGroups;
using UCA.Core.BaseClass;
using UCA.Core.GlobalInstance.Players;
using UCA.Core.Keybinds;
using UCA.Core.Utilities;

namespace UCA.Content.Items.Weapons.Magic.Ray
{
    public class NightsRayAlt : BaseMagicWeapon
    {
        public static int UseCount = 0;
        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
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
            Item.shoot = ModContent.ProjectileType<NightRayHeldProj>();
            Item.shootSpeed = 6f;

            Item.noUseGraphic = true;
            Item.channel = true;
            Item.LAP().UseWeaponSkill = true;
            Item.LAP().UseCustomWeaponSkill = true;
            Item.LAP().DrawUCASmallIcon = true;
            Item.LAP().WeaponSkillManaCost = 200;
        }
        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[ModContent.ProjectileType<NightRayHeldProj>()] < 1 && player.ownedProjectileCounts[ModContent.ProjectileType<NightRaySkillProj>()] < 1 && player.ownedProjectileCounts[ModContent.ProjectileType<NightRayHeldProjMelee>()] < 1;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                if (player.ownedProjectileCounts[ModContent.ProjectileType<NightRayHeldProjMelee>()] < 1)
                    Projectile.NewProjectileDirect(source, position, velocity, ModContent.ProjectileType<NightRayHeldProjMelee>(), damage, knockback, player.whoAmI);
            }
            else
            {
                if (player.ownedProjectileCounts[ModContent.ProjectileType<NightRayHeldProj>()] < 1)
                    Projectile.NewProjectileDirect(source, position, velocity, ModContent.ProjectileType<NightRayHeldProj>(), damage, knockback, player.whoAmI);
            }
            return false;
        }

        public override void WeaponSkill(Player player)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<NightRaySkillProj>()] < 1 && player.ownedProjectileCounts[ModContent.ProjectileType<NightRayHeldProj>()] < 1 && player.ownedProjectileCounts[ModContent.ProjectileType<NightRayHeldProjMelee>()] < 1)
                if (!player.HasCD<NightBoost>() && player.CheckMana(player.ActiveItem(), Item.LAP().WeaponSkillRealManaCost, true, false))
                    Projectile.NewProjectileDirect(Item.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<NightRaySkillProj>(), 0, 0, player.whoAmI);
        }

        public override void UpdateHoldItem(Player player)
        {
            if (!player.HasCD<NightShield>())
                player.AddCD(LAPContent.CDType<NightShield>(), UCAPlayer.NightShieldMaxHP);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Vilethorn).
                AddIngredient(ItemID.MagicMissile).
                AddRecipeGroup(UCARecipeGroup.PlasmaRodGroup).
                AddIngredient(ItemID.ThunderStaff).
                AddTile(TileID.DemonAltar).
                Register();
        }
    }
}
