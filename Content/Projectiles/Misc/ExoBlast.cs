using LAP.Assets.TextureRegister;
using LAP.Core.SystemsLoader;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using UCA.Content.VFXs.ExoBlasts;

namespace UCA.Content.Projectiles.Misc
{
    public class ExoBlast : ModProjectile
    {
        public override string Texture => LAPTextureRegister.InvisibleTexturePath;
        public override void SetStaticDefaults()
        {
            Projectile.AddProtectedProj();
        }
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.height = Projectile.width = 250;
            Projectile.timeLeft = 30;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.noEnchantmentVisuals = true;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Projectile.ai[0] < 9)
                return false;
            else
                return base.Colliding(projHitbox, targetHitbox);
        }
        public override void AI()
        {
            if (Projectile.LAP().FirstFrame)
                ExoBlastVFX.Spawn(Projectile.Center, Main.rand.NextFloat(MathHelper.TwoPi));
            Projectile.ai[0]++;
        }
    }
}
