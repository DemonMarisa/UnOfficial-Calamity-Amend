using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using UCA.Assets;
using UCA.Core.ParticleSystem;
using UCA.Core.Utilities;

namespace UCA.Content.Particiles
{
    public class BrokenGlass : BaseParticle
    {
        public BrokenGlass(Vector2 position, Vector2 velocity, Color color, int lifetime, float Rot, float opacity, float scale, bool useGravity)
        {
            Position = position;
            Velocity = velocity;
            DrawColor = color;
            Lifetime = lifetime;
            Rotation = Rot;
            Opacity = opacity;
            BeginOpacity = opacity;
            Scale = scale;
            UseGravity = useGravity;
        }
        public override BlendState BlendState => BlendState.Additive;
        public bool UseGravity = false;
        public bool UseAltTexture = false;
        public float BeginOpacity;
        public override void OnSpawn()
        {
            UseAltTexture = Main.rand.NextBool();
        }
        public override void Update()
        {
            if (UseGravity)
            {
                Velocity.Y += 0.1f;
            }
            Velocity *= 0.9f;
            Rotation += 0.1f * Math.Sign(Velocity.X);
            Opacity = MathHelper.Lerp(BeginOpacity, 0f, EasingHelper.EaseInCubic(LifetimeRatio));
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = UCATextureRegister.Hahen01.Value;
            if (UseAltTexture)
                texture = UCATextureRegister.Hahen02.Value;
            spriteBatch.Draw(texture, Position - Main.screenPosition, null, DrawColor * Opacity, Rotation, texture.Size() / 2, Scale, 0, 0f);
        }
    }
}
