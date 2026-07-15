using LAP.Assets.Sounds;
using LAP.Core.Presets.Content;
using LAP.Core.SpecificEffectManagers;
using LAP.Core.StateMachine.SynedHitEffect;
using LAP.Core.SystemsLoader;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;

namespace UCA.Content.HitEffect
{
    public class VividClarityParryHit : BaseHitEffect
    {
        public override void HitEffect(Entity entity, IEntitySource source, Player owner)
        {
            SoundEngine.PlaySound(LAPSoundsMenu.MagicHit02, entity.Center);
            for (int j = 0; j < 30; j++)
            {
                Vector2 vel = Vector2.UnitX.RotateRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.5f, 1f) * 24f;
                ParticlePreset.NewGlowLozenge(entity.Center, vel, Color.White, 60, 0.4f);
            }
            for (int k = 0; k < 25; k++)
            {
                Color RandomColor = LAPUtilities.LerpColor(Color.WhiteSmoke, Color.LightGreen);
                ParticlePreset.NewTGlowBall(entity.Center, Vector2.Zero, RandomColor, 75, 0.4f, Main.rand.NextFloat(7f, 9f));
            }
            for (int j = 0; j < 10; j++)
            {
                Color color = LAPUtilities.LerpColor(Color.WhiteSmoke, Color.LightGreen);
                ParticlePreset.NewDustGlow(entity.Center, Main.rand.NextVector2CircularEdge(12, 12) * Main.rand.NextFloat(0.5f, 1f), 0, color, 45, 0.15f, 0);
            }
            LAPContent.AddScreenCaustics(25, entity.Center, 0.1f, 0.15f, 0.05f, 1f);
            ScreenShakeSystem.AddScreenShake_Sin(entity.Center, 5, 25, MathHelper.PiOver2, 2);
        }
    }
    public class VividClarityWeakParryHit : BaseHitEffect
    {
        public override void HitEffect(Entity entity, IEntitySource source, Player owner)
        {
            SoundEngine.PlaySound(LAPSoundsMenu.MagicHit02, entity.Center);
            for (int j = 0; j < 30; j++)
            {
                Vector2 vel = Vector2.UnitX.RotateRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.5f, 1f) * 24f;
                ParticlePreset.NewGlowLozenge(entity.Center, vel, Color.White, 60, 0.4f);
            }
            for (int k = 0; k < 25; k++)
            {
                Color RandomColor = LAPUtilities.LerpColor(Color.WhiteSmoke, Color.LightGreen);
                ParticlePreset.NewTGlowBall(entity.Center, Vector2.Zero, RandomColor, 75, 0.4f, Main.rand.NextFloat(7f, 9f));
            }
            for (int j = 0; j < 10; j++)
            {
                Color color = LAPUtilities.LerpColor(Color.WhiteSmoke, Color.LightGreen);
                ParticlePreset.NewDustGlow(entity.Center, Main.rand.NextVector2CircularEdge(12, 12) * Main.rand.NextFloat(0.5f, 1f), 0, color, 45, 0.15f, 0);
            }
            ScreenShakeSystem.AddScreenShake_Sin(entity.Center, 5, 25, MathHelper.PiOver2, 2);
        }
    }
}
