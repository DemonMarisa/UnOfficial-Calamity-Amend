using CalamityMod.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Common.Misc;
using UCA.Content.Paths;
using UCA.Content.Projectiles.HeldProj.Magic.ElementRayHeld;
using UCA.Core.BaseClass;
using UCA.Core.Utilities;

namespace UCA.Content.ItemOverride.Magic
{
    public class ElementRayOverride : BaseWeaponOverride
    { 
        public override bool InstancePerEntity => true;
        public override int OverrideType { get => ModContent.ItemType<ElementalRay>(); set => ModContent.ItemType<ElementalRay>(); }
        public override void SetStaticDefaults()
        {
            TextureAssets.Item[ModContent.ItemType<ElementalRay>()] = ModContent.Request<Texture2D>($"{ItemOverridePaths.MagicWeaponsPath}" + "ElementRayOverride");
            Item.staff[ModContent.ItemType<ElementalRay>()] = true;
        }

        public override void SetDefaults(Item item)
        {
            item.damage = 45;
            item.DamageType = DamageClass.Magic;
            item.mana = 10;
            item.width = 46;
            item.height = 46;
            item.useTime = 35;
            item.useAnimation = 35;
            item.useStyle = ItemUseStyleID.Shoot;
            item.noMelee = true;
            item.knockBack = 3.25f;
            item.value = UCAShopValue.RarityLightRedBuyPrice;
            item.rare = ItemRarityID.LightRed;
            item.UseSound = null;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<ElementRayHeldProj>();
            item.shootSpeed = 6f;

            item.noUseGraphic = true;
            item.channel = true;
            item.UCA().UseWeaponSkill = true;
            item.UCA().DrawSmallIcon = true;
        }
        public override bool CanUseItem(Item item, Player player)
        {
            return player.ownedProjectileCounts[ModContent.ProjectileType<ElementRayHeldProj>()] < 1;
        }

        public override bool AltFunctionUse(Item item, Player player)
        {
            return true;
        }
        public override void CustomShoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                if (player.ownedProjectileCounts[ModContent.ProjectileType<ElementRaySpecialHeldProj>()] < 1)
                {
                    Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<ElementRaySpecialHeldProj>(), damage, knockback, player.whoAmI);
                }
            }
            else
            {
                if (player.ownedProjectileCounts[ModContent.ProjectileType<ElementRayHeldProj>()] < 1)
                {
                    Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<ElementRayHeldProj>(), damage, knockback, player.whoAmI);
                }
            }

        }
    }
}
