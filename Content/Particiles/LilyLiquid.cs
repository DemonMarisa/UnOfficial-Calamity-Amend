using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using UCA.Core.ParticleSystem;

namespace UCA.Content.Particiles
{
    public class LilyLiquid : BaseParticle
    {
        public override BlendState BlendState => BlendState.NonPremultiplied;
        public LilyLiquid(Vector2 position, Vector2 velocity, Color color, int lifetime, float Rot, float opacity, float scale)
        {
            Position = position;
            Velocity = velocity;
            DrawColor = color;
            Lifetime = lifetime;
            Rotation = Rot;
            Opacity = opacity;
            Scale = scale;
        }
        public int LineCount = 0;
        public int rowCount = 0;

        public float RotOffset = 0;
        public override string Texture => "UCA/Assets/ParticilesTextures/LilyLiquid";
        public override void OnSpawn()
        {
            RotOffset = Main.rand.NextFloat(-0.15f, 0.15f);
            rowCount = Main.rand.Next(0, 8);
            LineCount = Main.rand.Next(0, 8);
        }

        public override void Update()
        {
            /*
            if (LifetimeRatio > 0.1f)
                Velocity.Y += 1;
            */
            Velocity *= 0.9f;
            Opacity = MathHelper.Lerp(Opacity, MathHelper.Lerp(Opacity, 0, 0.3f), 0.12f);
            Scale = MathHelper.Lerp(Scale, MathHelper.Lerp(Scale, 0, 0.3f), 0.12f);
            Rotation += RotOffset;
        }
        // 这里采样没有问题，他贴图就是这样
        public override void Draw(SpriteBatch spriteBatch)
        {
            float brightness = (float)Math.Pow(Lighting.Brightness((int)(Position.X / 16f), (int)(Position.Y / 16f)), 0.15);
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            Rectangle frame = texture.Frame(8, 8, rowCount, LineCount);
            Vector2 origin = frame.Size() * 0.5f;

            spriteBatch.Draw(texture, Position - Main.screenPosition, frame, DrawColor * brightness * Opacity, Rotation, origin, Scale, 0, 0f);
        }
    }
}
