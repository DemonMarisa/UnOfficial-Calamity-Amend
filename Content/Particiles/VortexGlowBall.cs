using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using UCA.Assets;
using LAP.Core.ParticleSystem;
using LAP.Core.Utilities;
using LAP.Core.Enums;
using LAP.Core.Presets.Content;

namespace UCA.Content.Particiles
{
    public class VortexGlowBall : BaseParticle
    {
        public override int UseBlendStateID => BlendStateID.Additive;
        public float BeginScale;
        public VortexGlowBall(Vector2 position, Vector2 velocity, Color color, int lifetime, float scale)
        {
            Position = position;
            Velocity = velocity;
            DrawColor = color;
            Lifetime = lifetime;
            Scale = scale;
            BeginScale = scale;
        }
        public override void OnSpawn()
        {
        }

        public override void Update()
        {
            Scale = MathHelper.Lerp(BeginScale, 0f, EasingHelper.EaseOutCubic(LifetimeRatio));
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = UCATextureRegister.SmallGlowBall.Value;
            spriteBatch.Draw(texture, Position - Main.screenPosition, null, DrawColor * Opacity, Rotation, texture.Size() / 2, Scale, SpriteEffects.None, 0);
        }
    }
}
