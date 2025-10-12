using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using UCA.Assets;
using UCA.Core.ParticleSystem;
using UCA.Core.Utilities;

namespace UCA.Content.Particiles
{
    public class AbsorbGlowBall : BaseParticle
    {
        public override BlendState BlendState => BlendState.Additive;
        public AbsorbGlowBall(Vector2 position, Color color, int lifetime, float scale,float rot, float rotSpeed, int Owner, int Length)
        {
            Position = position;
            DrawColor = color;
            Lifetime = lifetime;
            Scale = scale;
            Rotation = rot;
            RotSpeed = rotSpeed;
            OwnerID = Owner;
            DrawLength = Length;
        }
        public int OwnerID;
        public float RotSpeed;
        public float DrawLengthOffset;
        public float DrawLength;
        public Player Owner => Main.player[OwnerID];
        public override void OnSpawn()
        {
        }
        public override void Update()
        {
            Velocity = Vector2.Zero;
            Position = Owner.Center;
            Rotation += RotSpeed;
            DrawLengthOffset = MathHelper.Lerp(DrawLength, 0, EasingHelper.EaseOutCubic(LifetimeRatio));
            if (LifetimeRatio < 0.5f)
                Opacity = MathHelper.Lerp(0f, 1f, EasingHelper.EaseOutCubic(LifetimeRatio * 2));
            else
                Opacity = MathHelper.Lerp(2f, 0f, EasingHelper.EaseInCubic(LifetimeRatio));
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = UCATextureRegister.SmallGlowBall.Value;
            spriteBatch.Draw(texture, Position - Main.screenPosition + new Vector2(DrawLengthOffset, 0).RotatedBy(Rotation), null, Color.White * Opacity, 0, texture.Size() / 2, Scale * 0.5f, SpriteEffects.None, 0);
            spriteBatch.Draw(texture, Position - Main.screenPosition + new Vector2(DrawLengthOffset, 0).RotatedBy(Rotation), null, DrawColor * Opacity, 0, texture.Size() / 2, Scale, SpriteEffects.None, 0);
        }
    }
}
