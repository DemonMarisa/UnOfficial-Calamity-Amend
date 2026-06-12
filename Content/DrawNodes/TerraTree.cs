using LAP.Assets.TextureRegister;
using LAP.Core.Enums;
using LAP.Core.Graphics.DrawNode;
using LAP.Core.Graphics.Primitives.Trail;
using LAP.Core.ParticleSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using UCA.Assets;
using UCA.Assets.Effects;

namespace UCA.Content.DrawNodes
{
    public class TerraTree : DrawNode
    {
        public override int BlendState => BlendStateID.Additive;
        public TerraTree(Vector2 position, Vector2 velocity, Color color, float Rot)
        {
            Position = position;
            Velocity = velocity;
            DrawColor = color;
            Rotation = Rot;

            XScale = Main.rand.NextFloat(2, 5);
            Filp = Main.rand.NextBool() ? 1 : -1;
            Height = Main.rand.NextFloat(9, 18f);
        }
        public TerraTree(Vector2 position, Vector2 velocity, Color color, float Rot, float Xscale, int filp, float height)
        {
            Position = position;
            Velocity = velocity;
            DrawColor = color;
            Rotation = Rot;
            Filp = filp;

            XScale = Xscale;
            Filp = filp;
            Height = height;
        }
        public List<Vector2> OldPos = [];
        public List<float> OldRot = [];
        public Vector2 oldDustPos;
        public Vector2 DustPos;
        public override DrawLayer Layer => DrawLayer.AfterDusts;
        public float XScale;
        public int Filp;
        public float Height;
        public bool CanAdd = true;

        public int TotalPoint = 90;
        public bool FireFrame = true;
        public override void OnSpawn()
        {
            Lifetime = 640;
            ExtraUpdate = 4;
            Opacity = 1f;
        }
        public override void Update()
        {
            if (!CanAdd)
            {
                Time = 2;
                ExtraUpdate = 0;
                Opacity = MathHelper.Lerp(Opacity, 0f, 0.04f);
                if (Opacity < 0.02f)
                    Kill();
                return;
            }

            Opacity = MathHelper.Lerp(Opacity, 1f, 0.08f);

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
            Main.graphics.GraphicsDevice.Textures[0] = LAPTextureRegister.Wood.Value;
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;

            Effect shader = UCAShaderRegister.TerraRayVinesShader.Value;
            shader.Parameters["progress"].SetValue(Opacity);
            shader.Parameters["UVMult"].SetValue(new Vector2(0.2f, 0.5f));
            shader.Parameters["UVAdd"].SetValue(new Vector2(Main.GlobalTimeWrappedHourly * 0.01f, 1));
            shader.CurrentTechnique.Passes[0].Apply();
            
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
                Vertexlist.Add(new VertexPositionColorTexture2D(DrawPos - new Vector2(0, 2 * YScale).RotatedBy(OldRot[i]), DrawColor * 0.8f, new Vector3(progress, 0, 0)));
                Vertexlist.Add(new VertexPositionColorTexture2D(DrawPos + new Vector2(0, 2 * YScale).RotatedBy(OldRot[i]), DrawColor * 0.8f, new Vector3(progress, 1, 0)));
            }
            VertexPositionColorTexture2D[] VertexArray = Vertexlist.ToArray();
            
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, VertexArray, 0, Vertexlist.Count - 2);
        }
    }
}
