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
        public void Reset_PostUpdate()
        {
            ResetRay_PostUpdate();
            ResetHeal_PostUpdate();
        }
        //别出去跟别人说我整了个这个
        public void ResetHeal_PostUpdate()
        {
        }
        public void ResetRay_PostUpdate()
        {
            HeldNightShield = false;
            WeakHeldNightShield = false;
            if (TerraRestore)
            {
                Player.NCHeal(Player.statLifeMax2 / 4);
                TerraRestore = false;
            }
        }
    }
}
