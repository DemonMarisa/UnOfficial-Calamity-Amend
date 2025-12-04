using Terraria.ModLoader;

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
        public void Reset_PostUpdateMiscEffects()
        {
            ShouldHandleHammerStealth = false;
            StealthToMaxHPBonus = false;
        }
    }
}
