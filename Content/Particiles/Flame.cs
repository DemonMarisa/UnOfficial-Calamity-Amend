using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Core.ParticleSystem;

namespace UCA.Content.Particiles
{
    public class Flame : BaseParticle
    {
        public override BlendState BlendState => BlendState.Additive;
        public Flame(Vector2 position, Vector2 velocity, Color color, int lifetime, float Rot, float opacity, float scale)
        {
            Position = position;
            Velocity = velocity;
            DrawColor = color;
            Lifetime = lifetime;
            Rotation = Rot;
            Opacity = opacity;
            Scale = scale;
        }

        public override void Update()
        {

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            float brightness = (float)Math.Pow(Lighting.Brightness((int)(Position.X / 16f), (int)(Position.Y / 16f)), 0.15);

            Rectangle frame = UCATextureRegister.Flame.Frame(8, 8, (int)(LifetimeRatio * 64) % 8, (int)(LifetimeRatio * 8));
            Vector2 origin = frame.Size() * 0.5f;

            spriteBatch.Draw(UCATextureRegister.Flame.Value, Position - Main.screenPosition, frame, DrawColor * brightness * Opacity, Rotation, origin, Scale, 0, 0f);
        }
    }
}
