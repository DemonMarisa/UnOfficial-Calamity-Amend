using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Assets.Effects;
using LAP.Core.ParticleSystem;
using UCA.Core.Utilities;
using LAP.Core.Utilities;

namespace UCA.Content.Particiles
{
    public class NoiseShockRing : BaseParticle
    {
        public override int UseBlendStateID => BlendStateID.Additive;
        public float BeginScale;
        public int Index;
        public Projectile Father => Main.projectile[Index];
        public Vector2 Offset;
        public bool Follow = true;
        public NoiseShockRing(Vector2 position, Vector2 velocity, Color color, int lifetime, float opacity, float scale, int father, Vector2 offset)
        {
            Position = position;
            Velocity = velocity;
            DrawColor = color;
            Lifetime = lifetime;
            Opacity = opacity;
            Scale = scale;
            BeginScale = scale;
            Index = father;
            Offset = offset;
            Important = true;
        }
        public NoiseShockRing(Vector2 position, Vector2 velocity, Color color, int lifetime, float opacity, float scale, int father, Vector2 offset, bool follow)
        {
            Position = position;
            Velocity = velocity;
            DrawColor = color;
            Lifetime = lifetime;
            Opacity = opacity;
            Scale = scale;
            BeginScale = scale;
            Index = father;
            Offset = offset;
            Follow = follow;
            Important = true;
        }
        public override void OnSpawn()
        {
            Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
        }
        public override void Update()
        {
            if (Follow)
                Position = Father.Center + Offset.RotatedBy(Father.rotation);

            if (LifetimeRatio < 0.5f)
            {
                Scale = MathHelper.Lerp(0f, BeginScale, EasingHelper.EaseOutCubic(LifetimeRatio * 2));
            }
            else
            {
                float progress = LifetimeRatio - 0.5f;
                Opacity = MathHelper.Lerp(1f, 0f, EasingHelper.EaseOutCubic(progress * 2));
            }
            Rotation += 0.05f;
        }
        // 这里采样没有问题，他贴图就是这样
        public override void Draw(SpriteBatch spriteBatch)
        {
            LAPUtilities.ReSetToBeginShader();

            UCAShaderRegister.PolarDistortShaderWithR.Parameters["uWidthMult"].SetValue(1f);
            UCAShaderRegister.PolarDistortShaderWithR.Parameters["uRingMult"].SetValue(4f);
            UCAShaderRegister.PolarDistortShaderWithR.Parameters["uYTime"].SetValue(Main.GlobalTimeWrappedHourly);
            Main.instance.GraphicsDevice.Textures[1] = UCATextureRegister.BloomRing.Value;

            UCAShaderRegister.PolarDistortShaderWithR.CurrentTechnique.Passes[0].Apply();

            Texture2D texture = UCATextureRegister.Aura_01.Value;
            Vector2 orig = texture.Size() / 2;
            Main.spriteBatch.Draw(texture, Position - Main.screenPosition, null, DrawColor * Opacity, Rotation, orig, Scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(texture, Position - Main.screenPosition, null, DrawColor * Opacity, Rotation, orig, Scale, SpriteEffects.None, 0);
            LAPUtilities.ReSetToEndShader(BlendState.Additive);
        }
    }
}
