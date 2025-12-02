using CalamityMod.Graphics.Primitives;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using UCA.Assets.Sounds;
using UCA.Content.Particiles;
using UCA.Core.BaseClass;

namespace UCA.Content.Projectiles.Rogue.ThunderProj.RightHandHammer
{
    public partial class ThunderHandler: RogueProjClass, IPixelatedPrimitiveRenderer
    {
        #region 特殊攻击模组 - 下砸
        private int PlatformIndex = -1;
        private Vector2 PlatformPos = Vector2.Zero;
        private void UpdateMode_Strike()
        { 
            switch (StrikeType)
            {
                case DoStrike.IsFlyingUp:
                    Strike_FlyingUp();
                    break;
                case DoStrike.IsStrikingDown:
                    Strike_StrikingDown();
                    break;
                case DoStrike.IsHandleUp:
                    Strike_HanldeUp();
                    break;
            }
        }
        /// <summary>
        /// 锤子战技 - 上升
        /// </summary>
        private void Strike_FlyingUp()
        {
            if (AttackTimer == 0f)
            {
                //给予射弹向上的初速度，并提供额外更新
                Projectile.velocity = new Vector2(0f, -24f);
                Projectile.extraUpdates = 6;
                //于指针位置处构造假平台
            }

            AttackTimer += 1;
            Projectile.velocity.Y -= AttackTimer * 0.75f;
            Strike_DrawFlyingupTrailDust();
            if ((Projectile.Center - Owner.Center).Length() > 1600f)
            {
                //重置射弹状态
                StrikeType = DoStrike.IsStrikingDown;
                Projectile.Center = new(Owner.MountedCenter.X, Owner.MountedCenter.Y - 1600f);
                Projectile.extraUpdates = 6;
                AttackTimer = 0f;
                Projectile.rotation = Owner.direction > 0 ? MathHelper.PiOver4 : -(MathHelper.Pi + MathHelper.PiOver4);
                Projectile.netUpdate = true;
                PlatformPos = Owner.LocalMouseWorld();
            }
        }
        /// <summary>
        /// 锤子战技 - 从天而降的坠落
        /// </summary>
        private void Strike_StrikingDown()
        {
            //将速度飞向过去。用追踪方法
            Vector2 mw = Owner.LocalMouseWorld();
            AttackTimer += 1;
            Projectile.HomingTarget(mw, 99999, 40f, 20f);
            Strike_DrawFlyingupTrailDust();
            Rectangle hitbox = new Rectangle((int)(mw.X - 30), (int)(mw.Y - 30), 30, 30);
            if (Projectile.Hitbox.Intersects(hitbox))
            {
                Projectile.extraUpdates = 0;
                StrikeType = DoStrike.IsHandleUp;
                AttackTimer = 0f;
                //砸地成功时，音效
                SoundEngine.PlaySound(SoundsMenu.Smash_GroundHeavy, Projectile.Center);
            }
        }
        /// <summary>
        /// 锤子战技 - 控制拔出
        /// </summary>
        private void Strike_HanldeUp()
        {
            //清零当前速度
            Projectile.velocity = Vector2.Zero;
            AttackTimer += 1;
            //砸地，释放冲击波粒子
            if (AttackTimer < 20f)
                DrawSmashDust();
            //时机一旦合适，信号发送给平台处死，并回归常规攻击模组
            if(AttackTimer > 60f)
            {
                StrikeType = DoStrike.IsFlyingUp;
                //重置AttackTimer并发送数据包
                AttackTimer = 0;
                Projectile.netUpdate = true;
            }
        }
        /// <summary>
        /// 锤子完全落地静止后的常态粒子
        /// </summary>
        private void DrawGroundIdleDust()
        {
            short HigherDust = DustID.BlueTorch;
            short BottemDust = DustID.UnusedWhiteBluePurple;
            for (int i = 0; i < 60; i++)
            {
                float height = Main.rand.NextFloat();
                Dust flame = Dust.NewDustPerfect(new(Projectile.Center.X, Projectile.Center.Y + 80f), Main.rand.NextFloat() >= height ? HigherDust : BottemDust);
                flame.noGravity = true;
                flame.position += new Vector2(Main.rand.NextFloat(-1, 1) * 2 * Projectile.width, -height * 25f * 5 - Main.rand.NextFloat(-5f, 5f));
                flame.velocity.Y -= 3f;
                if (Main.rand.NextBool(4))
                {
                    flame.position.X -= 12;
                    flame.velocity.X += 0.02f;
                    flame.scale *= 2f;
                }
                else
                    flame.velocity.Y -= Main.rand.NextFloat(2f, 4f);
            }
        }
        private int InitPhase = 10;
        private int EarlyPhase = 20;
        /// <summary>
        /// 砸地时的粒子
        /// </summary>
        private void DrawSmashDust()
        {
            short HigherDust = DustID.GemSapphire;
            short BottemDust = DustID.UnusedWhiteBluePurple;
            Vector2 scale = GetScale;
            Vector2 initSpawnPos = new(Projectile.Center.X, Projectile.Center.Y + 30f);
            //只在前20帧更新粒子运动
            if (AttackTimer < EarlyPhase)
            {
                for (int i = 0; i < 5; i++)
                {
                    float rnd = Main.rand.NextFloat(-1, 1);
                    Dust side = Dust.NewDustPerfect(initSpawnPos, Main.rand.NextBool() ? HigherDust : BottemDust);
                    side.noGravity = true;
                    side.position.X += rnd * 5 * scale.X;
                    side.velocity = new Vector2(rnd * 15, -Main.rand.NextFloat(3));
                    side.scale *= 2f;
                }

                for (int i = 0; i < 30; i++)
                {
                    float rnd = Main.rand.NextFloat(-1, 1);
                    Dust side = Dust.NewDustPerfect(initSpawnPos, Main.rand.NextBool() ? HigherDust : BottemDust);
                    side.noGravity = true;
                    side.position.X += rnd * 5 * scale.X;
                    side.velocity = new Vector2(rnd * 15, -Main.rand.NextFloat(2));
                    side.scale *= 2f;
                }

                for (int i = 0; i < 30; i++)
                {
                    float height = Main.rand.NextFloat();
                    Dust flame = Dust.NewDustPerfect(initSpawnPos, Main.rand.NextFloat() >= height ? HigherDust : BottemDust);
                    flame.noGravity = true;
                    flame.position += new Vector2(Main.rand.NextFloat(height - 1, 1 - height) * 20 * scale.X, -height * 1 * scale.Y);
                    flame.velocity.Y -= 10f;
                    if (Main.rand.NextBool(4))
                    {
                        flame.position.X -= 12;
                        flame.velocity.X += 0.02f;
                        flame.scale *= 2f;
                    }
                    else
                        flame.velocity.Y -= Main.rand.NextFloat(2f, 6f);
                }
            }
        }
        private Vector2 GetScale
        {
            get
            {
                Vector2 scale;
                Vector2 start = new(2f, 5f);
                Vector2 middle = new(8f, 8f);
                Vector2 late = new(5f, 2f);
                if (AttackTimer < InitPhase)
                    scale = Vector2.SmoothStep(start, middle, AttackTimer / InitPhase);
                else
                    scale = Vector2.SmoothStep(middle, late, (AttackTimer - InitPhase) / EarlyPhase);
                return scale;
            }
        }
        
        private void Strike_DrawFlyingupTrailDust()
        {
            //正弦波频率
            float freq = 0.2f;
            //振幅
            float amp = 35f;
            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            //基础速度
            Vector2 speedValue = direction * 2.5f;
            for (int k = 0; k < 6; k++)
            {
                for (int i = -1; i < 2; i += 2)
                {
                    //基础横向偏移，用于控制射弹与路径的距离。
                    float baseOffset = 5f;
                    //让相位差不变，使他们在零点上同步
                    float angle = AttackTimer * freq;
                    //曲线1使用Sin，曲线2使用-Sin确保反向运动
                    float wave = (float)Math.Sin(angle) * i;
                    //计算垂直方向向量。
                    Vector2 perpendDir = direction.RotatedBy(MathHelper.PiOver2);
                    //最终确定生成位置的偏差
                    Vector2 waveOffset = perpendDir * wave * amp + perpendDir * baseOffset;
                    //修改粒子生成位置。
                    Vector2 spawnPosition = Projectile.Center + waveOffset;
                    //计算例子速度，粒子需要在零点反向运动。因为总体上，他们是在原点位置被“推开”的
                    //这里是一个数学问题：Sin开导实际上就是Cos曲线。也就是“速度”
                    float verticleVel = (float)Math.Cos(angle) * 1.2f * i;
                    Vector2 realVel = speedValue + perpendDir * verticleVel;
                    //跳过屏幕外绘制
                    if (LAPUtilities.OutOffScreen(spawnPosition))
                        continue;
                    //最终生成粒子。
                    Color drawColor = i > 0 ? Color.AliceBlue : Color.RoyalBlue;
                    ShinyOrbParticle shinyOrbParticle = new ShinyOrbParticle(spawnPosition, realVel, drawColor, 140, 1.2f);
                    shinyOrbParticle.Spawn();
                }
            }
        }
        #endregion

    }
}
