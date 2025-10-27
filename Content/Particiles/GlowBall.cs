using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using UCA.Assets;
using LAP.Core.ParticleSystem;
using UCA.Core.Utilities;
using LAP.Core.Utilities;

namespace UCA.Content.Particiles
{
    public class GlowBall : BaseParticle
    {
        public bool UseRot = false;
        public override int UseBlendStateID => BlendStateID.Additive;
        public GlowBall(Vector2 position, Vector2 velocity, Color color, int lifetime, float Rot, float opacity, float scale)
        {
            Position = position;
            Velocity = velocity;
            DrawColor = color;
            Lifetime = lifetime;
            Rotation = Rot;
            Opacity = opacity;
            Scale = scale;
        }

        public GlowBall(Vector2 position, Vector2 velocity, Color color, int lifetime, float Rot, float opacity, float scale, bool useRot)
        {
            Position = position;
            Velocity = velocity;
            DrawColor = color;
            Lifetime = lifetime;
            Rotation = Rot;
            Opacity = opacity;
            Scale = scale;
            UseRot = useRot;
        }
        public override void OnSpawn()
        {
        }

        public override void Update()
        {
            if (!UseRot)
                Velocity *= 0.9f;
            else
                Velocity *= 0.94f;
            Scale = MathHelper.Lerp(Scale, 0, EasingHelper.EaseInCubic(LifetimeRatio));

            if (UseRot)
            {
                Velocity = Velocity.RotatedBy(0.03f);
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = UCATextureRegister.GlowBall.Value;
            spriteBatch.Draw(texture, Position - Main.screenPosition, null, DrawColor * Opacity, Rotation, texture.Size() / 2, Scale, SpriteEffects.None, 0);
        }
    }
}
