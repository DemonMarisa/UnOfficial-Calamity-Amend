using LAP.Core.StateMachine.SynedHitEffect;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using UCA.Content.Particiles;

namespace UCA.Content.HitEffect
{
    public class SoulPiercerSkillHit : BaseHitEffect
    {
        public override void HitEffect(Entity entity, IEntitySource source, Player owner)
        {
            for (int i = 0; i < 10; i++)
            {
                Color DrawColor;
                DrawColor = Color.Lerp(Color.Violet, Color.DarkViolet, Main.rand.NextFloat());
                new TrailGlowBall_T(entity.Center, DrawColor, Main.rand.Next(45, 90), 0.15f, Main.rand.NextFloat(2f, 4f), Main.rand.NextFloat(MathHelper.TwoPi), 1f).Spawn();
            }
            for (int i = 0; i < 20; i++)
            {
                Color Firecolor = LAPUtilities.LerpColor(Color.Violet, Color.Purple);
                new Fire(entity.Center, Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.3f, 1f) * 6, Firecolor, 64, Main.rand.NextFloat(MathHelper.TwoPi), 1f, 0.3f).Spawn();
            }
        }
    }
}
