using CalamityMod;
using Terraria.ModLoader;

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
        public override void PostUpdate()
        {
            Reset_PostUpdate();
        }
        //为了这个潜伏砖模我单独开了一个方法，你自己去看要放哪吧
        public override void UpdateEquips()
        {
            base.UpdateEquips();
        }
    }
}
