using CalamityMod.Cooldowns;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Localization;

namespace UCA.Content.UCACooldowns
{
    public class SolorShield : CooldownHandler
    {
        public static new string ID => "UCASolorShieldCooldown";
        public override bool ShouldDisplay => true;
        public override LocalizedText DisplayName => Language.GetOrRegister($"Mods.UCA.Cooldowns.{ID}");
        public override string Texture => "UCA/Content/UCACooldowns/SolorShield";
        public override string OutlineTexture => "UCA/Content/UCACooldowns/SolorShield_OutLine";
        public override string OverlayTexture => "UCA/Content/UCACooldowns/SolortShield_Overlay";
        public override Color OutlineColor => Color.Lerp(Color.OrangeRed, Color.Orange, (float)Math.Sin(Main.GlobalTimeWrappedHourly) * 0.5f + 0.5f);
        public override Color CooldownStartColor => Color.Orange;
        public override Color CooldownEndColor => Color.OrangeRed;
    }
}
