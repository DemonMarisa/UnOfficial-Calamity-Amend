using CalamityMod;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Common.Misc;
using UCA.Content.Projectiles.Rogue;
using UCA.Content.Projectiles.Rogue.PunishmentProj;
using UCA.Core.Utilities;

namespace UCA.Content.Items.Weapons.Rogue.Hammer
{
    public class PunishmentHammer: ThrownHammerItem
    {
        public override int ShootProjID => ModContent.ProjectileType<PunishmentHammerProj>();
        public override void ExSD()
        {
            Item.width = Item.height = 58;
            Item.damage = 50;
            Item.useTime = 13;
            Item.useAnimation = 13;
            Item.shootSpeed = 18f;
            Item.rare = ItemRarityID.LightRed;
            Item.value = UCAShopValue.RarityLightRedBuyPrice;
        }
        public override float StealthDamageMultipler => 0.25f;
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D tex = TextureAssets.Item[Type].Value;
            Vector2 position = Item.position - Main.screenPosition + tex.Size() / 2;
            Rectangle iFrame = tex.Frame();
            //为锤子添加描边，并时刻更新大小
            for (int i = 0; i < 16; i++)
                spriteBatch.Draw(tex, position + MathHelper.ToRadians(i * 60f).ToRotationVector2() * 2.4f, null, Color.White with { A = 0 }, 0f, tex.Size() / 2, scale, 0, 0f);
            //然后绘制锤子本身。
            spriteBatch.Draw(tex, position, iFrame, Color.White, 0f, tex.Size() / 2, scale, 0f, 0f);
            Lighting.AddLight(position, TorchID.UltraBright);
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Pwnhammer).
                AddIngredient(ItemID.SoulofNight, 10).
                AddIngredient(ItemID.DarkShard, 2).
                AddIngredient(ItemID.Diamond, 5).
                AddIngredient(ItemID.Amethyst, 5).
                AddTile(TileID.Anvils).
                Register();
        }
    }
    public abstract class ThrownHammerItem : ModItem, ILocalizedModType 
    {
        public new string LocalizationCategory => "Weapons.Rogue";
        public virtual int ShootProjID { get; }
        //锤类武器初始提供的潜伏值
        public const float BaseMaxStealth = 0.1f;
        public override bool WeaponPrefix() => true;
        //你灾组到现在都没让盗贼伤害成功不吃远程词缀的加成
        public override bool RangedPrefix() => false;
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ExSSD();
        }
        public virtual void ExSSD() {}
        public override void SetDefaults()
        {
            Item.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.shoot = ShootProjID;
            Item.knockBack = 18f;
            Item.LAP().DrawUCASmallIcon = true;
            Item.Calamity().devItem = true;
            ExSD();
        }
        
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftAlt))
                tooltips.ReplaceAllTooltip($"{Temp.UCALocalPrefix}Weapons.Rogue.Hammer_General");
            else
            {
                tooltips.QuickAddTooltip($"{Temp.UCALocalPrefix}HoldAltToShow", Color.Yellow, LineName: "Hammer_Special");
                ExModifyTooltips(tooltips);
            }
        }
        public virtual void ExModifyTooltips(List<TooltipLine> tooltips) {}
        public virtual void ExSD() { }
        public virtual float StealthDamageMultipler => 0.5f; 
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            bool stealth = player.Calamity().StealthStrikeAvailable();
            //锤子的潜伏固定1.5倍伤害
            damage = (int)(damage * (1 + StealthDamageMultipler * stealth.ToInt()));
            Projectile st = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
            st.Calamity().stealthStrike = stealth;
            return false;
        }
        public override void HoldItem(Player player)
        {
            var UCAPlayer = player.UCA();
            UCAPlayer.ShouldHandleHammerStealth = true;
            UCAPlayer.StealthToMaxHPBonus = true;
            //在Update内更新这一段
            int maxHP = (int)(player.Calamity().rogueStealthMax * 100f);
            player.statLifeMax2 += maxHP;
        }
    }
}
