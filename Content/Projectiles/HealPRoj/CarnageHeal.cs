using Microsoft.Xna.Framework;
using Terraria;
using UCA.Content.MetaBalls;
using UCA.Core.BaseClass;

namespace UCA.Content.Projectiles.HealPRoj
{
    public class CarnageHeal : BaseHealProj
    {
        public override void ExAI()
        {
            CarnageMetaBall.SpawnParticle(Projectile.Center, Projectile.rotation.ToRotationVector2(), 0.15f, Projectile.rotation, true);
        }

        public override void ExKill()
        {
            for (int i = 0; i < 15; i++)
            {
                Vector2 spawnVec = Projectile.velocity.RotateRandom(0.3f) * Main.rand.NextFloat(0.1f, 1.6f);
                CarnageMetaBall.SpawnParticle(Projectile.Center, spawnVec, Main.rand.NextFloat(0.15f, 0.2f), 0, true);
            }
        }
    }
}
