using CalamityMod;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Magic;
using LAP.Core.Enums;
using LAP.Core.Keybind;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Common.Misc;
using UCA.Content.GUI;
using UCA.Content.Paths;
using UCA.Content.Projectiles.HeldProj.Magic.ElementRayHeld;
using UCA.Core.BaseClass;
using UCA.Core.Utilities;

namespace UCA.Content.Items.Weapons.Magic.Ray
{
    public class ElementRayAlt : BaseMagicWeapon
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 70;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 10;
            Item.width = 46;
            Item.height = 46;
            Item.useTime = 35;
            Item.useAnimation = 35;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3.25f;
            Item.value = UCAShopValue.RarityLightRedBuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<ElementRayHeldProj>();
            Item.shootSpeed = 6f;

            Item.noUseGraphic = true;
            Item.channel = true;

            Item.LAP().UseWeaponSkill = true;
            Item.LAP().DrawUCASmallIcon = true;

            Item.LAP().UseCICalStatInflation = true;
            Item.LAP().WeaponTier = AllWeaponTier.PostMoonLord;
            Item.LAP().UseCustomStatInflationMult = true;
            Item.LAP().UseCustomWeaponSkill = true;
            Item.LAP().StatInflationMult = 1f;
            Item.LAP().WeaponSkillManaCost = 200;
        }
        public override bool CanUseItem( Player player)
        {
            if (ElementalRayUI.Active)
                return false;
            return player.ownedProjectileCounts[ModContent.ProjectileType<ElementRayHeldProj>()] < 1;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                if (player.ownedProjectileCounts[ModContent.ProjectileType<ElementRaySpecialHeldProj>()] < 1)
                {
                    if (CheckAllMana(player))
                    {
                        if (player.LAP().NameIsMAGNOLIA)
                            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<ElementRaySpecialHeldProj>(), damage, knockback, player.whoAmI, player.UCA().ElementalRayStates, 1);
                        else
                            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<ElementRaySpecialHeldProj>(), damage, knockback, player.whoAmI, player.UCA().ElementalRayStates, 0);
                    }
                }
            }
            else
            {
                if (player.ownedProjectileCounts[ModContent.ProjectileType<ElementRayHeldProj>()] < 1)
                {
                    Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<ElementRayHeldProj>(), damage, knockback, player.whoAmI, 0, 0, player.UCA().ElementalRayStates);
                }
            }
            return false;
        }
        //我看你好像改了什么，反正记得这里自己同步同步就行了
        internal int SolarSkillManaCost = 190;
        internal int NeublaSkillManaCost = 190;
        internal int StardustSkillManaCost = 349;
        internal int VortexSkillManaCost = 190;
        internal int CombinedSkillManaCost = 40;
        public bool CheckAllMana(Player player)
        {
            if (player.UCA().ElementalRayStates == ElementalRayState.Misc)
            {
                if (player.CheckMana(player.ActiveItem(), Item.LAP().WeaponSkillRealManaCost, true, false))
                    return true;
            }
            if (player.UCA().ElementalRayStates == ElementalRayState.Solar)
            {
                if (player.CheckMana(player.ActiveItem(), Item.LAP().WeaponSkillRealManaCost, true, false))
                    return true;
            }
            if (player.UCA().ElementalRayStates == ElementalRayState.Nebula)
            {
                if (player.CheckMana(player.ActiveItem(), Item.LAP().WeaponSkillRealManaCost, true, false))
                    return true;
            }
            if (player.UCA().ElementalRayStates == ElementalRayState.StarDust)
            {
                if (player.CheckMana(player.ActiveItem(), Item.LAP().WeaponSkillRealManaCost, true, false))
                    return true;
            }
            if (player.UCA().ElementalRayStates == ElementalRayState.Vortex)
            {
                if (player.CheckMana(player.ActiveItem(), Item.LAP().WeaponSkillRealManaCost, true, false))
                    return true;
            }
            return false;
        }
        public override void WeaponSkill(Player player)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<ElementRayHeldProj>()] < 1 && player.ownedProjectileCounts[ModContent.ProjectileType<ElementRaySpecialHeldProj>()] < 1)
            {
                if (ElementalRayUI.Active)
                    ElementalRayUI.BeginFadeOut = true;
                else
                {
                    ElementalRayUI.BeginFadeOut = false;
                    ElementalRayUI.Active = true;
                }
                if (player.ownedProjectileCounts[ModContent.ProjectileType<ElementRaySkillHeldProj>()] > 0)
                    return;
                Projectile.NewProjectile(Item.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<ElementRaySkillHeldProj>(), 0, 0, player.whoAmI, player.UCA().ElementalRayStates);
            }
        }
        public override void UpdateHoldItem(Player player)
        {
            if (player.UCA().ElementalRayStates == ElementalRayState.Misc)
            {
                Item.LAP().WeaponSkillManaCost = 40;
                TextureAssets.Item[Type] = UCATextureRegister.ElementalRayMisc;
            }
            if (player.UCA().ElementalRayStates == ElementalRayState.Solar)
            {
                Item.LAP().WeaponSkillManaCost = 190;
                TextureAssets.Item[Type] = UCATextureRegister.ElementalRaySolor;
            }
            if (player.UCA().ElementalRayStates == ElementalRayState.Nebula)
            {
                Item.LAP().WeaponSkillManaCost = 190;
                TextureAssets.Item[Type] = UCATextureRegister.ElementalRayNebula;
            }
            if (player.UCA().ElementalRayStates == ElementalRayState.StarDust)
            {
                Item.LAP().WeaponSkillManaCost = 340;
                TextureAssets.Item[Type] = UCATextureRegister.ElementalRayStarDust;
            }
            if (player.UCA().ElementalRayStates == ElementalRayState.Vortex)
            {
                Item.LAP().WeaponSkillManaCost = 190;
                TextureAssets.Item[Type] = UCATextureRegister.ElementalRayVortex;
            }
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            LAPUtilities.IntegrateHotkey(tooltips, LAPKeybind.WeaponSkillHotKey);
            Player player = Main.LocalPlayer;
            string MiscTooltip = LocalizedPath.ElementalRayMiscTooltip;

            if (player.UCA().ElementalRayStates == ElementalRayState.Misc)
            {
                MiscTooltip = LocalizedPath.ElementalRayMiscTooltip;
                ReplaceTooltipToNeedState(tooltips, MiscTooltip, $"CalamityMod/{nameof(GalacticaSingularity)}");
            }

            if (player.UCA().ElementalRayStates == ElementalRayState.Solar)
            {
                MiscTooltip = LocalizedPath.ElementalRaySolorTooltip;
                ReplaceTooltipToNeedState(tooltips, MiscTooltip, ItemID.FragmentSolar.ToString());
            }

            if (player.UCA().ElementalRayStates == ElementalRayState.Nebula)
            {
                MiscTooltip = LocalizedPath.ElementalRayNebulaTooltip;
                ReplaceTooltipToNeedState(tooltips, MiscTooltip, ItemID.FragmentNebula.ToString());
            }

            if (player.UCA().ElementalRayStates == ElementalRayState.StarDust)
            {
                MiscTooltip = LocalizedPath.ElementalRayStarDustTooltip;
                ReplaceTooltipToNeedState(tooltips, MiscTooltip, ItemID.FragmentStardust.ToString());
            }

            if (player.UCA().ElementalRayStates == ElementalRayState.Vortex)
            {
                MiscTooltip = LocalizedPath.ElementalRayVortexrTooltip;
                ReplaceTooltipToNeedState(tooltips, MiscTooltip, ItemID.FragmentVortex.ToString());
            }
        }
        public static void ReplaceTooltipToNeedState(List<TooltipLine> tooltips, string miscTooltip, string signal)
        {
            //将Signal转化为需要进行插值的物品名
            string signalItem = $"[i:{signal}]";
            LAPUtilities.FindAndReplace(tooltips, "[UCAElementRayReplaceKey]", miscTooltip);
            LAPUtilities.FindAndReplace(tooltips, "[ItemIconKey]", signalItem);
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<TerraRay>().
                AddIngredient(ItemID.LunarBar, 5).
                AddIngredient<LifeAlloy>(5).
                AddIngredient<GalacticaSingularity>(5).
                AddTile(TileID.LunarCraftingStation).
                Register();

            CreateRecipe().
                AddIngredient<Photosynthesis>().
                AddIngredient(ItemID.LunarBar, 5).
                AddIngredient<LifeAlloy>(5).
                AddIngredient<GalacticaSingularity>(5).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
