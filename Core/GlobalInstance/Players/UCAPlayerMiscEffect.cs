using System;
using System.Threading;
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
            UpdateTimer();
        }

        
        public void Reset_PostUpdateMiscEffects()
        {
        }
    }
}
