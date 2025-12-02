using CalamityMod;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;
using UCA.Common.Misc;
using UCA.Content.Projectiles.Misc;
using UCA.Content.Projectiles.Rogue.DivineProj;
using UCA.Core.Utilities;

namespace UCA.Content.Items.Weapons.Rogue.Hammer
{
    public class DivineHammer: ThrownHammerItem
    {
        public override int ShootProjID => ModContent.ProjectileType<DivineHammerProj>();
        public override void ExSSD()
        {
        }
        public override void ExSD()
        {
            Item.width = Item.height = 86;
            Item.damage = 420;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.shootSpeed = 20f;
            Item.rare = ModContent.RarityType<DarkBlue>();
            Item.value = UCAShopValue.RarityDarkBlueBuyPrice;
            Item.consumable = false;
        }
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
            return false;
        }
        public override bool AltFunctionUse(Player player)
        {
            return false;
            bool canAltFunction = !player.UCA().CanDisableGuideForGrandHammer && DownedBossSystem.downedExoMechs && DownedBossSystem.downedCalamitas;
            return true;
            return canAltFunction;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //正常情况下， 他应该只会执行一次…… 
            if (player.altFunctionUse == 2)
            {
                Projectile.NewProjectile(source, position, new Vector2(0f, -28f), ModContent.ProjectileType<DivineHammerFlyingUpProj>(), 0, 0f,player.whoAmI);
                return false;
            }
            else
            {
            Projectile st = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
                st.Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
            }
            return false;
        }
        public override bool ConsumeItem(Player player)
        {
            return false;
            bool ifCanConsume = DownedBossSystem.downedExoMechs && DownedBossSystem.downedCalamitas;
            if (!player.UCA().CanDisableGuideForGrandHammer && ifCanConsume && player.controlUseTile)
                return true;

            return false;
        }
    }
    public class GodsHammerShimmerIL : ModSystem
    {
        public override void OnModLoad()
        {
            On_ShimmerTransforms.IsItemTransformLocked += ShimmerRequirementHandler;
        }
        public static bool ShimmerRequirementHandler(On_ShimmerTransforms.orig_IsItemTransformLocked orig, int type)
        {
            if (type == ModContent.ItemType<NightmareHammer>())
                return !DownedBossSystem.downedDoG;
            return orig(type);
        }
    }
    //internal class GodsHammerCanConsume: ModPlayer
    //{
    //    public bool isToggle = false;
    //    public bool isPressed = false;
    //    public override void PostUpdate()
    //    {
    //        //终极史山
    //        if(Main.mouseMiddle && Player.HeldItem.type == ModContent.ItemType<TlipocasScythe>())
    //        {
    //            if (Main.mouseMiddleRelease)
    //            {
    //                if (isPressed)
    //                {
    //                    isPressed = false;
    //                    Player.Center.CirclrDust(36, 1.26f, DustID.GemRuby, 6, 3f);
    //                    SoundEngine.PlaySound(SoundID.Item103 with { MaxInstances = 4, Pitch = 0.4f });
    //                }
    //                else
    //                {
    //                    isPressed = true;
    //                    Player.Center.CirclrDust(36, 1.26f, DustID.GemRuby, -6, 3f);
    //                    SoundEngine.PlaySound(SoundID.Item103 with { MaxInstances = 4, Pitch = 0.4f });
    //                }

    //            }
    //        }
    //    }
    //}
}
