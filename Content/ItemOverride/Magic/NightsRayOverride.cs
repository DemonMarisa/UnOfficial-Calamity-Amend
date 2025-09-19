using CalamityMod;
using CalamityMod.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using UCA.Common.Misc;
using UCA.Content.Paths;
using UCA.Content.Projectiles.HeldProj;
using UCA.Content.UCACooldowns;
using UCA.Core.GlobalInstance.Players;
using UCA.Core.Keybinds;
using UCA.Core.Utilities;
using static System.Net.Mime.MediaTypeNames;

namespace UCA.Content.ItemOverride.Magic
{
    public class NightsRayOverride : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Item item, bool lateInstatiation)
        {
            return item.type == ModContent.ItemType<NightsRay>();
        }

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
        }
        public override bool CanUseItem(Item item, Player player)
        {
            return player.ownedProjectileCounts[ModContent.ProjectileType<NightRayHeldProj>()] < 1 && player.ownedProjectileCounts[ModContent.ProjectileType<NightRaySkillProj>()] < 1 && player.ownedProjectileCounts[ModContent.ProjectileType<NightRayHeldProjMelee>()] < 1;
        }

        public override bool AltFunctionUse(Item item, Player player)
        {
            return true;
        }
        public override bool AllowPrefix(Item item, int pre)
        {
            return true;
        }

        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
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

        public override void HoldItem(Item item, Player player)
        {
            if (UCAKeybind.WeaponSkillHotKey.JustPressed)
            {
                if (player.ownedProjectileCounts[ModContent.ProjectileType<NightRaySkillProj>()] < 1 && player.ownedProjectileCounts[ModContent.ProjectileType<NightRayHeldProj>()] < 1 && player.ownedProjectileCounts[ModContent.ProjectileType<NightRayHeldProjMelee>()] < 1)
                    if (player.CheckMana(player.ActiveItem(), (int)(200 * player.manaCost), true, false))
                    Projectile.NewProjectileDirect(item.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<NightRaySkillProj>(), 0, 0, player.whoAmI);
            }

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
                AddIngredient(ItemID.WandofSparking).
                AddIngredient(ItemID.ThunderStaff).
                AddTile(TileID.DemonAltar).
                Register();
        }
    }
}
