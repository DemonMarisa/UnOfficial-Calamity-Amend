using CalamityMod.Cooldowns;
using LAP.Content.Particles;
using LAP.Core.LAPUI.CustomCD;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Localization;
using UCA.Content.Particiles;

namespace UCA.Content.UCACooldowns
{
    public class TerraBoost : BaseCD
    {
        public override LocalizedText DisplayName()
        {
            return Language.GetOrRegister($"Mods.UCA.Cooldowns.UCATerraBoostCooldown");
        }
        public override void Update(Player player)
        {
            player.manaCost *= 0.8f;
            player.LAP().ExternalDR += 0.1f;
            player.endurance += 0.15f;
            player.statDefense += 30;
            if (player.miscCounter % 5 == 0)
            {
                Vector2 pos = player.Center - new Vector2(Main.rand.Next(-50, 50), -player.height / 2);
                Color RandomColor = Color.Lerp(Color.LightGreen, Color.ForestGreen, Main.rand.NextFloat(0, 1));
                new Butterfly(pos, Vector2.Zero, RandomColor, 120, 0, 1, 0.2f, Main.rand.NextFloat(0.2f, 1f)).Spawn();

                Color RandomColor2 = Color.Lerp(Color.Pink, Color.Green, Main.rand.NextFloat(0, 1));
                new Petal(pos, -Vector2.UnitY, RandomColor2, 360, 0, 1, 0.1f, Main.rand.NextFloat(0.5f, 0.7f)).Spawn();
            }
            if (player.miscCounter % 6 == 0)
            {
                player.Heal(1);
            }
        }
    }
}
