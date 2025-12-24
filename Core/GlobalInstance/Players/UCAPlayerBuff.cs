using CalamityMod;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using UCA.Content.MetaBalls;
using UCA.Content.Particiles;
using UCA.Content.UCACooldowns;

namespace UCA.Core.GlobalInstance.Players
{
    // Todo : 将下列所有移动到CD系统中
    public partial class UCAPlayer : ModPlayer
    {
        #region 永夜射线
        public void AddNightBoost()
        {
            // 张开护盾的免伤
            if (HeldNightShield)
            {
                Player.LAP().ExternalDR += 0.1f;
                Player.statDefense += 30;
                if (WeakHeldNightShield)
                {
                    Player.LAP().ExternalDR -= 0.05f;
                    Player.statDefense -= 15;
                }
            }
        }
        #endregion
    }
}
