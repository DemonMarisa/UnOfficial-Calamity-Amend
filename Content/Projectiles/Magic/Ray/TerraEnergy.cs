using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
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
    public class TerraEnergy : BaseMagicProj
    {
        public override string Texture => UCATextureRegister.InvisibleTexturePath;
        public int MaxTime = 360;
        public int FadeOut = 0;
        public int MaxFade = 30;
        public float LaserLength = 0;
        public Vector2 BeginPos = Vector2.Zero;
        public List<TerraEnergyTree> Vine = [];
        public float Opacity = 0;
        public bool inToFadeOut = false;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 4400;
        }
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.extraUpdates = 5;
            Projectile.friendly = true;
            Projectile.timeLeft = MaxTime;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10 * (Projectile.extraUpdates + 1);
        }

        public override void OnSpawn(IEntitySource source)
        {
            #region 生成伴随主弹幕的树
            for (int i = 0; i < 1; i++)
            {
                float XScale = Main.rand.NextFloat(4, 12);
                float Height = Main.rand.NextFloat(1f, 3);

                Vine.Add(new TerraEnergyTree(Projectile.Center, Projectile.velocity, Color.SaddleBrown, MaxTime, XScale, 1, Height));
                Vine.Add(new TerraEnergyTree(Projectile.Center, Projectile.velocity, Color.LightGreen, MaxTime, XScale, -1, Height));

                float XScale2 = Main.rand.NextFloat(5, 10);
                float Height2 = Main.rand.NextFloat(3, 6);
                Vine.Add(new TerraEnergyTree(Projectile.Center, Projectile.velocity, Color.DarkGreen, MaxTime, XScale2, 1, Height2));
                Vine.Add(new TerraEnergyTree(Projectile.Center, Projectile.velocity, Color.ForestGreen, MaxTime, XScale2, -1, Height2));
            }
            #endregion
            BeginPos = Projectile.Center;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            LaserLength = (Projectile.Center - BeginPos).Length();
            UpdateVine();
            for (int i = 0; i < Vine.Count; i++)
            {
                if (Vine[i].CanAdd == false)
                    inToFadeOut = true;
            }
            if (inToFadeOut)
            {
                Opacity = MathHelper.Lerp(Opacity, 0f, 0.01f);
                if (Opacity < 0.05f)
                    Projectile.Kill();
                Projectile.velocity *= 0.8f;
                Projectile.damage = 0;
            }
            else
            {
                #region 发射粒子
                if (Projectile.timeLeft % 25 == 0)
                {
                    Color RandomColor = Color.Lerp(Color.LightGreen, Color.Green, Main.rand.NextFloat(0, 1));
                    new MediumGlowBall(Projectile.Center, -Projectile.velocity, RandomColor, 180, 0, 1, 0.12f, Main.rand.NextFloat(0.5f, 0.7f)).Spawn();

                    Color RandomColor2 = Color.Lerp(Color.Pink, Color.Green, Main.rand.NextFloat(0, 1));
                    new Petal(Projectile.Center, -Vector2.UnitY * 9f, RandomColor2, 360, 0, 1, 0.1f, Main.rand.NextFloat(0.5f, 0.7f)).Spawn();
                }
                #endregion
                Opacity = MathHelper.Lerp(Opacity, 1f, 0.1f);
            }
            if (Projectile.timeLeft < MaxTime / 10)
                inToFadeOut = true;
        }
        public void UpdateVine()
        {
            for (int i = 0; i < Vine.Count; i++)
            {
                Vine[i].Update();
                Vine[i].Position += Vine[i].Velocity;
                Vine[i].Time++;

                if (Vine[i].Time >= Vine[i].Lifetime)
                {
                    Vine[i].OnKill();
                    Vine.Remove(Vine[i]);
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            for (int i = 0; i < Vine.Count; i++)
            {
                Vine[i].Draw(Main.spriteBatch);
            }
            
            DrawLaser(Color.ForestGreen, 0.15f);
            DrawLaser(Color.White, 0.02f);
            DrawLaser(Color.LimeGreen, 0.1f);
            
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
        public void DrawLaser(Color colro, float height = 0.2f)
        {
            float TextureHeight = UCATextureRegister.TerrarRayFlow.Height();
            float TextureWidth = UCATextureRegister.TerrarRayFlow.Width();

            UCAShaderRegister.TerrarRayLaser.Parameters["LaserTextureSize"].SetValue(UCATextureRegister.TerrarRayFlow.Size());
            UCAShaderRegister.TerrarRayLaser.Parameters["targetSize"].SetValue(new Vector2(LaserLength, TextureHeight));
            UCAShaderRegister.TerrarRayLaser.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -50);
            UCAShaderRegister.TerrarRayLaser.Parameters["uColor"].SetValue(colro.ToVector4() * Opacity);
            UCAShaderRegister.TerrarRayLaser.Parameters["uFadeoutLength"].SetValue(0.1f);
            UCAShaderRegister.TerrarRayLaser.Parameters["uFadeinLength"].SetValue(0.1f);
            UCAShaderRegister.TerrarRayLaser.CurrentTechnique.Passes[0].Apply();

            Main.graphics.GraphicsDevice.Textures[0] = UCATextureRegister.TerrarRayFlow.Value;
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            Vector2 orig = new Vector2(0, TextureHeight / 2);
            float xScale = LaserLength / TextureWidth;
            Main.spriteBatch.Draw(UCATextureRegister.TerrarRayFlow.Value, BeginPos - Main.screenPosition,null, Color.White, Projectile.rotation, orig, new Vector2(xScale, height), SpriteEffects.None, 0);
      
        }
        public override void OnKill(int timeLeft)
        {            
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < Vine.Count; i++)
            {
                Vine[i].CanAdd = false;
            }
            // 生成枝条
            Vector2 firPos = Projectile.Center;
            for (int i = 0; i < 2; i++)
            {
                float rot = MathHelper.TwoPi / 3;
                float XScale = Main.rand.NextFloat(9, 12);
                float Height = Main.rand.NextFloat(4f, 6f);

                Vector2 firVec = Vector2.UnitX.RotatedBy(rot * i).RotatedByRandom(MathHelper.TwoPi);
                Color color = Main.rand.NextBool() ? Color.DarkGreen : Color.SaddleBrown;
                new TerraTree(firPos, firVec * Main.rand.NextFloat(0.3f, 0.6f), color, 0, DrawLayer.BeforeDust, XScale, Main.rand.NextBool() ? 1 : -1, Height).Spawn();
            }
            for (int i = 0; i < 5; i++)
            {
                float offset = MathHelper.TwoPi / 5;
                Color RandomColor = Color.Lerp(Color.LightGreen, Color.ForestGreen, Main.rand.NextFloat(0, 1));
                Vector2 firVel = Vector2.UnitX.BetterRotatedBy(offset * i, default, 0.75f, 1f);
                new MediumGlowBall(firPos, firVel * 1.5f, RandomColor, 60, 0, 1, 0.2f, Main.rand.NextFloat(2, 3)).Spawn();
            }
        }
    }
}
