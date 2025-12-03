using CalamityMod;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Content.Projectiles.Rogue;
using UCA.Content.Projectiles.Rogue.ThunderProj;
using UCA.Content.Projectiles.Rogue.ThunderProj.RightHandHammer;
using UCA.Core.Utilities;

namespace UCA.Content.Items.Weapons.Rogue.Hammer
{
    public class ThunderHammer : ModItem, ILocalizedModType
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            //这里维持return false，因为大锤子没有制作完成
            return false;
        }
        public new string LocalizationCategory => "Weapons.Rogue";
        //先做掉他
        public override void SetDefaults()
        {
            Item.width = Item.height = 132;
            Item.damage = 4000;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.shootSpeed = 24f;
            Item.knockBack = 10f;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.value = Item.buyPrice(platinum: 12);
            Item.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<ThunderHammerProj>();
            Item.knockBack = 18f;

        }
        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if(line.Name == "ItemName" && line.Mod == "Terraria")
            {
                line.CustomTooltipDraw(ref yOffset, Color.Blue, Color.White);
                return false;
            }
            return base.PreDrawTooltipLine(line, ref yOffset);
        }
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D tex = TextureAssets.Item[Type].Value;
            Vector2 position = Item.position - Main.screenPosition + tex.Size() / 2;
            Rectangle iFrame = tex.Frame();
            //为锤子添加描边，并时刻更新大小
            for (int i = 0; i < 16; i++)
                spriteBatch.Draw(tex, position + MathHelper.ToRadians(i * 60f).ToRotationVector2() * 2.4f, null, Color.Pink with { A = 0 }, 0f, tex.Size() / 2, scale, 0, 0f);
            //然后绘制锤子本身。
            spriteBatch.Draw(tex, position, iFrame, Color.White, 0f, tex.Size() / 2, scale, 0f, 0f);
            Lighting.AddLight(position, TorchID.UltraBright);
            return false;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            bool canStealth = player.Calamity().StealthStrikeAvailable();
            int thunderHammer = ModContent.ProjectileType<ThunderHandler>();
            bool ownerThunder = player.ownedProjectileCounts[thunderHammer] > 0;
            ////允许启用潜伏条的情况下，让玩家强行向上掷出一个幻影锤
            //if (canStealth && !ownerThunder)
            //{
            //    Projectile.NewProjectileDirect(source, position, new Vector2(0f, -15f), thunderHammer, damage, knockback);
            //    //投掷出去后清空玩家身上所有潜伏值
            //    player.Calamity().rogueStealth = 0;
            //}

            ////其余情况下正常攻击，我们才让其释放允许挂载的巨型锤
            //if (ownerThunder)
            //{
                Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback);
                proj.Calamity().stealthStrike = canStealth;
            //}
            //杀死其余攻击模组
            return false;
        }
    }
}