using CalamityMod;
using CalamityMod.Items.Weapons.Magic;
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
using UCA.Content.Projectiles.HeldProj.Magic;
using UCA.Content.UCACooldowns;
using UCA.Core.BaseClass;
using UCA.Core.GlobalInstance.Players;
using UCA.Core.Keybinds;
using UCA.Core.Utilities;

namespace UCA.Content.ItemOverride.Magic
{
    public class NightsRayOverride : BaseWeaponOverride
    {
        public static int UseCount = 0;
        public override bool InstancePerEntity => true;
        public override int OverrideType { get => ModContent.ItemType<NightsRay>(); set => ModContent.ItemType<NightsRay>(); }

        public override void Load()
        {
            MethodInfo originalMethod = typeof(NightsRay).GetMethod(nameof(NightsRay.AddRecipes));
            MonoModHooks.Add(originalMethod, AddRecipes_Hook);
        }

        public override void SetStaticDefaults()
        {
            TextureAssets.Item[ModContent.ItemType<NightsRay>()] = ModContent.Request<Texture2D>($"{ItemOverridePaths.MagicWeaponsPath}" + "NightsRayOverride");
            Item.staff[ModContent.ItemType<NightsRay>()] = true;
        }

        public override void SetDefaults(Item item)
        {
            item.damage = 45;
            item.DamageType = DamageClass.Magic;
            item.mana = 10;
            item.width = 46;
            item.height = 46;
            item.useTime = 25;
            item.useAnimation = 25;
            item.useStyle = ItemUseStyleID.Shoot;
            item.noMelee = true;
            item.knockBack = 3.25f;
            item.value = UCAShopValue.RarityLightRedBuyPrice;
            item.rare = ItemRarityID.LightRed;
            item.UseSound = null;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<NightRayHeldProj>();
            item.shootSpeed = 6f;

            item.noUseGraphic = true;
            item.channel = true;
            item.UCA().UseWeaponSkill = true;
            item.UCA().DrawSmallIcon = true;
        }
        public override bool CanUseItem(Item item, Player player)
        {
            return player.ownedProjectileCounts[ModContent.ProjectileType<NightRayHeldProj>()] < 1 && player.ownedProjectileCounts[ModContent.ProjectileType<NightRaySkillProj>()] < 1 && player.ownedProjectileCounts[ModContent.ProjectileType<NightRayHeldProjMelee>()] < 1;
        }

        public override bool AltFunctionUse(Item item, Player player)
        {
            return true;
        }
        public override void CustomShoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
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
        }

        public override void WeaponSkill(Item item, Player player)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<NightRaySkillProj>()] < 1 && player.ownedProjectileCounts[ModContent.ProjectileType<NightRayHeldProj>()] < 1 && player.ownedProjectileCounts[ModContent.ProjectileType<NightRayHeldProjMelee>()] < 1)
                if (!player.HasCooldown(NightBoost.ID) && player.CheckMana(player.ActiveItem(), (int)(200 * player.manaCost), true, false))
                    Projectile.NewProjectileDirect(item.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<NightRaySkillProj>(), 0, 0, player.whoAmI);
        }

        public override void UpdateHoldItem(Item item, Player player)
        {
            player.AddCooldown(NightShield.ID, UCAPlayer.NightShieldMaxHP);
            if (player.Calamity().cooldowns.TryGetValue(NightShield.ID, out var Durability))
                Durability.timeLeft = player.UCA().NightShieldHP;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            string newKey = UCAKeybind.WeaponSkillHotKey.TooltipHotkeyString();
            string totalStatBuffs = UCA.Instance.GetLocalization("CalMagicWeaponsChange.NightRay").Format(newKey);

            TooltipLine line = new(Mod, "UCAWeaponsChange", totalStatBuffs);

            tooltips.Add(line);
        }

        public static void AddRecipes_Hook(NightsRay self)
        {
            self.CreateRecipe().
                AddIngredient(ItemID.Vilethorn).
                AddIngredient(ItemID.MagicMissile).
                AddIngredient(ModContent.ItemType<PlasmaRod>()).
                AddIngredient(ItemID.ThunderStaff).
                AddTile(TileID.DemonAltar).
                Register();
        }
    }
}
