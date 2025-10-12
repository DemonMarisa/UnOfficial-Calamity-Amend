using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using UCA.Core.Keybinds;

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

            if (UCAKeybind.WeaponSkillHotKey.JustPressed && !Main.blockMouse && !Main.mouseText)
                WeaponSkill(player);

            UpdateHoldItem(player);
        }

        public virtual void WeaponSkill(Player player)
        {

        }

        public virtual void UpdateHoldItem(Player player)
        {

        }
    }
}
