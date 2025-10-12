using CalamityMod.Graphics.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using UCA.Assets;
using UCA.Assets.Effects;
using UCA.Core.Graphics;
using UCA.Core.Graphics.DrawNode;
using UCA.Core.Graphics.Primitives.Trail;
using UCA.Core.Utilities;

namespace UCA.Content.DrawNodes
{
    public class TerraTree : DrawNode
    {
        public TerraTree(Vector2 position, Vector2 velocity, Color color, float Rot, DrawLayer layer)
        {
            Position = position;
            Velocity = velocity;
            DrawColor = color;
            Rotation = Rot;
            Layer = layer;

            XScale = Main.rand.NextFloat(2, 5);
            Filp = Main.rand.NextBool() ? 1 : -1;
            Height = Main.rand.NextFloat(9, 18f);
        }

        public TerraTree(Vector2 position, Vector2 velocity, Color color, float Rot, DrawLayer layer, float Xscale, int filp, float height)
        {
            Position = position;
            Velocity = velocity;
            DrawColor = color;
            Rotation = Rot;
            Filp = filp;
            Layer = layer;

            XScale = Xscale;
            Filp = filp;
            Height = height;
        }
        public DynamicVertexBuffer VertexBuffer;
        public short VerticesIndex;

        public List<Vector2> OldPos = [];
        public List<float> OldRot = [];
        public Vector2 oldDustPos;
        public Vector2 DustPos;

        public float XScale;
        public int Filp;
        public float Height;
        public bool CanAdd = true;

        public int TotalPoint = 90;
        public bool FireFrame = true;
        public override void OnSpawn()
        {
            Lifetime = 360;
            ExtraUpdate = 4;
            Opacity = 1f;
        }
        public override void Update()
        {
            if (!CanAdd)
            {
                Opacity = MathHelper.Lerp(Opacity, 1f, 0.01f);
                if (Opacity > 0.95f)
                    Time = Lifetime;
                return;
            }

            Opacity = MathHelper.Lerp(Opacity, 0.2f, 0.08f);

            if (Time > TotalPoint)
                CanAdd = false;

            if (Time % 2 == 0)
            {
                // 每两帧才会添加一次数据，优化一下
                oldDustPos = DustPos;
                // 设置弹幕旋转
                Rotation = Velocity.ToRotation();
                // 半径的缩放
                float radiusScale = MathHelper.Lerp(0f, 1f, Utils.GetLerpValue(0f, 5f, Time, true));
                // X向量，为了和外部速度联动这样写
                float standVector2X = Velocity.Length();
                // Y向量偏移
                float standVector2Y = (float)(Math.Sin(Time / XScale) * Height * radiusScale * Filp);
                // 应用第二个Sin偏移，来造成噪波的效果
                standVector2Y = (float)(standVector2Y + Math.Cos(Time) * Height / 10);
                // 最终应用偏移
                Vector2 PreAddVector = new(standVector2X, standVector2Y);
                // 根据弹幕旋转，将固定向右转换为向量的旋转
                PreAddVector = PreAddVector.RotatedBy(Rotation);
                // 最终粒子的点
                DustPos = Position + PreAddVector;
                // 转向上一个点
                float rot = (oldDustPos - DustPos).ToRotation();
                // 记录
                OldPos.Add(DustPos);
                OldRot.Add(rot);
            }
        }
        public override void Draw(SpriteBatch sb)
        {
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            Main.graphics.GraphicsDevice.Textures[0] = UCATextureRegister.Wood.Value;
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

            Main.graphics.GraphicsDevice.Textures[1] = UCATextureRegister.Noise.Value;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;

            UCAShaderRegister.TerraRayVinesShader.Parameters["progress"].SetValue(Opacity);
            UCAShaderRegister.TerraRayVinesShader.Parameters["InPutTextureSize"].SetValue(new Vector2(1024, 1024));
            UCAShaderRegister.TerraRayVinesShader.Parameters["EdgeColor"].SetValue(DrawColor.ToVector4());
            UCAShaderRegister.TerraRayVinesShader.Parameters["EdgeWidth"].SetValue(0.2f);
            UCAShaderRegister.TerraRayVinesShader.CurrentTechnique.Passes[0].Apply();
            
            List<VertexPositionColorTexture2D> Vertexlist = new List<VertexPositionColorTexture2D>();
            float fadeOut = 0;
            for (int i = 0; i < OldPos.Count; i ++)
            {
                // 淡入
                float YScale = i / 10f;
                // 淡出
                if (i > OldPos.Count - 10f)
                {
                    fadeOut++;
                    YScale = 1 - (fadeOut / 10f);
                }
                if (YScale > 1)
                    YScale = 1;
                float progress = (float)i / OldPos.Count;
                // 绘制位置
                Vector2 DrawPos = OldPos[i] - Main.screenPosition;
                Vertexlist.Add(new VertexPositionColorTexture2D(DrawPos - new Vector2(0, 3 * YScale).RotatedBy(OldRot[i]), DrawColor, new Vector3(progress, 0, 0)));
                Vertexlist.Add(new VertexPositionColorTexture2D(DrawPos + new Vector2(0, 3 * YScale).RotatedBy(OldRot[i]), DrawColor, new Vector3(progress, 1, 0)));
            }
            VertexPositionColorTexture2D[] VertexArray = Vertexlist.ToArray();
            
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, VertexArray, 0, Vertexlist.Count - 2);

            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }
}
