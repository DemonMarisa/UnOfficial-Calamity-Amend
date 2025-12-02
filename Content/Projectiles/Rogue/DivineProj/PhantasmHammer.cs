using System;
using System.IO;
using System.Xml.Schema;
using CalamityMod;
using CalamityMod.Graphics.Primitives;
using LAP.Core.ParticleSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Serialization;
using Steamworks;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Content.Items.Weapons.Rogue.Hammer;
using UCA.Content.Particiles;
using UCA.Core.BaseClass;
using UCA.Core.GlobalInstance.Projectiles;
using UCA.Core.Utilities;

namespace UCA.Content.Projectiles.Rogue.DivineProj
{
    public class PhantasmalHammer: RogueProjClass, ILocalizedModType
    {
        public UCAGlobalProj ModProj => Projectile.UCA();
        public override string Texture => ModContent.GetInstance<DivineHammer>().Texture;
        public int TargetIndex
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public ref float AttackTimer => ref Projectile.ai[1];
        public ref float TotalArcAngle => ref Projectile.ai[2];
        public bool IsFlip
        {
            get => ModProj.ExtraAI[0] is 1f;
            set => ModProj.ExtraAI[0] = value ? 1f : 0f;
        }
        public ref float SpriteRotation => ref ModProj.ExtraAI[1];
        public ref float ArcRotation => ref ModProj.ExtraAI[1];
        const int SetUpdate = 3;
        //是否画圆
        private bool _isArcRotating = false; 
        //旋转起始角
        private float _arcStartRotation;
        private bool ShouldDrawVertex = true;
        //总转角
        //持续帧数
        private const int ArcDuration = 15 * SetUpdate;
        //发起旋转前的原始速度
        private float _originalSpeed;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 30;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 66;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.extraUpdates = SetUpdate;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            Projectile.Opacity = 0f;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(IsFlip);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            IsFlip = reader.ReadBoolean();
        }
        public override bool? CanDamage() => AttackTimer > ArcDuration;
        public override void AI()
        {
            if(AttackTimer == 0f)
            {
                IsFlip = TotalArcAngle < 0;
                SpriteRotation = Projectile.velocity.ToRotation();
            }

            if (Projectile.timeLeft < 50)
                Projectile.Kill();

            //生成，渐变
            if (!ShouldDrawVertex)
                Projectile.rotation += 0.2f * IsFlip.ToDirectionInt();
            else
            {
                Projectile.rotation = Projectile.velocity.ToRotation();
            }

            Projectile.Opacity += 0.1f;
            Projectile.Opacity = MathHelper.Clamp(Projectile.Opacity, 0f, 1f);
            AttackTimer += 1f;
            //绘制圆弧运动
            if (AttackTimer < ArcDuration)
            {
                DrawArc();
                return;
            }
        
            if (Projectile.GetTargetSafe(out NPC target, TargetIndex, true))
                Projectile.HomingNPCBetter(target, 1800f, 20f, 20f, ignoreDist: true);
            else
            {
                Projectile.AccelerateToTarget(Owner.Center, 20f, 1.8f, 4800);
                if (Projectile.Hitbox.Intersects(Owner.Hitbox))
                {
                    Projectile.Kill();
                    Projectile.netUpdate = true;
                }
            }
        }

        private void DrawArc()
        {
            if (!_isArcRotating)
            {
                _isArcRotating = true;
                _arcStartRotation = Projectile.velocity.ToRotation();
                _originalSpeed = Projectile.velocity.Length();
                Projectile.velocity *= 0.40f;
            }

            if (_isArcRotating)
            {
                float arcProgress = (float)AttackTimer / ArcDuration;
                //计算当前的角度
                ArcRotation = _arcStartRotation + TotalArcAngle * arcProgress;
                //同步旋转角度与速度
                Projectile.velocity = ArcRotation.ToRotationVector2() * Projectile.velocity.Length();
                //?
                if (AttackTimer >= ArcDuration)
                {
                    //重置速度
                    Projectile.velocity = ArcRotation.ToRotationVector2() * _originalSpeed;
                    //跳出循环
                    _isArcRotating = false;
                }
                return;
            }
        }
        public override Color? GetAlpha(Color lightColor) => new(251, 184, 255, 100);
        private SpriteBatch SB { get => Main.spriteBatch; }
        #region  Draw
        public float SetProjWidth(float ratio)
        {
            float width = Projectile.width;
            width *= MathHelper.SmoothStep(0.8f, 0.6f, Utils.GetLerpValue(0f, 0.5f, ratio, true));
            return width;
        }
        public Color SetTrailColor(float ratio)
        {
            float velocityOpacityFadeout = Utils.GetLerpValue(1f, 5f, Projectile.velocity.Length(), true);
            Color c = DivineHammerProj.TrailColor * Projectile.Opacity * (1f - ratio);
            return c * Utils.GetLerpValue(0.04f, 0.1f, ratio, true) * velocityOpacityFadeout;
        }
        //DrawOffset
        public Vector2 PrimitiveOffsetFunction(float ratio)
        {
            return Projectile.Size * 0.5f + Projectile.velocity.SafeNormalize(Vector2.Zero) * Projectile.scale * 0.5f * 0.5f;
        }

        public void DrawVertex()
        {
            float spinRotation = Main.GlobalTimeWrappedHourly * 5.2f;
            GameShaders.Misc["CalamityMod:SideStreakTrail"].UseImage1("Images/Misc/Perlin");
            PrimitiveRenderer.RenderTrail(Projectile.oldPos, new(SetProjWidth, SetTrailColor, PrimitiveOffsetFunction, shader: GameShaders.Misc["CalamityMod:SideStreakTrail"]), 51);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (ShouldDrawVertex)
            {
                SB.EnterShaderRegion(BlendState.Additive);
                DrawVertex();
                SB.ExitShaderRegion();
            }
            Projectile.QuickDrawBloomEdge(Color.LightPink, rotOffset: -MathHelper.PiOver4);
            Projectile.QuickDrawWithTrailing(0.7f, Color.GhostWhite, -MathHelper.PiOver4);
            return false;
        }
        #endregion
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            ShouldDrawVertex = false;
            //从灾厄上抄下来的, 由于有一些特殊效果所以粒子会少一点
            float numberOfDusts = 4f;
            float rotFactor = 360f / numberOfDusts;
            for (int i = 0; i < numberOfDusts; i++)
            {
                int dType = Main.rand.NextBool(2) ? DustID.GemDiamond : DustID.WitherLightning;
                float rot = MathHelper.ToRadians(i * rotFactor);
                Vector2 offset = new Vector2(4.8f, 0).RotatedBy(rot * Main.rand.NextFloat(3.1f, 4.1f));
                Vector2 velOffset = new Vector2(4f, 0).RotatedBy(rot * Main.rand.NextFloat(3.1f, 4.1f));
                Dust dust = Dust.NewDustPerfect(Projectile.Center + offset, dType, new Vector2(velOffset.X, velOffset.Y), 0, default, 0.3f);
                dust.noGravity = true;
                dust.velocity = velOffset;
                dust.scale = 1.2f;
            }
            SoundEngine.PlaySound(DivineHammerProj.HitSound with {Volume = 0.8f }, Projectile.Center);
            SoundEngine.PlaySound(SoundID.Item109 with {MaxInstances = 1, Pitch = 0.2f, PitchVariance = 0.1f }, Owner.Center);
            TargetIndex = target.whoAmI;
            if (Projectile.numHits % 2 is 0)
            {
                int ownedProjCounts = Owner.ownedProjectileCounts[Type]; 
                //每轮生成两个，超过3把以上的锤子在场时生成一个
                int maxCount = ownedProjCounts < 3 ? 2 : 1;
                SpawnNebulaShot(Owner, Projectile, target, maxCount);
            }
            
        }

        public static void SpawnNebulaShot(Player owner, Projectile projectile, NPC target, int maxSpawnCounts = 2, bool canSpawnDust = true)
        {
            //灾厄抄写下来的
            projectile.netUpdate = true;
            for (int i = 0; i < maxSpawnCounts; ++i)
            {
                //确定位置
                Vector2 spawnPosBase = (owner.MountedCenter - target.Center).SafeNormalize(Vector2.UnitX);
                float warpRadians = Main.rand.NextFloat(-MathHelper.PiOver2 * 0.45f, MathHelper.PiOver2 * 0.45f);
                Vector2 warpOffset = 150f * spawnPosBase.RotatedBy(warpRadians);
                Vector2 spawnPos =  owner.MountedCenter + warpOffset * Main.rand.Next(6, 9) * 0.25f;
                //确定初始速度，精准一些。
                Vector2 velDir = (target.Center - spawnPos).SafeNormalize(Vector2.UnitX);
                SpawnDust(spawnPos, velDir);
                if (projectile.owner == Main.myPlayer)
                {
                    Projectile proj = Projectile.NewProjectileDirect(projectile.GetSource_FromThis(), spawnPos, velDir * Main.rand.NextFloat(15f, 19f), ModContent.ProjectileType<NebulaEnegry>(), projectile.damage, 2.5f, projectile.owner, target.whoAmI);
                    proj.DamageType = ModContent.GetInstance<RogueDamageClass>();
                    proj.ai[0] = target.whoAmI;
                    proj.UCA().ExtraAI[1] = canSpawnDust.ToInt();
                }
            }
        }
        private static void SpawnDust(Vector2 spawnPos, Vector2 dir)
        {
            float baseRot = dir.ToRotation() + MathHelper.PiOver2;
            int totalParticleCounts = 8;
            int repeatedCountForAxis = 24;
            for (int k = 4; k < repeatedCountForAxis - 4; k++)
            {
                //在外部调用这个以整体对点位进行偏移。
                //整体扩大一下，因为距离明显过小了
                float shortAxis = k * 1.7f;
                float longAxis = (repeatedCountForAxis - k) * 1.7f;
                for (int j = 0; j < totalParticleCounts; j++)
                {
                    float angle = j * (float)(MathHelper.TwoPi / totalParticleCounts);
                    Vector2 edge = spawnPos + GetCertainPointBaseOnVectorCircle(angle, shortAxis, longAxis, baseRot);
                    Color drawColor = Color.Lerp(DivineHammerProj.TrailColor with { A = 75 }, Color.MediumPurple with { A = 75 }, (totalParticleCounts - j) / (float)totalParticleCounts);
                    ShinyOrbParticle orbs = new ShinyOrbParticle(edge, dir * 0.2f, drawColor, 30, Main.rand.NextFloat(0.11f, 0.22f), BlendStateID.Alpha);
                    orbs.Spawn();
                }
            }
            //在中心点位额外绘制一个orb
             new ShinyOrbParticle(spawnPos, dir * 0.2f, Color.Violet, 30, 0.75f, glowCenter:false).Spawn();
             new ShinyOrbParticle(spawnPos, dir * 0.2f, Color.MediumPurple, 30, 0.45f, glowCenter:false).Spawn();
        }
        /// <summary>
        /// 基于圆+极坐标的复杂计算来获取需要的位置
        /// </summary>
        public static Vector2 GetCertainPointBaseOnVectorCircle(float radians, float shortAxis, float longAxis, float rotation = 0f)
        {
            //极坐标转化
            float x = longAxis * (float)Math.Cos(radians);
            float y = shortAxis * (float)Math.Sin(radians);

            //转化你输入的rotation，让整个图整体旋转一定角度
            float cosRot = (float)Math.Cos(rotation);
            float sinRot = (float)Math.Sin(rotation);

            //最后转化为实际需要的点位
            float rotX = x * cosRot - y * sinRot;
            float rotY = x * sinRot + y * cosRot;
            return new Vector2(rotX, rotY);
        }

        public override bool PreKill(int timeLeft)
        {
            //即将死亡的时候，生成一个克隆锤子。
            int projID = ModContent.ProjectileType<PhantasmalHammerClone>();
            //获取当前锤子到玩家的向量，归一化后转90°
            Vector2 dir = (Projectile.Center - Owner.Center).SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.PiOver2 * IsFlip.ToDirectionInt());
            //转化为实际速度
            Vector2 vel = dir * 18f;
            //直接追加这个射弹。
            Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, vel, projID, Projectile.damage, Projectile.knockBack, Owner.whoAmI);
            proj.ai[2] = TargetIndex;
            proj.localAI[0] = IsFlip.ToDirectionInt();
            return true;
        }
        
    }
}