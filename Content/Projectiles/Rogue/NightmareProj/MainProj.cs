using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Projectiles.BaseProjectiles;
using LAP.Core.ParticleSystem;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Assets.Sounds;
using UCA.Content.Items.Weapons.Rogue;
using UCA.Content.Particiles;
using UCA.Content.Projectiles.Rogue.PunishmentProj;
using UCA.Core.Utilities;
namespace UCA.Content.Projectiles.Rogue.NightmareProj
{
    public class NightmareHammerProj: ThrownHammerProj, ILocalizedModType
    {
        internal ref bool Update => ref Projectile.netUpdate;
        public static readonly SoundStyle UseSound = new("CalamityMod/Sounds/Item/PwnagehammerSound") { MaxInstances = 0,Pitch = 0.35f, Volume = 0.35f };
        //攻击枚举
        private enum DoType
        {
            IsShooted,
            IsReturning,
            IsStealth
        }
        private DoType AttackType
        {
            get => (DoType)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }
        protected override BoomerangDefault BoomerangStat => new(
            //不准修改这个returnTime低于35
            returnTime: 35,
            returnSpeed: 26f,
            acceleration: 1.5f,
            killDistance: 1800
        );
        public override string Texture => ModContent.GetInstance<NightmareHammer>().Texture;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void ExSD()
        {
            //夜明后的锤子应该上4eu了
            Projectile.height = Projectile.width = 66;
            Projectile.timeLeft = 300;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.extraUpdates = 4;
        }
        public override void AI()
        {
            Projectile.rotation += 0.2f;
            Lighting.AddLight(Projectile.Center, TorchID.Purple);
            DrawTrailingDust();
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
        public override void PostAI()
        {
            if (Owner.HasProj<NightmareHammerMinion>())
            {
                Vector2 fireVelocity = Projectile.velocity.SafeNormalize(Vector2.Zero);
                Color Firecolor = LAPUtilities.LerpColor(Color.Black, Color.DarkViolet);
                new Fire(Projectile.Center + Main.rand.NextVector2Circular(8,8), fireVelocity * 4.5f, Firecolor, Main.rand.Next(60,90), Main.rand.NextFloat(MathHelper.TwoPi), 1f, Main.rand.NextFloat(0.20f,0.25f)).SpawnToPriorityNonPreMult();
            }
        }
        private void DoStealth()
        {
            if (Projectile.GetTargetSafe(out NPC target, TargetIndex))
                Projectile.HomingNPCBetter(target, 24f, 20f, 1);

            //如果超出了玩家屏幕范围，且玩家仍然没有仆从锤，生成仆从锤
            if (LAPUtilities.OutOffScreen(Projectile.Center, 1.2f) && !Owner.HasProj<NightmareHammerMinion>())
            {
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<NightmareHammerMinion>(), Projectile.damage, 0f, Owner.whoAmI);
                proj.Calamity().stealthStrike = true;
                SoundEngine.PlaySound(SoundsMenu.Mana_Toss, Owner.Center);
                //而后，杀死射弹。
                Projectile.Kill();
            }
        }
        //首次投掷出去时的AI
        private void DoShooted()
        {
            if (AttackTimer == 0)
            {
                if (Owner.HasProj<NightmareHammerMinion>())
                {
                    //压制音量，这里由仆从锤的射线声作为主导
                    SoundEngine.PlaySound(SoundsMenu.Mana_Toss with { Pitch = 0.4f, Volume = 0.2f }, Owner.Center);
                }
                else
                    SoundEngine.PlaySound(SoundID.Item103, Owner.Center);
            }
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
                }
                else
                {
                    Update = true;
                    AttackType = DoType.IsStealth;
                    Projectile.velocity *= -1;
                }
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(SoundID.Item88, Projectile.Center);
            target.AddBuff(BuffID.ShadowFlame, 360);
            //移除普通攻击的梦魇之箭生成
            if (!Stealth && Projectile.numHits % 2 == 0)
                NightmareArrowDrop(target, Projectile.damage / 2);
            //处于潜伏回击时，击中敌人传入这个单位
            if (AttackType != DoType.IsStealth)
                return;

            SoundEngine.PlaySound(UseSound, Projectile.Center);
            //优先生成挂载射弹
            if (!Owner.HasProj<NightmareHammerProjClone>())
            {
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<NightmareHammerProjClone>(), Projectile.damage, 0f, Owner.whoAmI);
                proj.Calamity().stealthStrike = true;
            }
            else if (!Owner.HasProj<NightmareHammerMinion>())
            {
                Projectile hangingProj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<NightmareHammerMinion>(), Projectile.damage, Projectile.knockBack, Owner.whoAmI);
                hangingProj.Calamity().stealthStrike = true;
                SoundEngine.PlaySound(SoundsMenu.Mana_Toss, Projectile.Center);
            }
            //然后直接处死这个射弹
            Projectile.Kill();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.QuickDrawBloomEdge(Color.DarkMagenta, 6);
            Projectile.QuickDrawWithTrailing(0.7f, Color.White);
            return false;
        }
        private void NightmareArrowDrop(NPC target, int flareDamage)
        {
            //这下面一长串都是为了处理……生成的
            //返程写的挺fuck的
            float xDist = Main.rand.NextFloat(10f, 100f) * Main.rand.NextBool().ToDirectionInt();
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

        private void DrawTrailingDust()
        {
            if (Stealth && Main.rand.NextBool(2) && AttackType == DoType.IsStealth)
                return;
            Vector2 spawnPos = Projectile.Center + Main.rand.NextVector2Circular(11, 11);
            Color Firecolor = LAPUtilities.LerpColor(Color.Purple, Color.DarkViolet);
            new TurbulenceGlowBall(spawnPos, 0.8f, Firecolor, 40, 0.32f, Projectile.velocity.SafeNormalize(Vector2.UnitX).ToRotation()).Spawn();
            bool drawBlack = Main.rand.NextBool();
            Color glowColor = drawBlack ? Color.Black : LAPUtilities.LerpColor(Color.Violet, Color.DarkViolet);
            Vector2 glowDustVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4 / 2f, MathHelper.PiOver4 / 2f))* 4f;
            new ShinyOrbParticle(spawnPos, glowDustVelocity, glowColor, 40, 0.8f, drawBlack ? BlendStateID.Alpha : BlendStateID.Additive).Spawn();
        }
    }
}