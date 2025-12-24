using CalamityMod.Cooldowns;
using LAP.Content.Particles;
using LAP.Core.LAPUI.CustomCD;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using UCA.Content.Particiles;

namespace UCA.Content.UCACooldowns
{
    public class StarDustBoost : BaseCD
    {
        public override LocalizedText DisplayName()
        {
            return Language.GetOrRegister($"Mods.UCA.Cooldowns.UCAStarDustBoostCooldown");
        }
        public override void Update(Player player)
        {
            player.manaCost *= 0.5f;
            if (player.miscCounter % 5 == 0)
            {
                Vector2 GenPos = player.Center + -new Vector2(Main.rand.Next(-25, 25), Main.rand.Next(-35, 35));
                Color Firecolor = LAPUtilities.LerpColor(Color.SkyBlue, Color.DeepSkyBlue);
                new StarLine(GenPos, Main.rand.NextFloat(MathHelper.TwoPi), Firecolor, 25, 0.06f, 1f).Spawn();
            }
        }
    }
}
