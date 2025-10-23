using CalamityMod.Cooldowns;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Localization;

namespace UCA.Content.UCACooldowns
{
    public class NebulaBoost : CooldownHandler
    {
        public static new string ID => "UCANebulaBoostCooldown";
        public override bool ShouldDisplay => true;
        public override LocalizedText DisplayName => Language.GetOrRegister($"Mods.UCA.Cooldowns.{ID}");
        public override string Texture => "UCA/Content/UCACooldowns/NebulaBoost";
        public override string OutlineTexture => "UCA/Content/UCACooldowns/NebulaBoost_OutLine";
        public override string OverlayTexture => "UCA/Content/UCACooldowns/NebulaBoost_Overlay";
        public override Color OutlineColor => Color.Lerp(Color.Violet, Color.DarkViolet, (float)Math.Sin(Main.GlobalTimeWrappedHourly) * 0.5f + 0.5f);
        public override Color CooldownStartColor => Color.Violet;
        public override Color CooldownEndColor => Color.DarkViolet;
    }
}
