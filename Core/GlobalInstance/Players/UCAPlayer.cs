using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using UCA.Content.Projectiles.HeldProj;

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
        public override void ResetEffects()
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

            ExternalDR = 0;
            ReSetCount();
            
        }

        public override void PostUpdateMiscEffects()
        {
            AddNightBoost();
        }

        public override void PostUpdate()
        {
            // 在最后一帧重置，这样就可以延迟一帧取到效果
            HeldNightShield = false;
            WeakHeldNightShield = false;
        }
    }
}
