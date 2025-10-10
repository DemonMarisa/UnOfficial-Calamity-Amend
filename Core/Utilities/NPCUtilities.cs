using Terraria;
using UCA.Core.GlobalInstance.NPCs;
using UCA.Core.GlobalInstance.Players;

namespace UCA.Core.Utilities
{
    public static partial class UCAUtilities
    {
        public static UCAGlobalNPC UCA(this NPC npc)
        {
            return npc.GetGlobalNPC<UCAGlobalNPC>();
        }
    }
}
