using CalamityMod.Cooldowns;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Localization;

namespace UCA.Content.UCACooldowns
{
    public class StarDustBoost : CooldownHandler
    {
        public static new string ID => "UCAStarDustBoostCooldown";
        public override bool ShouldDisplay => true;
        public override LocalizedText DisplayName => Language.GetOrRegister($"Mods.UCA.Cooldowns.{ID}");
        public override string Texture => "UCA/Content/UCACooldowns/StarDustBoost";
        public override string OutlineTexture => "UCA/Content/UCACooldowns/StarDustBoost_OutLine";
        public override string OverlayTexture => "UCA/Content/UCACooldowns/StarDustBoost_Overlay";
        public override Color OutlineColor => Color.Lerp(Color.SkyBlue, Color.DeepSkyBlue, (float)Math.Sin(Main.GlobalTimeWrappedHourly) * 0.5f + 0.5f);
        public override Color CooldownStartColor => Color.SkyBlue;
        public override Color CooldownEndColor => Color.DeepSkyBlue;
    }
}
