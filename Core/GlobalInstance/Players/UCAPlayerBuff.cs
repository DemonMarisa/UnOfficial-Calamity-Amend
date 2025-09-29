using CalamityMod;
using CalamityMod.Items.Weapons.Magic;
using Humanizer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using UCA.Content.MetaBalls;
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

                if (Player.miscCounter % 6 == 0)
                {
                    Player.Heal(1);
                }
            }
        }

    }
}
