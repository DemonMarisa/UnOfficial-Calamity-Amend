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
        #region 
        //一个EModPlayer塞tm3000行我都不知道我的内容塞也太他妈绝望了
        //反正这里专门处理锤子的各种有的没的特殊情况
        //分别用到钩子都会有注释专门写明白
        public int _cacheHammer = -1;
        public bool _anyHammerStriking;
        public bool _anyHammerAttacking;
        // 用于检测是否手持锤子，在player更新完毕后会重置为false
        // 现在尽在MiscEffect与ResetEffect调用
        public bool ShouldHandleHammerStealth;
        public bool CanDisableGuideForGodsHammer;
        public bool CanDisableGuideForGrandHammer;
        public bool ShouldGiveSpareGodsHammer;
        //为禁用潜伏条增伤的武器提供生命上限转模
        public bool StealthToMaxHPBonus = false;
        #endregion
    }
}
