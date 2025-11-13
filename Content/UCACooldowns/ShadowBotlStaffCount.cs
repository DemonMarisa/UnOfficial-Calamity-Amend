using CalamityMod.Cooldowns;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Localization;

namespace UCA.Content.UCACooldowns
{
    public class ShadowBotlStaffCount : CooldownHandler
    {
        public static new string ID => "UCAShadowBotlStaffCountCooldown";
        public override bool ShouldDisplay => true;
        public override LocalizedText DisplayName => Language.GetOrRegister($"Mods.UCA.Cooldowns.{ID}");
        public override string Texture => "UCA/Content/UCACooldowns/ShadowBotlStaffCount";
        public override string OutlineTexture => "UCA/Content/UCACooldowns/ShadowBotlStaffCount_OutLine";
        public override string OverlayTexture => "UCA/Content/UCACooldowns/ShadowBotlStaffCount_Overlay";
        public override Color OutlineColor => Color.Lerp(new Color(148, 0, 211), new Color(141, 112, 219), (float)Math.Sin(Main.GlobalTimeWrappedHourly) * 0.5f + 0.5f);
        public override Color CooldownStartColor => Color.Purple;
        public override Color CooldownEndColor => Color.DarkViolet;
    }
}
