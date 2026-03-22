using LAP.Assets.TextureRegister;
using LAP.Content.Configs;
using LAP.Core.AnimationHandle;
using LAP.Core.Enums;
using LAP.Core.SystemsLoader;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Assets.Effects;
using UCA.Content.MetaBalls;
using UCA.Content.Particiles;
using UCA.Content.Projectiles.HealPRoj;
using UCA.Core.BaseClass;

namespace UCA.Content.Projectiles.Magic.Ray
{
    public class CosmicLaser : BaseMagicProj
    {
        public override string Texture => LAPTextureRegister.InvisibleTexturePath;
        public int Time;
        public int MaxTime = 45;
        public int LaserLength = 2200;
        public AniHelper AniHelper = new AniHelper(3);
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 4400;
            Projectile.AddProtectedProj();
        }
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.extraUpdates = 0;
            Projectile.friendly = true;
            Projectile.timeLeft = MaxTime;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Projectile.Opacity < 0.1f) return false;
            float _ = float.NaN;
            Vector2 beamEndPos = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * LaserLength;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, beamEndPos, 24f, ref _);
        }
        public override bool ShouldUpdatePosition() { return false; }
        public override void AI()
        {
            if (Projectile.LAP().FirstFrame)
                FirstFrame();
            UpdateFade();
        }
        public void FirstFrame()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            AniHelper.MaxAniProgress[AniState.Begin] = 5;
            AniHelper.MaxAniProgress[AniState.End] = 10;
            for (int i = 0; i < 25; i++)
            {
                Vector2 spawnVec = Projectile.velocity.RotateRandom(0.15f) * Main.rand.NextFloat(0.1f, 1.1f) * 36f;
                CosmicMetaBall.SpawnLozengeParticle(Projectile.Center, spawnVec, 0.6f, 90);
            }
            for (int i = 0; i < 35; i++)
            {
                Vector2 spawnVec = Projectile.velocity.RotateRandom(0.4f) * Main.rand.NextFloat(0.1f, 1.1f) * 18f;
                CosmicMetaBall.SpawnLozengeParticle(Projectile.Center, spawnVec, 0.6f, 90);
            }

            for (int i = 0; i < 260; i++)
            {
                Vector2 SpawnPos = Projectile.Center + Projectile.velocity * i * 8 + Main.rand.NextVector2Circular(6, 6);
                Vector2 spawnVec = Projectile.velocity * Main.rand.NextFloat(0.1f, 1.1f) * 18f + new Vector2(0, Main.rand.NextFloat(-0.2f, 0.2f));
                CosmicMetaBall.SpawnLozengeParticle(SpawnPos, spawnVec, 0.7f, 90);
            }
            new CrossGlow(Projectile.Center, Vector2.Zero, Color.DarkViolet, 60, 1f, 0.4f).Spawn();
            new CrossGlow(Projectile.Center , Vector2.Zero, Color.Violet, 60, 1f, 0.3f).Spawn();
            new CrossGlow(Projectile.Center , Vector2.Zero, Color.Violet, 60, 1f, 0.2f).Spawn();
            CosmicMetaBall.SpawnCrossParticle(Projectile.Center, 0.4f);
            GenUnDeathSign(Projectile.Center, 1.4f);
        }
        public static void GenUnDeathSign(Vector2 firePos, float speedMult = 1)
        {
            if (LAPConfig.Instance.PerformanceMode)
                speedMult *= 0.7f;
            // 生成星形
            for (int i = 0; i < 180; i++)
            {
                float offsetAngle = MathHelper.TwoPi * i / 180f;
                // Parametric equations for an asteroid.
                float unitOffsetX = (float)Math.Pow(Math.Cos(offsetAngle), 5D) * 1.5f;
                float unitOffsetY = (float)Math.Pow(Math.Sin(offsetAngle), 5D);
                Vector2 puffDustVelocity = new Vector2(unitOffsetX, unitOffsetY) * 7f * speedMult;
                CosmicMetaBall.SpawnCircleParticle(firePos, puffDustVelocity, 0.13f, 90);
            }

            // 生成四条线
            for (int i = 0; i < 4; i++)
            {
                float offsetAngle = MathHelper.TwoPi * i / 4f + MathHelper.PiOver4;
                Vector2 vector = offsetAngle.ToRotationVector2() * 4 * speedMult;
                for (int j = 0; j < 18; j++)
                {
                    CosmicMetaBall.SpawnCircleParticle(firePos, vector + vector * (j / 20f), 0.14f, 90);
                }
            }
            // 生成四条线的切线
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    Vector2 beginVector = new(1, 0.3f);
                    Vector2 endVector = new(1, -0.3f);
                    Vector2 vector = Vector2.Lerp(beginVector, endVector, j / 20f);
                    CosmicMetaBall.SpawnCircleParticle(firePos, vector.RotatedBy(MathHelper.PiOver4 + MathHelper.PiOver2 * i) * 5.7f * speedMult, 0.14f, 90);
                }
            }
        }
        public void UpdateFade()
        {
            if (!AniHelper.HasFinish[AniState.Begin])
            {
                AniHelper.AniProgress[AniState.Begin]++;
                float progress = AniHelper.AniProgress[AniState.Begin] / (float)AniHelper.MaxAniProgress[AniState.Begin];
                Projectile.Opacity = MathHelper.Lerp(0f, 1f, progress);
                Projectile.scale = MathHelper.Lerp(0f, 1f, progress);
                if (AniHelper.AniProgress[AniState.Begin] >= AniHelper.MaxAniProgress[AniState.Begin])
                    AniHelper.HasFinish[AniState.Begin] = true;
            }
            else if (Projectile.timeLeft < AniHelper.MaxAniProgress[AniState.End])
            {
                AniHelper.AniProgress[AniState.End]++;
                float progress = AniHelper.AniProgress[AniState.End] / (float)AniHelper.MaxAniProgress[AniState.End];
                Projectile.Opacity = MathHelper.Lerp(1f, 0f, progress);
                Projectile.scale = MathHelper.Lerp(1f, 0f, progress);
                if (AniHelper.AniProgress[AniState.End] >= AniHelper.MaxAniProgress[AniState.End])
                    AniHelper.HasFinish[AniState.End] = true;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.LAP().OnceHitEffect)
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 FirPos = target.Center + new Vector2(Main.rand.Next(300, 600), 0).RotatedByRandom(MathHelper.TwoPi);
                    Vector2 Vel = LAPUtilities.GetVector2(FirPos, target.Center);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), FirPos, Vel, ModContent.ProjectileType<CosmicSlash>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
                Vector2 SpawnPos = target.Center + new Vector2(target.width, 0).RotatedByRandom(MathHelper.TwoPi);
                Vector2 ProjVel = LAPUtilities.GetVector2(target.Center, SpawnPos) * 9;
                Projectile.Owner().SpawnLifeStealProj(target, Projectile.GetSource_FromThis(), ProjectileType<CosmicHeal>(), SpawnPos, ProjVel);
            }
        }
        public override void OnKill(int timeLeft)
        {
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overWiresUI.Add(index);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.LAP().FirstFrame)
                return false;
            LAPUtilities.ReSetToBeginShader();
            DrawLaser(Color.DarkViolet, 0.15f * Projectile.scale, 0.02f);
            DrawLaser(Color.Violet, 0.05f * Projectile.scale, 0.02f);
            DrawLighting(Color.Violet, 0.1f * Projectile.scale, 0.02f, -25);
            LAPUtilities.ReSetToEndShader();
            return false;
        }
        public void DrawLaser(Color colro, float height = 0.2f, float op = 0.1f, int Speed = -50, float Timeoffset = 0)
        {
            float TextureHeight = UCATextureRegister.LaserHighContrast.Height();
            float TextureWidth = UCATextureRegister.LaserHighContrast.Width();
            Effect shader = UCAShaderRegister.StandardFlowShader.Value;
            shader.Parameters["LaserTextureSize"].SetValue(UCATextureRegister.LaserHighContrast.Size());
            shader.Parameters["targetSize"].SetValue(new Vector2(LaserLength, TextureHeight));
            shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * Speed + Timeoffset);
            shader.Parameters["uColor"].SetValue(colro.ToVector4() * Projectile.Opacity);
            shader.Parameters["uFadeoutLength"].SetValue(op);
            shader.Parameters["uFadeinLength"].SetValue(op);
            shader.CurrentTechnique.Passes[0].Apply();

            Vector2 orig = new(0, TextureHeight / 2);
            float xScale = LaserLength / TextureWidth;
            Main.spriteBatch.Draw(UCATextureRegister.LaserHighContrast.Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, orig, new Vector2(xScale, height), SpriteEffects.None, 0);
        }
        public void DrawLighting(Color colro, float height = 0.2f, float op = 0.1f, int Speed = -50, float Timeoffset = 0)
        {
            float TextureHeight = UCATextureRegister.Lightning.Height();
            float TextureWidth = UCATextureRegister.Lightning.Width();
            Effect shader = UCAShaderRegister.StandardFlowShader.Value;
            shader.Parameters["LaserTextureSize"].SetValue(UCATextureRegister.Lightning.Size());
            shader.Parameters["targetSize"].SetValue(new Vector2(LaserLength / 5f, TextureHeight));
            shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * Speed + Timeoffset);
            shader.Parameters["uColor"].SetValue(colro.ToVector4() * Projectile.Opacity);
            shader.Parameters["uFadeoutLength"].SetValue(op);
            shader.Parameters["uFadeinLength"].SetValue(op);
            shader.CurrentTechnique.Passes[0].Apply();

            Vector2 orig = new(0, TextureHeight / 2);
            float xScale = LaserLength / TextureWidth;
            Main.spriteBatch.Draw(UCATextureRegister.Lightning.Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, orig, new Vector2(xScale, height), SpriteEffects.None, 0);
        }
    }
}
