using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using UCA.Content.Items.Weapons.Magic.Ray;
using UCA.Content.Projectiles.HeldProj;
using UCA.Content.Projectiles.HeldProj.Magic.ElementRayHeld;

namespace UCA.Core.GlobalInstance.Players
{
    public partial class UCAPlayer : ModPlayer
    {
        // 外围的玩家伤害减免
        public float ExternalDR = 0;
        public bool HeldNightShield = false;
        public bool WeakHeldNightShield = false;
        public static int NightShieldMaxHP = 400;
        public int NightShieldHP = 400;
        public bool NightShieldCanDefense = true;

        public static int MaxTerraRayRestore = 3;
        public int TerraRayRestore = 3;
        public bool TerraRestore = false;
        public static int TerraRayChargeCD = 2700;
        public int TerraRayCharge = 0;
        public int TerraRayHealCD = 0;
        public int TerraRayUseSkillCount = 0;

        public int ElementalRayStates = ElementalRayState.Misc;
        public override void ResetEffects()
        {
            ExternalDR = 0;

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

            UpdateTerraRayCD();
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
        public override void PostUpdateEquips()
        {
        }
        public override void PostUpdateMiscEffects()
        {
            AddNightBoost();
            AddCarnageBoost();
            AddTerraBoost();
        }

        public override void PostUpdate()
        {
            // 在最后一帧重置，这样就可以延迟一帧取到效果
            HeldNightShield = false;
            WeakHeldNightShield = false;
            if (TerraRestore)
            {
                Player.Heal(Player.statLifeMax2 / 4);
                TerraRestore = false;
            }
            UpdateMouseWorld();
        }
    }
}
