using LAP.Content.Particles.CalParticiles;
using LAP.Core.StateMachine.SynedHitEffect;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using UCA.Assets.Sounds;
using UCA.Content.Particiles;

namespace UCA.Content.HitEffect
{                       
    public class NightRayShieldHit : BaseHitEffect
    {
        public override void HitEffect(Entity entity, IEntitySource source, Player owner)
        {
            float bloomScaleFactor = Main.rand.NextFloat(0.6f, 0.95f) * 0.4f;
            for (int i = 0; i < 3; i++)
            {
                new StrongBloom(entity.Center, Vector2.Zero, Color.DeepPink, bloomScaleFactor * 0.56f, 9).Spawn();
                new StrongBloom(entity.Center, Vector2.Zero, Color.MediumPurple * 0.6f, bloomScaleFactor * 0.95f, 12).Spawn();
                new StrongBloom(entity.Center, Vector2.Zero, Color.White * 0.35f, bloomScaleFactor * 1.5f, 14).Spawn();
            }
            Vector2 TangentVector = entity.velocity.RotatedBy(MathHelper.PiOver2);
            for (int i = 0; i < 10; i++)
            {
                Color color = Color.Lerp(Color.LightPink, Color.DarkViolet, Main.rand.NextFloat(0, 1f));
                new GlowBall(entity.Center, TangentVector.RotatedByRandom(0.1f) * Main.rand.NextFloat(0.1f, 0.3f) * 0.35f, color, Main.rand.Next(30, 60), 0, 1, 0.1f).Spawn();
            }
            for (int i = 0; i < 10; i++)
            {
                Color color = Color.Lerp(Color.LightPink, Color.DarkViolet, Main.rand.NextFloat(0, 1f));
                new GlowBall(entity.Center, -TangentVector.RotatedByRandom(0.1f) * Main.rand.NextFloat(0.1f, 0.3f) * 0.35f, color, Main.rand.Next(30, 60), 0, 1, 0.1f).Spawn();
            }

            SoundEngine.PlaySound(SoundsMenu.NightShieldHit, entity.Center);
        }
    }
    public class NightRayShieldBreakHit : BaseHitEffect
    {
        public override void HitEffect(Entity entity, IEntitySource source, Player owner)
        {
            SoundEngine.PlaySound(SoundsMenu.NightRayShieldBreak, entity.Center);
            for (int i = 0; i < 50; i++)
            {
                Color color = Color.Lerp(Color.LightPink, Color.Purple, Main.rand.NextFloat(0, 1f));
                new GlowBall(entity.Center, Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(2f, 10f), color, Main.rand.Next(90, 120), 0, 1, 0.1f, true).Spawn();
            }
        }
    }
}
