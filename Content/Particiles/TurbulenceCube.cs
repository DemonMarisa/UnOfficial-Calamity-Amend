using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using UCA.Assets;
using UCA.Core.ParticleSystem;

namespace UCA.Content.Particiles
{
    public class TurbulenceCube : BaseParticle
    {
        public TurbulenceCube(Vector2 position, Vector2 velocity, Color color, int lifetime, float Rot, float opacity, float scale)
        {
            Position = position;
            Velocity = velocity;
            DrawColor = color;
            Lifetime = lifetime;
            Scale = scale;
        }
        public override void Update()
        {
            Opacity = (float)Math.Sin(LifetimeRatio * MathHelper.Pi);

            Velocity *= 0.95f;
            Scale *= 0.98f;

            if (Time % 3 == 0)
            {
                Position += Main.rand.NextVector2Circular(13, 13);
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = UCATextureRegister.WhiteCube.Value;
            spriteBatch.Draw(texture, Position - Main.screenPosition, null, DrawColor * Opacity, Rotation, texture.Size() / 2, Scale, SpriteEffects.None, 0f);
        }
    }
}
