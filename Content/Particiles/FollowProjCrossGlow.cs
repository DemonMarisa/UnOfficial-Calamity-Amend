using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using UCA.Assets;
using LAP.Core.ParticleSystem;
using UCA.Core.Utilities;
using LAP.Core.Utilities;
using LAP.Core.Enums;
namespace UCA.Content.Particiles
{
    public class FollowProjCrossGlow : BaseParticle
    {
        public override int UseBlendStateID => BlendStateID.Additive;
        public float BeginScale;
        public SpriteEffects se = SpriteEffects.None;
        public int OwnerID;
        public Vector2 Offset;
        public Projectile Owner => Main.projectile[OwnerID];
        public bool HasUnActive = false;
        public FollowProjCrossGlow(Vector2 position, Color color, int lifetime, float scale, int owner, Vector2 offset)
        {
            Position = position;
            DrawColor = color;
            Lifetime = lifetime;
            Scale = scale;
            BeginScale = scale;
            OwnerID = owner;
            Offset = offset;
            Important = true;
        }
        public override void OnSpawn()
        {
            if (Main.rand.NextBool())
                se = SpriteEffects.FlipHorizontally;
        }
        public override void Update()
        {
            Velocity = Vector2.Zero;

            if (!Owner.active)
                HasUnActive = true;

            if (!HasUnActive)
                Position = Owner.Center + Offset.RotatedBy(Owner.rotation);

            if (LifetimeRatio < 0.5f)
            {
                Scale = MathHelper.Lerp(0f, BeginScale, EasingHelper.EaseOutCubic(LifetimeRatio * 2));
            }
            else
            {
                float progress = LifetimeRatio - 0.5f;
                Scale = MathHelper.Lerp(BeginScale, 0f, EasingHelper.EaseInCubic(progress * 2));
            }
        }
        // 这里采样没有问题，他贴图就是这样
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = UCATextureRegister.CrossGlow.Value;
            spriteBatch.Draw(texture, Position - Main.screenPosition, null, DrawColor * Opacity, Rotation, texture.Size() / 2, Scale, se, 0f);
        }
    }
}
