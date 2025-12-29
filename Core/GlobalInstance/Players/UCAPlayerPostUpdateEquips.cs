using CalamityMod;
using Terraria;
using Terraria.ModLoader;
using UCA.Content.Items.Weapons.Rogue.Hammer;
using UCA.Core.Utilities;

namespace UCA.Core.GlobalInstance.Players
{
    public partial class UCAPlayer : ModPlayer
    {
        public override void PostUpdateEquips()
        {
            if (BanChangeArmorsetStealth) 
                BanChangeArmorSetStealth();
            if (ShouldHandleHammerStealth)
                RefreshAllHammerStateOnNeed();
            Reset_PostUpdateEquips();
        }
        public void BanChangeArmorSetStealth()
        {
            if (Player.armor[0].type != _cacheHeadType || Player.armor[1].type != _cacheBodyType || Player.armor[2].type != _cacheLegsType)
            {
                Player.Calamity().rogueStealth = 0f;
                _cacheHeadType = Player.armor[0].type;
                _cacheBodyType = Player.armor[1].type;
                _cacheLegsType = Player.armor[2].type;
            }
        }
        #region 杂项效果
        // 因为原灾在ResetEffect里重置重置
        public void RefreshAllHammerStateOnNeed()
        {
            var calPlayer = Player.Calamity();
            //只有锤子才会重置潜伏条
            int heldType = Player.HeldItem.type;
            //无论什么情况下，切换至锤子时都强行重置一次潜伏条
            if (heldType != _cacheHammer)
            {
                Player.Calamity().rogueStealth = 0f;
                _cacheHammer = heldType;
                //切换后需要重置幽灵冲刺所有的属性，除了进程
                GhostDash_Time = 0;
                GhostDash_ChargeTime = 0;
            }
            //所有锤子启用潜伏条，潜伏值减半
            calPlayer.wearingRogueArmor = true;
            calPlayer.stealthStrikeHalfCost = true;
            //锤子常驻10潜伏值，这个效果不再有任何条件制约
            calPlayer.rogueStealthMax += ThrownHammerItem.BaseMaxStealth;
            BanChangeArmorsetStealth = true;
            Player.statLifeMax2 += (int)(calPlayer.rogueStealthMax * 100f);
            Player.lifeRegen += (int)(calPlayer.rogueStealthMax * 10f);
            //全体锤子无法生效任何形式的潜伏增伤
            //下方的代码从灾厄那复制
            Player.DisabelStealthDamageBoost();
        }
        #endregion
        public void Reset_PostUpdateEquips()
        {
            BanChangeArmorsetStealth = false;
        }
    }
}
