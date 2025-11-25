using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace UCA.Core.GlobalInstance.Players
{
    public partial class UCAPlayer : ModPlayer
    {
        public override void LoadData(TagCompound tag)
        {
            HammerTagLoad(tag);
        }
        public override void SaveData(TagCompound tag)
        {
            HammerTagSave(tag);
        }
        public override void Load()
        {
        }
        public override void Unload()
        {
        }
        public override void OnEnterWorld()
        {
        }
        //存储。
        private void HammerTagSave(TagCompound tag)
        {
            tag.Add(nameof(CanDisableGuideForGodsHammer), CanDisableGuideForGodsHammer);
            tag.Add(nameof(CanDisableGuideForGrandHammer), CanDisableGuideForGrandHammer);
            tag.Add(nameof(ShouldGiveSpareGodsHammer), ShouldGiveSpareGodsHammer);
        }

        private void HammerTagLoad(TagCompound tag)
        {
            CanDisableGuideForGodsHammer = tag.GetBool(nameof(CanDisableGuideForGodsHammer));
            CanDisableGuideForGrandHammer = tag.GetBool(nameof(CanDisableGuideForGrandHammer));
            ShouldGiveSpareGodsHammer = tag.GetBool(nameof(ShouldGiveSpareGodsHammer));
        }
    }
}
