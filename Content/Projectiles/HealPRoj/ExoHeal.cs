using LAP.Core.BaseClass.Projectiles;
using LAP.Core.Presets.Content;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using UCA.Content.Particiles;

namespace UCA.Content.Projectiles.HealPRoj
{
    public class ExoHeal : BaseHealProj
    {
        public override int HealAmt => Main.rand.Next(20, 30);
        public override float FlySpeed => 12f;
        public override float Acceleration => 35f;
        public override void ExAI()
        {
            if (Projectile.LAP().FirstFrame)
            {
                for (int i = 0; i < 2; i++)
                {
                    Color RandomColor2 = Color.Lerp(Color.LightGreen, Color.GhostWhite, Main.rand.NextFloat(0, 1));
                    Vector2 vel = Vector2.UnitX.RotateRandom(MathHelper.TwoPi) * Main.rand.NextFloat(2f, 4f);
                    ParticlePreset.NewDustGlow(Projectile.Center, vel, 0, RandomColor2, 45, 0.1f, 0);
                }
            }
            for (int i = 0; i < 5; i++)
            {
                Color RandomColor2 = Color.Lerp(Color.LightGreen, Color.ForestGreen, Main.rand.NextFloat(0, 1));
                Vector2 vel = -Projectile.velocity / 5;
                ParticlePreset.NewTMGlowBall(Projectile.Center + vel * i, Vector2.Zero, RandomColor2, 15, 0.12f, Main.rand.NextFloat(0.1f, 0.3f));
            }
        }
        public override void ExKill()
        {
            Color RandomColor2 = Color.Lerp(Color.LightGreen, Color.GhostWhite, Main.rand.NextFloat(0, 1));
            new NoiseShockRing(Projectile.Center, Vector2.Zero, RandomColor2, 60, 1f, 0.3f, -1, Vector2.Zero).Spawn();
            for (int i = 0; i < 2; i++)
            {
                Vector2 vel = Vector2.UnitX.RotateRandom(MathHelper.TwoPi) * Main.rand.NextFloat(2f, 4f);
                ParticlePreset.NewDustGlow(Projectile.Center, vel, 0, RandomColor2, 45, 0.1f, 0);
            }
        }
    }
}
