using LAP.Core.SystemsLoader;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using UCA.Assets.Sounds;
using UCA.Content.Particiles;
using UCA.Content.UCACooldowns;

namespace UCA.Core.GlobalInstance.Players
{
    public partial class UCAPlayer : ModPlayer
    {
        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            if (TerraRayUseSkillCount > 0)
            {
                modifiers.SourceDamage *= 0.25f;
            }
            if (SoulPiercerSGSUse > 0)
            {
                modifiers.SourceDamage *= 0.5f;
            }

        }
        public override bool ConsumableDodge(Player.HurtInfo info)
        {
            if (Player.HasCD<ShadowBotlStaffDodge>())
            {
                Player.RemoveCD(LAPContent.CDType<ShadowBotlStaffDodge>());
                Player.SetImmuneTimeForAllTypes(180);
                return true;
            }
            return false;
        }
    }
}
