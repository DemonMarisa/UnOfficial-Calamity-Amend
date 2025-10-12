using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Assets.Effects;
using UCA.Content.DrawNodes;
using UCA.Content.Particiles;
using UCA.Core.BaseClass;
using UCA.Core.Graphics;
using UCA.Core.Graphics.Primitives.Trail;
using UCA.Core.Utilities;

namespace UCA.Content.Projectiles.Magic.Ray
{
    public class TerraLaser : BaseMagicProj
    {
        public override string Texture => UCATextureRegister.InvisibleTexturePath;
        public int LaserLength = 500;
        public List<Vector2> OldPos = [];
        public float Opacity = 0f;
        public int MaxLife = 75;
        public int AniProgress = 0;
        public List<Vector2> FirePos = [];
        public int HitCount = 0;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 4400;
        }
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.extraUpdates = 0;
            Projectile.friendly = true;
            Projectile.timeLeft = MaxLife;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20 * (Projectile.extraUpdates + 1);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Opacity < 0.1f)
                return false;
            if (projHitbox.Intersects(targetHitbox))
            {
                return true;
            }
            float _ = float.NaN;
            Vector2 beamEndPos = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * LaserLength;
            bool c = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, beamEndPos, 24f, ref _);
            return c;
        }
        public override void OnSpawn(IEntitySource source)
        {
        }
        public override void AI()
        {
            LaserLength = (int)MathHelper.Lerp(LaserLength, 2000, 0.2f);
            Projectile.rotation = Projectile.velocity.ToRotation();
            #region 透明度
            if (Projectile.timeLeft > MaxLife / 2)
                Opacity = MathHelper.Lerp(Opacity, 1f, 0.2f);
            else
                Opacity = MathHelper.Lerp(Opacity, 0f, 0.15f);
            #endregion
            #region 保存位置进度
            // 重置表单
            if (AniProgress < 130)
            {
                OldPos.Clear();
                // 这样写可以不是瞬间出现
                if (AniProgress < 130)
                    AniProgress += 130 / 15;
                for (int i = 0; i < AniProgress; i++)
                {
                    OldPos.Add(Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.UnitX) * i * 13);
                }
            }
            #endregion
            #region 当全部出现后发射的粒子
            if (Projectile.ai[0] == 0 && AniProgress > 125)
            {
                for (int i = 0; i < OldPos.Count; i += 5)
                {
                    Color RandomColor = Color.Lerp(Color.LightGreen, Color.Green, Main.rand.NextFloat(0, 1));
                    new MediumGlowBall(OldPos[i], -Projectile.velocity, RandomColor, 180, 0, 1, 0.12f, Main.rand.NextFloat(1f, 1.4f)).Spawn();
                }
                for (int i = 0; i < OldPos.Count; i += 20)
                {
                    Color RandomColor = Color.Lerp(Color.Pink, Color.Green, Main.rand.NextFloat(0, 1));
                    new Petal(OldPos[i], -Vector2.UnitY * 12f, RandomColor, 360, 0, 1, 0.1f, Main.rand.NextFloat(1f, 1.4f)).Spawn();
                }
                Projectile.ai[0]++;
            }
            #endregion
            #region 发射弹幕
            ShootLaser();

            if (Projectile.timeLeft == 45)
                ShootLance();

            #endregion
        }
        #region 发射长矛
        public void ShootLance()
        {
            for (int i = 0; i < FirePos.Count; i++)
            {
                NPC npc = Projectile.FindClosestTarget(1500, true);
                GenStar(FirePos[i], MathHelper.PiOver2 + Projectile.rotation, 1);
                if (npc != null)
                {
                    float DistanceToNPC = Vector2.Distance(FirePos[i], npc.Center);
                    float PredictMult = DistanceToNPC / 45;
                    Vector2 ToNPCVel = (npc.Center - FirePos[i] + npc.velocity * PredictMult).SafeNormalize(Projectile.rotation.ToRotationVector2());
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), FirePos[i], ToNPCVel * Main.rand.NextFloat(8, 10), ModContent.ProjectileType<TerraLance>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
            }
        }
        #endregion
        #region 发射激光
        public void ShootLaser()
        {
            int MaxGenProj = 6;
            if (Projectile.ai[1] < MaxGenProj && Projectile.timeLeft % 4 == 0)
            {
                Vector2 genPos = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.UnitX) * 350f * (Projectile.ai[1]);
                if (Projectile.ai[1] == 0)
                    genPos = Projectile.Center + new Vector2(30, 0).RotatedBy(Projectile.rotation);
                NPC npc = Projectile.FindClosestTarget(1500, genPos, true);
                if (npc is not null)
                {
                    float DistanceToNPC = Vector2.Distance(genPos, npc.Center);
                    float PredictMult = DistanceToNPC / 60;
                    Vector2 direction = (npc.Center + npc.velocity * PredictMult - genPos).SafeNormalize(Vector2.Zero) * 12;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), genPos, direction, ModContent.ProjectileType<TerraEnergy>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
                FirePos.Add(genPos);

                if (Projectile.ai[1] != 0)
                    GenStar(genPos, 0, 1f);

                Projectile.ai[1]++;
            }
        }
        #endregion
        #region 生成星星
        public static void GenStar(Vector2 pos, float rotoffset, float Xmult = 0.8f)
        {
            // 控制属性分别是：多少个点，生成步进，生成位置
            int PointCount = 9;
            int GenStep = 5;
            float OutSidePoint = 50f;
            float InSidePoint = 35f;
            float RotOffset = rotoffset;
            float FirstRotOffset = MathHelper.ToRadians(12);
            for (int i = 0; i < PointCount; ++i)
            {
                // 生成外部的点
                float OutSideoffset = MathHelper.TwoPi / PointCount;
                float InSideoffset = MathHelper.TwoPi / PointCount;
                float HalfOffsetPoint = MathHelper.TwoPi / PointCount / 2;
                // 先正向生成，再逆向生成
                for (int k = 0; k < 2; k++)
                {
                    int Dir = k == 0 ? 1 : -1;
                    for (float j = 0; j < GenStep; j++)
                    {
                        float Progress = j / GenStep;
                        // 向量计算
                        Vector2 OutSideGenPos = Vector2.UnitX.BetterRotatedBy(OutSideoffset * i + FirstRotOffset, default, 1f, Xmult) * OutSidePoint;
                        // 内部点的位置
                        Vector2 InSideGenPos = Vector2.UnitX.BetterRotatedBy(OutSideoffset * i + HalfOffsetPoint * Dir + FirstRotOffset, default, 1f, Xmult) * InSidePoint;
                        // 整体的旋转
                        OutSideGenPos = OutSideGenPos.RotatedBy(RotOffset);
                        InSideGenPos = InSideGenPos.RotatedBy(RotOffset);
                        // 插值位置，决定最终位置
                        Vector2 LerpPos = Vector2.Lerp(OutSideGenPos, InSideGenPos, Progress);
                        // 插值向量，形成花瓣的形状
                        float LerpVelMult = MathHelper.Lerp(1f, 0.7f, EasingHelper.EaseInCubic(Progress));
                        Vector2 FinalVel = LerpPos * 0.085f * LerpVelMult;
                        // 生成粒子
                        // SpawnParticle(Projectile.Center + FireOffset, FinalVel, 0.1f);
                        Color RandomColor = Color.Lerp(Color.LightGreen, Color.ForestGreen, Main.rand.NextFloat(0, 1));
                        new MediumGlowBall(pos, FinalVel, RandomColor, 70, 0, 1, 0.1f, 0).Spawn();
                    }
                }
            }
            // 生成枝条
            
            Vector2 firPos = pos;
            
            for (int i = 0; i < 5; i++)
            {
                float rot = MathHelper.TwoPi / 5;
                float XScale = Main.rand.NextFloat(9, 12);
                float Height = Main.rand.NextFloat(4f, 6f);

                Vector2 firVec = Vector2.UnitX.RotatedBy(rot * i);
                Color color = Main.rand.NextBool() ? Color.DarkGreen : Color.SaddleBrown;
                new TerraTree(firPos, firVec * Main.rand.NextFloat(0.3f, 0.6f), color, 0, DrawLayer.BeforeDust, XScale, Main.rand.NextBool() ? 1 : -1, Height).Spawn();
            }
            
            for (int i = 0; i < 2; i++)
            {
                Color RandomColor = Color.Lerp(Color.LightGreen, Color.ForestGreen, Main.rand.NextFloat(0, 1));
                new Butterfly(firPos, Vector2.Zero, RandomColor, 120, 0, 1, 0.2f, Main.rand.NextFloat(0.3f, 1.4f)).Spawn();
            }
            for (int i = 0; i < 5; i++)
            {
                Color RandomColor = Color.Lerp(Color.LightGreen, Color.ForestGreen, Main.rand.NextFloat(0, 1));
                new MediumGlowBall(firPos, Vector2.Zero, RandomColor, 60, 0, 1, 0.2f, 0).Spawn();
            }
        }
        #endregion
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (HitCount > 3)
                return;
            HitCount++;
            // 生成枝条
            Vector2 firPos = target.Center;
            for (int i = 0; i < 3; i++)
            {
                float rot = MathHelper.TwoPi / 3;
                float XScale = Main.rand.NextFloat(9, 12);
                float Height = Main.rand.NextFloat(4f, 9f);

                Vector2 firVec = Vector2.UnitX.RotatedBy(rot * i).RotatedByRandom(MathHelper.TwoPi);
                Color color = Main.rand.NextBool() ? Color.ForestGreen : Color.SaddleBrown;
                new TerraTree(firPos, firVec * Main.rand.NextFloat(0.8f, 1.4f), color, 0, DrawLayer.BeforeDust, XScale, Main.rand.NextBool() ? 1 : -1, Height).Spawn();
            }
            for (int i = 0; i < 2; i++)
            {
                Color RandomColor = Color.Lerp(Color.LightGreen, Color.ForestGreen, Main.rand.NextFloat(0, 1));
                new Butterfly(firPos, Vector2.Zero, RandomColor, 120, 0, 1, 0.2f, Main.rand.NextFloat(0.3f, 1.4f)).Spawn();
            }
            for (int i = 0; i < 10; i++)
            {
                Color RandomColor = Color.Lerp(Color.LightGreen, Color.ForestGreen, Main.rand.NextFloat(0, 1));
                new MediumGlowBall(firPos, Vector2.Zero, RandomColor, 60, 0, 1, 0.2f, Main.rand.NextFloat(3f, 4f)).Spawn();
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            DrawLaser(Color.Green, 1.2f);
            DrawLaser(Color.LimeGreen, 1f);
            DrawLaser(Color.White, 0.2f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }

        public void DrawLaser(Color Color, float heightmult)
        {
            float TextureHeight = UCATextureRegister.TerrarRayFlow.Height();

            float laserLength = LaserLength;
            UCAShaderRegister.TerrarRayLaser.Parameters["LaserTextureSize"].SetValue(UCATextureRegister.TerrarRayFlow.Size());
            UCAShaderRegister.TerrarRayLaser.Parameters["targetSize"].SetValue(new Vector2(laserLength, TextureHeight));
            UCAShaderRegister.TerrarRayLaser.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -50);
            UCAShaderRegister.TerrarRayLaser.Parameters["uColor"].SetValue(Color.ToVector4() * Opacity);
            UCAShaderRegister.TerrarRayLaser.Parameters["uFadeoutLength"].SetValue(0.1f);
            UCAShaderRegister.TerrarRayLaser.Parameters["uFadeinLength"].SetValue(0.05f);
            UCAShaderRegister.TerrarRayLaser.CurrentTechnique.Passes[0].Apply();

            Main.graphics.GraphicsDevice.Textures[0] = UCATextureRegister.TerrarRayFlow.Value;
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;


            List<VertexPositionColorTexture2D> Vertexlist = new List<VertexPositionColorTexture2D>();
            for (int i = 0; i < OldPos.Count; i++)
            {
                float progress = (float)i / OldPos.Count;
                // 绘制位置
                Vector2 DrawPos = OldPos[i] - Main.screenPosition;
                Vertexlist.Add(new VertexPositionColorTexture2D(DrawPos - new Vector2(0, -36 * heightmult).RotatedBy(Projectile.rotation), Color.White, new Vector3(progress, 0, 0)));
                Vertexlist.Add(new VertexPositionColorTexture2D(DrawPos + new Vector2(0, -36 * heightmult).RotatedBy(Projectile.rotation), Color.White, new Vector3(progress, 1, 0)));
            }
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, Vertexlist.ToArray(), 0, Vertexlist.Count - 2);
        }
    }
}
