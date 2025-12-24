using CalamityMod.Particles;
using LAP.Core.Graphics.Primitives.Trail;
using LAP.Core.MetaBallsSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using UCA.Assets;
using UCA.Assets.Effects;

namespace UCA.Content.MetaBalls
{
    public partial class CosmicMetaBall : BaseMetaBall
    {
        public override Color EdgeColor => Color.DarkViolet;
        public static List<CosmicParticle> CircleParticles = [];
        public static List<CrossStar> CrossParticles = [];
        public static List<LozengeParticle> LozengeParticles = [];
        public override Texture2D BgTexture => UCATextureRegister.CosmicBG.Value;
        public static void SpawnCircleParticle(Vector2 position, Vector2 velocity, float size, int maxTime) => CircleParticles.Add(new(position, velocity, size, maxTime));
        public static void SpawnCrossParticle(Vector2 position, float size) => CrossParticles.Add(new(position, size));
        public static void SpawnLozengeParticle(Vector2 position, Vector2 velocity, float size, int maxTime) => LozengeParticles.Add(new(position, velocity, size, maxTime));
        public override bool Active()
        {
            int TotalCount = CircleParticles.Count + CrossParticles.Count + LozengeParticles.Count;
            if (TotalCount == 0)
                return false;
            else
                return true;
        }

        public override void Update()
        {
            for (int i = 0; i < CircleParticles.Count; i++)
            {
                CircleParticles[i].Update();
                if (CircleParticles[i].Time >= CircleParticles[i].MaxTime)
                    CircleParticles.RemoveAt(i);
            }
            for (int i = 0; i < CrossParticles.Count; i++)
            {
                CrossParticles[i].Update();
                if (CrossParticles[i].Time >= CrossParticles[i].MaxTime)
                {
                    CrossParticles[i].OnKill();
                    CrossParticles.RemoveAt(i);
                }
            }
            for (int i = 0; i < LozengeParticles.Count; i++)
            {
                LozengeParticles[i].Update();
                if (LozengeParticles[i].Time >= LozengeParticles[i].MaxTime)
                    LozengeParticles.RemoveAt(i);
            }
        }

        public override void PrepareRenderTarget()
        {
            if (CircleParticles.Count != 0)
            {
                for (int i = 0; i < CircleParticles.Count; i++)
                {
                    Main.spriteBatch.Draw(UCATextureRegister.WhiteCircle.Value, CircleParticles[i].Center - Main.screenPosition, null, Color.White, 0, UCATextureRegister.WhiteCircle.Size() / 2, CircleParticles[i].Scale, SpriteEffects.None, 0f);
                }
            }
            if (CrossParticles.Count != 0)
            {
                for (int i = 0; i < CrossParticles.Count; i++)
                {
                    Main.spriteBatch.Draw(UCATextureRegister.Star.Value, CrossParticles[i].Center - Main.screenPosition, null, Color.White, 0, UCATextureRegister.Star.Size() / 2, CrossParticles[i].Scale, SpriteEffects.None, 0f);
                }
            }
            if (LozengeParticles.Count != 0)
            {
                for (int i = 0; i < LozengeParticles.Count; i++)
                {
                    Main.spriteBatch.Draw(UCATextureRegister.Lozenge.Value, LozengeParticles[i].Center - Main.screenPosition, null, Color.White, LozengeParticles[i].Rot, UCATextureRegister.Lozenge.Size() / 2, LozengeParticles[i].Scale, SpriteEffects.None, 0f);
                }
            }
        }

        public override void PrepareShader()
        {
            Main.graphics.GraphicsDevice.Textures[0] = AlphaTexture;
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            Main.graphics.GraphicsDevice.Textures[1] = BgTexture;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
            Effect shader = UCAShaderRegister.MetaballShader.Value;
            shader.Parameters["renderTargetSize"].SetValue(AlphaTexture.Size());
            shader.Parameters["bakcGroundSize"].SetValue(BgTexture.Size() / 2);
            shader.Parameters["edgeColor"].SetValue(EdgeColor.ToVector4());
            shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 4);
            shader.CurrentTechnique.Passes[0].Apply();
        }
    }
}
