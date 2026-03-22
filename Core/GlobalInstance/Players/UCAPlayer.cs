using LAP.Core.Utilities;
using Terraria;
using Terraria.ModLoader;
using UCA.Core.Utilities;

namespace UCA.Core.GlobalInstance.Players
{
    public partial class UCAPlayer : ModPlayer
    {
        public override void PostUpdate()
        {
            Reset_PostUpdate();
        }
    }
}
