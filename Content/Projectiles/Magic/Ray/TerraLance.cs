using CalamityMod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Assets.Effects;
using UCA.Content.DrawNodes;
using UCA.Content.Particiles;
using UCA.Content.Projectiles.HeldProj.Magic.TerraRayHeld;
using UCA.Core.BaseClass;
using UCA.Core.Graphics;
using UCA.Core.Graphics.Primitives.Trail;
using UCA.Core.Utilities;

namespace UCA.Content.Projectiles.Magic.Ray
{
    public class TerraLance : BaseMagicProj
    {
        public override string Texture => UCATextureRegister.InvisibleTexturePath;
        public int MaxLife = 240;
        public float LaserLength = 0;
        public float Opacity = 0;
        public List<Vector2> AvailableOldPos = [];
        public List<TerraLanceVine> Vine = [];
        public bool CanFadeOut = false;
        public int FadeOut;
        public override void SetStaticDefaults()
        {
            // 保存旧朝向与旧位置
            ProjectileID.Sets.TrailingMode[Type] = 2;
            // 一共保存25个数据
            ProjectileID.Sets.TrailCacheLength[Type] = 25;
        }
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.extraUpdates = 5;
            Projectile.friendly = true;
            Projectile.timeLeft = MaxLife;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10 * (Projectile.extraUpdates + 1);
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(CanFadeOut);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            CanFadeOut = reader.ReadBoolean();
        }

        public override void AI()
        {
            if (Projectile.UCA().FirstFrame)
            {
                SoundEngine.PlaySound(SoundsMenu.TerraLanceShoot, Projectile.Center);
                float XScale = Main.rand.NextFloat(6, 12);
                float Height = Main.rand.NextFloat(2, 6);
                Vine.Add(new TerraLanceVine(Projectile.Center, Projectile.velocity, Color.ForestGreen, DrawLayer.BeforeDust, MaxLife, XScale, 1, Height));
                Vine.Add(new TerraLanceVine(Projectile.Center, Projectile.velocity, Color.LightGreen, DrawLayer.AfterDust, MaxLife, XScale, -1, Height));
                float XScale2 = Main.rand.NextFloat(12, 18);
                float Height2 = Main.rand.NextFloat(3, 11);
                Vine.Add(new TerraLanceVine(Projectile.Center, Projectile.velocity, Color.DarkGreen, DrawLayer.BeforeDust, MaxLife, XScale2, 1, Height2));
                Vine.Add(new TerraLanceVine(Projectile.Center, Projectile.velocity, Color.SaddleBrown, DrawLayer.AfterDust, MaxLife, XScale2, -1, Height2));
                if (Projectile.ai[0] != 0)
                {
                    TerraRayHeldProj.GenStar(Projectile.Center, MathHelper.PiOver2 + Projectile.rotation);
                }
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            FadeInOut();
            CatchLength();
            UpdateVine();
            if (CanFadeOut)
            {
                Projectile.velocity *= 0.94f;
            }
            else
                CalamityUtils.HomeInOnNPC(Projectile, true, 1500, 12, 35);
        }
        #region 更新藤蔓
        public void UpdateVine()
        {
            for (int i = 0; i < Vine.Count; i++)
            {
                Vine[i].Update();
                Vine[i].Time++;
                Vine[i].Position = Projectile.Center;
                Vine[i].Velocity = Projectile.velocity;

                if (Vine[i].Time >= Vine[i].Lifetime)
                {
                    Vine[i].OnKill();
                    Vine.Remove(Vine[i]);
                }
            }
        }
        #endregion
        #region 记录所有点的长度
        public void CatchLength()
        {
            if (UCAUtilities.OutOffScreen(Projectile.Center))
                return;
            AvailableOldPos.Clear();
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                if (Projectile.oldPos[i] != Vector2.Zero)
                    AvailableOldPos.Add(Projectile.oldPos[i] + Projectile.HalfProjectile());
            }
            LaserLength = Projectile.velocity.Length() * 25;
        }
        #endregion
        #region 淡入淡出
        public void FadeInOut()
        {
            if (Projectile.timeLeft > MaxLife / 2)
            {
                Opacity = MathHelper.Lerp(Opacity, 1f, 0.1f);
            }
            else
                Opacity = MathHelper.Lerp(Opacity, 0f, 0.1f);

            if (Projectile.timeLeft == 60 && CanFadeOut)
            {
                if (UCAUtilities.OutOffScreen(Projectile.Center))
                    return;
                for (int i = 0; i < 5; i++)
                {
                    Color RandomColor = Color.Lerp(Color.LightGreen, Color.Green, Main.rand.NextFloat(0, 1));
                    new MediumGlowBall(Projectile.Center, Vector2.Zero, RandomColor, 180, 0, 1, 0.12f, Main.rand.NextFloat(0.5f, 0.7f)).Spawn();
                }
            }
        }
        #endregion
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!CanFadeOut)
                Projectile.timeLeft = 120;
            Projectile.netUpdate = true;
            CanFadeOut = true;
            SoundEngine.PlaySound(SoundsMenu.TerraRayHit, Projectile.Center);
        }
        public override void OnKill(int timeLeft)
        {
            if (UCAUtilities.OutOffScreen(Projectile.Center))
                return;

            if (Main.rand.NextBool(3))
            {
                Color RandomColor2 = Color.Lerp(Color.Pink, Color.Green, Main.rand.NextFloat(0, 1));
                new Petal(Projectile.Center, -Vector2.UnitY * 9f, RandomColor2, 360, 0, 1, 0.1f, Main.rand.NextFloat(0.5f, 0.7f)).Spawn();
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (UCAUtilities.OutOffScreen(Projectile.Center))
                return false;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            for (int i = 0; i < Vine.Count; i++)
            {
                Vine[i].Draw(Main.spriteBatch);
            }
           
            DrawLaser(Color.DarkGreen, 0.8f);
            DrawLaser(Color.LightGreen, 0.4f);
            DrawLaser(Color.White, 0.2f);
            
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            
            return false;
        }
        public void DrawLaser(Color Color, float heightmult)
        {
            float TextureHeight = UCATextureRegister.TerrarRayFlow.Height();

            UCAShaderRegister.TerrarRayLaser.Parameters["LaserTextureSize"].SetValue(UCATextureRegister.TerrarRayFlow.Size());
            UCAShaderRegister.TerrarRayLaser.Parameters["targetSize"].SetValue(new Vector2(LaserLength, TextureHeight));
            UCAShaderRegister.TerrarRayLaser.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 50);
            UCAShaderRegister.TerrarRayLaser.Parameters["uColor"].SetValue(Color.ToVector4() * Opacity);
            UCAShaderRegister.TerrarRayLaser.Parameters["uFadeoutLength"].SetValue(0.2f);
            UCAShaderRegister.TerrarRayLaser.Parameters["uFadeinLength"].SetValue(0.2f);
            UCAShaderRegister.TerrarRayLaser.CurrentTechnique.Passes[0].Apply();

            Main.graphics.GraphicsDevice.Textures[0] = UCATextureRegister.TerrarRayFlow.Value;
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

            List<VertexPositionColorTexture2D> Vertexlist = new List<VertexPositionColorTexture2D>();
            for (int i = 0; i < AvailableOldPos.Count; i++)
            {
                float progress = (float)i / AvailableOldPos.Count;
                Vector2 DrawPos = AvailableOldPos[i] - Main.screenPosition;
                Vertexlist.Add(new VertexPositionColorTexture2D(DrawPos - new Vector2(0, 36 * heightmult).RotatedBy(Projectile.rotation), Color.White, new Vector3(progress, 0, 0)));
                Vertexlist.Add(new VertexPositionColorTexture2D(DrawPos + new Vector2(0, 36 * heightmult).RotatedBy(Projectile.rotation), Color.White, new Vector3(progress, 1, 0)));
            }
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, Vertexlist.ToArray(), 0, Vertexlist.Count - 2);
        }
    }
}
