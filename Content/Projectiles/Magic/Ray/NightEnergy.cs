using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Content.MetaBalls;
using UCA.Content.Projectiles.HeldProj.Magic;
using UCA.Core.BaseClass;
using UCA.Core.Utilities;

namespace UCA.Content.Projectiles.Magic.Ray
{
    public class NightEnergy : BaseMagicProj
    {
        public override string Texture => UCATextureRegister.InvisibleTexturePath;

        public bool MainRay => Projectile.ai[0] == 0 ? false : true;

        public float ScaleMult => MainRay ? 1 : 0.6f;

        public int DustCount => MainRay ? 7 : 4;

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.extraUpdates = 5;
            Projectile.friendly = true;
            Projectile.timeLeft = 79;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10 * (Projectile.extraUpdates + 1);
        }

        public override void AI()
        {

            for (int i = 0; i < DustCount; i++)
            {
                ShadowMetaBall.SpawnParticle(Projectile.Center + Projectile.velocity / DustCount * i + new Vector2(Main.rand.Next(-2, 2), Main.rand.Next(-2, 2)),
                    new Vector2(1f, 0).RotatedBy(Main.rand.NextFloat(-6, 6)) * Main.rand.NextFloat(0, 1),
                    Main.rand.NextFloat(0.10f, 0.15f) * ScaleMult);
            }
            
            Projectile.velocity *= 1.03f;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.timeLeft % 20 == 0)
            {
                if (MainRay)
                {
                    NPC npc = Projectile.FindClosestTarget(1500, true);
                    NightRayHeldProj.GenUnDeathSign(Projectile.Center, 0.6f);
                    if (npc is not null)
                    {
                        float DistanceToNPC = Vector2.Distance(Projectile.Center, npc.Center);
                        float PredictMult = DistanceToNPC / 48;
                        Vector2 direction = (npc.Center + npc.velocity * PredictMult - Projectile.Center).SafeNormalize(Vector2.Zero) * 3;
                        int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, direction, ModContent.ProjectileType<NightEnergy>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner, 0);
                        Main.projectile[p].penetrate = 1;
                    }
                    else
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity.SafeNormalize(Vector2.UnitY).RotatedBy(MathHelper.PiOver2 * (Main.rand.NextBool() ? 1 : -1)),
                            ModContent.ProjectileType<NightEnergy>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner, 0);
                    }
                }
                else
                {
                    int dustCounts = 15;
                    float rotArg = 360f / dustCounts;
                    for (int i = 0; i < dustCounts; i++)
                    {
                        float rorate = MathHelper.ToRadians(i * rotArg);
                        Vector2 dustVelocity = new Vector2(2, 0).BetterRotatedBy(rorate, default);
                        dustVelocity = new Vector2(dustVelocity.X * 0.5f, dustVelocity.Y);
                        dustVelocity = dustVelocity.RotatedBy(Projectile.rotation);

                        ShadowMetaBall.SpawnParticle(Projectile.Center,
                            dustVelocity,
                            0.2f * ScaleMult);
                    }
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (MainRay)
            {
                for (int i = 0; i < 25; i++)
                {
                    Vector2 spawnVec = Projectile.velocity.RotateRandom(0.3f) * Main.rand.NextFloat(0.1f, 1.1f);
                    ShadowMetaBall.SpawnParticle(Projectile.Center, spawnVec, Main.rand.NextFloat(0.15f, 0.2f) * ScaleMult);
                }
            }
            else
            {
                for (int i = 0; i < 10; i++)
                {
                    Vector2 spawnVec = Projectile.velocity.RotateRandom(0.2f) * Main.rand.NextFloat(0.05f, 0.6f);
                    ShadowMetaBall.SpawnParticle(Projectile.Center, spawnVec, Main.rand.NextFloat(0.15f, 0.2f) * ScaleMult);
                }
            }

            SoundEngine.PlaySound(SoundsMenu.NightRayHit, Projectile.Center);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            Player player = Main.player[Projectile.owner];
            float distanecToNPC = Vector2.Distance(player.Center, target.Center);
            float mult = 1f;
            int mindistance = 300;
            if (distanecToNPC < mindistance)
                mult = 2f;
            else
                mult = MathHelper.Clamp(2f - (distanecToNPC - mindistance) / 600f, 0.75f, 2f);

            modifiers.FinalDamage *= mult;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 10; i++)
            {
                Vector2 spawnVec = Projectile.velocity.RotateRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.2f, 0.3f);
                ShadowMetaBall.SpawnParticle(target.Center, spawnVec, 0.3f * ScaleMult);
            }

            target.AddBuff(BuffID.ShadowFlame, 180);
        }
    }
}
