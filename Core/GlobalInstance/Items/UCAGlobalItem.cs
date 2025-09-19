using CalamityMod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using UCA.Core.Keybinds;

namespace UCA.Core.GlobalInstance.Items
{
    public partial class UCAGlobalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (true)
            {
                if (!Main.dedServ)
                {
                    string newKey = UCAKeybind.WeaponSkillHotKey.TooltipHotkeyString();
                    tooltips.FindAndReplace("[UCASkillKey]", newKey);
                }
            }
        }
    }
}
