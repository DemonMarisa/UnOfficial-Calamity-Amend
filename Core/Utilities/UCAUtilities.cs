using CalamityMod;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using UCA.Core.AnimationHandle;
using UCA.Core.Enums;
using UCA.Core.GlobalInstance.Items;
using UCA.Core.GlobalInstance.Projectiles;

namespace UCA.Core.Utilities
{
    public static partial class UCAUtilities
    {
        public static UCAGlobalItem UCA(this Item item)
        {
            return item.GetGlobalItem<UCAGlobalItem>();
        }
        public static UCAGlobalProj UCA(this Projectile proj)
        {
            return proj.GetGlobalProjectile<UCAGlobalProj>();
        }

        public static bool PressLeftAndRightClick()
        {
            return Main.mouseLeft && Main.mouseRight;
        }
        public static bool JustPressLeftClick()
        {
            return Main.mouseLeft && !Main.mouseRight;
        }

        public static bool JustPressRightClick()
        {
            return !Main.mouseLeft && Main.mouseRight;
        }
        public static bool PressLeftAndRightClick(this Player player)
        {
            return player.UCA().MouseLeft && player.UCA().MouseRight;
        }
        public static bool JustPressLeftClick(this Player player)
        {
            return player.UCA().MouseLeft && !player.UCA().MouseRight;
        }

        public static bool JustPressRightClick(this Player player)
        {
            return !player.UCA().MouseLeft && player.UCA().MouseRight;
        }

        public static Vector2 LocalMouseWorld(this Player player)
        {
            return player.UCA().SyncedMouseWorld;
        }
        public static void UpDateAni(this AnimationHelper animationHelper, int index, int Break = 0)
        {
            if (animationHelper.AniProgress[index] < animationHelper.MaxAniProgress[index])
                animationHelper.AniProgress[index]++;

            if (animationHelper.AniProgress[index] >= animationHelper.MaxAniProgress[index])
            {
                animationHelper.Auxfloat[index]++;
                if (animationHelper.Auxfloat[index] >= Break)
                    animationHelper.HasFinish[index] = true;
            }
        }
        public static float UpDateAngle(this AnimationHelper animationHelper,float BeginAngle, float EndAngle, int Filp, float Progress, float PreFilpAdd = 0)
        {
            float startAngleOffset = MathHelper.ToRadians(BeginAngle);
            float endAngleOffset = MathHelper.ToRadians(EndAngle);
            float baseRotation = MathHelper.Lerp(startAngleOffset, endAngleOffset, Progress) + PreFilpAdd;
            if (Filp == -1)
                baseRotation = baseRotation * Filp;
            return baseRotation;
        }
    }
}
