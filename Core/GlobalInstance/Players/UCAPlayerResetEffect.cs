using Terraria;
using Terraria.ModLoader;
using UCA.Core.Utilities;

namespace UCA.Core.GlobalInstance.Players
{
    public partial class UCAPlayer : ModPlayer
    {
        #region ResetEffects
        public override void ResetEffects()
        {
            ResetRay();
            RogueHammerReset(); 
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
        #region 重置锤子
        // 大幅简化，相关判定直接移动到物品中
        //为禁用潜伏条增伤的武器提供生命上限转模
        public bool StealthToMaxHPBonus = false;
        public void RogueHammerReset()
        {
            _anyHammerAttacking = false;
            
        }
        #endregion
        #endregion
        #region ResetPostUpdateMiscEffects
        // 在PostUpdateMiscEffects的最后调用
        // 在这里写是因为HoldItem的调用在MiscEffect后，PostUpdate前，为了保证HoldItem中更新的字段可以正常取到，所以在这里重置
        public void Reset_PostUpdateMiscEffects()
        {
            ShouldHandleHammerStealth = false;
            StealthToMaxHPBonus = false;
        }
        public void Reset_PostUpdateEquips()
        {
            BanChangeArmorsetStealth = false;
        }
        #endregion
        #region PostUpdate
        // 在PostUpdate的最后调用
        public int HealAmt = 0;
        public void Reset_PostUpdate()
        {
            ResetRay_PostUpdate();
            ResetHeal_PostUpdate(); 
        }
            //别出去跟别人说我整了个这个
        public void ResetHeal_PostUpdate()
        {
            if (HealAmt > 0)
            {
                Player.HealDirect(HealAmt);
                HealAmt = 0;
            }
        }
        public void ResetRay_PostUpdate()
        {
            HeldNightShield = false;
            WeakHeldNightShield = false;
            if (TerraRestore)
            {
                Player.Heal(Player.statLifeMax2 / 4);
                TerraRestore = false;
            }
        }
        #endregion
    }
}
