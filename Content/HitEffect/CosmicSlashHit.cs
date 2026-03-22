using LAP.Core.StateMachine.SynedHitEffect;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using UCA.Content.MetaBalls;

namespace UCA.Content.HitEffect
{
    public class CosmicSlashHit : BaseHitEffect
    {
        public override void HitEffect(Entity entity, IEntitySource source, Player owner)
        {
            for (int i = 0; i < 10; i++)
                CosmicMetaBall.SpawnLozengeParticle(entity.Center, Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.3f, 1f) * 18, 0.4f, 60);
        }
    }
}
