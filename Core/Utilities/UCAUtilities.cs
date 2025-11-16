using CalamityMod;
using LAP.Core.AnimationHandle;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using System;
using Terraria;
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
        public static bool LineThroughRect(Vector2 start, Vector2 end, Rectangle rect, int lineWidth = 4, int checkDistance = 8)
        {
            float point = 0f;
            return rect.Contains((int)start.X, (int)start.Y) || rect.Contains((int)end.X, (int)end.Y) || Collision.CheckAABBvLineCollision(rect.TopLeft(), rect.Size(), start, end, lineWidth, ref point);
        }
        /// <summary>
        /// 为射弹获取目标，重载Out与判定方法
        /// </summary>
        /// <param name="proj"></param>
        /// <param name="target"></param>
        /// <param name="targetIndex"></param>
        /// <param name="anotherDistance"></param>
        /// <param name="canSearchSecondTarget">是否允许再搜索一个目标单位（如果输入的TargetIndex不合法）</param>
        /// <returns></returns>
        public static bool GetTargetSafe(this Projectile proj, out NPC target, int? targetIndex = null, bool canSearchSecondTarget = true, float anotherDistance = 1800f)
        {
            NPC npc;
            if (targetIndex.HasValue)
            {
                npc = Main.npc[targetIndex.Value];
                //当前敌人不可被追踪，跳过这一步并进行下一步
                if (!npc.CanBeChasedBy(proj) || canSearchSecondTarget)
                    npc = proj.FindClosestTarget(anotherDistance);
                else
                    npc = null;
            }
            else
                npc = proj.FindClosestTarget(anotherDistance);

            target = npc;
            return npc != null;
        }
    }
}
