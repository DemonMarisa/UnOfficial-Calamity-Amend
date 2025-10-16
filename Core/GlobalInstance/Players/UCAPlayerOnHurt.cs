using CalamityMod;
using CalamityMod.Items.Weapons.Magic;
using Terraria;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using UCA.Core.Utilities;

namespace UCA.Core.GlobalInstance.Players
{
    public partial class UCAPlayer : ModPlayer
    {
        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            if (ExternalDR != 0)
            {
                ExternalDR = ExternalDR / (1f + ExternalDR);
                modifiers.SourceDamage *= 1 - ExternalDR;
            }
            if (TerraRayUseSkillCount > 0)
            {
                modifiers.SourceDamage *= 0.25f;
            }
        }
        /*
        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
        {
        }

        public override bool FreeDodge(Player.HurtInfo info)
        {
            return base.FreeDodge(info);
        }
        */
    }
}
