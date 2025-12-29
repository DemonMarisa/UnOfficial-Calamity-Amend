using Terraria.ModLoader;
using UCA.Content.Projectiles.HeldProj.Magic.ElementRayHeld;

namespace UCA.Core.GlobalInstance.Players
{
    public partial class UCAPlayer : ModPlayer
    { 
        #region 射线杖
        #region 永夜射线
        public bool HeldNightShield = false;
        public bool WeakHeldNightShield = false;
        public static int NightShieldMaxHP = 400;
        public int NightShieldHP = 400;
        public bool NightShieldCanDefense = true;
        #endregion
        #region 泰拉射线
        public static int MaxTerraRayRestore = 3;
        public int TerraRayRestore = 3;
        public bool TerraRestore = false;
        public static int TerraRayChargeCD = 2700;
        public int TerraRayCharge = 0;
        public int TerraRayHealCD = 0;
        public int TerraRayUseSkillCount = 0;
        #endregion
        // 元素
        public int ElementalRayStates = ElementalRayState.Misc;
        // 影流射线
        public static int MaxShadowPlayer = 12;
        // 灵魂穿透者
        public int SoulPiercerSGSUse = 0;
        #endregion
    }
}
