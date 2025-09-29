using CalamityMod.Cooldowns;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Localization;

namespace UCA.Content.UCACooldowns
{
    public class CarnageBoost : CooldownHandler
    {
        public static new string ID => "UCACarnageBoostCooldown";
        public override bool ShouldDisplay => true;
        public override LocalizedText DisplayName => Language.GetOrRegister($"Mods.UCA.Cooldowns.{ID}");
        public override string Texture => "UCA/Content/UCACooldowns/CarnageBoost";
        public override string OutlineTexture => "UCA/Content/UCACooldowns/CarnageBoost_OutLine";
        public override string OverlayTexture => "UCA/Content/UCACooldowns/CarnageBoost_Overlay";
        public override Color OutlineColor => Color.Lerp(Color.Red, Color.DarkRed, (float)Math.Sin(Main.GlobalTimeWrappedHourly) * 0.5f + 0.5f);
        public override Color CooldownStartColor => Color.Red;
        public override Color CooldownEndColor => Color.DarkRed;
    }
}
