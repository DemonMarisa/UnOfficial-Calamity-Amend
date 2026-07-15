using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Terraria;

namespace UCA.Content.Projectiles.HeldProj.Magic.VividClarityHeld
{
    public partial class VividClaritySupportMinion
    {
        public void UpdateIdle()
        {
            Projectile.velocity = Vector2.Zero;
            Projectile.Center = Vector2.SmoothStep(Projectile.Center, Owner.Center + IdlePos, 0.2f);
            Projectile.rotation = Utils.AngleLerp(Projectile.rotation, IdleRot, 0.2f);
            NPC npc = LAPUtilities.FindClosestTarget(Owner.Center, 1500);
            if (npc is not null)
            {
                ChangeState(RangedAttack);
                Projectile.netUpdate = true;
                Projectile.netSpam = 0;
            }
        }
    }
}
