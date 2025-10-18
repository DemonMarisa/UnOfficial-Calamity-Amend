using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using UCA.Assets;
using UCA.Core.ParticleSystem;
using UCA.Core.Utilities;
using static tModPorter.ProgressUpdate;

namespace UCA.Content.Particiles.Lightnings
{
    public class Lightning03 : BaseParticle
    {
        public override BlendState BlendState => BlendState.Additive;
        public int XFrame;
        public int YFrame;
        public Lightning03(Vector2 position, Vector2 velocity, Color color, int lifetime, float Rot, float scale)
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
            XFrame = Main.rand.Next(0, 2);
            YFrame = Main.rand.Next(0, 2);
        }
        public override void Update()
        {
            Opacity = MathHelper.Lerp(1f, 0f, EasingHelper.EaseOutCubic(LifetimeRatio));
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = UCATextureRegister.Lightning03.Value;
            Rectangle frame = texture.Frame(2, 2, XFrame, YFrame);
            Vector2 origin = frame.Size() * 0.5f;

            spriteBatch.Draw(texture, Position - Main.screenPosition, frame, Color.White * Opacity, Rotation, origin, Scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, Position - Main.screenPosition, frame, DrawColor * Opacity, Rotation, origin, Scale, SpriteEffects.None, 0f);
        }
    }
}
