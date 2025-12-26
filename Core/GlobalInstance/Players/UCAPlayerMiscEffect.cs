using Terraria;
using Terraria.ModLoader;

namespace UCA.Core.GlobalInstance.Players
{
    public partial class UCAPlayer : ModPlayer
    {
        public override void PostUpdateMiscEffects()
        {
            AddNightBoost();
            Reset_PostUpdateMiscEffects();
        }
        public void Reset_PostUpdateMiscEffects()
        {
            ShouldHandleHammerStealth = false;
            StealthToMaxHPBonus = false;
        }
    }
}
