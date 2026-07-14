using LAP.Assets.TextureRegister;
using LAP.Core.Enums;
using LAP.Core.ParticleSystem;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace UCA.Content.VFXs.ExoBlasts
{
    public class ExoDustGlow : BaseParticle
    {
        public ExoDustGlow(Vector2 beginPos, Vector2 EndPos, Color color, float Scale, float rot, int fadeinTime,int life)
        {
            Position = beginPos;
            BeginPos = beginPos;
            TargetPos = EndPos;
            BeginScale = Scale;
            DrawColor = color;
            Rotation = rot;
            FadeinTime = fadeinTime;
            Lifetime = life;
        }
        public override int UseBlendStateID => BlendStateID.Additive;
        public Vector2 BeginPos;
        public Vector2 TargetPos;
        public float BeginScale;
        public int FadeinTime ;
        public override void Update()
        {
            if (Time < FadeinTime)
            {
                float progress = Time / (float)FadeinTime;
                Position = Vector2.Lerp(BeginPos, TargetPos, EasingHelper.EaseOutExpo(progress));
                Scale = MathHelper.Lerp(0f, BeginScale, EasingHelper.EaseOutExpo(progress));
            }
            else
            {
                float progress = (Time - FadeinTime) / (float)(Lifetime - FadeinTime);
                Scale = MathHelper.Lerp(Scale, 0f, EasingHelper.EaseInCubic(progress));
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = LAPTextureRegister.DustGlow_NB.Value;
            Main.spriteBatch.Draw(texture, Position - Main.screenPosition, null, DrawColor * Opacity, Rotation, texture.Size() / 2, Scale, SpriteEffects.None, 0);
            Texture2D lightpoint = LAPTextureRegister.LightPoint_NB.Value;
            float rot = Main.GlobalTimeWrappedHourly;
            float scale = MathF.Sin(rot) + 10f;
            scale = scale / 10f;
            Main.spriteBatch.Draw(lightpoint, Position - Main.screenPosition, null, DrawColor * Opacity, rot, lightpoint.Size() / 2, scale * Scale * 0.5f, SpriteEffects.None, 0);
        }
    }
}
