using LAP.Core.SpecificEffectManagers;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Assets.Sounds;
using UCA.Content.Configs;
using UCA.Content.DrawNodes;
using UCA.Content.MetaBalls;
using UCA.Content.Projectiles.Misc;
using UCA.Core.BaseClass;

namespace UCA.Content.Projectiles.Magic.Ray
{
    public class CosmicSlash : BaseMagicProj
    {
        public override string Texture => UCATextureRegister.InvisibleTexturePath;
        public int Time;
        public int MaxTime = 60;
        public int LaserLength;
        public Vector2 BeginPos;
        public Vector2 EndPos;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 4400;
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
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), BeginPos, EndPos, 24f, ref _);
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override void AI()
        {
            if(Projectile.LAP().FirstFrame)
            {
                FirstFrame();
            }
            Time++;
            if (Time > 6)
            {
                for (int i = 0; i < 260; i++)
                {
                    float Progress = i / 260f;
                    Vector2 SpawnPos = Vector2.Lerp(BeginPos, Projectile.Center, Progress) + Main.rand.NextVector2Circular(6, 6);
                    Vector2 spawnVec = Projectile.velocity * Main.rand.NextFloat(0.1f, 1.1f) * 18f + new Vector2(0, Main.rand.NextFloat(-0.2f, 0.2f));
                    CosmicMetaBall.SpawnLozengeParticle(SpawnPos, spawnVec, 1, 45);
                }
                for (int i = 0; i < 130; i++)
                {
                    float Progress = i / 130f;
                    Vector2 SpawnPos = Vector2.Lerp(EndPos, Projectile.Center, Progress) + Main.rand.NextVector2Circular(6, 6);
                    Vector2 spawnVec =- Projectile.velocity * Main.rand.NextFloat(0.1f, 1.1f) * 18f + new Vector2(0, Main.rand.NextFloat(-0.2f, 0.2f));
                    CosmicMetaBall.SpawnLozengeParticle(SpawnPos, spawnVec, 1, 45);
                }
                Projectile.Kill();
            }
        }
        public void FirstFrame()
        {
            SoundEngine.PlaySound(SoundsMenu.SwordSwing2 with { Volume = 0.6f}, Projectile.Center);
            Projectile.rotation = Projectile.velocity.ToRotation();
            int Filp = Main.rand.NextBool() ? 1 : -1;
            new CosmicDustEmitting(Projectile.Center, Filp).Spawn();
            BeginPos = Projectile.Center + Projectile.velocity * 8 * 260;
            EndPos = Projectile.Center + -Projectile.velocity * 8 * 130;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.LAP().OnceHitEffect)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<UseForOnHitNPCProj>(), 0, 0, Projectile.owner, Type);
                ScreenShakeSystem.AddScreenShakes(Projectile.Center, 2, 5, Projectile.rotation, 0);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
        public static void GenUnDeathSign(Vector2 firePos, float speedMult = 1)
        {
            if (UCAConfig.Instance.PerformanceMode)
                speedMult *= 0.7f;
            // 生成星形
            for (int i = 0; i < 120; i++)
            {
                float offsetAngle = MathHelper.TwoPi * i / 120f;
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
                    CosmicMetaBall.SpawnCircleParticle(firePos, vector + vector * (j / 20f), 0.15f, 90);
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
                    CosmicMetaBall.SpawnCircleParticle(firePos, vector.RotatedBy(MathHelper.PiOver4 + MathHelper.PiOver2 * i) * 5.7f * speedMult, 0.15f, 90);
                }
            }
        }
    }
}
