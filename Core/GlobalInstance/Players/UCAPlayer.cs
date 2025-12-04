using CalamityMod;
using Terraria.ModLoader;
using UCA.Core.Utilities;

namespace UCA.Core.GlobalInstance.Players
{
    public partial class UCAPlayer : ModPlayer
    {
        // 是否ban掉切装贼
        // 在最后一帧重置这个字段，确保任何地方调用都可以生效
        public bool BanChangeArmorsetStealth = false;
        public int _cacheHeadType = -1;
        public int _cacheBodyType = -1;
        public int _cacheLegsType = -1;
        // 储存应该恢复多少
        public int HealAmt = 0;
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
    }
}
