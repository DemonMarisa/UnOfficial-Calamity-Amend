using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Content.MetaBalls;
using UCA.Content.Projectiles.HeldProj.Magic;
using UCA.Content.Projectiles.HeldProj.Magic.NightRatHeld;
using UCA.Core.BaseClass;
using UCA.Core.Utilities;

namespace UCA.Content.Projectiles.Magic.Ray
{
    public class NightEnergySplit : BaseMagicProj
    {
        public override string Texture => UCATextureRegister.InvisibleTexturePath;

        public float ScaleMult = 0.6f;

        public int DustCount = 4;

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
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.extraUpdates);
            writer.Write(Projectile.penetrate);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.extraUpdates = reader.ReadInt32();
            Projectile.penetrate = reader.ReadInt32();
        }
        public override void AI()
        {
            if (Projectile.UCA().FirstFrame)
            {
                NightRayHeldProj.GenUnDeathSign(Projectile.Center, Projectile.ai[0]);
                Projectile.netUpdate = true;
            }
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

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Vector2 spawnVec = Projectile.velocity.RotateRandom(0.2f) * Main.rand.NextFloat(0.05f, 0.6f);
                ShadowMetaBall.SpawnParticle(Projectile.Center, spawnVec, Main.rand.NextFloat(0.15f, 0.2f) * ScaleMult);
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
            target.AddBuff(BuffID.ShadowFlame, 180);

            if (!Projectile.UCA().OnceHitEffect)
                return;

            for (int i = 0; i < 10; i++)
            {
                Vector2 spawnVec = Projectile.velocity.RotateRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.2f, 0.3f);
                ShadowMetaBall.SpawnParticle(target.Center, spawnVec, 0.3f * ScaleMult);
            }
        }

    }
}
