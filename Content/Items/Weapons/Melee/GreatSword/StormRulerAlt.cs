using LAP.Content.Particles;
using LAP.Core.LAPSource;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Common.Misc;
using UCA.Content.Projectiles.HeldProj.Magic.NightRatHeld;
using UCA.Content.Projectiles.HeldProj.Melee.StormRuler;
using UCA.Core.BaseClass;
using UCA.Core.Utilities;

namespace UCA.Content.Items.Weapons.Melee.GreatSword
{
    public class StormRulerAlt : BaseMeleeWeapon
    {
        public override string Texture => UCATextureRegister.StormRulerAlt.Path;
        public override void SetDefaults()
        {
            Item.damage = 600;
            Item.DamageType = DamageClass.Melee;
            Item.width = 84;
            Item.height = 84;
            Item.useTime = 50;
            Item.useAnimation = 50;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 6.25f;
            Item.value = UCAShopValue.RarityRedBuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.shoot = ProjectileType<StormRulerHeldSwingProj>();
            Item.shootSpeed = 38f;

            Item.noUseGraphic = true;
            Item.channel = true;

            Item.LAP().UseWeaponSkill = true;
            Item.LAP().DrawUCASmallIcon = true;
            Item.LAP().UseCustomWeaponSkill = true;
            Item.LAP().WeaponSkillFocusCost = 50;

            Item.LAP().SkillShoot = ProjectileType<StormRulerHeldSkillProj>();
            Item.LAP().SkillShootSpeed = 0;
        }
        public override bool MeleePrefix()
        {
            return true;
        }
        public override bool CanUseItem(Player player)
        {
            return !player.HasProj<StormRulerHeldSwingProj>();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 1, 0.8f, player.GetPlayerToMouseVector2().ToRotation());
            return false;
        }
        public override void WeaponSkill(Player player, EntitySource_ItemUse_WeaponSkill source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (!player.HasProj<StormRulerHeldSkillProj>())
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
        }
        public override void UpdateHoldItem(Player player)
        {
            if (player.UCA().KingOfStorm)
            {
                Vector2 SpawnPos = new Vector2(player.Center.X + Main.rand.Next(-16, 16), player.Center.Y + 24);
                Vector2 firevel = Vector2.UnitY * -6;
                new CampSmoke(SpawnPos, firevel, Color.White, 60, Main.rand.NextFloat(MathHelper.TwoPi), 0.25f, 0.4f).Spawn();
            }
        }
        public override bool PreCheckFocus(Player player, int focusCost)
        {
            return false;
        }
        public override bool PrePayFocus(Player player, int focusCost)
        {
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                //AddIngredient<StormSaber>().
                //AddIngredient<WindBlade>().
                AddIngredient(ItemID.FragmentVortex, 6).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
