using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Content.Particiles;
using UCA.Content.Projectiles.HeldProj.Magic;
using UCA.Core.BaseClass;
using UCA.Core.Utilities;

namespace UCA.Content.Projectiles.Magic.Ray
{
    public class TerraEnergy : BaseMagicProj
    {
        public override string Texture => UCATextureRegister.InvisibleTexturePath;
        public bool MainRay => Projectile.ai[0] == 0 ? false : true;
        public float ScaleMult => MainRay ? 1 : 0.6f;
        public int DustCount => MainRay ? 7 : 4; 
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.extraUpdates = 20;
            Projectile.friendly = true;
            Projectile.timeLeft = 210;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10 * (Projectile.extraUpdates + 1);
        }

        public override void OnSpawn(IEntitySource source)
        {
        }

        public override void AI()
        {/*
            for (int i = 0; i < 2; i++)
            {
                Color RandomColor = Color.Lerp(Color.LightGreen, Color.Green, Main.rand.NextFloat(0, 1));
                new MediumGlowBall(Projectile.Center + Projectile.velocity / 2 * i, -Projectile.velocity, RandomColor, 180, 0, 1, 0.12f, Main.rand.NextFloat(1f, 2)).Spawn();
            }
            */
            if (Projectile.timeLeft % 2 == 0)
            {
                Color RandomColor = Color.Lerp(Color.LightGreen, Color.Green, Main.rand.NextFloat(0, 1));
                new MediumGlowBall(Projectile.Center + Projectile.velocity, -Projectile.velocity, RandomColor, 60, 0, 1, 0.12f, Main.rand.NextFloat(1f, 1.3f)).Spawn();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            /*
            for (int i = 0; i < 35; i++)
            {
                float offset = MathHelper.TwoPi / 35;
                Color RandomColor = Color.Lerp(Color.LightGreen, Color.ForestGreen, Main.rand.NextFloat(0, 1));
                new MediumGlowBall(Projectile.Center, Projectile.velocity.RotatedBy(offset * i), RandomColor, 60, 0, 1, 0.2f, Main.rand.NextFloat(2f, 2.2f)).Spawn();
            }
            */
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
        }
    }
}
