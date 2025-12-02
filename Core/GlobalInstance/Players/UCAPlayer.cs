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
    }
}
