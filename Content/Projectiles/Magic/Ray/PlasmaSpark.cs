using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Content.Particiles;
using UCA.Core.BaseClass;
using UCA.Core.Utilities;

namespace UCA.Content.Projectiles.Magic.Ray
{
    public class PlasmaSpark : BaseMagicProj
    {
        public override string Texture => UCATextureRegister.CollectableLightPath;

        public bool CanHit = true;

        public bool CanSplits => Projectile.ai[0] == 0;

        public NPC Target = null; 
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 240;
            Projectile.extraUpdates = 50;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.extraUpdates);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.extraUpdates = reader.ReadInt32();
        }
        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
        }
        public override bool? CanHitNPC(NPC target)
        {
            if (target.friendly)
                return false;
            else
                return CanHit;
        }
        public override void AI()
        {
            if (Projectile.UCA().FirstFrame)
            {
                Projectile.netUpdate = true;
            }
            if (!CanSplits)
            {
                if (Projectile.timeLeft > 160)
                    CanHit = false;
                else
                    CanHit = true;

                if (CanHit)
                    TrackTarget();

                Projectile.velocity *= 1.002f;
            }
            else
            Projectile.velocity *= 1.006f;

            if (Projectile.velocity.Length() > 0.5f)
            {
                if (CanSplits)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Color RandomColor = Color.Lerp(Color.Violet, Color.Purple, Main.rand.NextFloat(0, 1));
                        new MediumGlowBall(Projectile.Center + Projectile.velocity / 2 * i, -Projectile.velocity, RandomColor, 180, 0, 1, 0.12f, Main.rand.NextFloat(0.2f, 0.5f)).Spawn();
                    }
                }
                else
                {
                    if (Projectile.timeLeft % 4 == 0)
                    {
                        Color RandomColor = Color.Lerp(Color.Violet, Color.Purple, Main.rand.NextFloat(0, 1));
                        new MediumGlowBall(Projectile.Center + Projectile.velocity, -Projectile.velocity, RandomColor, 180, 0, 1, 0.12f, Main.rand.NextFloat(0.2f, 0.5f)).Spawn();
                    }
                }
            }

            Lighting.AddLight(Projectile.Center, Color.Violet.ToVector3() * 0.5f);
        }

        public void TrackTarget()
        {
            if (Target is null)
            {
                Target = Projectile.FindClosestTarget(600, true, true);
                Projectile.velocity *= 0.97f;
            }
            else
            {
                float inertia = 70f;
                float speed = 3f;
                //开始追踪target
                Vector2 home = (Target.Center - Projectile.Center).SafeNormalize(Vector2.UnitY);
                Vector2 velo = (Projectile.velocity * inertia + home * speed) / (inertia + 1f);
                Projectile.velocity = velo;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            // 只会在本地玩家进行分裂，同步依靠newProj的效果
            if (Projectile.owner != Main.myPlayer)
                return;

            if (CanSplits)
            {
                for (int i = 0; i < 15; i++)
                {
                    Color RandomColor = Color.Lerp(Color.Violet, Color.Purple, Main.rand.NextFloat(0, 1));
                    float offset = MathHelper.TwoPi / 15;
                    new MediumGlowBall(Projectile.Center, Projectile.velocity.RotatedBy(offset * i), RandomColor, 60, 0, 1, 0.2f, Main.rand.NextFloat(1.3f, 1.6f)).Spawn();
                }

                Vector2 baseVector = Vector2.UnitX.RotateRandom(MathHelper.TwoPi);

                for (int i = 0; i < 8; i++)
                {
                    float offset = MathHelper.TwoPi / 8;
                    int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, baseVector.RotatedBy(offset * i),
                        ModContent.ProjectileType<PlasmaSpark>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner, 1);
                    Main.projectile[p].extraUpdates = 5;
                }
            }
            else
            {
                for (int i = 0; i < 6; i++)
                {
                    Color RandomColor = Color.Lerp(Color.Violet, Color.Purple, Main.rand.NextFloat(0, 1));
                    float offset = MathHelper.TwoPi / 6;
                    new MediumGlowBall(Projectile.Center, Projectile.velocity.RotatedBy(offset * i), RandomColor, 60, 0, 1, 0.2f, Main.rand.NextFloat(1f, 1.2f)).Spawn();
                }
            }
            SoundEngine.PlaySound(SoundsMenu.PlasmaSparkHit, Projectile.Center);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.ShadowFlame, 180);
        }
    }
}
