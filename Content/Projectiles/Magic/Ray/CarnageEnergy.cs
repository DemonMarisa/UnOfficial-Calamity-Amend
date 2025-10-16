using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatBuffs;
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
using UCA.Assets;
using UCA.Content.MetaBalls;
using UCA.Content.Particiles;
using UCA.Content.Projectiles.HeldProj.Magic;
using UCA.Core.BaseClass;
using UCA.Core.Utilities;

namespace UCA.Content.Projectiles.Magic.Ray
{
    public class CarnageEnergy : BaseMagicProj
    {
        public override string Texture => UCATextureRegister.InvisibleTexturePath;

        public bool MainRay => Projectile.ai[0] != 0;

        public float ScaleMult => MainRay ? 1 : 0.6f;

        public int DustCount => MainRay ? 7 : 4;
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
            if (Projectile.UCA().FirstFrame)
            {
                for (int i = 0; i < 75; i++)
                {
                    new LilyLiquid(Projectile.Center, Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedByRandom(MathHelper.PiOver4 * 0.6f) * Main.rand.NextFloat(0f, 1.2f) * 24f, Color.Red, 64, 0, 1, 1.5f).Spawn();
                }
                for (int i = 0; i < 35; i++)
                {
                    new LilyLiquid(Projectile.Center, Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedByRandom(MathHelper.PiOver4 * 0.6f) * Main.rand.NextFloat(0f, 1.2f) * 24f, Color.Black, 64, 0, 1, 1.5f).Spawn();
                }
            }

            Projectile.velocity *= 1.03f;
            Projectile.rotation = Projectile.velocity.ToRotation();

            for (int i = 0; i < DustCount; i++)
            {
                CarnageMetaBall.SpawnParticle(Projectile.Center + Projectile.velocity / DustCount * i + new Vector2(Main.rand.Next(-9, 9), Main.rand.Next(-9, 9)),
                    Projectile.rotation.ToRotationVector2(),
                    Main.rand.NextFloat(0.4f, 0.55f) * ScaleMult,
                    Projectile.rotation);
            }

            if (Projectile.timeLeft % 2 == 0)
            {
                Color color = Main.rand.NextBool(3) ? Color.Black : Color.DarkRed;
                new BloodDrop(Projectile.Center, Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(0.1f, 1.1f) * -12f, color, Main.rand.Next(60, 90), 0, 1, 0.1f).Spawn();
            }

            for (int i = 0; i < 2; i++)
            {
                new LilyLiquid(Projectile.Center + Projectile.velocity / DustCount * i, Projectile.velocity.RotatedByRandom(MathHelper.PiOver4 * 0.3f) * Main.rand.NextFloat(0f, 1f), Color.Black, 64, 0, 1, 1.2f).Spawn();

                new LilyLiquid(Projectile.Center + Projectile.velocity / DustCount * i, Projectile.velocity.RotatedByRandom(MathHelper.PiOver4 * 0.3f) * Main.rand.NextFloat(0f, 1f), Color.Red, 64, 0, 1, 1.2f).Spawn();
            }

            if (Projectile.timeLeft % 15 == 0)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * 3f, ModContent.ProjectileType<CarnageBall>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner, 0);
                for (int i = 0; i < 15; i++)
                {
                    new LilyLiquid(Projectile.Center, Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0f, 1f) * 3f, Color.Black, 64, 0, 1, 1.2f).Spawn();

                    new LilyLiquid(Projectile.Center, Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0f, 1f) * 3f, Color.Red, 64, 0, 1, 1.2f).Spawn();
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
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
            float mult = 1f;
            int mindistance = 300;
            if (distanecToNPC < mindistance)
                mult = 0.75f;
            else
                mult = MathHelper.Clamp(2f - (distanecToNPC - mindistance) / 600f, 2f, 0.75f);

            modifiers.FinalDamage *= mult;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

            target.AddBuff(ModContent.BuffType<BurningBlood>(), 180);
        }
    }
}
