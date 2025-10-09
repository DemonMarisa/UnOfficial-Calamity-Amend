using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Xml.Linq;
using Terraria;
using Terraria.ModLoader;
using UCA.Core.ParticleSystem;
using UCA.Core.Utilities;

namespace UCA.Content.Particiles
{
    public class Butterfly : BaseParticle
    {
        public override BlendState BlendState => BlendState.Additive;

        public float Speed = 5f;

        public int SeedOffset = 0;

        public float BeginOpacity = 1f;

        public int XFrame = 0;
        public Butterfly(Vector2 position, Vector2 velocity, Color color, int lifetime, float Rot, float opacity, float scale, float speed)
        {
            Position = position;
            Velocity = velocity;
            DrawColor = color;
            Lifetime = lifetime;
            Rotation = Rot;
            Opacity = opacity;
            BeginOpacity = opacity;
            Scale = scale;
            Speed = speed;
        }

        public override void OnSpawn()
        {
            SeedOffset = Main.rand.Next(0, 100000);
            XFrame = Main.rand.Next(1, 7);
        }

        public override void Update()
        {
            if (Speed != 0)
            {
                Vector2 idealVelocity = -Vector2.UnitY.RotatedBy(MathHelper.Lerp(0, MathHelper.TwoPi, (float)Math.Sin(Time / 36f + SeedOffset) * 0.5f + 0.5f)) * Speed;
                float movementInterpolant = MathHelper.Lerp(0f, 0.1f, Utils.GetLerpValue(0, Lifetime / 2, Time, true));
                Velocity = Vector2.Lerp(Velocity, idealVelocity, movementInterpolant);
                Velocity = Velocity.SafeNormalize(-Vector2.UnitY) * Speed;
            }
            Rotation = Velocity.ToRotation() + MathHelper.Pi;
            Opacity = MathHelper.Lerp(BeginOpacity, 0, EasingHelper.EaseInCubic(LifetimeRatio));

            if (Time % 6 == 0)
                XFrame++;
            if (XFrame > 7)
                XFrame = 0;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = texture.Frame(8, 1, XFrame, 0);
            Vector2 origin = frame.Size() * 0.5f;

            spriteBatch.Draw(texture, Position - Main.screenPosition, frame, DrawColor * Opacity, Rotation, origin, Scale, SpriteEffects.None, 0);
        }
    }
}
