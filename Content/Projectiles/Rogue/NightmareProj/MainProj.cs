using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using LAP.Core.ParticleSystem;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Content.Items.Weapons.Rogue.Hammer;
using UCA.Content.Particiles;
using UCA.Core.BaseClass;
using UCA.Core.Utilities;
namespace UCA.Content.Projectiles.Rogue.NightmareProj
{
    public class NightmareHammerProj: BaseHammerClass, ILocalizedModType
    {
        #region Typedef
        internal ref bool Update => ref Projectile.netUpdate;
        public ref float NotHangingReleaseDarkEnegryTimer => ref Projectile.UCA().ExtraAI[0];
        public static readonly SoundStyle UseSound = new("CalamityMod/Sounds/Item/PwnagehammerSound") { MaxInstances = 0,Pitch = 0.35f, Volume = 0.35f };
        #endregion
        //攻击枚举
        private enum DoType
        {
            IsShooted,
            IsReturning,
            IsStealth,
            IsHanging
        }
        private DoType AttackType
        {
            get => (DoType)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }
        #region 基础数值
        protected override BoomerangDefault BoomerangStat => new(
            //不准修改这个returnTime低于35
            returnTime: 35,
            returnSpeed: 26f,
            acceleration: 1.5f,
            killDistance: 1800
        );
        protected override BaseProjSD ProjStat => new (
            HitCooldown: 30,
            LifeTime:300,
            Width:66,
            Height:66,
            rotation: 0.2f
        );
        //潜伏攻击的总时间，为了这个锤子所有攻击方式，如果你知道你在做什么，你不应该修改这个值“低于”480
        private const int TotalSpinTime = 480;
        //我也不知道这个是干嘛的，但是我建议别改（
        private int StartSpinTime => 30 * Projectile.extraUpdates;
        #endregion
        #region 其余与平衡无关的杂项。
        public ref float NebulaArrowRotation => ref ModProj.ExtraAI[0];
        private int FlaresCounts = 1;
        private float DrawTrailTimer = 0f;
        public override string Texture => ModContent.GetInstance<NightmareHammer>().Texture;
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NotHangingReleaseDarkEnegryTimer);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NotHangingReleaseDarkEnegryTimer = reader.ReadSingle();
        }
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        #endregion
        public override void ExSD()
        {
            //夜明后的锤子应该上4eu了
            Projectile.extraUpdates = 4;
        }
        public override void AI()
        {
            DoGeneric();
            switch (AttackType)
            {
                case DoType.IsShooted:
                    DoShooted();
                    break;
                case DoType.IsReturning:
                    DoReturning();
                    break;
                case DoType.IsStealth:
                    DoStealth();
                    break;
                case DoType.IsHanging:
                    DoHanging();
                    break;
            }
        }
        //全局AI
        private void DoGeneric() 
        {
            Projectile.rotation += ProjStat.RotationSpeed;
            if (!Stealth)
                Projectile.ArmorPenetration = 50;
            Lighting.AddLight(Projectile.Center, TorchID.Purple);
            DrawTrailingDust();
        }
        //首次投掷出去时的AI
        private void DoShooted()
        {
            AttackTimer += 1;
            if (AttackTimer > BoomerangStat.ReturnTime)
            {
                AttackTimer = 0;
                AttackType = DoType.IsReturning;
                Update = true;
            }
        }
        //返程AI
        private void DoReturning()
        {
            Projectile.AccelerateToTarget(Owner.Center, BoomerangStat.ReturnSpeed, BoomerangStat.Acceleration, BoomerangStat.KillDistance);
            if (Projectile.Hitbox.Intersects(Owner.Hitbox))
            {
                //当前有任何挂载锤，所有的攻击都会直接在返回后杀掉弹幕
                if (!Stealth)
                {
                    Projectile.Kill();
                    Update = true;
                    return;
                }
                else
                {
                    AttackType = DoType.IsStealth;
                    //重新设定无敌帧
                    Projectile.usesLocalNPCImmunity = true;
                    Projectile.timeLeft = TotalSpinTime;
                    Projectile.extraUpdates = 3;
                    Projectile.localNPCHitCooldown = 25 * Projectile.extraUpdates;
                    Update = true;
                }
            }
        }
        //潜伏AI
        private void DoStealth()
        {
            DoGeneric();
            //直接回击敌人
            if (Projectile.GetTargetSafe(out NPC target, TargetIndex, true))
                Projectile.HomingNPCBetter(target, 1f, 20f, 20f, 1, ignoreDist: true);
            else
                Projectile.Kill();
        }

         private bool _isArcRotating = false;
        private float _arcStartRotation;
        private float TotalArcAngle;
        private float _originalSpeed;
        private float _prevArcAngle;
        private bool _isReverse = false;
        private void DoHanging()
        {
            //标记进入挂载状态
            _isHanging = true;
            //轨迹粒子
            if (AttackTimer > 8)
                DrawTrailingDust();
            
            DrawArc();
            //Timer应延后自增避免出现执行问题
            AttackTimer += 1;
            //敌对单位非空
            if (Projectile.GetTargetSafe(out NPC target, TargetIndex, true))
            {
                //只有在特定帧后才允许锤子进行挂载
                if (AttackTimer > StartSpinTime * 2)
                {
                    //直接冲向你的敌人
                    Projectile.HomingNPCBetter(target, 1f, 24f, 20f, 1, ignoreDist: true);
                    DoGeneric();
                    return;
                }
                ReleaseDarkEnegry();
            }
        }
        private void ReleaseDarkEnegry()
        {
            /*
            下方是一段基于主射弹当前速度而做出动态变化的射弹生成代码
            表现来说是，衍生射弹的生成频率将会与主射弹的速度会成正比，并尽可能控制在需要的固定间隔内
            也就是，主射弹速度越快，生成频率越高，速度越慢，则生成频率越慢
            这样一定程度上会抑制盗贼弹幕速度加成对输出的影响
            如果你真的很需要调整平衡，请结合下方的注释好好理解
            */
            //基础生成间隔
            const int BaseSpawnSpeed = 20;
            //射弹的飞行速度
            const float BaseTravelSpeed = 22f;
            //最小生成间隔
            const float MinSpawnSpeed = 15f;
            //最大生成间隔
            const float MaxSpawnSpeed = 24;
            //计算当前速度的模长
            float curSpeed = Projectile.velocity.Length();
            //基于射弹速度间隔进行生成刻计算
            float dynamicSpawnSpeed = BaseTravelSpeed / curSpeed * BaseSpawnSpeed;
            //控制在合理范围内
            dynamicSpawnSpeed = MathHelper.Clamp(dynamicSpawnSpeed, MinSpawnSpeed, MaxSpawnSpeed);
            //向下取整
            int spawnRates = (int)Math.Round(dynamicSpawnSpeed);
            Vector2 direction = (Owner.Center - Projectile.Center).SafeNormalize(Vector2.UnitX);
            //将方向转为实际的角度并临时存储进去
            NebulaArrowRotation = direction.ToRotation();
            if (AttackTimer % spawnRates == 0)
            {
                //这里的暗温魔能会必中
                float baseFlareSpeed = Main.rand.NextFloat(12f, 16f);
                //依据锤子当前的速度，以对数的形式给予伤害加成
                int flareDamage = (int)(Projectile.damage + 2 * (float)Math.Log(Projectile.velocity.Length() / 1.5));
                Vector2 velocity = direction * baseFlareSpeed;
                if (Projectile.owner != Main.myPlayer)
                    return;
                //鬼魂音效
                SoundEngine.PlaySound(SoundID.Item103 with { MaxInstances = 4, Pitch = 0.7f });
                //生成
                Projectile flares = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<DarkEnergy>(), flareDamage, 1.1f, Owner.whoAmI, 0f, Main.rand.Next(3));
                flares.Calamity().stealthStrike = true;
                flares.extraUpdates = 3;
                flares.tileCollide = false;
            }
        }
        //圆弧运动总控
        private void DrawArc()
        {
            //如果已反向，且AttackTimer不会在被置零，返回，不执行下方AI
            if (_isReverse && AttackTimer > StartSpinTime * 2)
                return;
            bool firstArc = !_isArcRotating && AttackTimer is 0 && !_isReverse;
            bool secondArc = !_isArcRotating && AttackTimer > StartSpinTime && _isReverse;
            if (firstArc || secondArc)
            {
                float wtf = MathHelper.TwoPi;
                //随机取用角度
                TotalArcAngle = Main.rand.NextBool() ? wtf : -wtf;
                _isArcRotating = true;
                _arcStartRotation = Projectile.velocity.ToRotation();
                _originalSpeed = Projectile.velocity.Length();
                //尚未反向，缓存这个角度
                if (!_isReverse)
                    _prevArcAngle = TotalArcAngle;
                else
                    TotalArcAngle = -_prevArcAngle;

                Projectile.velocity /= 3;
            }
            if (_isArcRotating)
            {
                //首次画圆，执行0~StartSpin，第二次画圆，执行StartSpinTime ~ StartSpinTime * 2
                float progress = !_isReverse
                    ? (float)AttackTimer / StartSpinTime
                    : (float)(AttackTimer - StartSpinTime) / StartSpinTime;
                Projectile.rotation = _arcStartRotation + TotalArcAngle * progress;
                //加速
                float speed = Projectile.velocity.Length() + 0.21f * AttackTimer;
                if (speed > _originalSpeed)
                    speed = _originalSpeed;

                Projectile.velocity = Projectile.rotation.ToRotationVector2() * speed;
                //如果进程结束
                if (progress >= 1f)
                {
                    Projectile.velocity = Projectile.rotation.ToRotationVector2() * _originalSpeed;
                    _isArcRotating = false;
                    //首次反向结束，重置计时器准备反向
                    if (!_isReverse)
                    {
                        AttackTimer = StartSpinTime;
                        //标记启用反向状态
                        _isReverse = true;
                    }
                    else 
                        _isReverse = false;
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //Debuff
            DebuffsHandler(target);
            SoundEngine.PlaySound(SoundID.Item88, Projectile.Center);
            StealthHitDust();
            //如果没有潜伏攻击，正常生成梦魇之箭
            if (!Stealth && Projectile.numHits < 1)
            {
                for (int i = 0; i < 2; i++)
                    NightmareArrowDrop(target, Projectile.damage);
            }

            //处于潜伏回击时，击中敌人传入这个单位
            if (AttackType is DoType.IsStealth)
            {
                SoundEngine.PlaySound(UseSound, Projectile.Center);
                TargetIndex = target.whoAmI;
                AttackType = DoType.IsHanging;
                Update = true;
                return;
            }
            //此处，处理挂载情况下的射弹操作
            if (AttackType is DoType.IsHanging)
                OnHitHanging(target);
        }

        public override void OnKill(int timeLeft)
        {
            //记得重置玩家类的状态。
            if (AttackType is DoType.IsHanging)
            {
                _isHanging = false;
                Update = true;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.QuickDrawBloomEdge(Color.DarkMagenta, 6);
            Projectile.QuickDrawWithTrailing(0.7f, Color.White);
            return false;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overWiresUI.Add(index);
        }
        private void DebuffsHandler(NPC target)
        {
            //常态造成元素调谐
            target.AddBuff(ModContent.BuffType<ElementalMix>(), 360);
            //白天，造成破晓 + 双足翼龙诅咒
            if (Main.dayTime)
            {
                target.AddBuff(BuffID.Daybreak, 360);
                target.AddBuff(BuffID.BetsysCurse, 360);
            }
            //夜晚，造成夜魇 + "死亡低语"
            else
            {
                target.AddBuff(ModContent.BuffType<Nightwither>(), 360);
                target.AddBuff(ModContent.BuffType<WhisperingDeath>(), 360);
            }

        }
        private void OnHitHanging(NPC target)
        {
            bool hasOver2Hammer = false;
            int hammerCounts = 0;
            foreach (var proj in Main.ActiveProjectiles)
            {
                if (proj.type == Type && proj.Calamity().stealthStrike)
                    hammerCounts++;
                if (hammerCounts > 1)
                {
                    hasOver2Hammer = true;
                    break;
                }
            }
            //注意这里的逻辑：
            //假定玩家同时拥有两把以上的挂载锤，下方的射弹生成数量会被很大程度上降低.
            //每次攻击，都会使梦魇箭数量与伤害提升
            Update = true;
            int numFlares = hasOver2Hammer ? 2 : 3;
            if (FlaresCounts < numFlares)
                FlaresCounts += 1;
            //用对数计算控制伤害
            int flareDamage = (int)(Projectile.damage / 2 * Math.Log(1 + FlaresCounts));
            
            for (int i = 0; i < numFlares; i++)
                NightmareArrowDrop(target, flareDamage);
            //每次攻击缩减
            //双锤子以上时，每把锤子最低只有15的攻击频率
            int leastHitCD = hasOver2Hammer ? 12 * Projectile.extraUpdates : 6 * Projectile.extraUpdates;
            Projectile.localNPCHitCooldown -= 5 * Projectile.extraUpdates;
            if (Projectile.localNPCHitCooldown < leastHitCD)
                Projectile.localNPCHitCooldown = leastHitCD;
        }
        private void NightmareArrowDrop(NPC target, int flareDamage)
        {
            //这下面一长串都是为了处理……生成的
            //返程写的挺fuck的
            float xDist = Main.rand.NextFloat(10f, 220f) * Main.rand.NextBool().ToDirectionInt();
            float yDist = Main.rand.NextFloat(800f, 1000f);
            Vector2 srcPos = target.Center + new Vector2(xDist, -yDist);
            if (Projectile.owner != Main.myPlayer)
                return;

            //在滞留所有的射弹
            Projectile sparks = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), srcPos, Vector2.Zero, ModContent.ProjectileType<NightmareArrow>(), flareDamage, 1.1f, Owner.whoAmI);
            sparks.Calamity().stealthStrike = true;
            sparks.ai[2] = target.whoAmI;
            sparks.localAI[0] = xDist;
            sparks.localAI[1] = yDist;
        }

        private void StealthHitDust()
        {
            Vector2 dire = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            float rotFactor = MathHelper.Pi / 15;
            float dScaleCap = 1.8f;
            float dSacleBase = 0.5f;
            for (int i = -7; i < 7; i++)
            {
                float rot = MathHelper.ToRadians(i * rotFactor);
                Vector2 dVelocity = (Projectile.velocity.Length() / 1.5f * dire).RotatedBy(rot);
                Dust d = Dust.NewDustDirect(Projectile.Center, Projectile.width / 4, Projectile.height / 4, DustID.GemAmethyst, dVelocity.X, dVelocity.Y);
                d.scale = dSacleBase;
                d.noGravity = true;
                d.alpha = 100;
                dSacleBase += 0.2f;
                if (dSacleBase > dScaleCap)
                    dSacleBase = dScaleCap;
            }
        }
        private void DrawTrailingDust()
        {
            DrawTrailTimer++;
            if (DrawTrailTimer < 5f)
                return;
            //正弦波频率
            float freq = 0.2f;
            //振幅
            float amp = 35f;
            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            //基础速度
            Vector2 speedValue = direction * 2.5f;
            for (int i = -1; i < 2; i+= 2)
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
                Color drawColor = i > 0 ? Color.Black : new(75, 0, 130);
                ShinyOrbParticle shinyOrbParticle = new ShinyOrbParticle(spawnPosition, realVel, drawColor, 140, 1.2f, i < 0 ? BlendStateID.Additive : BlendStateID.Alpha);
                shinyOrbParticle.Spawn();
            }
        }
    }
}