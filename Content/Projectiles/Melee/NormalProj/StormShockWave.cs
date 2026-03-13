using LAP.Assets.Effects;
using LAP.Assets.TextureRegister;
using LAP.Content.Configs;
using LAP.Content.Particles;
using LAP.Core.Enums;
using LAP.Core.Graphics.PixelatedRender;
using LAP.Core.Graphics.Primitives.Trail;
using LAP.Core.SystemsLoader;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Core.BaseClass;

namespace UCA.Content.Projectiles.Melee.NormalProj
{
    public class StormShockWave : BaseMeleeProj, IPixelatedRenderer
    {
        public override string Texture => LAPTextureRegister.InvisibleTexturePath;
        public ref float Heigh => ref Projectile.ai[0];
        public List<Vector2> CenterPos = [];
        Vector2 TopLeft = new Vector2(-300, -20);
        Vector2 MiddleRightUp = new Vector2(25, -100);
        Vector2 MiddleRightDown = new Vector2(25, 100);
        Vector2 BottomLeft = new Vector2(-300, 20);
        public float RanddomOffset;
        public float RanddomOffset2;
        public float RanddomOffset3;
        public float RanddomOffset4;
        public override void SetStaticDefaults()
        {
            Projectile.AddProtectedProj();
        }
        public override void SetDefaults()
        {
            Projectile.width = 600;
            Projectile.height = 600;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 450;
            Projectile.timeLeft = 120;
            Projectile.noEnchantmentVisuals = true;
            Projectile.extraUpdates = 1;
            Projectile.Opacity = 0f;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (targetHitbox.Intersects(projHitbox))
            {
                for (int i = 0; i < CenterPos.Count; i++)
                {
                    Rectangle ProjHitbox = Utils.CenteredRectangle(CenterPos[i] + Projectile.Center - Projectile.velocity, new Vector2(20, 20));
                    if (targetHitbox.Intersects(ProjHitbox))
                        return true;
                }
            }
            return false;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.LAP().FirstFrame)
            {
                // 防止多人同步收包不及时
                if (Heigh <= 0)
                    Heigh = 0.8f;
                RanddomOffset = Main.rand.NextFloat(0f, 10f);
                RanddomOffset2 = Main.rand.NextFloat(0f, 10f);
                RanddomOffset3 = Main.rand.NextFloat(0f, 10f);
                RanddomOffset4 = Main.rand.NextFloat(0f, 10f);
                float Max = 50;
                float YMult = 2f * Heigh;
                float XMult = 5f * Heigh;
                for (int i = 0; i < Max; i++)
                {
                    float Progress = i / Max;
                    Vector2 finalPos = Vector2.CatmullRom(TopLeft, MiddleRightUp, MiddleRightDown, BottomLeft, Progress);
                    finalPos.Y *= YMult;
                    finalPos.X *= XMult;
                    CenterPos.Add(finalPos.RotatedBy(Projectile.rotation));
                }
            }
            for (int i = 0; i < 4; i++)
            {
                int Index = Main.rand.Next(10, CenterPos.Count - 10);
                Vector2 RandomPos = CenterPos[Index] + Projectile.Center;
                new CampSmoke(RandomPos, -Projectile.velocity * Main.rand.NextFloat(0f, 0.4f), Color.White, 45, Main.rand.NextFloat(MathHelper.TwoPi), 0.5f, Main.rand.NextFloat(0.3f, 0.5f)).Spawn();
            }
            for (int i = 0; i < 4; i++)
            {
                int Index = Main.rand.Next(10, CenterPos.Count - 10);
                Vector2 RandomPos = CenterPos[Index] + Projectile.Center;
                new Fire(RandomPos, -Projectile.velocity * Main.rand.NextFloat(0f, 0.4f), Color.White, 25, Main.rand.NextFloat(MathHelper.TwoPi), 0.5f, Main.rand.NextFloat(0.3f, 0.5f)).Spawn();
            }
            for (int i = 0; i < 2; i++)
            {
                int Index = Main.rand.Next(5, CenterPos.Count - 5);
                Vector2 RandomPos = CenterPos[Index] + Projectile.Center;
                new TrailGlowBall(RandomPos, -Projectile.velocity * 0.05f, Color.Gray, Main.rand.Next(25, 35), 0.1f, true).Spawn();
            }
            for (int i = 0; i < 3; i++)
            {
                int Index2 = Main.rand.Next(CenterPos.Count);
                Vector2 RandomPos2 = CenterPos[Index2] + Projectile.Center;
                new SmallGlowBall(RandomPos2, -Projectile.velocity * Main.rand.NextFloat(0f, 0.4f), Color.White, Main.rand.Next(30, 45), 0.15f, 3f).Spawn();
            }
            Projectile.Opacity = MathHelper.Lerp(Projectile.Opacity , 1f, 0.12f);
        }
        public override void OnKill(int timeLeft)
        {
            Vector2 Center = Projectile.Center + new Vector2(140, 0).RotatedBy(Projectile.rotation);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Center, Vector2.Zero, ProjectileType<StormBlast>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            PixelatedRenderManger.BeginDrawProj = true;
            return false;
        }
        public DrawLayer LayerToRenderTo => DrawLayer.BeforeDusts;
        public BlendState BlendState => BlendState.Additive;
        public void RenderPixelated(SpriteBatch spriteBatch)
        {
            LAPContent.ReSetToBeginShader_Pixel(BlendState.Additive);
            Texture2D texture = LAPTextureRegister.StandardGradient.Value;
            Effect effect = LAPShaderRegister.AlphaFade.Value;
            effect.Parameters["uFadeoutLeftLength"].SetValue(0.1f);
            effect.Parameters["uFadeinRigtLength"].SetValue(0.1f);
            effect.Parameters["uFadeinTopLength"].SetValue(0);
            effect.Parameters["uFadeinBottomLength"].SetValue(0.4f);
            effect.Parameters["UVMult"].SetValue(new Vector2(1f, 1f));
            effect.CurrentTechnique.Passes[0].Apply();
            DrawBaseWave(texture, Color.Gray * 0.8f, 1f);
            DrawBaseWave(texture, Color.White * 0.5f, 3f);
            DrawBaseWave(texture, Color.White * 0.5f, 10f);
            DrawBaseWave(texture, Color.White * 0.2f, 15f);
            if (!LAPConfig.Instance.PerformanceMode)
            {
                Texture2D texture2 = UCATextureRegister.Aura_01.Value;
                Vector4 vector4 = new (0.2f, 0.2f, 0.02f, 0.6f);
                LAPUtilities.ApplyAlphaCut(vector4, new(0, -Main.GlobalTimeWrappedHourly * 0.5f + RanddomOffset), new Vector2(1.6f, 0.07f), Color.White);
                DrawBaseWave(texture2, Color.White, 10f);

                LAPUtilities.ApplyAlphaCut(vector4, new(0, -Main.GlobalTimeWrappedHourly * 0.65f + RanddomOffset2), new Vector2(2f, 0.1f), Color.White);
                DrawBaseWave(texture2, Color.White, 20f);

                texture2 = UCATextureRegister.Aura_02.Value;
                LAPUtilities.ApplyAlphaCut(vector4, new(0, -Main.GlobalTimeWrappedHourly * 0.45f + RanddomOffset3), new Vector2(2f, 0.15f), Color.White);
                DrawBaseWave(texture2, Color.White, 10f);

                LAPUtilities.ApplyAlphaCut(vector4, new(0, -Main.GlobalTimeWrappedHourly * 0.35f + RanddomOffset4), new Vector2(1.5f, 0.1f), Color.White);
                DrawBaseWave(texture2, Color.White, 15f);
            }
            LAPContent.ReSetToEndShader_Pixel();
        }
        public void DrawBaseWave(Texture2D texture, Color color, float SourceMult = 1f)
        {
            List<VertexPositionColorTexture2D> VertexList = [];
            Vector2 ProjVel = Projectile.velocity.SafeNormalize(Vector2.UnitX) * 36;
            for (int i = 0; i < CenterPos.Count; i++)
            {
                float progress = (float)i / CenterPos.Count;
                Vector2 DrawPos_Head = CenterPos[i] + Projectile.Center - Main.screenPosition;
                Vector2 DrawPos_Source = CenterPos[i] + Projectile.Center - Main.screenPosition - ProjVel * SourceMult * Projectile.Opacity;
                VertexList.Add(new VertexPositionColorTexture2D(DrawPos_Head, color, new Vector3(progress, 0, 0)));
                VertexList.Add(new VertexPositionColorTexture2D(DrawPos_Source, color, new Vector3(progress, 1, 0)));
            }
            if (VertexList.Count < 3)
                return;
            Main.graphics.GraphicsDevice.Textures[0] = texture;
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, VertexList.ToArray(), 0, VertexList.Count - 2);
        }
    }
}
