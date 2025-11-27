using CalamityMod;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Assets.Sounds;
using UCA.Content.Items.Weapons.Rogue;
using UCA.Core.GlobalInstance.Players;
using UCA.Core.GlobalInstance.Projectiles;
using UCA.Core.Utilities;

namespace UCA.Content.Projectiles.Rogue.PunishmentProj
{
    public class PunishmentHammerProj: ThrownHammerProj
    {
        
        #region 攻击逻辑的枚举
        private enum DoType
        {
            IsShooted,
            IsReturning,
            IsStealth,
            IsAddition
        }
        private DoType AttackType
        {
            get => (DoType)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }
        #endregion

        #region 一些其他的东西，如果你真的很需要调整平衡，就修改这里
        protected override BoomerangDefault BoomerangStat => new(
            returnTime: 30,
            acceleration: 0.7f,
            returnSpeed: 12f,
            killDistance: 1800
        );
        #endregion
        #region Typedef
        //没啥必要，我写这个纯因为觉得长单词麻烦
        internal ref bool Update => ref Projectile.netUpdate;
        public static readonly SoundStyle AdditionHitSigSound = new("CalamityMod/Sounds/Item/PwnagehammerSound") { MaxInstances = 0, Volume = 0.80f };
        public override string Texture => ModContent.GetInstance<PunishmentHammer>().Texture;
        #endregion
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 4;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void ExSD()
        {
            //气笑了
            Projectile.width = Projectile.height = 66;
            Projectile.localNPCHitCooldown = 20;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.timeLeft = 300;
            Projectile.scale *= 1.1f;
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, TorchID.White);
            DoGeneric();
            switch (AttackType)
            {
                case DoType.IsShooted:
                    DoShooted();
                    break;
                case DoType.IsReturning:
                    DoReturning();
                    break;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            TargetIndex = target.whoAmI;
            float vol = Owner.ownedProjectileCounts[ModContent.ProjectileType<PunishmentHammerLock>()] > 0 ? 0.4f : 0.7f;
            if (Projectile.numHits % 2 == 0)
            {
                NormalShootPunishmentStar(target);
                SoundStyle pickSound2 = Utils.SelectRandom(Main.rand, SoundsMenu.Smash_AirHeavy);
                SoundEngine.PlaySound(pickSound2 with { Pitch = Main.rand.NextFloat(0.6f, 0.7f), Volume = 0.4f, MaxInstances = 1 }, target.Center);
            }
        }
        //手动绘制这个射弹，我不想用你灾的绘制方式
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.QuickDrawBloomEdge();
            Projectile.QuickDrawWithTrailing(0.4f, Color.White);
            return false;
        }

        #region AI方法合集
        private void DoGeneric()
        {
            Projectile.rotation += 0.2f;
            if (Main.rand.NextBool())
            {
                Vector2 offset = new Vector2(10, 0).RotatedByRandom(MathHelper.ToRadians(360f));
                Vector2 velOffset = new Vector2(2, 0).RotatedBy(offset.ToRotation());
                Dust dust = Dust.NewDustPerfect(new Vector2(Projectile.Center.X, Projectile.Center.Y) + offset, DustID.GemDiamond, new Vector2(Projectile.velocity.X * 0.2f + velOffset.X, Projectile.velocity.Y * 0.2f + velOffset.Y), 100, default, 0.8f);
                dust.noGravity = true;
            }
            if (Main.rand.NextBool(5))
            {
                Vector2 offset = new Vector2(12, 0).RotatedByRandom(MathHelper.ToRadians(360f));
                Vector2 velOffset = new Vector2(4, 0).RotatedBy(offset.ToRotation());
                Dust dust = Dust.NewDustPerfect(new Vector2(Projectile.Center.X, Projectile.Center.Y) + offset, DustID.HallowedWeapons, new Vector2(Projectile.velocity.X * 0.15f + velOffset.X, Projectile.velocity.Y * 0.15f + velOffset.Y), 100, default, 0.8f);
                dust.noGravity = true;
            }
        }
        private void NormalShootPunishmentStar(NPC target)
        {
            Vector2 center = target.Center;
            float randsRad = MathHelper.Pi;
            Vector2 dir = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            int div = 3;
            //已有挂载射弹，将生成位置更新在玩家身上
            if (Owner.ownedProjectileCounts[ModContent.ProjectileType<PunishmentHammerLock>()] > 0)
            {
                center = Owner.Center;
                randsRad = MathHelper.PiOver2;
                dir = -(target.Center - center).SafeNormalize(Vector2.UnitX);
                div = 4;
            }
            else
                center.CirclrDust(36, Main.rand.NextFloat(1.2f, 1.4f), Main.rand.NextBool() ? DustID.GemDiamond : DustID.HallowedWeapons, 3);
            for (int i = 1 ; i < 3; i++)
            {
                Vector2 velocity = dir.RotatedBy(Main.rand.NextFloat(-randsRad / div, randsRad / div)) * 8f;
                Projectile star = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), center, velocity, ModContent.ProjectileType<PunishmentStar>(), Projectile.damage, Projectile.knockBack, Owner.whoAmI);
                star.timeLeft = 100;
                star.penetrate = 1;
            }
        }
        //返程AI
        private void DoReturning()
        {
            //返程时执行类似回旋镖的AI
            Projectile.AccelerateToTarget(Owner.Center, BoomerangStat.ReturnSpeed, BoomerangStat.Acceleration, BoomerangStat.KillDistance);
            if (Projectile.Hitbox.Intersects(Owner.Hitbox))
            {
                //无潜伏属性，处死射弹
                if (!Stealth)
                {
                    Update = true;
                }
                //其余情况下，根据情况进行潜伏攻击
                else
                {
                    //音效
                    SoundEngine.PlaySound(AdditionHitSigSound, Projectile.Center);
                    //当前没有任何挂载锤，则正常进入挂载状态
                    if (Owner.ownedProjectileCounts[ModContent.ProjectileType<PunishmentHammerLock>()] < 1)
                    {
                        Projectile.Center.CirclrDust(24, 3f, DustID.HallowedWeapons, 10);
                        Projectile lockHammer = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<PunishmentHammerLock>(), Projectile.damage, 0f, Owner.whoAmI);
                        lockHammer.ai[1] = TargetIndex;
                        lockHammer.Calamity().stealthStrike = true;
                        //处死射弹。
                    }
                    //否则，执行其他AI
                    else
                    {
                        Owner.Center.CirclrDust(24, 3f, DustID.GemRuby, 10);
                        //锤子本身会在进入这个AI逻辑后处死
                        Update = true;
                        //追加射弹，然后处死锤子
                        DoAddition();
                    }
                }
                //无论如何都直接处死射弹
                Projectile.Kill();
            }
        }
        
        
        private void DoShooted()
        {
            AttackTimer += 1;
            //满足返程时间，返回
            if (AttackTimer > BoomerangStat.ReturnTime)
            {
                //重置计时器
                AttackTimer = 0;
                //切换攻击模组
                AttackType = DoType.IsReturning;
                //网络同步
                Update = true;
            }
        }
        private void DoAddition()
        {
            if (Projectile.GetTargetSafe(out NPC target, TargetIndex, true))
            {
                for (int i = 0; i < 6; i++)
                {
                    Vector2 dir = Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(Main.rand.NextFloat(Main.rand.NextFloat(MathHelper.PiOver4)));
                    Vector2 spawnSpeed = dir * 12f;
                    float ai1 = target.whoAmI;
                    Projectile hammer = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Owner.Center, spawnSpeed, ModContent.ProjectileType<PunishmentStar>(), Projectile.damage, Projectile.knockBack * 1.5f, Projectile.owner, 0f, ai1);
                    hammer.DamageType = ModContent.GetInstance<RogueDamageClass>();
                    hammer.ai[2] = 1f;
                    Update = true;
                }
            }
        }
        #endregion


    }
    /// <summary>
    /// 基础回旋镖的相关数据
    /// </summary>
    /// <param name="returnTime">返程时间m/param>
    /// <param name="returnSpeed">返程基础速度</param>
    /// <param name="acceleration">返程加速度</param>
    /// <param name="killDistance">超出距离处死</param>
    public struct BoomerangDefault(int returnTime, float returnSpeed, float acceleration, int killDistance)
    {
        public int ReturnTime = returnTime;
        public float Acceleration = acceleration;
        public float ReturnSpeed = returnSpeed;
        public int KillDistance = killDistance;
    }

    public abstract class ThrownHammerProj : ModProjectile, ILocalizedModType
    {
        public Player Owner => Main.player[Projectile.owner];
        public UCAPlayer ModPlayer => Owner.UCA();
        public UCAGlobalProj ModProj => Projectile.UCA();
        public bool Stealth => Projectile.Calamity().stealthStrike;
        public ref bool _isHanging => ref ModPlayer._anyHammerAttacking;
        public int AttackTimer
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }
        public int TargetIndex
        {
            get => (int)Projectile.ai[2];
            set => Projectile.ai[2] = value;
        }
        public new string LocalizationCategory => "Projectiles.Rogue";
        /// <summary>
        /// 基础射弹数据
        /// </summary>
        /// <summary>
        /// 基础回旋镖类模组数据。
        /// returnTime：返程时间
        /// returnSpeed：返程基础速度
        /// acceleration：返程加速度
        /// killDistance：处死距离
        /// </summary>
        protected abstract BoomerangDefault BoomerangStat{ get; }
        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 3;
            ExSD();
        }
        public virtual void ExSD() { }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (!Stealth)
                modifiers.DefenseEffectiveness *= 0.3f;
        }
        public virtual void ExModifyHit(NPC target, ref NPC.HitModifiers modifiers) { }
    }

}