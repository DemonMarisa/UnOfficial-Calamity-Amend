using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
using UCA.Core.ParticleSystem;
using UCA.Core.Utilities;

namespace UCA.Content.Particiles
{
    public class Line : BaseParticle
    {
        public override BlendState BlendState => BlendState.AlphaBlend;
        public Vector2 TargetPos;
        public float Mult = 1;
        public bool IsIn;
        public Line(Vector2 position, Vector2 velocity, Color color, int lifetime, float Rot, float opacity, float scale , bool mode, Vector2 target)
        {
            Position = position;
            Velocity = velocity;
            DrawColor = color;
            Lifetime = lifetime;
            Rotation = Rot;
            Opacity = opacity;
            Scale = scale;
            IsIn = mode;
            TargetPos = target;
        }

        public override void OnSpawn()
        {
        }

        public override void Update()
        {

            if (IsIn)
            {
                Mult = MathHelper.Lerp(12, 0, EasingHelper.EaseOutExpo(LifetimeRatio));

                Scale = (float)Math.Pow(EasingHelper.EaseInOutSin(LifetimeRatio), 2) * 0.5f;
                if (Mult == 0)
                    return;

                float target = (TargetPos - Position).ToRotation();
                Rotation = Rotation.AngleLerp(target, 0.1f);
                Velocity = Rotation.ToRotationVector2() * Mult;
            }
            else
            {
                Scale = 1 - EasingHelper.EaseInCubic(LifetimeRatio);
                Scale *= 0.5f;
                Velocity = Velocity.RotatedBy(0.02f);
                Velocity *= 0.94f;
                Rotation = Velocity.ToRotation();
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>("UCA/Content/Particiles/Line").Value;
            spriteBatch.Draw(texture, Position - Main.screenPosition, null, DrawColor * Opacity, Rotation, texture.Size() / 2, Scale, SpriteEffects.None, 0);
        }
    }
}
