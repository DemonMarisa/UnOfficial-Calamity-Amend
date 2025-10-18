using CalamityMod;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Content.Particiles;
using UCA.Content.Particiles.Lightnings;
using UCA.Core.BaseClass;
using UCA.Core.Graphics.Primitives.Trail;
using UCA.Core.Utilities;

namespace UCA.Content.Projectiles.Magic.Ray
{
    public class VortexMissle : BaseMagicProj
    {
        public override string Texture => UCATextureRegister.InvisibleTexturePath;
        public bool CanHit = true;
        public int MaxLife = 540;
        public Vector2 TargetVelocity;
        public override void SetStaticDefaults()
        {
            // 保存旧朝向与旧位置
            ProjectileID.Sets.TrailingMode[Type] = 2;
            // 一共爆粗20个数据
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
        }
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.extraUpdates = 10;
            Projectile.friendly = true;
            Projectile.timeLeft = MaxLife;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }
        public override bool? CanHitNPC(NPC target)
        {
            return base.CanHitNPC(target);
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.UCA().FirstFrame)
            {
                for (int i = 0; i < 15; i++)
                {
                    Vector2 velocity = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.5f, 1.5f) * 6;
                    new TurbulenceCube(Projectile.Center, velocity, Main.rand.NextBool() ? Color.White : Color.Turquoise, Main.rand.Next(25, 35), 0f, 1f, Main.rand.NextFloat(0.3f, 0.6f)).Spawn();
                }
                for (int i = 0; i < 5; i++)
                {
                    float rot = MathHelper.TwoPi / 5;
                    new Lightning02(Projectile.Center, Vector2.Zero, Main.rand.NextBool() ? Color.PaleTurquoise : Color.Turquoise, Main.rand.Next(25, 35),
                        rot * i + Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextFloat(0.1f, 0.2f)).Spawn();
                }
                new BloomShockwave(Projectile.Center, Vector2.Zero, Color.PaleTurquoise, 25, 1f, 0.15f).Spawn();
                new BloomShockwave(Projectile.Center, Vector2.Zero, Color.Turquoise, 35, 1f, 0.25f).Spawn();
                new CrossGlow(Projectile.Center, Vector2.Zero, Color.PaleTurquoise, 25, 1f, 0.35f).Spawn();
                new CrossGlow(Projectile.Center, Vector2.Zero, Color.Turquoise, 25, 1f, 0.45f).Spawn();

                for (int i = 0; i < 6; i++)
                {
                    Vector2 velocity = Projectile.rotation.ToRotationVector2().RotatedByRandom(MathHelper.PiOver4 * 0.4f) * Main.rand.NextFloat(0.5f, 1.5f) * 12;
                    new TurbulenceCube(Projectile.Center, velocity, Main.rand.NextBool() ? Color.White : Color.Turquoise, Main.rand.Next(25, 35), 0f, 1f, Main.rand.NextFloat(0.3f, 0.6f)).Spawn();
                }
            }
            for (int i = 0; i < 5; i++)
            {
                Vector2 vel = -Projectile.velocity / 5;
                new VortexGlowBall(Projectile.Center + vel * i, Vector2.Zero, Color.Turquoise, 25, 0.1f).Spawn();
            }
            new Lightning01(Projectile.Center, Vector2.Zero, Main.rand.NextBool() ? Color.PaleTurquoise : Color.Turquoise, Main.rand.Next(25, 45), Projectile.rotation + MathHelper.PiOver2, Main.rand.NextFloat(0.2f, 0.4f)).Spawn();
            NPC target = Projectile.FindClosestTarget(1500);
            if (target is not null)
            {
                Vector2 home = (target.Center - Projectile.Center).SafeNormalize(Vector2.UnitY);
                Vector2 velo = (Projectile.velocity * 45f + home * 12f) / (45f + 1f);
                //设定速度
                Projectile.velocity = velo;
            }
        }
        public override void OnKill(int timeLeft)
        {
            Projectile.ExpandHitboxBy((float)7);
            Projectile.Damage();
            for (int i = 0; i < 15; i++)
            {
                Vector2 velocity = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.5f, 1.5f) * 6;
                new TurbulenceCube(Projectile.Center, velocity, Main.rand.NextBool() ? Color.White : Color.Turquoise, Main.rand.Next(25, 35), 0f, 1f, Main.rand.NextFloat(0.3f, 0.6f)).Spawn();
            }
            
            new CrossGlow(Projectile.Center, Vector2.Zero, Color.PaleTurquoise, 25, 1f, 0.45f).Spawn();
            new BloomShockwave(Projectile.Center, Vector2.Zero, Color.PaleTurquoise, 25, 1f, 0.2f).Spawn();
            new BloomShockwave(Projectile.Center, Vector2.Zero, Color.Turquoise, 35, 1f, 0.3f).Spawn();

            Vector2 SpawnOffset = Projectile.Center + new Vector2(Main.rand.Next(-300, 300), Main.rand.Next(-800, -600));
            Vector2 FireVel = UCAUtilities.GetVector2(SpawnOffset, Projectile.Center) * 12;
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), SpawnOffset, FireVel, ModContent.ProjectileType<VortexLightning>(), Projectile.damage * 5, Projectile.knockBack, Projectile.owner);
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overWiresUI.Add(index);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            UCAUtilities.ReSetToBeginShader();
            DrawTrail(48);
            DrawTrail();
            UCAUtilities.ReSetToEndShader();
            return false;
        }
        public void DrawTrail(int height = 36)
        {
            Vector2 HalfProj = new Vector2(Projectile.width / 2, Projectile.height / 2);
            List<TrailDrawDate> trailDrawDate = [];
            DrawSetting drawSetting = new(UCATextureRegister.Thrust02.Value, false, false);
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                if (Projectile.oldPos[i] != Vector2.Zero)
                {
                    Vector2 DrawPos = Projectile.oldPos[i] - Main.screenPosition + HalfProj;

                    TrailDrawDate TrailDrawDate = new(DrawPos, Color.Turquoise, new Vector2(0, height), Projectile.oldRot[i]);
                    trailDrawDate.Add(TrailDrawDate);
                }
            }
            TrailRender.RenderTrail(trailDrawDate.ToArray(), drawSetting);
        }
    }
}
