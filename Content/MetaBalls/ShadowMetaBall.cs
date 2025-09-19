using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using UCA.Assets;
using UCA.Assets.Effects;
using UCA.Core.MetaBallsSystem;

namespace UCA.Content.MetaBalls
{
    public class ShadowMetaBall : BaseMetaBall
    {
        public class ShadownParticle
        {
            public float Scale;
            public Vector2 Velocity;
            public Vector2 Center;

            public ShadownParticle(Vector2 center, Vector2 velocity, float scale)
            {
                Center = center;
                Velocity = velocity;
                Scale = scale;
            }

            public void Update()
            {
                Center += Velocity;
                Velocity *= 0.9f;
                Scale *= 0.96f;
            }
        }
        public override Color EdgeColor => new Color(147, 0, 255, 255);
        public static List<ShadownParticle> Particles = [];
        public override Texture2D BgTexture => UCATextureRegister.ShadowNebulaBackGround.Value;
        public static void SpawnParticle(Vector2 position, Vector2 velocity, float size) => Particles.Add(new(position, velocity, size));
        public override bool Active()
        {
            if (Particles.Count == 0)
                return false;
            else
                return true;
        }

        public override void Update()
        {
            for (int i = 0; i < Particles.Count; i++)
            {
                Particles[i].Update();
            }

            // 移除生命周期已结束的粒子
            Particles.RemoveAll(particle =>
            {
                if (particle.Scale < 0.01f)
                {
                    return true;
                }
                return false;
            });
        }

        public override void PrepareRenderTarget()
        {
            if (Particles.Count != 0)
            {
                for (int i = 0; i < Particles.Count; i++)
                {
                    Main.spriteBatch.Draw(UCATextureRegister.WhiteCircle.Value, Particles[i].Center - Main.screenPosition, null, Color.White, 0, UCATextureRegister.WhiteCircle.Size() / 2, Particles[i].Scale, SpriteEffects.None, 0f);
                }
            }
        }

        public override void PrepareShader()
        {
            Main.graphics.GraphicsDevice.Textures[0] = AlphaTexture;
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

            Main.graphics.GraphicsDevice.Textures[1] = BgTexture;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;

            UCAShaderRegister.MetaballShader.Parameters["renderTargetSize"].SetValue(AlphaTexture.Size());
            UCAShaderRegister.MetaballShader.Parameters["bakcGroundSize"].SetValue(BgTexture.Size()/ 4);
            UCAShaderRegister.MetaballShader.Parameters["edgeColor"].SetValue(EdgeColor.ToVector4());
            UCAShaderRegister.MetaballShader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 4);

            UCAShaderRegister.MetaballShader.CurrentTechnique.Passes[0].Apply();
        }
    }
}
