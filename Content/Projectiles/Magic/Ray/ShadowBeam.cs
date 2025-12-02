using LAP.Content.Configs;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Content.Configs;
using UCA.Content.Particiles;
using UCA.Core.BaseClass;

namespace UCA.Content.Projectiles.Magic.Ray
{
    public class ShadowBeam : BaseMagicProj
    {
        public override string Texture => UCATextureRegister.InvisibleTexturePath;
        public Vector2 OldCollidePos = Vector2.Zero;
        public int HitCount = 0;
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 10;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 240;
            Projectile.extraUpdates = 50;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;

        }
        public override void AI()
        {
            FirstFrame();
            Projectile.rotation = Projectile.velocity.ToRotation();
            GenDust();
        }
        public void GenDust()
        {
            if (LAPConfig.Instance.PerformanceMode)
            {
                if (LAPUtilities.OutOffScreen(Projectile.Center, 0.3f))
                    return;
            }
            else if (LAPUtilities.OutOffScreen(Projectile.Center, 2f))
            {
                    return;
            }

            Vector2 offset = Main.rand.NextVector2Circular(12, 12) + new Vector2(-48, 0).RotatedBy(Projectile.rotation);
            Color color = LAPUtilities.LerpColor(Color.Violet, Color.DarkViolet);
            Vector2 fireVelocity = Projectile.velocity.SafeNormalize(Vector2.Zero);
            new TrailGlowBall(Projectile.Center + offset, fireVelocity * 4.5f, color, 90, 0.1f, true).Spawn();
            Color Firecolor = LAPUtilities.LerpColor(Color.Black, Color.DarkViolet);
            new Fire(Projectile.Center, fireVelocity * 4.5f, Firecolor, 90, Main.rand.NextFloat(MathHelper.TwoPi), 1f, 0.2f).SpawnToPriorityNonPreMult();
            for (int i = 0; i < 3; i++)
            {
                Vector2 VecOffset = Projectile.velocity / 3f;
                new MediumGlowBall(Projectile.Center + VecOffset * i, fireVelocity * 0.5f, Color.Violet, 60, 0, 1, 0.15f, 0.4f).Spawn();
            }
        }
        public void FirstFrame()
        {
            if (!Projectile.LAP().FirstFrame)
                return;
            new CrossGlow(Projectile.Center, Vector2.Zero, Color.Violet, 30, 1f, 0.4f).Spawn();
            new CrossGlow(Projectile.Center, Vector2.Zero, Color.DarkViolet, 30, 1f, 0.4f).Spawn();
            for (int i = 0; i < 10; i++)
            {
                Color Firecolor = LAPUtilities.LerpColor(Color.Black, Color.DarkViolet);
                new Fire(Projectile.Center, Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.2f, 1.2f) * 4, Firecolor, 90, Main.rand.NextFloat(MathHelper.TwoPi), 1f, 0.3f).SpawnToPriorityNonPreMult();
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.damage = (int)(Projectile.damage * 1.15f);
            Projectile.penetrate--;
            if (Projectile.penetrate <= 0)
                Projectile.Kill();
            else
            {
                if (Projectile.velocity.X != oldVelocity.X)
                {
                    Projectile.velocity.X = 0f - oldVelocity.X;
                }

                if (Projectile.velocity.Y != oldVelocity.Y)
                {
                    Projectile.velocity.Y = 0f - oldVelocity.Y;
                }
            }
            if (OldCollidePos != Vector2.Zero)
            {
                float DistanceToOldP = Vector2.Distance(OldCollidePos, Projectile.Center);
                if (DistanceToOldP < 48)
                    return false;
            }
            new CrossGlow(Projectile.Center, Vector2.Zero, Color.Violet, 30, 1f, 0.4f).Spawn();
            new CrossGlow(Projectile.Center, Vector2.Zero, Color.DarkViolet, 30, 1f, 0.4f).Spawn();
            for (int i = 0; i < 10; i++)
            {
                Color Firecolor = LAPUtilities.LerpColor(Color.Black, Color.DarkViolet);
                new Fire(Projectile.Center, Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.2f, 1.2f) * 4, Firecolor, 90, Main.rand.NextFloat(MathHelper.TwoPi), 1f, 0.3f).SpawnToPriorityNonPreMult();
            }
            OldCollidePos = Projectile.Center;
            HitCount++;
            return false;
        }


        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (HitCount > 12)
                HitCount = 12;
            modifiers.SourceDamage *= 1 + 0.15f * HitCount;
            HitCount++;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.ShadowFlame, 300);
        }
        public override void OnKill(int timeLeft)
        {
            new CrossGlow(Projectile.Center, Vector2.Zero, Color.Violet, 30, 1f, 0.4f).Spawn();
            new CrossGlow(Projectile.Center, Vector2.Zero, Color.DarkViolet, 30, 1f, 0.4f).Spawn();
            for (int i = 0; i < 10; i++)
            {
                Color Firecolor = LAPUtilities.LerpColor(Color.Black, Color.DarkViolet);
                new Fire(Projectile.Center, Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.2f, 1.2f) * 4, Firecolor, 90, Main.rand.NextFloat(MathHelper.TwoPi), 1f, 0.3f).SpawnToPriorityNonPreMult();
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
