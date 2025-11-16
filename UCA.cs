using System.IO;
using Terraria.ModLoader;
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
        public Mod CalamityInheritance = null;
        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            UCANetCode.HandlePacket(reader, whoAmI);
        }
        public override void Load()
        {
            Instance = this;

            CalamityInheritance = null;
            ModLoader.TryGetMod("CalamityInheritance", out CalamityInheritance);
        }

        public override void Unload()
        {
            Instance = null;

            CalamityInheritance = null;
        }
    }
}
