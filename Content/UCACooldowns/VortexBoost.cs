using LAP.Core.LAPUI.CustomCD;
using LAP.Core.Presets.Content;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;

namespace UCA.Content.UCACooldowns
{
    public class VortexBoost : BaseCD
    {
        public override LocalizedText DisplayName()
        {
            return Language.GetOrRegister($"Mods.UCA.Cooldowns.UCAVortexBoostCooldown");
        }
        public override void Update(Player player)
        {
            player.LAP().DamageMult += 0.1f;
            if (player.miscCounter % 3 == 0)
            {
                Vector2 firepos = player.Center + new Vector2(Main.rand.Next(-25, 25), 0);
                ParticlePreset.NewLightning03(firepos, Vector2.Zero, Main.rand.NextBool() ? Color.PaleTurquoise : Color.Turquoise, 12, 0.2f, Main.rand.NextFloat(MathHelper.TwoPi));
            }
        }
    }
}
