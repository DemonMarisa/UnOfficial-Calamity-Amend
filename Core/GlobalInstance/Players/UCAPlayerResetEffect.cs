using Terraria.ModLoader;

namespace UCA.Core.GlobalInstance.Players
{
    public partial class UCAPlayer : ModPlayer
    {
        #region ResetEffects
        public override void ResetEffects()
        {
            ResetRay();
        }
        #region 射线
        public void ResetRay()
        {
            UpdateNightRayCD();
            UpdateTerraRayCD();
            // 控制灵魂巨剑的免伤
            if (SoulPiercerSGSUse > 0)
                SoulPiercerSGSUse--;
        }
        public void UpdateNightRayCD()
        {
            if (NightShieldHP > NightShieldMaxHP)
                NightShieldHP = NightShieldMaxHP;

            if (NightShieldHP < 0)
                NightShieldHP = 0;

            // 充满后才会激活护盾
            if (NightShieldHP >= NightShieldMaxHP)
                NightShieldCanDefense = true;

            // 如果护盾归零则失效，必须充满才可以抵挡伤害
            if (NightShieldHP <= 0)
                NightShieldCanDefense = false;
        }
        public void UpdateTerraRayCD()
        {
            // 存储与更新回血次数限制
            if (TerraRayRestore > MaxTerraRayRestore)
                TerraRayRestore = MaxTerraRayRestore;
            if (TerraRayRestore < 0)
                TerraRayRestore = 0;
            if (TerraRayRestore < MaxTerraRayRestore && TerraRayCharge < TerraRayChargeCD)
            {
                TerraRayCharge++;
            }
            if (TerraRayCharge >= TerraRayChargeCD)
            {
                TerraRayCharge = 0;
                TerraRayRestore++;
            }
            // 吸血CD
            if (TerraRayHealCD > 0)
                TerraRayHealCD--;
            // 控制免伤
            if (TerraRayUseSkillCount > 0)
                TerraRayUseSkillCount--;
        }
        #endregion
        #endregion
    }
}
