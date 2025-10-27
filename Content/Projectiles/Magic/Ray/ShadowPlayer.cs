using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Core.BaseClass;

namespace UCA.Content.Projectiles.Magic.Ray
{
    public class ShadowPlayer : BaseMagicProj
    {
        public override string Texture => UCATextureRegister.InvisibleTexturePath;
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 48;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 10;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 240;
            Projectile.extraUpdates = 0;
        }
        public override void AI()
        {
            
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
