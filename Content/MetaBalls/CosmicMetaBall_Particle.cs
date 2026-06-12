using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Terraria;

namespace UCA.Content.MetaBalls
{
    public partial class CosmicMetaBall
    {
        public class CosmicParticle(Vector2 center, Vector2 velocity, float scale, int maxTime)
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
        public class CrossStar(Vector2 center, float scale)
        {
            public Vector2 Scale;
            public float BeginScale = scale;
            public Vector2 Center = center;
            public int Time;
            public int MaxTime = 20;
            public void Update()
            {
                Time++;
                Scale = new Vector2(1.5f, 1f) * MathHelper.Lerp(0f, BeginScale, EasingHelper.EaseOutBack(Time / (float)MaxTime));
            }
            public void OnKill()
            {
                for (int i = 0; i < 4f; i++)
                {
                    SpawnLozengeParticle(Center, Vector2.UnitX * i * 4, BeginScale * 2.5f, 60);
                }
                for (int i = 0; i < 4f; i++)
                {
                    SpawnLozengeParticle(Center, -Vector2.UnitX * i * 4, BeginScale * 2.5f, 60);
                }
                for (int i = 0; i < 4f; i++)
                {
                    SpawnLozengeParticle(Center, Vector2.UnitY * i * 2, BeginScale * 2.5f, 60);
                }
                for (int i = 0; i < 4f; i++)
                {
                    SpawnLozengeParticle(Center, -Vector2.UnitY * i * 2, BeginScale * 2.5f, 60);
                }
            }
        }
        public class LozengeParticle(Vector2 center, Vector2 velocity, float scale, int maxTime)
        {
            public float Scale = scale;
            public float BeginScale = scale;
            public float Rot;
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
                Rot = Velocity.ToRotation() + MathHelper.PiOver2;
            }
        }
    }
}
