using System.Collections.Generic;
using CalamityMod;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Content.Projectiles.Rogue;
using UCA.Core.Utilities;

namespace UCA.Core.BaseClass
{
    public abstract class BaseHammerItem : ModItem, ILocalizedModType 
    {
        public new string LocalizationCategory => "Weapons.Rogue";
        public virtual int ShootProjID { get; }
        //锤类武器初始提供的潜伏值
        public const float BaseMaxStealth = 0.1f;
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
            ExSD();
        }
        //移除潜伏倍率加成，因为我删除了所有锤子的增伤
        //别他妈动我的锤子系列！！！！
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            ExModifyTooltips(tooltips);
            string path = $"{Temp.UCALocalPrefix}Weapons.Rogue.Hammer_General";
            tooltips.QuickAddTooltip(path, Color.Yellow);
        }
        public virtual void ExModifyTooltips(List<TooltipLine> tooltips) {}
        public virtual void ExSD() { }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            bool stealth = player.Calamity().StealthStrikeAvailable();
            damage = (int)(damage * (1 + 0.5f * stealth.ToInt()));
            Projectile st = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
            st.Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
            return false;
        }
        public override void HoldItem(Player player)
        {
            var UCAPlayer = player.UCA();
            UCAPlayer.ShouldHandleHammerStealth = true;
        }
    }
}
