using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace UCA.Core.Keybinds
{
    public class UCAKeybind : ModSystem
    {
        public static ModKeybind WeaponSkillHotKey { get; private set; }

        public override void Load()
        {
            // Register keybinds            
            WeaponSkillHotKey = KeybindLoader.RegisterKeybind(Mod, "WeaponSkill", "LeftAlt");
        }

        public override void Unload()
        {
            WeaponSkillHotKey = null;
        }
    }
}
