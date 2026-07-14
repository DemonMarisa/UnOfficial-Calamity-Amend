using LAP.Content.Configs;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using UCA.Content.Particiles;
using UCA.Core.BaseClass;
using LAP.Assets.TextureRegister;
using LAP.Core.Presets.Content;

namespace UCA.Content.Projectiles.Magic.Ray
{
    public class VortexLightning : BaseMagicProj
    {
        public override string Texture => LAPTextureRegister.InvisibleTexturePath;
        public bool CanHit = true;
        public int MaxLife = 75;
        public Vector2 TargetVelocity;
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.extraUpdates = 50;
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
            if (Projectile.LAP().FirstFrame)
            {
                if (!LAPConfig.Instance.PerformanceMode)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        Vector2 velocity = Vector2.UnitX * Main.rand.NextFloat(0.5f, 1.5f) * 6 * (Main.rand.NextBool() ? 1 : -1);
                        new TurbulenceCube(Projectile.Center, velocity, Main.rand.NextBool() ? Color.White : Color.Turquoise, Main.rand.Next(25, 35), 0f, 1f, Main.rand.NextFloat(0.3f, 0.6f)).Spawn();
                    }
                    new CrossGlow(Projectile.Center, Vector2.Zero, Color.PaleTurquoise, 25, 1f, 0.35f).Spawn();
                    new CrossGlow(Projectile.Center, Vector2.Zero, Color.Turquoise, 25, 1f, 0.45f).Spawn();
                    for (int i = 0; i < 4; i++)
                    {
                        Vector2 velocity = Projectile.rotation.ToRotationVector2().RotatedByRandom(MathHelper.PiOver4 * 0.4f) * Main.rand.NextFloat(0.5f, 1.5f) * 12;
                        new TurbulenceCube(Projectile.Center, velocity, Main.rand.NextBool() ? Color.White : Color.Turquoise, Main.rand.Next(25, 35), 0f, 1f, Main.rand.NextFloat(0.3f, 0.6f)).Spawn();
                    }
                    ParticlePreset.NewLightning03(Projectile.Center, Vector2.Zero, Main.rand.NextBool() ? Color.PaleTurquoise : Color.Turquoise, Main.rand.Next(25, 45), Main.rand.NextFloat(0.2f, 0.4f), Projectile.rotation);
                    ParticlePreset.NewLightning03(Projectile.Center, Vector2.Zero, Main.rand.NextBool() ? Color.PaleTurquoise : Color.Turquoise, Main.rand.Next(25, 45), Main.rand.NextFloat(0.2f, 0.4f), Projectile.rotation);
                }
                else
                {
                    for (int i = 0; i < 5; i++)
                    {
                        Vector2 velocity = Vector2.UnitX * Main.rand.NextFloat(0.5f, 1.5f) * 4 * (Main.rand.NextBool() ? 1 : -1);
                        new TurbulenceCube(Projectile.Center, velocity, Main.rand.NextBool() ? Color.White : Color.Turquoise, Main.rand.Next(15, 25), 0f, 1f, Main.rand.NextFloat(0.3f, 0.6f)).Spawn();
                    }
                    new CrossGlow(Projectile.Center, Vector2.Zero, Color.Turquoise, 25, 1f, 0.25f).Spawn();
                    for (int i = 0; i < 3; i++)
                    {
                        Vector2 velocity = Projectile.rotation.ToRotationVector2().RotatedByRandom(MathHelper.PiOver4 * 0.4f) * Main.rand.NextFloat(0.5f, 1.5f) * 6;
                        new TurbulenceCube(Projectile.Center, velocity, Main.rand.NextBool() ? Color.White : Color.Turquoise, Main.rand.Next(25, 35), 0f, 1f, Main.rand.NextFloat(0.3f, 0.6f)).Spawn();
                    }
                    ParticlePreset.NewLightning03(Projectile.Center, Vector2.Zero, Main.rand.NextBool() ? Color.PaleTurquoise : Color.Turquoise, Main.rand.Next(25, 45), Main.rand.NextFloat(0.2f, 0.4f), Projectile.rotation);
                    ParticlePreset.NewLightning03(Projectile.Center, Vector2.Zero, Main.rand.NextBool() ? Color.PaleTurquoise : Color.Turquoise, Main.rand.Next(25, 45), Main.rand.NextFloat(0.2f, 0.4f), Projectile.rotation);
                }  
            }
            if(Projectile.velocity != Vector2.Zero)
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector2 vel = -Projectile.velocity / 4;
                    ParticlePreset.NewTGlowBall(Projectile.Center + vel * i, Vector2.Zero, Color.Turquoise, 25, 0.15f);
                }
                ParticlePreset.NewLightning01(Projectile.Center, Vector2.Zero, Main.rand.NextBool() ? Color.PaleTurquoise : Color.Turquoise, Main.rand.Next(25, 45), Main.rand.NextFloat(0.2f, 0.4f), Projectile.rotation + MathHelper.PiOver2);
            }
        }
        public override void OnKill(int timeLeft)
        {
            Projectile.Resize(150, 150);
            Projectile.Damage();
            if (!LAPConfig.Instance.PerformanceMode)
            {
                for (int i = 0; i < 10; i++)
                {
                    Vector2 velocity = Vector2.UnitX * Main.rand.NextFloat(0.5f, 1.5f) * 6 * (Main.rand.NextBool() ? 1 : -1);
                    new TurbulenceCube(Projectile.Center, velocity, Main.rand.NextBool() ? Color.White : Color.Turquoise, Main.rand.Next(25, 35), 0f, 1f, Main.rand.NextFloat(0.3f, 0.6f)).Spawn();
                }
                new CrossGlow(Projectile.Center, Vector2.Zero, Color.PaleTurquoise, 25, 1f, 0.45f).Spawn();
            }
            else
            {
                for (int i = 0; i < 5; i++)
                {
                    Vector2 velocity = Vector2.UnitX * Main.rand.NextFloat(0.5f, 1.5f) * 3 * (Main.rand.NextBool() ? 1 : -1);
                    new TurbulenceCube(Projectile.Center, velocity, Main.rand.NextBool() ? Color.White : Color.Turquoise, Main.rand.Next(25, 35), 0f, 1f, Main.rand.NextFloat(0.3f, 0.6f)).Spawn();
                }
                new CrossGlow(Projectile.Center, Vector2.Zero, Color.PaleTurquoise, 15, 1f, 0.25f).Spawn();
            }
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overWiresUI.Add(index);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
