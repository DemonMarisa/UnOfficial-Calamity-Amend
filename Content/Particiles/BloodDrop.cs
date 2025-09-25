using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
using UCA.Core.ParticleSystem;

namespace UCA.Content.Particiles
{
    public class BloodDrop : BaseParticle
    {
        public BloodDrop(Vector2 position, Vector2 velocity, Color color, int lifetime, float Rot, float opacity, float scale)
        {
            Position = position;
            Velocity = velocity;
            DrawColor = color;
            Lifetime = lifetime;
            Rotation = Rot;
            Opacity = opacity;
            Scale = scale;
        }

        public override BlendState BlendState => base.BlendState;
        public bool flag = false;
        public override string Texture => "UCA/Assets/ParticilesTextures/BloodDrop";

        public override void OnSpawn()
        {
        }

        public override void Update()
        {
            if (Velocity.Y < 0f)
            {
                Velocity.Y = MathHelper.Lerp(Velocity.Y, 0, 0.12f);
            }
            else
            {
                Velocity.Y *= 1.1f;
                if (Velocity.Y > 16f)
                {
                    Velocity.Y = 16f;
                }
            }
            if (!flag && Velocity.Y > -0.4f && Velocity.Y < 0.4f)
            {
                flag = true;
                Velocity.Y = 0.4f;
            }
            
            Velocity.X *= 0.9f;
            Rotation = Velocity.ToRotation() + MathHelper.PiOver2;
            Scale *= 0.98f;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            float brightness = (float)Math.Pow(Lighting.Brightness((int)(Position.X / 16f), (int)(Position.Y / 16f)), 0.15);
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            spriteBatch.Draw(texture, Position - Main.screenPosition, null, DrawColor * brightness, Rotation, texture.Size() / 2, Scale, 0, 0f);
        }
    }
}
