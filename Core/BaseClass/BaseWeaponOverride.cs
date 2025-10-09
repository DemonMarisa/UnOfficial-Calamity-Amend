using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using UCA.Core.Keybinds;

namespace UCA.Core.BaseClass
{
    public abstract class BaseWeaponOverride : GlobalItem
    {
        public virtual int OverrideType { get; set; }
        // 用于与大修的适配，防止同时发射两个弹幕
        public bool CalOverHaulCanShoot = true;
        public override bool AppliesToEntity(Item item, bool lateInstatiation)
        {
            return item.type == OverrideType;
        }

        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (!CalOverHaulCanShoot)
                return false;

            CustomShoot(item, player, source, position, velocity, type, damage, knockback);

            CalOverHaulCanShoot = false;
            return false;
        }

        public virtual void CustomShoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

        }

        public override void HoldItem(Item item, Player player)
        {
            // 只在本地调用
            if (player.whoAmI != Main.myPlayer)
                return;

            if (UCAKeybind.WeaponSkillHotKey.JustPressed && !Main.blockMouse && !Main.mouseText)
                WeaponSkill(item, player);

            UpdateHoldItem(item, player);

            CalOverHaulCanShoot = true;
        }

        public virtual void WeaponSkill(Item item, Player player)
        {

        }

        public virtual void UpdateHoldItem(Item item, Player player)
        {

        }
    }
}
