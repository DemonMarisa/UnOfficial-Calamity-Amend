using CalamityMod.Cooldowns;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Localization;

namespace UCA.Content.UCACooldowns
{
    public class VortexBoost : CooldownHandler
    {
        public static new string ID => "UCAVortexBoostCooldown";
        public override bool ShouldDisplay => true;
        public override LocalizedText DisplayName => Language.GetOrRegister($"Mods.UCA.Cooldowns.{ID}");
        public override string Texture => "UCA/Content/UCACooldowns/VortexBoost";
        public override string OutlineTexture => "UCA/Content/UCACooldowns/VortexBoost_OutLine";
        public override string OverlayTexture => "UCA/Content/UCACooldowns/VortexBoost_Overlay";
        public override Color OutlineColor => Color.Lerp(Color.Turquoise, Color.DarkTurquoise, (float)Math.Sin(Main.GlobalTimeWrappedHourly) * 0.5f + 0.5f);
        public override Color CooldownStartColor => Color.Turquoise;
        public override Color CooldownEndColor => Color.DarkTurquoise;
    }
}
