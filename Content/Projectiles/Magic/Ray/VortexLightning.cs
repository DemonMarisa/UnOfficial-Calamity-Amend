using CalamityMod;
using Microsoft.Xna.Framework;
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
    public class VortexLightning : BaseMagicProj
    {
        public override string Texture => UCATextureRegister.InvisibleTexturePath;
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
            if (Projectile.UCA().FirstFrame)
            {
                for (int i = 0; i < 15; i++)
                {
                    Vector2 velocity = Vector2.UnitX * Main.rand.NextFloat(0.5f, 1.5f) * 6 * (Main.rand.NextBool() ? 1 : -1);
                    new TurbulenceCube(Projectile.Center, velocity, Main.rand.NextBool() ? Color.White : Color.Turquoise, Main.rand.Next(25, 35), 0f, 1f, Main.rand.NextFloat(0.3f, 0.6f)).Spawn();
                }
                new CrossGlow(Projectile.Center, Vector2.Zero, Color.PaleTurquoise, 25, 1f, 0.35f).Spawn();
                new CrossGlow(Projectile.Center, Vector2.Zero, Color.Turquoise, 25, 1f, 0.45f).Spawn();
                for (int i = 0; i < 6; i++)
                {
                    Vector2 velocity = Projectile.rotation.ToRotationVector2().RotatedByRandom(MathHelper.PiOver4 * 0.4f) * Main.rand.NextFloat(0.5f, 1.5f) * 12;
                    new TurbulenceCube(Projectile.Center, velocity, Main.rand.NextBool() ? Color.White : Color.Turquoise, Main.rand.Next(25, 35), 0f, 1f, Main.rand.NextFloat(0.3f, 0.6f)).Spawn();
                }
                new Lightning03(Projectile.Center, Vector2.Zero, Main.rand.NextBool() ? Color.PaleTurquoise : Color.Turquoise, Main.rand.Next(25, 45), Projectile.rotation, Main.rand.NextFloat(0.2f, 0.4f)).Spawn();
                new Lightning03(Projectile.Center, Vector2.Zero, Main.rand.NextBool() ? Color.PaleTurquoise : Color.Turquoise, Main.rand.Next(25, 45), Projectile.rotation, Main.rand.NextFloat(0.2f, 0.4f)).Spawn();
            }
            for (int i = 0; i < 5; i++)
            {
                Vector2 vel = -Projectile.velocity / 5;
                new VortexGlowBall(Projectile.Center + vel * i, Vector2.Zero, Color.Turquoise, 25, 0.1f).Spawn();
            }

            new Lightning01(Projectile.Center, Vector2.Zero, Main.rand.NextBool() ? Color.PaleTurquoise : Color.Turquoise, Main.rand.Next(25, 45), Projectile.rotation + MathHelper.PiOver2, Main.rand.NextFloat(0.2f, 0.4f)).Spawn();
        }
        public override void OnKill(int timeLeft)
        {
            Projectile.ExpandHitboxBy((float)7);
            Projectile.Damage();

            for (int i = 0; i < 15; i++)
            {
                Vector2 velocity = Vector2.UnitX * Main.rand.NextFloat(0.5f, 1.5f) * 6 * (Main.rand.NextBool() ? 1 : -1);
                new TurbulenceCube(Projectile.Center, velocity, Main.rand.NextBool() ? Color.White : Color.Turquoise, Main.rand.Next(25, 35), 0f, 1f, Main.rand.NextFloat(0.3f, 0.6f)).Spawn();
            }
            new CrossGlow(Projectile.Center, Vector2.Zero, Color.PaleTurquoise, 25, 1f, 0.45f).Spawn();
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
