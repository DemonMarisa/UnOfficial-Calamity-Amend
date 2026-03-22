using LAP.Core.StateMachine.SynedHitEffect;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using UCA.Content.MetaBalls;
using UCA.Content.Particiles;

namespace UCA.Content.HitEffect
{
    public class CarnageRayMeleeHit : BaseHitEffect
    {
        public override void HitEffect(Entity entity, IEntitySource source, Player Owner)
        {
            for (int i = 0; i < Main.rand.Next(5, 9); i++)
            {
                Vector2 SpawnPos = Owner.Center + new Vector2(Main.rand.Next(300, 500), 0).RotatedByRandom(MathHelper.TwoPi);

                for (int j = 0; j < 10; j++)
                {
                    new LilyLiquid(SpawnPos, Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0f, 1.2f) * -6f, Color.Red, 64, 0, 1, 1.5f).Spawn();
                }
                for (int x = 0; x < 5; x++)
                {
                    new LilyLiquid(SpawnPos, Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0f, 1.2f) * -6f, Color.Black, 64, 0, 1, 1.5f).Spawn();
                }
            }

            for (int i = 0; i < 15; i++)
            {
                Vector2 spawnVec = entity.velocity.RotateRandom(0.3f) * Main.rand.NextFloat(0.1f, 1.6f) * 24f;
                CarnageMetaBall.SpawnParticle(entity.Center, spawnVec, Main.rand.NextFloat(0.4f, 0.6f), 0, true);
            }
        }
    }
}
