using CalamityMod.Cooldowns;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Localization;

namespace UCA.Content.UCACooldowns
{
    public class MiscBoost : CooldownHandler
    {
        public static new string ID => "UCAMiscBoostCooldown";
        public override bool ShouldDisplay => true;
        public override LocalizedText DisplayName => Language.GetOrRegister($"Mods.UCA.Cooldowns.{ID}");
        public override string Texture => "UCA/Content/UCACooldowns/MiscBoost";
        public override string OutlineTexture => "UCA/Content/UCACooldowns/MiscBoost_OutLine";
        public override string OverlayTexture => "UCA/Content/UCACooldowns/MiscBoost_Overlay";
        public override Color OutlineColor => Color.Lerp(Color.White, Color.WhiteSmoke, (float)Math.Sin(Main.GlobalTimeWrappedHourly) * 0.5f + 0.5f);
        public override Color CooldownStartColor => Color.White;
        public override Color CooldownEndColor => Color.WhiteSmoke;
    }
}
