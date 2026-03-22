using LAP.Assets.TextureRegister;
using LAP.Content.Configs;
using LAP.Core.Presets.Content;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Assets.Sounds;
using UCA.Content.MetaBalls;
using UCA.Content.Particiles;
using UCA.Core.BaseClass;

namespace UCA.Content.Projectiles.Magic.Ray
{
    public class NightEnergyHeavy : BaseMagicProj
    {
        public override string Texture => LAPTextureRegister.InvisibleTexturePath;

        public float ScaleMult = 1;

        public int DustCount = 7;

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.extraUpdates = 5;
            Projectile.friendly = true;
            Projectile.timeLeft = 99;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10 * (Projectile.extraUpdates + 1);
            Projectile.netImportant = true;
        }
        public override void AI()
        {
            if (Projectile.LAP().FirstFrame)
            {
                ParticlePreset.GenUnDeathSign(Projectile.Center, Projectile.ai[0]);
                for (int i = 0; i < 10; i++)
                {
                    Color color = Color.Lerp(Color.DarkOrchid, Color.DarkViolet, Main.rand.NextFloat(0, 1f));
                    new Line(Projectile.Center, Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(3, 7), color, Main.rand.Next(60, 90), 0, 1, 0.1f, false, Projectile.Center).Spawn();
                }
                SoundEngine.PlaySound(SoundsMenu.NightRayAttack, Projectile.Center);
                Projectile.netUpdate = true;
            }
            Vector2 Offset;
            if (LAPConfig.Instance.PerformanceMode)
            {
                DustCount = 4;
                Offset = new Vector2(Main.rand.Next(-1, 1), Main.rand.Next(-1, 1));
            }
            else
                Offset = new Vector2(Main.rand.Next(-2, 2), Main.rand.Next(-2, 2));
            for (int i = 0; i < DustCount; i++)
            {
                ShadowMetaBall.SpawnParticle(Projectile.Center + Projectile.velocity / DustCount * i + Offset,
                    new Vector2(1f, 0).RotatedBy(Main.rand.NextFloat(-6, 6)) * Main.rand.NextFloat(0, 1),
                    Main.rand.NextFloat(0.10f, 0.15f) * ScaleMult);
            }

            Projectile.velocity *= 1.03f;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.timeLeft % 20 == 0)
            {
                NPC npc = LAPUtilities.FindClosestTarget(Projectile.Center, 1500, true);
                if (npc is not null)
                {
                    float DistanceToNPC = Vector2.Distance(Projectile.Center, npc.Center);
                    float PredictMult = DistanceToNPC / 48;
                    Vector2 direction = (npc.Center + npc.velocity * PredictMult - Projectile.Center).SafeNormalize(Vector2.Zero) * 3;
                    if (Projectile.owner == Main.myPlayer)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, direction, ProjectileType<NightEnergySplit>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner, 0.6f);
                    }
                }
                else
                {
                    if (Projectile.owner == Main.myPlayer)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity.SafeNormalize(Vector2.UnitY).RotatedBy(MathHelper.PiOver2 * (Main.rand.NextBool() ? 1 : -1)),
                            ProjectileType<NightEnergySplit>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner, 0.6f);
                    }
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 25; i++)
            {
                Vector2 spawnVec = Projectile.velocity.RotateRandom(0.3f) * Main.rand.NextFloat(0.1f, 1.1f);
                ShadowMetaBall.SpawnParticle(Projectile.Center, spawnVec, Main.rand.NextFloat(0.15f, 0.2f) * ScaleMult);
            }

            SoundEngine.PlaySound(SoundsMenu.NightRayHit, Projectile.Center);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            Player player = Main.player[Projectile.owner];
            float distanecToNPC = Vector2.Distance(player.Center, target.Center);
            float mult;
            int mindistance = 150;
            if (distanecToNPC < mindistance)
                mult = 2f;
            else
                mult = MathHelper.Clamp(2f - (distanecToNPC - mindistance) / 450f, 0.75f, 2f);

            modifiers.FinalDamage *= mult;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.ShadowFlame, 180);

            if (!Projectile.LAP().OnceHitEffect)
                return;

            for (int i = 0; i < 10; i++)
            {
                Vector2 spawnVec = Projectile.velocity.RotateRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.2f, 0.3f);
                ShadowMetaBall.SpawnParticle(target.Center, spawnVec, 0.3f * ScaleMult);
            }
        }
    }
}
