using CalamityMod;
using Terraria;
using Terraria.ModLoader;
using UCA.Core.GlobalInstance.Players;
using UCA.Core.Utilities;

namespace UCA.Core.GlobalInstance.Projectiles
{
    public class UCAGlobalProj : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public bool HasThroughNightShield = false;

        public bool HasThroughNightShieldOverMax = false;

        public int DamageDefence = 0;
        public override void ModifyHitPlayer(Projectile projectile, Player target, ref Player.HurtModifiers modifiers)
        {
            if (HasThroughNightShield)
            {
                modifiers.ModifyHurtInfo += ModifyHurtInfo_NightShield;
            }
        }

        public void ModifyHurtInfo_NightShield(ref Player.HurtInfo info)
        {
            Player player = Main.player[Main.myPlayer];
            UCAPlayer uCAPlayer = player.UCA();
            if (!HasThroughNightShieldOverMax)
            {
                info.Damage = 0;
                player.Calamity().freeDodgeFromShieldAbsorption = true; 
            }
            else
            {
                info.Damage -= DamageDefence;
                uCAPlayer.NightShieldCanDefense = false;
            }
        }
    }
}
