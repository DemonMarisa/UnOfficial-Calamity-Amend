using LAP.Core.MetaBallsSystem;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;

using UCA.Assets;
using UCA.Assets.Effects;

namespace UCA.Content.MetaBalls
{
    public class NebulaMetaBall : BaseMetaBall
    {
        public class NebulaParticle(Vector2 center, Vector2 velocity, float scale, int maxTime)
        {
            public float Scale = scale;
            public float BeginScale = scale;
            public Vector2 Velocity = velocity;
            public Vector2 Center = center;
            public int Time;
            public int MaxTime = maxTime;

            public void Update()
            {
                Time++;
                Center += Velocity;
                Velocity *= 0.9f;
                Scale = MathHelper.Lerp(BeginScale, 0f, EasingHelper.EaseOutCubic(Time / (float)MaxTime));
            }
        }
        public override Color EdgeColor => Color.Violet;
        public static List<NebulaParticle> Particles = [];
        public override Texture2D BgTexture => UCATextureRegister.NebulaBG.Value;
        public static void SpawnParticle(Vector2 position, Vector2 velocity, float size, int maxTime) => Particles.Add(new(position, velocity, size, maxTime));
        public override bool Active()
        {
            if (Particles.Count == 0)
                return false;
            else
                return true;
        }

        public override void Update()
        {
            for (int i = 0; i < Particles.Count; i++)
            {
                Particles[i].Update();
            }

            // 移除生命周期已结束的粒子
            Particles.RemoveAll(particle =>
            {
                if (particle.Time >= particle.MaxTime)
                {
                    return true;
                }
                return false;
            });
        }
        public override void PrepareRenderTarget()
        {
            if (Particles.Count != 0)
            {
                for (int i = 0; i < Particles.Count; i++)
                {
                    Main.spriteBatch.Draw(UCATextureRegister.WhiteCircle.Value, Particles[i].Center - Main.screenPosition, null, Color.White, 0, UCATextureRegister.WhiteCircle.Size() / 2, Particles[i].Scale, SpriteEffects.None, 0f);
                }
            }
        }
        public override float BGTimeMult => 4;
    }
}
