using LAP.Core.ParticleSystem;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using UCA.Assets;

namespace UCA.Content.Particiles
{
    public class TLineOF : BaseParticle
    {
        public override int UseBlendStateID => BlendStateID.Additive;
        public float BeginScale;
        public float Speed;
        public int SeedOffset;
        public float TurBulenceDirection = 0f;
        public TLineOF(Vector2 position, float speed, Color color, int lifetime, float scale, float direction)
        {
            Position = position;
            Speed = speed;
            DrawColor = color;
            Lifetime = lifetime;
            Scale = scale;
            BeginScale = scale;
            TurBulenceDirection = direction;
        }
        public override void OnSpawn()
        {
            SeedOffset = Main.rand.Next(0, 100000);
        }
        public override void Update()
        {
            if (Speed != 0)
            {
                Vector2 idealVelocity = -Vector2.UnitY.RotatedBy(MathHelper.Lerp(-TurBulenceDirection, TurBulenceDirection, (float)Math.Sin(Time / 36f + SeedOffset) * 0.5f + 0.5f)) * Speed;
                float movementInterpolant = MathHelper.Lerp(0.01f, 0.25f, Utils.GetLerpValue(0, Lifetime / 2, Time, true));
                Velocity = Vector2.Lerp(Velocity, idealVelocity, movementInterpolant);
                Velocity = Velocity.SafeNormalize(-Vector2.UnitY) * Speed;
            }
            Scale = MathHelper.Lerp(BeginScale, 0, EasingHelper.EaseOutCubic(LifetimeRatio));
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = UCATextureRegister.OpticalFlaresLine.Value;
            spriteBatch.Draw(texture, Position - Main.screenPosition, null, DrawColor * Opacity, Rotation, texture.Size() / 2, Scale, SpriteEffects.None, 0);
        }
    }
}
