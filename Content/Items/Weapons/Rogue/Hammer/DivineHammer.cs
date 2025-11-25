using CalamityMod;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;
using UCA.Common.Misc;
using UCA.Content.Projectiles.Misc;
using UCA.Content.Projectiles.Rogue.DivineProj;
using UCA.Core.BaseClass;
using UCA.Core.Utilities;

namespace UCA.Content.Items.Weapons.Rogue.Hammer
{
    public class DivineHammer: BaseHammerItem
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
        public override bool AltFunctionUse(Player player)
        {
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
