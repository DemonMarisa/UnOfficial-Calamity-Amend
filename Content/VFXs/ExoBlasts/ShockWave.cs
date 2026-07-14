using LAP.Assets.TextureRegister;
using LAP.Core.Enums;
using LAP.Core.Graphics.DeepGlow;
using LAP.Core.ParticleSystem;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using UCA.Assets;

namespace UCA.Content.VFXs.ExoBlasts
{
    public class ShockWave : BaseParticle
    {
        public override int UseBlendStateID => BlendStateID.Additive;
        public float TargetScale;
        public ShockWave(Vector2 position, float rot,Color color, int lifetime, float scale)
        {
            Position = position;
            Rotation = rot;
            DrawColor = color;
            Lifetime = lifetime;
            Scale = scale;
            TargetScale = scale;
            Important = true;
        }
        public override void Update()
        {
            Scale = MathHelper.Lerp(0f, TargetScale, EasingHelper.EaseOutExpo(LifetimeRatio));
            Opacity = MathHelper.Lerp(1f, 0f, EasingHelper.EaseInCubic(LifetimeRatio));
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = LAPTextureRegister.WhiteRing.Value;
            Main.spriteBatch.Draw(texture, Position - Main.screenPosition, null, DrawColor * Opacity, Rotation, texture.Size() / 2, Scale, SpriteEffects.None, 0);
            DeepGlow.SubmitCustomGlow(() =>
            {
                Main.spriteBatch.Draw(texture, Position - Main.screenPosition, null, DrawColor * Opacity * 0.5f, Rotation, texture.Size() / 2, Scale, SpriteEffects.None, 0);
            });
        }
    }
}
