using LAP.Core.BaseClass;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using UCA.Content.MetaBalls;

namespace UCA.Content.Projectiles.HealPRoj
{
    public class CosmicHeal : BaseHealProj
    {
        public override int HealAmt => Main.rand.Next(8, 15);
        public override void ExAI()
        {
            CosmicMetaBall.SpawnCircleParticle(Projectile.Center, Vector2.Zero, 0.1f, 120);
        }

        public override void ExKill()
        {
            for (int i = 0; i < 10; i++)
            {
                Vector2 spawnVec = Vector2.UnitX.RotateRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.2f, 0.3f) * 6;
                CosmicMetaBall.SpawnCircleParticle(Projectile.Center, spawnVec, 0.2f, 45);
            }
        }
    }
}
