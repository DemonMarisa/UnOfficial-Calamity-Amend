using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Core.ParticleSystem;
using UCA.Core.Utilities;

namespace UCA.Content.Particiles
{
    public class BloodSplash : BaseParticle
    {
        public override BlendState BlendState => base.BlendState;
        public BloodSplash(Vector2 position, Vector2 velocity, Color color, int lifetime, float Rot, float opacity, float scale)
        {
            Position = position;
            Velocity = velocity;
            DrawColor = color;
            Lifetime = lifetime;
            Rotation = Rot;
            Opacity = opacity;
            Scale = scale;
        }
        public override string Texture => "UCA/Assets/ParticilesTextures/BloodSplash";
        public override void OnSpawn()
        {
        }

        public override void Update()
        {
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            float brightness = (float)Math.Pow(Lighting.Brightness((int)(Position.X / 16f), (int)(Position.Y / 16f)), 0.15);
            Texture2D texture = UCATextureRegister.BloodSplash.Value;
            Rectangle frame = texture.Frame(1, 3, 0, (int)(LifetimeRatio * 3f));
            Vector2 origin = frame.Size() * 0.5f;

            spriteBatch.Draw(texture, Position - Main.screenPosition, frame, DrawColor * brightness, Rotation, origin, Scale, 0, 0f);
        }
    }
}
