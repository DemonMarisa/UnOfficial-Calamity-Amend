using LAP.Core.LAPSource;
using LAP.Core.SystemsLoader;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Common.Misc;
using UCA.Content.Projectiles.HeldProj.Magic.CarnageRayHeld;
using UCA.Content.Projectiles.HeldProj.Magic.NightRatHeld;
using UCA.Content.UCACooldowns;
using UCA.Core.BaseClass;
using UCA.Core.GlobalInstance.Players;

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
            Item.value = UCAShopValue.RarityOrangeBuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.shoot = ProjectileType<NightRayHeldProj>();
            Item.shootSpeed = 6f;

            Item.noUseGraphic = true;
            Item.channel = true;
            Item.LAP().UseWeaponSkill = true;
            Item.LAP().UseCustomWeaponSkill = true;
            Item.LAP().DrawUCASmallIcon = true;
            Item.LAP().WeaponSkillFocusCost = 50;
            Item.LAP().WeaponSkillRealFocusCost = 50;
            Item.LAP().SkillShoot = ProjectileType<NightRaySkillProj>();
            Item.LAP().SkillShootSpeed = 0;
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
        public override bool CanUseWeaponSkill(Player player) => !player.HasCD<NightBoost>();
        public override void WeaponSkill(Player player, EntitySource_ItemUse_WeaponSkill source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (!player.HasProj<NightRaySkillProj>() && !player.HasProj<NightRayHeldProj>() && !player.HasProj<NightRayHeldProjMelee>())
            {
                Projectile.NewProjectileDirect(source, position, velocity, type, 0, 0, player.whoAmI);
            }
        }
        public override void UpdateHoldItem(Player player)
        {
            if (!player.HasCD<NightShield>())
                player.AddCD(LAPContent.CDType<NightShield>(), UCAPlayer.NightShieldMaxHP, false);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Vilethorn).
                AddIngredient(ItemID.MagicMissile).
                AddIngredient<PlasmaRodAlt>().
                AddIngredient(ItemID.ThunderStaff).
                AddTile(TileID.DemonAltar).
                Register();
        }
    }
}
