using CalamityMod;
using LAP.Core.GlobalInstance.Players;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using UCA.Core.BaseClass;
using UCA.Core.List;
using UCA.Core.Utilities;

namespace UCA.Core.GlobalInstance.Players
{
    public partial class UCAPlayer : ModPlayer
    {
        public override void PostUpdateMiscEffects()
        {
            AddNightBoost();
            AddCarnageBoost();
            AddTerraBoost();
            AddElementalBoost();
            AddShadowBoltStaffBoost();

            Reset_PostUpdateMiscEffects();
        }
    }
}
