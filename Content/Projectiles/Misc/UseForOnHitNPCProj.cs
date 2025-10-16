using CalamityMod;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Content.Items;
using UCA.Content.MetaBalls;
using UCA.Content.Particiles;
using UCA.Content.Projectiles.HeldProj.Magic.CarnageRayHeld;
using UCA.Content.Projectiles.HeldProj.Magic.PlasmaRodHeld;

namespace UCA.Content.Projectiles.Misc
{
    // 这个射弹用于一些需要在OnHitNPC的多人状态下同步数据的情况
    // 反正不用重新弄一套系统了
    public class UseForOnHitNPCProj : ModProjectile, ILocalizedModType
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<Sword>();
        public int ID => (int)Projectile.ai[0];
        public override string Texture => UCATextureRegister.InvisibleTexturePath;
        public Player Owner => Main.player[Projectile.owner];
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.timeLeft = 1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 0;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            // 先这样写，如果以后需要更多效果再写状态机存效果
            if (ID == ModContent.ProjectileType<PlasmaRodSkillProj>())
            {
                for (int i = 0; i < 35; i++)
                {
                    float offset = MathHelper.TwoPi / 35;
                    Color RandomColor = Color.Lerp(Color.DarkViolet, Color.LightPink, Main.rand.NextFloat(0, 1));
                    new MediumGlowBall(Projectile.Center, Projectile.velocity.RotatedBy(offset * i), RandomColor, 60, 0, 1, 0.2f, Main.rand.NextFloat(2f, 2.2f)).Spawn();
                }
            }
            if (ID == ModContent.ProjectileType<CarnageRaySkillProj>())
            {
                Vector2 ToMouseVector = Projectile.Center - Owner.Center;
                ToMouseVector = ToMouseVector.SafeNormalize(Vector2.UnitX);
                for (int i = 0; i < 10; i++)
                {
                    Vector2 shootVel = ToMouseVector.RotatedByRandom(MathHelper.PiOver4 * 0.7f) * Main.rand.NextFloat(0.2f, 1.2f) * 24f;

                    if (shootVel.ToRotation() > 0)
                        shootVel.Y *= 0.15f;

                    Color color = Main.rand.NextBool(3) ? Color.Black : Color.DarkRed;
                    new BloodDrop(Projectile.Center, shootVel, color, Main.rand.Next(60, 90), 0, 1, 0.1f).Spawn();
                }

                for (int i = 0; i < 10; i++)
                {
                    Vector2 SpawnVector = ToMouseVector.RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(0f, 1.2f) * 36f;
                    CarnageMetaBall.SpawnParticle(Projectile.Center,
                        SpawnVector,
                        1.5f, SpawnVector.ToRotation());

                }
            }
        }
    }
}
