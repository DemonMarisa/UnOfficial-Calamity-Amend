using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Content.MetaBalls;
using UCA.Content.Particiles;
using UCA.Content.Projectiles.HealPRoj;
using UCA.Core.BaseClass;
using UCA.Core.Utilities;

namespace UCA.Content.Projectiles.Magic.Ray
{
    public class CarnageBall : BaseMagicProj
    {
        public override string Texture => UCATextureRegister.InvisibleTexturePath;

        public NPC HomeInTarget = null;

        public bool CanSpawnHeal => Projectile.ai[0] != 0;

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.extraUpdates = 5;
            Projectile.friendly = true;
            Projectile.timeLeft = 180;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = true;
            Projectile.penetrate = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10 * (Projectile.extraUpdates + 1);
        }

        public override void AI()
        {
            CarnageMetaBall.SpawnParticle(Projectile.Center,
                Projectile.rotation.ToRotationVector2(),
                0.15f,
                Projectile.rotation,
                true);
            
            if (Projectile.timeLeft % 3 == 0)
            {
                Color color = Main.rand.NextBool(3) ? Color.Black : Color.DarkRed;
                new LilyLiquid(Projectile.Center, Projectile.velocity * Main.rand.NextFloat(0.1f, 1.1f), color, Main.rand.Next(60, 90), 0, 1, 1f).Spawn();
            }
            
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (HomeInTarget is null)
            {
                HomeInTarget = Projectile.FindClosestTarget(1500, true, true);
                Projectile.velocity *= 0.97f;
            }
            else
            {
                #region 生成圆环
                if (Projectile.timeLeft % 25 == 0)
                {
                    int dustCounts = 15;
                    float rotArg = 360f / dustCounts;
                    for (int i = 0; i < dustCounts; i++)
                    {
                        float rorate = MathHelper.ToRadians(i * rotArg);
                        Vector2 dustVelocity = new Vector2(2, 0).BetterRotatedBy(rorate, default);
                        dustVelocity = new Vector2(dustVelocity.X * 0.5f, dustVelocity.Y);
                        dustVelocity = dustVelocity.RotatedBy(Projectile.rotation);

                        CarnageMetaBall.SpawnParticle(Projectile.Center, dustVelocity, 0.2f, 0, true);
                    }
                }
                #endregion
                float inertia = 35f;
                float speed = 9f;
                //开始追踪target
                Vector2 home = (HomeInTarget.Center - Projectile.Center).SafeNormalize(Vector2.UnitY);
                Vector2 velo = (Projectile.velocity * inertia + home * speed) / (inertia + 1f);
                Projectile.velocity = velo;
            }
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 15; i++)
            {
                Vector2 spawnVec = Projectile.velocity.RotateRandom(0.3f) * Main.rand.NextFloat(0.1f, 1.6f);
                CarnageMetaBall.SpawnParticle(Projectile.Center, spawnVec, Main.rand.NextFloat(0.15f, 0.3f), 0, true);
            }
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
                mult = MathHelper.Clamp(2f - (distanecToNPC - mindistance) / 600f, 1.5f, 0.75f);

            modifiers.FinalDamage *= mult;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 10; i++)
            {
                Vector2 spawnVec = Vector2.UnitX.RotateRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.1f, 0.5f) * 10f;
                CarnageMetaBall.SpawnParticle(target.Center, spawnVec, 0.3f, 0, true);
            }

            if (CanSpawnHeal)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * 3f, ModContent.ProjectileType<CarnageHeal>(), 0, Projectile.knockBack, Projectile.owner, 1);
        }
    }
}
