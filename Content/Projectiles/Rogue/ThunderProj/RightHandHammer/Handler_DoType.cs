using CalamityMod;
using CalamityMod.Graphics.Primitives;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using UCA.Content.Items.Weapons.Rogue;
using UCA.Content.Projectiles.Rogue.ThunderProj.RightHandHammer;
using UCA.Core.BaseClass;

namespace UCA.Content.Projectiles.Rogue.ThunderProj.RightHandHammer
{
    public partial class ThunderHandler : RogueProjClass, IPixelatedPrimitiveRenderer
    {
        float Oscillation = 0;
        private void UpdateMode_General()
        {
            Oscillation += 0.025f;
            if (Main.mouseRight && Owner.HeldItem.type == ModContent.ItemType<ThunderHammer>())
                HandleToMouse();
            else
            {
                NPC target = Projectile.FindClosestTarget(1800f);
                float tofs = target is null ? 180 : (target.width + target.height) / 2f + 180;
                Offset *= 0.9f;
                //Main.NewText(Projectile.velocity);
                //Main.NewText(AttackType);
                switch (AttackType)
                {
                    case DoType.IsIdle:
                        HandleIdle();
                        break;
                    case DoType.IsRotatedTo:
                        HandleRotatedTo(target);
                        break;
                    case DoType.IsChasing:
                        HandleChasing(target, tofs);
                        break;
                    case DoType.IsStop:
                        HandleStop();
                        break;
                }
            }
        }

        private void HandleStop()
        {
            //减速，并逐步修正转角至玩家
            Projectile.velocity *= 0.97f;
            float tarRot = (Owner.Center - Projectile.Center).ToRotation();
            Projectile.rotation = RotateTowardsAngle(Projectile.rotation, tarRot, 0.01f, false);
            AttackTimer += 1;
            //时机差不多合适，时期返程
            if(AttackTimer > 45f && Projectile.velocity.Length() < 0.5f)
            {
                AttackType = DoType.IsIdle;
                Projectile.netUpdate = true;
                AttackTimer = 0;
                Projectile.velocity *= 0;
            }    
        }
        float Offset = 0;
        private void HandleChasing(NPC target, float tofs)
        {
            Projectile.velocity *= 0.8f;
            AttackTimer -= 1;
            if (target != null)
            {
                Vector2 tpos = target.Center + (Projectile.Center - target.Center).SafeNormalize(Vector2.Zero) * tofs;
                float dis = Vector2.Distance(Projectile.Center, tpos);
                Main.NewText(tofs);
                if (dis > 8)
                    Projectile.velocity += (tpos - Projectile.Center).SafeNormalize(Vector2.Zero) * 1f;
            }
            if (AttackTimer < 130)
                Offset = float.Lerp(Offset, tofs + 10, 0.04f);
            Projectile.rotation = InitRot + LosDirection * MathHelper.ToRadians(520) * GetRepeatedCosFromZeroToOne(1 - AttackTimer / 180f, 2);
            if (AttackTimer <= 0)
            {
                AttackType += 1;
                Projectile.netUpdate = true;
                Projectile.extraUpdates = 0;
                AttackTimer = 0;
                InitRot *= 0;
            }
        }
        //递归获取点位
        public static float GetRepeatedCosFromZeroToOne(float v, int repeat)
        {
            if (repeat <= 1)
            {
                return (float)(Math.Cos(v * MathHelper.Pi - MathHelper.Pi)) * 0.5f + 0.5f;
            }
            return (float)(Math.Cos(GetRepeatedCosFromZeroToOne(v, repeat - 1) * MathHelper.Pi - MathHelper.Pi)) * 0.5f + 0.5f;
        }
        private int LosDirection = 0;
        private float InitRot = 0;
        private void HandleRotatedTo(NPC target)
        {
            //计算一下需要追及的角度
            float toRotation = (Projectile.Center - target.Center).ToRotation();
            Projectile.rotation = RotateTowardsAngle(Projectile.rotation, toRotation, 0.03f, false);
            AttackTimer++;
            //转角完成，开始执行追踪弧线
            if(AttackTimer > 30f)
            {
                AttackType += 1;
                AttackTimer = 180;
                Projectile.extraUpdates = 2;
                Projectile.netUpdate = true;
                LosDirection = Main.rand.NextBool().ToDirectionInt();
                InitRot = Projectile.rotation;
            }
        }
        public static float RotateTowardsAngle(float currentRadians, float targetRadians, float rotateSpeed, bool useFixedSpeed = true)
        {
            currentRadians = MathHelper.WrapAngle(currentRadians);
            targetRadians = MathHelper.WrapAngle(targetRadians);

            float difference = targetRadians - currentRadians;
            float turnAmount = MathHelper.WrapAngle(difference);

            if (useFixedSpeed)
            {
                turnAmount = MathHelper.Clamp(turnAmount, -rotateSpeed, rotateSpeed);
            }
            else
            {
                turnAmount *= MathHelper.Clamp(rotateSpeed, 0f, 1f);
            }

            return currentRadians + turnAmount;
        }
        private void HandleIdle()
        {
            FloatingOnOwner();
            AttackTimer += 1;
            if (AttackTimer > 60f)
            {
                AttackType += 1;
                Projectile.netUpdate = true;
                AttackTimer = 0;
            }
        }

        /// <summary>
        /// 总控按住鼠标右键时的AI
        /// </summary>
        private void HandleToMouse()
        {
            UpdateMousePosition();
            UpdateAttackType();
        }
        int FireCounts = 0;
        int FireDelay = 5;
        bool CanFireStar = false;
        private void UpdateAttackType()
        {
            AttackTimer += 1;
            if (AttackTimer > 35f)
                CanFireStar = true;
            if (FireCounts >= 6)
            {
                CanFireStar = false;
                AttackTimer = 0;
                FireCounts = 0;
                return;
            }
            if(CanFireStar)
            {
                FireDelay--;
                if (FireDelay <= 0)
                {
                        //确定位置
                    Vector2 spawnPosBase = (Owner.MountedCenter - Projectile.Center).SafeNormalize(Vector2.UnitX);
                    float warpRadians = Main.rand.NextFloat(-MathHelper.PiOver2 * 0.45f, MathHelper.PiOver2 * 0.45f);
                    Vector2 warpOffset = 150f * spawnPosBase.RotatedBy(warpRadians);
                    Vector2 spawnPos = Owner.MountedCenter + warpOffset * Main.rand.Next(6, 9) * 0.25f;
                    //确定初始速度，精准一些。
                    Vector2 velDir = (Owner.LocalMouseWorld() - spawnPos).SafeNormalize(Vector2.UnitX);
                    if (Projectile.owner == Main.myPlayer)
                    {
                        Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), spawnPos, velDir * Main.rand.NextFloat(15f, 19f), ModContent.ProjectileType<PhantomRay>(), Projectile.damage, 2.5f, Projectile.owner);
                        proj.DamageType = ModContent.GetInstance<RogueDamageClass>();
                    }
                    FireCounts += 1;
                    FireDelay = 5;
                }
            }
        }

        #region 控制锤子的悬挂状态
        /// <summary>
        /// 按住右键过渡至指针位置中心
        /// </summary>
        private void UpdateMousePosition()
        {
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, 0);
            Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);
            //计算玩家到指针的方向
            Vector2 direction = (Main.MouseWorld - Owner.MountedCenter).SafeNormalize(Vector2.UnitX);
            //就这个方向去算实际的位置。
            Vector2 realPos = Owner.MountedCenter + direction * 150f + Owner.velocity;
            //让他尽量缓动于上下。
            realPos.Y -= 0 + 50f * MathF.Cos(Oscillation) / 8f;
            Projectile.Center = Vector2.Lerp(Projectile.Center, realPos, 0.1f);
            //转角处理
            Projectile.rotation = direction.ToRotation();
            Owner.itemTime = Owner.itemAnimation = 2;
        }
        #endregion
        #region 召唤物攻击模组-常规
        /// <summary>
        /// 悬挂于玩家身上
        /// </summary>
        private void FloatingOnOwner()
        {
            //基本的挂机状态，此处使用了正弦曲线
            Vector2 anchorPos = new Vector2(Owner.MountedCenter.X, Owner.MountedCenter.Y - (150f + 150f * (MathF.Sin(Oscillation) / 9f)));
            //计算鼠标对射弹方向的拉力
            Vector2 mouseDirFromPlayer = (Main.MouseWorld - Owner.MountedCenter).SafeNormalize(Vector2.UnitY);
            float mouseDistanceFromPlayer = Vector2.Distance(Main.MouseWorld, Owner.MountedCenter);
            float pullStrength = MathHelper.Clamp(mouseDistanceFromPlayer / 200f, 0f, 1f);
            //实际鼠标拉力
            Vector2 mousePull = mouseDirFromPlayer * 200f * pullStrength * 0.1f;
            //最后将目标位置纳入进去。
            Vector2 targetPos = anchorPos;
            Projectile.Center = Vector2.Lerp(Projectile.Center, targetPos, 0.1f);
            //平滑当前角度插值，但是做出一定程度的限制
            //计算射弹位置与鼠标位置的水平插值。
            Vector2 direction = Owner.velocity.SafeNormalize(Vector2.UnitX) * 2f;
            //为射弹做一定程度的指针角度修正。
            float toRot = MathHelper.WrapAngle(direction.ToRotation()).ToClamp(-MathHelper.PiOver4 / 4, MathHelper.PiOver4 / 4);
            Projectile.rotation = Projectile.rotation.AngleLerp(-MathHelper.PiOver2 + toRot, 0.1f);

        }
        #endregion

    }
}
