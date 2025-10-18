using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using UCA.Assets;
using UCA.Core.ParticleSystem;
using UCA.Core.Utilities;

namespace UCA.Content.Particiles.Lightnings
{
    public class Lightning01 : BaseParticle
    {
        public override BlendState BlendState => BlendState.Additive;
        public int XFrame;
        public int YFrame;
        public Lightning01(Vector2 position, Vector2 velocity, Color color, int lifetime, float Rot, float scale)
        {
            Position = position;
            Velocity = velocity;
            DrawColor = color;
            Lifetime = lifetime;
            Scale = scale;
            Rotation = Rot;
        }
        public override void OnSpawn()
        {
            XFrame = Main.rand.Next(0, 4);
            YFrame = Main.rand.Next(0, 2);
        }
        public override void Update()
        {
            Opacity = MathHelper.Lerp(1f, 0f, EasingHelper.EaseOutCubic(LifetimeRatio));
            Opacity = MathF.Pow(Opacity, 0.5f);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = UCATextureRegister.Lightning01.Value;
            Rectangle frame = texture.Frame(4, 2, XFrame, YFrame);
            Vector2 origin = frame.Size() * 0.5f;
            spriteBatch.Draw(texture, Position - Main.screenPosition, frame, Color.White * Opacity, Rotation, origin, Scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, Position - Main.screenPosition, frame, DrawColor * Opacity, Rotation, origin, Scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, Position - Main.screenPosition, frame, DrawColor * Opacity, Rotation, origin, Scale, SpriteEffects.None, 0f);
        }
    }
}
