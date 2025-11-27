using CalamityMod;
using CalamityMod.Balancing;
using CalamityMod.Cooldowns;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Weapons.Magic;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using UCA.Assets.Sounds;
using UCA.Content.Particiles;
using UCA.Content.UCACooldowns;
using UCA.Core.Utilities;

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
        /*
        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
        {
        }

        public override bool FreeDodge(Player.HurtInfo info)
        {
            return base.FreeDodge(info);
        }
        */
        public override bool ConsumableDodge(Player.HurtInfo info)
        {
            if (Player.HasCooldown(ShadowBotlStaffDodge.ID))
            {
                Player.RemoveCooldown(ShadowBotlStaffDodge.ID);
                Player.SetImmuneTimeForAllTypes(180);
                SoundEngine.PlaySound(SoundsMenu.FireBallBlast with { Pitch = -0.5f });
                Vector2 firpos = Player.Center;
                for (int i = 0; i < 100; i++)
                {
                    Color Firecolor = LAPUtilities.LerpColor(Color.Black, Color.DarkViolet);
                    new Fire(firpos, Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.6f, 1.2f) * 12, Firecolor, 90, Main.rand.NextFloat(MathHelper.TwoPi), 1f, 0.3f).SpawnToPriorityNonPreMult();
                }
                new CrossGlow(firpos, Vector2.Zero, Color.Violet, 60, 1f, 0.7f, true).Spawn();
                new CrossGlow(firpos, Vector2.Zero, Color.DarkViolet, 60, 1f, 0.7f, true).Spawn();
                return true;
            }
            return base.ConsumableDodge(info);
        }
    }
}
