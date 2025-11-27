using CalamityMod;
using CalamityMod.Projectiles.Typeless;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Content.Items.Weapons.Rogue;
using UCA.Content.Particiles;
using UCA.Content.Projectiles.Rogue.PunishmentProj;
using UCA.Core.Utilities;
using static UCA.Content.Projectiles.Rogue.BlazingProj.BlazingNamePick;

namespace UCA.Content.Projectiles.Rogue.BlazingProj
{
    //Todo：按下鼠标右键后应当刷新一次生命值
    public class BlazingHammerProj: ThrownHammerProj, ILocalizedModType
    {
        private enum DoType
        {
            IsShooted,
            IsReturning,
            IsStealth,
        }
        private DoType AttackType
        {
            get => (DoType)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }
        internal ref bool Update => ref Projectile.netUpdate;
        private bool MouseRight = false;
        public override string Texture => ModContent.GetInstance<BlazingHammer>().Texture;
        #region 基础数值
        protected override BoomerangDefault BoomerangStat => new
        (
            returnTime: 40,
            returnSpeed: 28f,
            acceleration: 1.2f,
            killDistance: 1000
        );
        //总潜伏攻击时长为五秒
        private int StealthTotalTime => 60 * Projectile.extraUpdates;
        //挂载锤子的攻击频率：5 * 额外更新
        private int HangingHitCooldown => 5 * Projectile.extraUpdates;
        private bool CanSpawnVolcano = true;
        #endregion
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 5;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void ExSD()
        {
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.width = Projectile.height = 66;
            Projectile.timeLeft = 120;
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, TorchID.Red);
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
            }
        }
        
        private void DirectlySpawnEruptionFireBall(float initTime, int eu = 1, int totalCount = 6)
        {
            for (int i = 0; i < totalCount; i++)
            {
                Vector2 dir = Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4 / 4, MathHelper.PiOver4 / 4)) * Main.rand.NextFloat(14f, 18f);
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, dir, ModContent.ProjectileType<BlazingEruption>(), Projectile.damage, Projectile.knockBack);
                proj.timeLeft = 480;
                proj.ai[0] = initTime;
                proj.extraUpdates = eu;
            }
        }
        //终 极 史 山
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Daybreak, 500);
            //攻击的敌怪传入
            TargetIndex = target.whoAmI;
            bool canFuckYou = AttackType == DoType.IsShooted && !Stealth || Stealth && AttackType != DoType.IsReturning;
            bool canFuckSound = Projectile.numHits < 1 && !Stealth || Stealth;
            if (AttackType == DoType.IsStealth && Projectile.timeLeft < 15 && CanSpawnVolcano)
            {
                Vector2 center = new Vector2(target.Center.X, target.Center.Y + 30f);
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), center, Vector2.Zero, ModContent.ProjectileType<BlazingVolcano>(), Projectile.damage * 5, Projectile.knockBack);
                proj.ai[1] = target.whoAmI;
                CanSpawnVolcano = false;
            }
            if (canFuckSound)
                SoundEngine.PlaySound(SoundID.Item89 with { MaxInstances = 0, Pitch = 0.8f }, Projectile.Center);
            //fuck you
            if (canFuckYou)
            {
                Projectile fuckYou = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<FuckYou>(), Projectile.damage / 2, Projectile.knockBack, Owner.whoAmI, 0f, 0.85f + Main.rand.NextFloat() * 1.15f);
                fuckYou.Calamity().stealthStrike = true;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            PickTagColor(out Color baseColor, out Color targetColor);
            Color lerpColor = Color.Lerp(baseColor, targetColor, Projectile.velocity.Length() / 26); 
            Projectile.QuickDrawBloomEdge(lerpColor);
            Projectile.QuickDrawWithTrailing(0.5f, Color.White);
            return false;   
        }
        public void DrawTrailingDust()
        {
            PickTagColor(out Color baseColor, out Color targetColor);
            //故意不采用循环，因为要稍微处理圆弧状态粒子，但是我技术力不够，先放着了
            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            Vector2 speedValue = direction * 3f;
            Vector2 spawnPosition = Projectile.Center + direction.RotatedBy(MathHelper.PiOver2) * 8f;
            Vector2 realVel = speedValue.RotatedBy(MathHelper.PiOver2);
            ShinyOrbParticle shinyOrbParticle = new ShinyOrbParticle(spawnPosition, realVel, Main.rand.NextBool() ? baseColor : targetColor, 20, 1.2f);
            shinyOrbParticle.Spawn();

            spawnPosition = Projectile.Center + direction.RotatedBy(-MathHelper.PiOver2) * 8f;
            realVel = speedValue.RotatedBy(-MathHelper.PiOver2);
            ShinyOrbParticle shinyOrbParticle2 = new ShinyOrbParticle(spawnPosition, realVel, Main.rand.NextBool() ? baseColor : targetColor, 20, 1.2f);
            shinyOrbParticle2.Spawn();
        }
        
        private void DoShooted()
        {
            AttackTimer += 1;
            if (AttackTimer > BoomerangStat.ReturnTime)
            {
                AttackType = DoType.IsReturning;
                AttackTimer = 0;
                Update = true;
            }
        }
        private void DoReturning()
        {
            Projectile.AccelerateToTarget(Owner.Center, BoomerangStat.ReturnSpeed, BoomerangStat.Acceleration, BoomerangStat.KillDistance);
            if (Projectile.Hitbox.Intersects(Owner.Hitbox))
            {
                //不是潜伏攻击，返回处死射弹
                if (!Stealth)
                {
                    Projectile.Kill();
                    Update = true;
                    return;
                }
                else
                {
                    //二级锤子有极其高频率的攻击方式
                    AttackType = DoType.IsStealth;
                    Update = true;
                    Projectile.localNPCHitCooldown = HangingHitCooldown;
                    Projectile.timeLeft = StealthTotalTime;
                    DirectlySpawnEruptionFireBall(18f);
                }
            }
        }
        private void DoStealth()
        {
            bool available = Projectile.GetTargetSafe(out NPC target, TargetIndex,true);
            if (Main.mouseRight && MouseRight is false)
            {
                MouseRight = true;
                Update = true;
                Projectile.timeLeft = 300;
            }

            //潜伏状态下，这个锤子会以正常的形式执行五秒内的超高速攻击，锁定敌怪
            if (!MouseRight)
            {
                //神人书架做的神人代码导致这个追踪的东西能秒杀蠕虫
                //你自己看着办
                if (available)
                    Projectile.HomingNPCBetter(target, 20f, 20f, 1);
            }
            else
            {
                //假定：此时玩家按下了右键，则完全做掉上面的情况，转而冲向玩家的鼠标位置
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<BlazingHammerProjClone>(), Projectile.damage, Projectile.knockBack, Owner.whoAmI);
                //经常会用到，直接写成这样了
                proj.UCA().TargetIndex = TargetIndex;
                //记得处死射弹。
                Projectile.Kill();
            }
        }
        private void DoGeneric()
        {
            Projectile.rotation += 0.2f;
            if (Stealth)
            {
                DrawTrailingDust();
                return;
            }

            //开潜伏的情况下下面的普攻粒子是全部不生成的
            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            //基础速度
            Vector2 speedValue = direction * 2.5f;
            //基础横向偏移，用于控制射弹与路径的距离。
            float baseOffset = 1f;
            //让相位差不变，使他们在零点上同步
            float angle = AttackTimer * 0.2f;
            //曲线1使用Sin，曲线2使用-Sin确保反向运动
            float wave = (float)Math.Sin(angle);
            //计算垂直方向向量。
            Vector2 perpendDir = direction.RotatedBy(MathHelper.PiOver2);
            //最终确定生成位置的偏差
            Vector2 waveOffset = perpendDir * wave * 35f + perpendDir * baseOffset;
            //修改粒子生成位置。
            Vector2 spawnPosition = Projectile.Center + waveOffset;
            //这里是一个数学问题：Sin开导实际上就是Cos曲线。也就是“速度”
            float verticleVel = (float)Math.Cos(angle) * 1.2f;
            Vector2 realVel = speedValue + perpendDir * verticleVel;
            //最终生成粒子。
            Dust d = Dust.NewDustPerfect(spawnPosition, PickTagDust, realVel);
            d.scale *= 2.1f;
            d.noGravity = true;
            //全局会一直生成的粒子
            if (Main.rand.NextBool())
                return;
            if (Main.rand.NextBool())
            {
                Vector2 offset = new Vector2(10, 0).RotatedByRandom(MathHelper.ToRadians(360f));
                Vector2 velOffset = new Vector2(2, 0).RotatedBy(offset.ToRotation());
                Dust dust = Dust.NewDustPerfect(new Vector2(Projectile.Center.X, Projectile.Center.Y) + offset, DustID.SolarFlare, new Vector2(Projectile.velocity.X * 0.2f + velOffset.X, Projectile.velocity.Y * 0.2f + velOffset.Y), 100, default, 0.8f);
                dust.noGravity = true;
            }

            if (Main.rand.NextBool(5))
            {
                Vector2 offset = new Vector2(12, 0).RotatedByRandom(MathHelper.ToRadians(360f));
                Vector2 velOffset = new Vector2(4, 0).RotatedBy(offset.ToRotation());
                Dust dust = Dust.NewDustPerfect(new Vector2(Projectile.Center.X, Projectile.Center.Y) + offset, DustID.GemRuby, new Vector2(Projectile.velocity.X * 0.15f + velOffset.X, Projectile.velocity.Y * 0.15f + velOffset.Y), 100, default, 0.8f);
                dust.noGravity = true;
            }
        }
        private short PickTagDust
        {
            get 
            {
                short Pick = Owner.name.SelectedName() switch
                {
                    NameType.TrueScarlet => DustID.CrimsonTorch,
                    NameType.WutivOrChaLost => DustID.YellowTorch,
                    NameType.Emma => DustID.HallowedTorch,
                    NameType.SherryOrAnnOrKino => DustID.BlueTorch,
                    NameType.Shizuku => DustID.WhiteTorch,
                    NameType.SerratAntler => DustID.DemonTorch,
                    NameType.Hanna => DustID.JungleTorch,
                    _ => DustID.OrangeTorch,
                };
                return Pick;
            }
        }
        private void PickTagColor(out Color baseColor, out Color targetColor)
        {
            switch (Owner.name.SelectedName())
            {
                case NameType.TrueScarlet:
                    baseColor = Color.Red;
                    targetColor = Color.Crimson;
                    break;
                //查 -- 金
                case NameType.WutivOrChaLost:
                    baseColor = new Color(255, 178, 36);
                    targetColor = Color.Gold;
                    break;
                case NameType.Emma:
                    baseColor = Color.HotPink;
                    targetColor = Color.Pink;
                    break;
                //锯角 - 紫
                case NameType.SerratAntler:
                    baseColor = Color.Purple;
                    targetColor = Color.DarkViolet;
                    break;
                //Kino - 蓝
                case NameType.SherryOrAnnOrKino:
                    baseColor = Color.RoyalBlue;
                    targetColor = Color.LightBlue;
                    break;
                case NameType.Shizuku:
                    baseColor = Color.LightSkyBlue;
                    targetColor = Color.AliceBlue;
                    break;
                //绿
                case NameType.Hanna:
                    baseColor = Color.Green;
                    targetColor = Color.LimeGreen;
                    break;
                default:
                    baseColor = Color.OrangeRed;
                    targetColor = Color.Orange;
                    break;
            }
        }

    }
}