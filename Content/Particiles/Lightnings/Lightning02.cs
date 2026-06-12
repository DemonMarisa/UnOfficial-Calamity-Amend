using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using UCA.Assets;
using LAP.Core.ParticleSystem;
using UCA.Core.Utilities;
using LAP.Core.Utilities;
using LAP.Core.Enums;

namespace UCA.Content.Particiles.Lightnings
{
    public class Lightning02 : BaseParticle
    {
        public override int UseBlendStateID => BlendStateID.Additive;
        public Lightning02(Vector2 position, Vector2 velocity, Color color, int lifetime, float Rot, float scale)
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
        }
        public override void Update()
        {
            Opacity = MathHelper.Lerp(1f, 0f, EasingHelper.EaseOutCubic(LifetimeRatio));
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = UCATextureRegister.Lightning02.Value;
            spriteBatch.Draw(texture, Position - Main.screenPosition, null, DrawColor * Opacity, Rotation, texture.Size() / 2, Scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, Position - Main.screenPosition, null, DrawColor * Opacity, Rotation, texture.Size() / 2, Scale, SpriteEffects.None, 0f);
        }
    }
}
