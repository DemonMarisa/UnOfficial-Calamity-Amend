using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Core.GlobalInstance.Players;
using UCA.Core.NetCode;

namespace UCA
{
	// Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
	public class UCA : Mod
	{
        public static UCA Instance;
        /*
        MethodInfo preDraw2 = typeof(BrimstoneBarrageOld).GetMethod(nameof(BrimstoneBarrageOld.PreDraw));
        MonoModHooks.Add(preDraw2, PreDraw2_Hook);
		*/
        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            UCANetCode.HandleMouseWorldPacket(reader, whoAmI);
        }
        public override void Load()
        {
            Instance = this;
        }

        public override void Unload()
        {
            Instance = null;
        }
    }
}
