using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Assets.Effects;
using UCA.Content.Particiles;

namespace UCA.Core.Utilities
{
    public static partial class UCAUtilities
    {
        /// <summary>
        /// 将当前渲染目标设置为提供的渲染目标。
        /// </summary>
        /// <param name="rt">要交换到的渲染目标</param>
        public static bool SwapToTarget(this RenderTarget2D rt)
        {
            GraphicsDevice gD = Main.graphics.GraphicsDevice;
            SpriteBatch spriteBatch = Main.spriteBatch;

            if (Main.gameMenu || Main.dedServ || spriteBatch is null || rt is null || gD is null)
                return false;

            gD.SetRenderTarget(rt);
            gD.Clear(Color.Transparent);
            return true;
        }

        public static void BaseProjPreDraw(this Projectile proj, Texture2D texture, Color lightColor, float rotOffset = 0f, float scale = 1f)
        {
            Vector2 drawPosition = proj.Center - Main.screenPosition;
            float drawRotation = proj.rotation + (proj.spriteDirection == -1 ? MathHelper.Pi : 0f);
            Vector2 rotationPoint = texture.Size() / 2f;
            SpriteEffects flipSprite = (proj.spriteDirection * Main.player[proj.owner].gravDir == -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Main.spriteBatch.Draw(texture, drawPosition, null, lightColor, drawRotation + rotOffset, rotationPoint, proj.scale * Main.player[proj.owner].gravDir * scale, flipSprite, 0f);
        }

        public static void FastApplyEdgeMeltsShader(float Opacity, Vector2 TextureSize, Color color, float EdgeWidth = 0.01f, int Pass = 0)
        {
            UCAShaderRegister.EdgeMeltsShader.Parameters["progress"].SetValue(Opacity);
            UCAShaderRegister.EdgeMeltsShader.Parameters["InPutTextureSize"].SetValue(TextureSize);
            UCAShaderRegister.EdgeMeltsShader.Parameters["EdgeColor"].SetValue(color.ToVector4());
            UCAShaderRegister.EdgeMeltsShader.Parameters["EdgeWidth"].SetValue(EdgeWidth);
            UCAShaderRegister.EdgeMeltsShader.CurrentTechnique.Passes[Pass].Apply();
        }

        public static void GenFlowersDust(Vector2 pos)
        {
            // 复制的原版小黄色烟花的代码
            Vector2 randomCirclePointVector = Vector2.One.RotatedByRandom(MathHelper.ToRadians(32f));
            float lerpStart = Main.rand.Next(9, 14) * 0.66f; ;
            float lerpEnd = Main.rand.Next(2, 4) * 0.66f;
            for (float i = 0; i < 9f; ++i)
            {
                for (int j = 0; j < 2; ++j)
                {
                    Vector2 randomCirclePointRotated = randomCirclePointVector.RotatedBy((j == 0 ? 1 : -1) * MathHelper.TwoPi / 18);
                    for (float k = 0f; k < 20f; ++k)
                    {
                        Vector2 randomCirclePointLerped = Vector2.Lerp(randomCirclePointVector, randomCirclePointRotated, k / 20f);
                        float lerpMultiplier = MathHelper.Lerp(lerpStart, lerpEnd, k / 20f);
                        int dustIndex = Dust.NewDust(pos, 6, 6, DustID.Firework_Pink, 0f, 0f, 100, default, 1.3f);
                        Main.dust[dustIndex].velocity *= 0.1f;
                        Main.dust[dustIndex].noGravity = true;
                        Main.dust[dustIndex].velocity += randomCirclePointLerped * lerpMultiplier;
                    }
                }
                randomCirclePointVector = randomCirclePointVector.RotatedBy(MathHelper.TwoPi / 9);
            }
        }

        public static void SetRasterizerState()
        {
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            rasterizerState.FillMode = FillMode.WireFrame;
            Main.instance.GraphicsDevice.RasterizerState = rasterizerState;
        }

        public static bool OutOffScreen(Vector2 pos)
        {
            if (pos.X < Main.screenPosition.X - Main.screenWidth / 2)
                return true;
            
            if (pos.Y < Main.screenPosition.Y - Main.screenHeight / 2)
                return true;
            
            if (pos.X > Main.screenPosition.X + Main.screenWidth * 1.5f)
                return true;
            if (pos.Y > Main.screenPosition.Y + Main.screenHeight * 1.5f)
                return true;
            
            return false;
        }
        #region shader
        public static void ReSetToBeginShader()
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
        }
        public static void ReSetToBeginShader(BlendState blendState)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, blendState, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
        }
        public static void ReSetToBeginShader(BlendState blendState, Matrix matrix)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, blendState, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, matrix);
        }
        public static void ReSetToBeginShader(BlendState blendState, SamplerState samplerState)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, blendState, samplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
        }
        public static void ReSetToBeginShader(BlendState blendState, SamplerState samplerState, Matrix matrix)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, blendState, samplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, matrix);
        }
        public static void ReSetToEndShader()
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
        }
        #endregion
        public static void UpDateFrame(int horizontalFrames, int verticalFrames, ref int FrameX, ref int FrameY)
        {
            if (FrameX < horizontalFrames - 1)
                FrameX++;
            else
            {
                FrameX = 0;
                if (FrameY < verticalFrames - 1)
                    FrameY++;
                else
                    FrameY = 0;
            }
        }

        public static void DrawCube(Vector2 pos)
        {
            Main.spriteBatch.Draw(UCATextureRegister.WhiteCube.Value, pos - Main.screenPosition, null,Color.White, 0, UCATextureRegister.WhiteCube.Size() / 2, 1, SpriteEffects.None, default);
        }
    }
}
