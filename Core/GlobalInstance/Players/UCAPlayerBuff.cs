using LAP.Core.Utilities;
using Terraria;
using Terraria.ModLoader;

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
                if (!NightShieldCanBlock)
                {
                    Player.LAP().ExternalDR -= 0.05f;
                    Player.statDefense -= 15;
                }
            }
        }
        #endregion
    }
}
