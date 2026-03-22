using Terraria;
using Terraria.ModLoader;
using UCA.Core.GlobalInstance.Players;
using UCA.Core.Utilities;

namespace UCA.Core.GlobalInstance.Projectiles
{
    public partial class UCAGlobalProj : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public bool NightShieldBeBlock = false;
        public bool NightShieldFallBlock = false;
        // 如果没能全部格挡，那这个弹幕要降低多少伤害
        public int DamageDefence = 0;
        public override void AI(Projectile projectile)
        {
        }
        public override void ModifyHitPlayer(Projectile projectile, Player target, ref Player.HurtModifiers modifiers)
        {
            if (NightShieldBeBlock)
            {
                modifiers.ModifyHurtInfo += ModifyHurtInfo_NightShield;
                projectile.netUpdate = true;
            }
        }
        public void ModifyHurtInfo_NightShield(ref Player.HurtInfo info)
        {
            if (NightShieldFallBlock)
            {
                info.Damage -= DamageDefence;
            }
            else
            {
                info.Damage = 0;
            }
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
        }
    }
}
