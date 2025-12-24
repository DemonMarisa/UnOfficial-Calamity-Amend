using CalamityMod;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Magic;
using LAP.Core.Keybind;
using LAP.Core.SystemsLoader;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Common.Misc;
using UCA.Content.Projectiles.HeldProj.Magic.TerraRayHeld;
using UCA.Content.UCACooldowns;
using UCA.Core.BaseClass;
using UCA.Core.GlobalInstance.Players;
using UCA.Core.Keybinds;
using UCA.Core.Utilities;

namespace UCA.Content.Items.Weapons.Magic.Ray
{
    public class TerraRay : BaseMagicWeapon
    {
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
            Item.width = 58;
            Item.height = 56;
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3.25f;
            Item.value = UCAShopValue.RarityLightRedBuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<TerraRayHeldProj>();
            Item.shootSpeed = 6f;

            Item.noUseGraphic = true;
            Item.channel = true;
            Item.LAP().UseWeaponSkill = true;
            Item.LAP().DrawUCASmallIcon = true;
            Item.LAP().UseCustomWeaponSkill = true;
            Item.LAP().WeaponSkillManaCost = 200;
        }
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
        public override bool CanUseItem(Player player)
        {
            return !player.HasProj<TerraRayHeldProj>();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                if (player.ownedProjectileCounts[ModContent.ProjectileType<TerraRayHeldProjSpecial>()] < 1)
                    Projectile.NewProjectileDirect(source, position, velocity, ModContent.ProjectileType<TerraRayHeldProjSpecial>(), damage, knockback, player.whoAmI);
            }
            else
            {
                if (player.ownedProjectileCounts[ModContent.ProjectileType<TerraRayHeldProj>()] < 1)
                    Projectile.NewProjectileDirect(source, position, velocity, ModContent.ProjectileType<TerraRayHeldProj>(), damage, knockback, player.whoAmI);
            }
            return false;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            // 因为调用了两次，所以额外插入一次
            LAPUtilities.IntegrateHotkey(tooltips, LAPKeybind.WeaponSkillHotKey);
        }
        public override void WeaponSkill(Player player)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<TerraRayHeldProj>()] < 1 && player.ownedProjectileCounts[ModContent.ProjectileType<TerraRayHeldProjSpecial>()] < 1 && player.ownedProjectileCounts[ModContent.ProjectileType<TerraRayHeldProjSkill>()] < 1)
            {
                if (player.CheckMana(player.ActiveItem(), Item.LAP().WeaponSkillRealManaCost, true, false))
                {
                    float kb = player.GetWeaponKnockback(Item);
                    int Damage = player.GetWeaponDamage(Item);
                    float Projectilai = 0;
                    if (player.UCA().TerraRayRestore > 0 && player.controlUp)
                    {
                        Projectilai = 1f;
                        player.UCA().TerraRayRestore--;
                    }
                    Projectile.NewProjectileDirect(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<TerraRayHeldProjSkill>(), Damage, kb, player.whoAmI, Projectilai);
                }
            }
        }
        public override void UpdateHoldItem(Player player)
        {
            if (!player.HasCD<TerraRayCount>())
                player.AddCD(LAPContent.CDType<TerraRayCount>(), UCAPlayer.MaxTerraRayRestore);
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ModContent.ItemType<ValkyrieRay>()).
                AddIngredient(ModContent.ItemType<CarnageRay>()).
                AddIngredient(ItemID.BrokenHeroSword).
                AddIngredient(ModContent.ItemType<LivingShard>(), 12).
                AddTile(TileID.DemonAltar).
                Register();

            CreateRecipe().
                AddIngredient(ModContent.ItemType<ValkyrieRay>()).
                AddIngredient(ModContent.ItemType<NightsRay>()).
                AddIngredient(ItemID.BrokenHeroSword).
                AddIngredient(ModContent.ItemType<LivingShard>(),12).
                AddTile(TileID.DemonAltar).
                Register();
        }
    }
}
