using LAP.Core.LAPUI.CustomCD;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using UCA.Content.Particiles.Lightnings;

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
                new Lightning03(firepos, Vector2.Zero, Main.rand.NextBool() ? Color.PaleTurquoise : Color.Turquoise, 12, Main.rand.NextFloat(MathHelper.TwoPi), 0.2f).Spawn();
            }
        }
    }
}
