using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using UCA.Assets;
using LAP.Core.ParticleSystem;
using UCA.Core.Utilities;
using LAP.Core.Utilities;

namespace UCA.Content.Particiles
{
    public class PoisonSmoke : BaseParticle
    {
        public override int UseBlendStateID => BlendStateID.Additive;
        public PoisonSmoke(Vector2 position, Vector2 velocity, Color color, int lifetime, float Rot, float opacity, float scale)
        {
            Position = position;
            Velocity = velocity;
            DrawColor = color;
            Lifetime = lifetime;
            Rotation = Rot;
            Opacity = opacity;
            Scale = scale;
        }
        public override void OnSpawn()
        {
        }

        public override void Update()
        {
            Velocity *= 0.9f;
            Opacity = MathHelper.Lerp(1f, 0, EasingHelper.EaseInCubic(LifetimeRatio));
        }
        // 这里采样没有问题，他贴图就是这样
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = UCATextureRegister.PoisonSmoke.Value;

            Rectangle frame = UCATextureRegister.PoisonSmoke.Frame(8, 8, (int)(LifetimeRatio * 64) % 8, (int)(LifetimeRatio * 8));
            Vector2 origin = frame.Size() * 0.5f;
            spriteBatch.Draw(texture, Position - Main.screenPosition, frame, DrawColor * Opacity, Rotation, origin, Scale, 0, 0f);
        }
    }
}
