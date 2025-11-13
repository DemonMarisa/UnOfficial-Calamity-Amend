using CalamityMod.Buffs.DamageOverTime;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Assets.Sounds;
using UCA.Content.Configs;
using UCA.Content.MetaBalls;
using UCA.Content.Particiles;
using UCA.Core.BaseClass;

namespace UCA.Content.Projectiles.Magic.Ray
{
    public class CarnageEnergy : BaseMagicProj
    {
        public override string Texture => UCATextureRegister.InvisibleTexturePath;

        public bool MainRay => Projectile.ai[0] != 0;

        public float ScaleMult => MainRay ? 1 : 0.6f;

        public int DustCount => UCAConfig.Instance.PerformanceMode ? 4 : 7;
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.extraUpdates = 5;
            Projectile.friendly = true;
            Projectile.timeLeft = 80;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10 * (Projectile.extraUpdates + 1);
        }
        public override void AI()
        {
            if (Projectile.LAP().FirstFrame)
            {
                if (UCAConfig.Instance.PerformanceMode)
                {
                    for (int i = 0; i < 25; i++)
                        new LilyLiquid(Projectile.Center, Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedByRandom(MathHelper.PiOver4 * 0.6f) * Main.rand.NextFloat(0f, 1.2f) * 12f, Color.Red, 64, 0, 1, 1.5f).Spawn();
                    for (int i = 0; i < 10; i++)
                        new LilyLiquid(Projectile.Center, Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedByRandom(MathHelper.PiOver4 * 0.6f) * Main.rand.NextFloat(0f, 1.2f) * 12f, Color.Black, 64, 0, 1, 1.5f).Spawn();
                }
                else
                {
                    for (int i = 0; i < 75; i++)
                        new LilyLiquid(Projectile.Center, Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedByRandom(MathHelper.PiOver4 * 0.6f) * Main.rand.NextFloat(0f, 1.2f) * 24f, Color.Red, 64, 0, 1, 1.5f).Spawn();
                    for (int i = 0; i < 35; i++)
                        new LilyLiquid(Projectile.Center, Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedByRandom(MathHelper.PiOver4 * 0.6f) * Main.rand.NextFloat(0f, 1.2f) * 24f, Color.Black, 64, 0, 1, 1.5f).Spawn();
                }
            }

            Projectile.velocity *= 1.03f;
            Projectile.rotation = Projectile.velocity.ToRotation();

            for (int i = 0; i < DustCount; i++)
            {
                Vector2 RandomOffset;

                if (UCAConfig.Instance.PerformanceMode)
                    RandomOffset = new Vector2(Main.rand.Next(-6, 6), Main.rand.Next(-6, 6));
                else
                    RandomOffset = new Vector2(Main.rand.Next(-9, 9), Main.rand.Next(-9, 9));

                CarnageMetaBall.SpawnParticle(Projectile.Center + Projectile.velocity / DustCount * i + RandomOffset,
                    Projectile.rotation.ToRotationVector2(),
                    Main.rand.NextFloat(0.4f, 0.55f) * ScaleMult,
                    Projectile.rotation);
            }
            if (UCAConfig.Instance.PerformanceMode)
            {
                if (Projectile.timeLeft % 6 == 0)
                {
                    Color color = Main.rand.NextBool(3) ? Color.Black : Color.DarkRed;
                    new BloodDrop(Projectile.Center, Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(0.1f, 1.1f) * -6f, color, Main.rand.Next(60, 90), 0, 1, 0.1f).Spawn();
                }
                for (int i = 0; i < 2; i++)
                {
                    new LilyLiquid(Projectile.Center + Projectile.velocity / DustCount * i, Projectile.velocity.RotatedByRandom(MathHelper.PiOver4 * 0.3f) * Main.rand.NextFloat(0f, 1f), Color.Black, 64, 0, 1, 0.6f).Spawn();

                    new LilyLiquid(Projectile.Center + Projectile.velocity / DustCount * i, Projectile.velocity.RotatedByRandom(MathHelper.PiOver4 * 0.3f) * Main.rand.NextFloat(0f, 1f), Color.Red, 64, 0, 1, 0.6f).Spawn();
                }
            }
            else
            {
                if (Projectile.timeLeft % 2 == 0)
                {
                    Color color = Main.rand.NextBool(3) ? Color.Black : Color.DarkRed;
                    new BloodDrop(Projectile.Center, Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(0.1f, 1.1f) * -12f, color, Main.rand.Next(60, 90), 0, 1, 0.1f).Spawn();
                }
                for (int i = 0; i < 1; i++)
                {
                    new LilyLiquid(Projectile.Center + Projectile.velocity / DustCount * i, Projectile.velocity.RotatedByRandom(MathHelper.PiOver4 * 0.3f) * Main.rand.NextFloat(0f, 1f), Color.Black, 64, 0, 1, 1.2f).Spawn();

                    new LilyLiquid(Projectile.Center + Projectile.velocity / DustCount * i, Projectile.velocity.RotatedByRandom(MathHelper.PiOver4 * 0.3f) * Main.rand.NextFloat(0f, 1f), Color.Red, 64, 0, 1, 1.2f).Spawn();
                }
            }

            if (Projectile.timeLeft % 15 == 0)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * 3f, ModContent.ProjectileType<CarnageBall>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner, 0);
                if (UCAConfig.Instance.PerformanceMode)
                    return;
                for (int i = 0; i < 15; i++)
                {
                    new LilyLiquid(Projectile.Center, Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0f, 1f) * 3f, Color.Black, 64, 0, 1, 1.2f).Spawn();

                    new LilyLiquid(Projectile.Center, Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0f, 1f) * 3f, Color.Red, 64, 0, 1, 1.2f).Spawn();
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (UCAConfig.Instance.PerformanceMode)
            {
                for (int i = 0; i < 15; i++)
                {
                    Vector2 spawnVec = Projectile.velocity.RotateRandom(0.3f) * Main.rand.NextFloat(0.1f, 1.1f);
                    CarnageMetaBall.SpawnParticle(Projectile.Center, spawnVec, Main.rand.NextFloat(0.1f, 0.2f) * ScaleMult, 0, true);
                }
                for (int i = 0; i < 5; i++)
                    new LilyLiquid(Projectile.Center + Projectile.velocity / DustCount * i, Projectile.velocity.RotatedByRandom(MathHelper.PiOver4 * 0.8f) * Main.rand.NextFloat(0f, 1f), Color.DarkRed, 64, 0, 1, 1.5f).Spawn();
                for (int i = 0; i < 10; i++)
                    new LilyLiquid(Projectile.Center + Projectile.velocity / DustCount * i, Projectile.velocity.RotatedByRandom(MathHelper.PiOver4 * 0.8f) * Main.rand.NextFloat(0f, 1f), Color.Black, 64, 0, 1, 1.5f).Spawn();
                SoundEngine.PlaySound(SoundsMenu.NightRayHit, Projectile.Center);
                return;
            }
            for (int i = 0; i < 25; i++)
            {
                Vector2 spawnVec = Projectile.velocity.RotateRandom(0.3f) * Main.rand.NextFloat(0.1f, 1.1f);
                CarnageMetaBall.SpawnParticle(Projectile.Center, spawnVec, Main.rand.NextFloat(0.25f, 0.4f) * ScaleMult, 0, true);
            }
            for (int i = 0; i < 10; i++)
            {
                new LilyLiquid(Projectile.Center + Projectile.velocity / DustCount * i, Projectile.velocity.RotatedByRandom(MathHelper.PiOver4 * 0.8f) * Main.rand.NextFloat(0f, 1f), Color.DarkRed, 64, 0, 1, 1.5f).Spawn();
            }
            for (int i = 0; i < 20; i++)
            {
                new LilyLiquid(Projectile.Center + Projectile.velocity / DustCount * i, Projectile.velocity.RotatedByRandom(MathHelper.PiOver4 * 0.8f) * Main.rand.NextFloat(0f, 1f), Color.Black, 64, 0, 1, 1.5f).Spawn();
            }
            SoundEngine.PlaySound(SoundsMenu.NightRayHit, Projectile.Center);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            Player player = Main.player[Projectile.owner];
            float distanecToNPC = Vector2.Distance(player.Center, target.Center);

            float mult;
            int mindistance = 250;
            if (distanecToNPC < mindistance)
                mult = 0.75f;
            else
                mult = MathHelper.Lerp(0.75f, 1.5f, (distanecToNPC - mindistance) / 450f);
            mult = MathHelper.Clamp(mult, 0.75f, 1.5f);

            modifiers.FinalDamage *= mult;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<BurningBlood>(), 180);
        }
    }
}
