using LAP.Core.StateMachine.SynedHitEffect;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using UCA.Content.Particiles;

namespace UCA.Content.HitEffect
{
    public class PlasmaRodHit : BaseHitEffect
    {
        public override void HitEffect(Entity entity, IEntitySource source, Player owner)
        {
            for (int i = 0; i < 35; i++)
            {
                float offset = MathHelper.TwoPi / 35;
                Color RandomColor = Color.Lerp(Color.DarkViolet, Color.LightPink, Main.rand.NextFloat(0, 1));
                new MediumGlowBall(entity.Center, entity.velocity.RotatedBy(offset * i), RandomColor, 60, 0, 1, 0.2f, Main.rand.NextFloat(2f, 2.2f)).Spawn();
            }
        }
    }
}
