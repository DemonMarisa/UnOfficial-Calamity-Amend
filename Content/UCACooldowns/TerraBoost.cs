using CalamityMod.Cooldowns;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Localization;

namespace UCA.Content.UCACooldowns
{
    public class TerraBoost : CooldownHandler
    {
        public static new string ID => "UCATerraBoostCooldown";
        public override bool ShouldDisplay => true;
        public override LocalizedText DisplayName => Language.GetOrRegister($"Mods.UCA.Cooldowns.{ID}");
        public override string Texture => "UCA/Content/UCACooldowns/TerraBoost";
        public override string OutlineTexture => "UCA/Content/UCACooldowns/TerraBoost_OutLine";
        public override string OverlayTexture => "UCA/Content/UCACooldowns/TerraBoost_Overlay";
        public override Color OutlineColor => Color.Lerp(Color.ForestGreen, Color.LawnGreen, (float)Math.Sin(Main.GlobalTimeWrappedHourly) * 0.5f + 0.5f);
        public override Color CooldownStartColor => Color.ForestGreen;
        public override Color CooldownEndColor => Color.LawnGreen;
    }
}
