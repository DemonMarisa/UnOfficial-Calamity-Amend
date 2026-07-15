using LAP.Core.SystemsLoader;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using UCA.Content.GUI.VividClarityUI;
using UCA.Content.Projectiles.HealPRoj;
using UCA.Core.Enum;
using UCA.Core.Utilities;

namespace UCA.Content.Projectiles.HeldProj.Magic.VividClarityHeld
{
    public partial class VividClaritySupportMinion
    {
        public float HealEffectScale;
        public void UpdateHealPlayer()
        {
            RealTimer = 0;
            Projectile.velocity = Vector2.Zero;
            Projectile.Center = Vector2.SmoothStep(Projectile.Center, Owner.Center + IdlePos, 0.2f);
            Projectile.rotation = Utils.AngleLerp(Projectile.rotation, IdleRot, 0.2f);
            HealEffectScale = MathHelper.Lerp(HealEffectScale, 1f, 0.2f);
            if (Owner.miscCounter % 15 == 0)
            {
                Vector2 vel = Vector2.UnitX.RotateRandom(MathHelper.TwoPi) * 6f;
                Vector2 fireOffset = new Vector2(52, 0).RotatedBy(Projectile.rotation);
                if (Projectile.IsLocalPlayer())
                    Projectile.Owner().SpawnHealProj(Projectile.GetSource_FromThis(), ProjectileType<ExoHeal>(), Projectile.Center + fireOffset, vel);
            }
            if (Owner.UCA().VividClarityStates == VividClarityState.Support && !Owner.HasProj<VividClaritySkillHeldProj>() && !LAPContent.GetUI<VividClarityUI>().Active)
            {
                if (Projectile.IsLocalPlayer())
                {
                    if (Main.mouseLeft && Main.mouseLeftRelease)
                    {
                        BlockHealStateChangeTimer = 2;
                        ChangeState(Idle);
                    }
                }
            }
        }
    }
}
