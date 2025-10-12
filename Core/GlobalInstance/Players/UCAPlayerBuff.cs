using CalamityMod;
using CalamityMod.Items.Weapons.Magic;
using Humanizer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using UCA.Content.MetaBalls;
using UCA.Content.Particiles;
using UCA.Content.UCACooldowns;

namespace UCA.Core.GlobalInstance.Players
{
    public partial class UCAPlayer : ModPlayer
    {
        public void AddNightBoost()
        {
            if (Player.HasCooldown(NightBoost.ID))
            {
                Player.GetDamage<GenericDamageClass>() += 0.15f;

                Player.manaCost *= 0.95f;
                ExternalDR += 0.05f;
                Player.statDefense += 10;
                if (Player.miscCounter % 2 == 0)
                    ShadowMetaBall.SpawnParticle(Player.Center - new Vector2(Main.rand.Next(-25, 25), -Player.height / 2), Vector2.UnitY * Main.rand.NextFloat(2, 6f) * -1f, Main.rand.NextFloat(0.1f, 0.15f));
            }

            if (HeldNightShield)
            {
                ExternalDR += 0.1f;
                Player.statDefense += 30;
                if (WeakHeldNightShield)
                {
                    ExternalDR -= 0.05f;
                    Player.statDefense -= 15;
                }
            }
        }

        public void AddCarnageBoost()
        {
            if (Player.HasCooldown(CarnageBoost.ID))
            {
                Player.GetCritChance<GenericDamageClass>() += 15;
                Player.manaCost *= 0.95f;

                if (Player.miscCounter % 2 == 0)
                    CarnageMetaBall.SpawnParticle(Player.Center - new Vector2(Main.rand.Next(-25, 25), 0),
                        Vector2.UnitY * Main.rand.NextFloat(2, 6f) * -1f,
                        Main.rand.NextFloat(0.3f, 0.5f), MathHelper.PiOver2);

                if (Player.miscCounter % 9 == 0)
                {
                    Player.Heal(1);
                }
            }
        }

        public void AddTerraBoost()
        {
            if (Player.HasCooldown(TerraBoost.ID))
            {
                Player.manaCost *= 0.8f;
                ExternalDR += 0.1f;
                Player.endurance += 0.15f;
                Player.statDefense += 30;
                if (Player.miscCounter % 5 == 0)
                {
                    Vector2 pos = Player.Center - new Vector2(Main.rand.Next(-50, 50), -Player.height / 2);
                    Color RandomColor = Color.Lerp(Color.LightGreen, Color.ForestGreen, Main.rand.NextFloat(0, 1));
                    new Butterfly(pos, Vector2.Zero, RandomColor, 120, 0, 1, 0.2f, Main.rand.NextFloat(0.2f, 1f)).Spawn();

                    Color RandomColor2 = Color.Lerp(Color.Pink, Color.Green, Main.rand.NextFloat(0, 1));
                    new Petal(pos, -Vector2.UnitY, RandomColor2, 360, 0, 1, 0.1f, Main.rand.NextFloat(0.5f, 0.7f)).Spawn();
                }

                if (Player.miscCounter % 6 == 0)
                {
                    Player.Heal(1);
                }
            }
        }
    }
}
