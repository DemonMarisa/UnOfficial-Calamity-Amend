using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using UCA.Content.Projectiles.Magic.Ray;

namespace UCA.Content.Projectiles.HeldProj.Magic.VividClarityHeld
{
    public partial class VividClaritySupportMinion
    {
        public int FireDely;
        public int FireLeft;
        public void UpdateRangedAttack()
        {
            if (MeleeCD > 0)
                MeleeCD--;
            NPC npc = LAPUtilities.FindClosestTarget(Owner.Center, 3000);
            if (npc is not null)
            {
                Vector2 TargetIdelPos = npc.Center + LAPUtilities.GetVector2(npc.Center, Projectile.Center) * 700;
                if (Projectile.Center.Distance(TargetIdelPos) > 100)
                    Projectile.velocity = Vector2.SmoothStep(Projectile.velocity, LAPUtilities.GetVector2(Projectile.Center, TargetIdelPos) * 24f, 0.1f);
                else
                    Projectile.velocity *= 0.9f;
                Projectile.rotation = Utils.AngleLerp(Projectile.rotation, LAPUtilities.GetVector2(Projectile.Center, npc.Center).ToRotation(), 0.15f);
                CheckFire(npc);
                if (MeleeCD <= 0)
                {
                    ChangeState(MeleeAttack);
                    MeleeCD = 300;
                }
            }
            else
            {
                ChangeState(Idle);
            }
        }
        public void CheckFire(NPC npc)
        {
            if (FireDely > 0)
                FireDely--;
            if (FireDely == 0)
            {
                FireLeft = 3;
                FireDely = 40;
            }
            if (FireDely > 0 && FireLeft > 0)
            {
                if (FireDely % 4 == 0)
                {
                    if (Projectile.IsLocalPlayer())
                    {
                        float f = Main.rand.NextFloat() * MathHelper.TwoPi;
                        float spreadX = 50f;
                        float spreadY = 50f;
                        Vector2 source = Projectile.Center + f.ToRotationVector2() * MathHelper.Lerp(spreadX, spreadY, Main.rand.NextFloat());
                        Vector2 firvel = LAPUtilities.GetVector2(source, npc.Center) * 12;
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), source, firvel, ProjectileType<VividBeam_Weak>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 1f);
                    }
                    FireLeft--;
                }
            }
        }
    }
}
