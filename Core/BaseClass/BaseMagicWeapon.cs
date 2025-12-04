using CalamityMod;
using LAP.Core.Keybind;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace UCA.Core.BaseClass
{
    public abstract class BaseMagicWeapon : ModItem,ILocalizedModType
    {
        public new string LocalizationCategory => "MagicWeapons";
        public override void HoldItem(Player player)
        {
            // 只在本地调用
            if (player.whoAmI != Main.myPlayer)
                return;

            if (LAPKeybind.WeaponSkillHotKey.JustPressed && !Main.blockMouse)
            {
                if (Main.playerInventory)
                {
                    if (Main.mouseText)
                        return;
                }
                WeaponSkill(player);
            }

            UpdateHoldItem(player);
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.IntegrateHotkey(LAPKeybind.WeaponSkillHotKey);
        }
        public virtual void WeaponSkill(Player player)
        {

        }
        public virtual void UpdateHoldItem(Player player)
        {

        }
    }
}
