using LAP.Assets.TextureRegister;
using LAP.Core.Graphics.Lightning;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using UCA.Core.BaseClass;

namespace UCA.Content.Projectiles.Magic.Ray
{
    public class ExoLightning : BaseMagicProj
    {
        public override string Texture => LAPTextureRegister.InvisibleTexturePath;
        public Vector2 beginPos;
        public Vector2 endPos;
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.extraUpdates = 0;
            Projectile.friendly = true;
            Projectile.timeLeft = 30;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Projectile.LAP().FirstFrame)
                return false;
            float _ = float.NaN;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), beginPos, endPos, 48f, ref _);
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override void AI()
        {
            if (Projectile.LAP().FirstFrame)
            {
                Vector2 NowBeginPos = new Vector2(Main.rand.NextFloat(-200, 200), -700);
                Vector2 BeginPos = Projectile.Center + NowBeginPos;
                Vector2 fireVel = LAPUtilities.GetVector2(BeginPos, Projectile.Center);
                Vector2 EndPos = BeginPos + fireVel * 1400;

                Color color = Main.rand.NextFromList(Color.White, Color.Orange, Color.LimeGreen, Color.SkyBlue, new Color(57, 46, 115));
                LightningSetting setting = new LightningSetting(BeginPos, EndPos, color, 20, 15, 45, 6, 0.5f, 3, 100, 0.6f, 30);
                LightningBuilder.SpawnLightning(setting);

                beginPos = BeginPos;
                endPos = EndPos;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
    }
}
