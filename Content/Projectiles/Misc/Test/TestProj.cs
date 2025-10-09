using CalamityMod;
using CalamityMod.Graphics.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Content.Items;
using UCA.Core.Graphics.Primitives.Trail;
using UCA.Core.Utilities;

namespace UCA.Content.Projectiles.Misc.Test
{
    public class TestProj : ModProjectile, ILocalizedModType, IPixelatedPrimitiveRenderer
    {
        public PixelationPrimitiveLayer LayerToRenderTo => PixelationPrimitiveLayer.AfterPlayers;

        public override LocalizedText DisplayName => CalamityUtils.GetItemName<Sword>();

        public override string Texture => UCATextureRegister.InvisibleTexturePath;

        public float Time = 0;

        public List<Vector2> OldPos = [];
        public List<float> OldRot = [];
        public Vector2 oldvec;
        public Vector2 DustPos;
        public Player Owner => Main.player[Projectile.owner];
        public bool CanAdd = true;

        public int FadeIn = 10;
        public int FadeOut = 10;
        public int TotalPoint = 30;
        public float SpeedMult = 2f;
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.timeLeft = 120;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 0;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10 * (Projectile.extraUpdates + 1);
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.ai[0] = Main.rand.NextFloat(3, 5);
            Projectile.ai[2] = Main.rand.NextFloat(24f, 36f);
        }

        public override void AI()
        {

            if (!CanAdd)
            {
                Projectile.velocity = Vector2.Zero;
                return;
            }
            
            if (OldPos.Count >= TotalPoint)
                CanAdd = false;

            Projectile.velocity *= 0.96f;
            SpeedMult *= 0.98f;
            Time++;
            oldvec = DustPos;
            // 设置弹幕旋转
            Projectile.rotation = Projectile.velocity.ToRotation();
            float XScale = Projectile.ai[0];
            float Filp = Projectile.ai[1];
            float Height = Projectile.ai[2];
            // 半径的缩放
            float radiusScale = MathHelper.Lerp(0f, 1f, Utils.GetLerpValue(0f, 15f, Time, true));
            // X向量，为了和外部弹幕速度联动这样写
            float standVector2X = Projectile.velocity.Length();
            // Y向量偏移
            float standVector2Y = (float)(Math.Sin((Time / XScale) * SpeedMult) * Height * radiusScale * Filp);
            // 应用第二个Sin偏移，来造成噪波的效果
            standVector2Y = (float)(standVector2Y + Math.Cos(Time) * Height / 10);
            // 最终应用偏移
            Vector2 PreAddVector = new(standVector2X, standVector2Y);
            // 根据弹幕旋转，将固定向右转换为向量的旋转
            PreAddVector = PreAddVector.RotatedBy(Projectile.rotation);
            // 最终粒子的点
            DustPos = Projectile.Center + PreAddVector;
            // 转向上一个点
            float rot = (oldvec - DustPos).ToRotation();
            // 记录
            OldPos.Add(DustPos);
            OldRot.Add(rot);
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            /*
            List<VertexPositionColorTexture2D> Vertexlist3 = new List<VertexPositionColorTexture2D>();
            float fadeOut = 0;
            for (int i = 0; i < OldPos.Count; i++)
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
                Color DrawColor = Color.LightGreen;
                Vertexlist3.Add(new VertexPositionColorTexture2D(DrawPos - new Vector2(0, 4 * YScale).RotatedBy(OldRot[i]), DrawColor, new Vector3(progress, 0, 0)));
                Vertexlist3.Add(new VertexPositionColorTexture2D(DrawPos + new Vector2(0, 4 * YScale).RotatedBy(OldRot[i]), DrawColor, new Vector3(progress, 1, 0)));
            }

            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, Vertexlist3.ToArray(), 0, Vertexlist3.Count - 2);
            */
            return false;
        }

        public void RenderPixelatedPrimitives(SpriteBatch spriteBatch, PixelationPrimitiveLayer layer)
        {
            List<VertexPositionColorTexture2D> Vertexlist = new List<VertexPositionColorTexture2D>();
            float fadeOut = 0;
            for (int i = 0; i < OldPos.Count; i++)
            {
                // 淡入
                float YScale = i / 5f;

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

                DrawPos = DrawPos / 2;
                Color DrawColor = Color.LimeGreen;
                Vertexlist.Add(new VertexPositionColorTexture2D(DrawPos - new Vector2(0, 1 * YScale).RotatedBy(OldRot[i]), DrawColor, new Vector3(progress, 0, 0)));
                Vertexlist.Add(new VertexPositionColorTexture2D(DrawPos + new Vector2(0, 1 * YScale).RotatedBy(OldRot[i]), DrawColor, new Vector3(progress, 1, 0)));
            }
            Main.graphics.GraphicsDevice.Textures[0] = UCATextureRegister.Wood.Value;
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, Vertexlist.ToArray(), 0, Vertexlist.Count - 2);
        }
    }
}
