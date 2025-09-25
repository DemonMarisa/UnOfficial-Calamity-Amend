using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace UCA.Core.Utilities
{
    public static partial class UCAUtilities
    {
        /// <summary>
        /// 用于搜索距离射弹最近的npc单位，并返回NPC实例。通常情况下与上方的追踪方法配套
        /// 这个方法会同时实现穿墙、数组、boss优先度的搜索。不过只能用于射弹。但也足够
        /// 这里Boss优先度的实现逻辑是如果我们但凡搜索到一个Boss，就把这个Boss临时存储，在返回实例的时候优先使用
        /// </summary>
        /// <param name="p">射弹</param>
        /// <param name="maxDist">最大搜索距离</param>
        /// <param name="bossFirst">boss优先度，这个还没实现好逻辑，所以填啥都没用（</param>
        /// <param name="ignoreTiles">穿墙搜索, 默认为</param>
        /// <param name="arrayFirst">数组优先, 这个将会使射弹优先针对数组内第一个单位,默认为否</param>
        /// <returns>返回一个NPC实例</returns>
        public static NPC FindClosestTarget(this Projectile p, float maxDist, bool bossFirst = false, bool ignoreTiles = true, bool arrayFirst = false)
        {
            //bro我真的要遍历整个NPC吗？
            float distStoraged = maxDist;
            NPC tryGetBoss = null;
            NPC acceptableTarget = null;
            bool alreadyGetBoss = false;
            foreach (NPC npc in Main.ActiveNPCs)
            {
                float exDist = npc.width + npc.height;
                //单位不可被追踪 或者 超出索敌距离则continue
                if (Vector2.Distance(p.Center, npc.Center) > distStoraged + exDist)
                    continue;

                if (!npc.active || npc.friendly || npc.lifeMax < 5 || !npc.CanBeChasedBy(p.Center, false))
                    continue;
                //补: 如果优先搜索Boss单位, 且附近至少有一个。我们直接存储这个Boss单位
                //已经获取到的会被标记，使其不会再跑一遍搜索.
                if (npc.boss && bossFirst && !alreadyGetBoss)
                {
                    tryGetBoss = npc;
                    alreadyGetBoss = true;
                }

                //搜索符合条件的敌人, 准备返回这个NPC实例
                float curNpcDist = Vector2.Distance(npc.Center, p.Center);
                if (curNpcDist < distStoraged && (ignoreTiles || Collision.CanHit(p.Center, 1, 1, npc.Center, 1, 1)))
                {
                    distStoraged = curNpcDist;
                    acceptableTarget = npc;
                    if (tryGetBoss != null & bossFirst)
                        acceptableTarget = tryGetBoss;
                    //如果是数组优先，直接在这返回实例
                    if (arrayFirst)
                        return acceptableTarget;
                }
            }
            //返回这个NPC实例
            return acceptableTarget;
        }

        public static float PostModeBoostProjDamage(float damage)
        {
            float realDamage = damage * 2;

            if (Main.masterMode)
                realDamage *= 1.5f;

            if(Main.expertMode)
                realDamage *= 2f;

            return realDamage;
        }

        public static float PreModeBoostProjDamage(float damage)
        {
            float realDamage = damage * 0.5f;
            
            if (Main.expertMode)
                realDamage *= 0.5f;

            if (Main.masterMode)
                realDamage *= 0.66f;

            return realDamage;
        }

        /// <summary>
        /// 用于跟踪指定地点的方法
        /// 只会跟踪你传进去的目标
        /// </summary>
        /// <param name="proj">射弹</param>
        /// <param name="target">射弹目标</param>
        /// <param name="distRequired">最大范围</param>
        /// <param name="speed">射弹速度</param>
        /// <param name="inertia">惯性</param>
        /// <param name="maxAngleChage">角度限制，默认为空. </param>
        public static void HomingTarget(this Projectile proj, Vector2 target, float distRequired, float speed, float inertia, float? maxAngleChage = null)
        {
            //开始追踪target
            Vector2 home = (target - proj.Center).SafeNormalize(Vector2.UnitY);
            Vector2 velo = (proj.velocity * inertia + home * speed) / (inertia + 1f);
            //这里给了一个角度限制
            if (maxAngleChage.HasValue)
            {
                float curAngle = proj.velocity.ToRotation();
                float tarAngle = velo.ToRotation();
                float angleDiffer = MathHelper.WrapAngle(tarAngle - curAngle);
                //转弧度
                float maxRadians = MathHelper.ToRadians(maxAngleChage.Value);
                if (Math.Abs(angleDiffer) > maxRadians)
                {
                    float clampedAngle = curAngle + Math.Sign(angleDiffer) * maxRadians;
                    float setSpeed = velo.Length();
                    velo = new Vector2((float)Math.Cos(clampedAngle), (float)Math.Sin(clampedAngle)) * setSpeed;
                }
            }
            //设定速度
            proj.velocity = velo;
        }
    }
}
