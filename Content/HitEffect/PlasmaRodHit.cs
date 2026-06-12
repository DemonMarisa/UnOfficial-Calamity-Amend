using LAP.Core.Presets.Content;
using LAP.Core.StateMachine.SynedHitEffect;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace UCA.Content.HitEffect
{
    public class PlasmaRodHit : BaseHitEffect
    {
        public override void HitEffect(Entity entity, IEntitySource source, Player owner)
        {
            for (int i = 0; i < 35; i++)
            {
                Color RandomColor = Color.Lerp(Color.DarkViolet, Color.LightPink, Main.rand.NextFloat(0, 1));
                ParticlePreset.NewTGlowBall(entity.Center, Vector2.Zero, RandomColor, 120, 0.2f, Main.rand.NextFloat(2f, 2.2f));
            }
        }
    }
}
