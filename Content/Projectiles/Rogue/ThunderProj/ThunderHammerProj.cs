using CalamityMod;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Content.Items.Weapons.Rogue;
using UCA.Content.Projectiles.Rogue.PunishmentProj;
using UCA.Content.Projectiles.Rogue.ThunderProj.RightHandHammer;
using UCA.Core.Utilities;

namespace UCA.Content.Projectiles.Rogue.ThunderProj
{
    public class ThunderHammerProj : ThrownHammerProj
    {
        public override string Texture => UCATextureRegister.InvisibleTexturePath;
        protected override BoomerangDefault BoomerangStat => new(
            //不准修改这个returnTime低于35
            returnTime: 35,
            returnSpeed: 26f,
            acceleration: 1.5f,
            killDistance: 1800
        );
        private enum DoType
        {
            //初始投掷
            IsShooted,
            //第一次返程
            IsReturning,
            //潜伏攻击 - 回击
            IsStealth,
            //潜伏攻击 - 命中后坠落
            IsDrop,
            //潜伏攻击 - 第二次返程，此处返程判定是否拥有悬空锤
            IsReturningAgain,
            //潜伏攻击 - 充能
            IsCharging,
            //潜伏攻击 - 第二次回击
            IsBackToLock,
            //潜伏攻击 - 挂载
            IsLockingOn

        }
        private DoType AttackType
        {
            get => (DoType)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 66;
            Projectile.timeLeft = 3000;
            Projectile.localNPCHitCooldown = 30;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.extraUpdates = 4;
        }
        public override void AI()
        {
            Projectile.timeLeft = 2;
            Projectile.rotation += 0.1f;
            //攻击枚举与切换
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
                case DoType.IsDrop:
                    DoDrop();
                    break;
                case DoType.IsReturningAgain:
                    DoReturningAgain();
                    break;
                case DoType.IsCharging:
                    DoCharging();
                    break;
                case DoType.IsBackToLock:
                    DoBackToLock();
                    break;
                case DoType.IsLockingOn:
                    DoLockingOn();
                    break;
            }
        }

        private void DoShooted()
        {
            AttackTimer += 1;
            if (AttackTimer > BoomerangStat.ReturnTime)
            {
                AttackTimer = 0;
                Projectile.netUpdate = true;
                AttackType = DoType.IsReturning;
            }
        }
        private void DoReturning()
        {
            Projectile.AccelerateToTarget(Owner.Center, BoomerangStat.ReturnSpeed, BoomerangStat.Acceleration);
            if (Projectile.Hitbox.Intersects(Owner.Hitbox))
            {
                if (Stealth)
                {
                    Projectile.netUpdate = true;
                    AttackTimer = 0;
                    AttackType = DoType.IsStealth;
                    return;
                }

                Projectile.netUpdate = true;
                Projectile.Kill();
            }
        }
        #region 潜伏攻击的所有逻辑
        //潜伏的回击
        private void DoStealth()
        {
            if (Projectile.GetTargetSafe(out NPC target, TargetIndex))
                Projectile.HomingNPCBetter(target, 24f, 20f, 1);
        }
        //潜伏的坠落
        private void DoDrop()
        {
            if (AttackTimer == 0)
            {
                Projectile.extraUpdates = 2;
                Projectile.netUpdate = true;
            }
            AttackTimer += 1;
            Projectile.velocity.Y += 0.217f;
            Projectile.velocity.X *= 0.98f;
            ShootPrettySpark();
            if (AttackTimer > 75)

            {
             //为玩家向上生成挂机锤
                if (Owner.ownedProjectileCounts[ModContent.ProjectileType<ThunderHandler>()] < 1)
                {
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<ThunderHandler>(), Projectile.damage, 0f);
                    //杀死这个射弹
                    Projectile.Kill();
                    return;
                }
                AttackType = DoType.IsReturningAgain;
                AttackTimer = 0;
                Projectile.extraUpdates = 2;
                Projectile.netUpdate = true;
            }
        }
        //第二次返回玩家
        private void DoReturningAgain()
        {
            Projectile.AccelerateToTarget(Owner.Center, BoomerangStat.ReturnSpeed * 1.2f, BoomerangStat.Acceleration * 1.2f);
            if (Projectile.Hitbox.Intersects(Owner.Hitbox))
            {
                AttackTimer = 0;
                Projectile.netUpdate = true;
                //生成完全一样的新锤子
                Vector2 vel = new Vector2(-18f * Owner.direction, 0f);
                //为玩家向上生成挂机锤
                Vector2 dir = Projectile.velocity.SafeNormalize(Vector2.UnitX);
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Owner.Center, vel, Type, Projectile.damage, Projectile.knockBack);
                //设置射弹直接跳转到这个状态。
                proj.ai[0] = (float)DoType.IsCharging;
                //重置其eu
                proj.extraUpdates = 2;
                proj.Calamity().stealthStrike = true;
                //杀死这个射弹。我们需要用一个新的绘制
                Projectile.Kill();
            }
        } 
        //充能
        private void DoCharging()
        {
            Projectile.velocity *= 0.98f;
            //大小逐渐递增
            //Projectile.scale += 0.0017f;
            if(Projectile.velocity.Length() < 0.1f)
            {
                Projectile.velocity *= 0;
                AttackType = DoType.IsBackToLock;
                Projectile.netUpdate = true;
                Projectile.extraUpdates = 4;
            }
        }
        //返回攻击
        private void DoBackToLock()
        {
            
            AttackTimer += 1;
            if (Projectile.GetTargetSafe(out NPC target, TargetIndex, true))
            {
                Projectile.HomingNPCBetter(target, 28f + AttackTimer / 14f, 20, 1);
                return;
            }
            else
                Projectile.Kill();
        }
        private void DoLockingOn()
        {
            if (Projectile.GetTargetSafe(out NPC target, TargetIndex))
            {

                if ((target.Center - Projectile.Center).Length() < 5f)
                    Projectile.Center = target.Center;
                else
                    Projectile.HomingNPCBetter(target, 24f, 20f, 1);
                    
            }
        }
        private void ShootPrettySpark()
        {
        }
        #endregion
        
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //非潜伏下方都不会造成。
            if (!Stealth)
                return;

            if (AttackType == DoType.IsStealth )
            {
                AttackType = DoType.IsDrop;
                Projectile.netUpdate = true;
            }
                
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.QuickDrawBloomEdge(Color.White, 8, -MathHelper.PiOver4);
            Projectile.QuickDrawWithTrailing(0.4f, Color.White, 6, -MathHelper.PiOver4);
            return false;
        }
    }
}
