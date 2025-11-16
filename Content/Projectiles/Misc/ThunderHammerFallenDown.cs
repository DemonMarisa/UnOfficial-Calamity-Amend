using CalamityMod.Graphics.Primitives;
using CalamityMod.Physics;
using LAP.Core.SpecificEffectManagers;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Assets.Sounds;
using UCA.Content.Items.Weapons.Rogue;
using UCA.Content.Particiles;
using UCA.Content.Projectiles.Rogue;
using UCA.Content.Projectiles.Rogue.ThunderProj;
using UCA.Core.BaseClass;
using UCA.Core.Utilities;

namespace UCA.Content.Projectiles.Misc
{
    public class ThunderHammerFallenDown : BaseRogueProj, ILocalizedModType, IPixelatedPrimitiveRenderer
    {
        public override string Texture => ModContent.GetInstance<ThunderHandler>().Texture;
        public ref float CurRotation => ref Projectile.ai[0];
        public bool CanSmashDust
        {
            get => Projectile.ai[1] == 1f;
            set => Projectile.ai[1] = value ? 1f : 0f;
        }
        public ref float Timer => ref Projectile.ai[2];
        private ref float InitTimer => ref Projectile.localAI[0];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 12;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            //故意做小碰撞箱，让御主贴近的距离合理一些
            Projectile.width = Projectile.height = 70;
            Projectile.extraUpdates = 0;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
            Projectile.timeLeft = 10000;
        }
        public override bool? CanDamage() => true;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
           return targetHitbox.Intersects(Utils.CenteredRectangle(Projectile.Center, new Vector2(1000, 300)));
        }
        public override void AI()
        {
            if (Timer == 0)
            {
                Init();
                return;
            }

            
            //整个过程检测御主的存活等状态
            CheckPlayerStatus();
            //别让Timer < 1，不然又会进行一次初始化
            Timer = MathHelper.Clamp(Timer, 1, Timer);
            //先初始一个向下的速度再说
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            if (Projectile.velocity != Vector2.Zero)
            {
                Timer++;
                //将这个存进去
                CurRotation = Projectile.rotation + Math.Abs(Projectile.velocity.X / 2) + Math.Abs(Projectile.velocity.Y / 2);
                Projectile.velocity.Y = MathHelper.Clamp(Projectile.velocity.Y + 0.8f, 0.01f, 25f);
                CurRotation += Timer;
                //简单处理一下让他不要撞到初始头顶的墙
                if (Timer < 60)
                    Projectile.tileCollide = false;
                else
                    Projectile.tileCollide = true;
                DrawTrailingDust();
            }
            else
            {
                Timer++;
                //延后震屏发生实践以保持同步
                Projectile.extraUpdates = 0;
                if (Timer == 2)
                {
                    ScreenShakeSystem.AddScreenShakes(Projectile.Center, 160 * -Owner.direction, 30, Vector2.UnitY.ToRotation(), 0.2f, true, 1550);
                    //在这里创建绳子实例
                    InitializeRope();
                    //震飞附近所有玩家
                    SmashPlayerToAir();
                    //做掉属性避免继续造成伤害
                }
                //在这里更新绳子
                UpdateRibbon();
                //常驻这些冲击波粒子，因为有射弹额外更新在搞怪，这里生成频率需要降低
                if (Timer % 6 == 0)
                    DrawGroundIdleDust();

                //考虑到实际地形问题，这里取用的是与地面物块完全无关的粒子
                if (CanSmashDust && Timer < 40)
                {
                    if (Main.zenithWorld)
                    {
                        Projectile.friendly = true;
                        Projectile.hostile = true;
                    }
                    DrawSmashDust();
                }
                else if (Timer > 40)
                {
                    //总控射弹对非御主的排斥与御主的弱排斥
                    HandleAllPlayer();
                    //粒子砸地完成确认后，我们才允许御主拾取
                    Projectile.friendly = false;
                    Projectile.hostile = false;
                    if (!Owner.Hitbox.Intersects(Projectile.Hitbox))
                        return;
                    Owner.QuickSpawnItem(Projectile.GetSource_FromThis(), ModContent.ItemType<ThunderHammer>());
                    //记得杀死射弹……
                    Projectile.Kill();
                }
            }
        }
        #region 粒子绘制
        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            Projectile.width = Projectile.height = 100;
            //创建绳子实例
            if (InitTimer == 0f)
                SoundEngine.PlaySound(SoundID.Item35, Owner.Center);
            InitTimer += 1f;
            if (InitTimer > 5f)
            {
                //别让Timer变成1就行了
                Timer = 1;
                Projectile.extraUpdates = 6;
            }
            if (Main.zenithWorld)
                Projectile.damage = 114514191;
        }
        /// <summary>
        /// 锤子下落过程中生成的轨迹粒子
        /// </summary>
        private void DrawTrailingDust()
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
                    float angle = Timer * freq;
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
                Dust flame = Dust.NewDustPerfect(new (Projectile.Center.X, Projectile.Center.Y + 80f), Main.rand.NextFloat() >= height ? HigherDust : BottemDust);
                flame.noGravity = true;
                flame.position += new Vector2(Main.rand.NextFloat(-1, 1) * 5 * GetScale.X, -height * 25f * GetScale.Y - Main.rand.NextFloat(-5f, 5f));
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
            if (Timer < EarlyPhase)
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

                for (int i = 0; i < 60; i++)
                {
                    float height = Main.rand.NextFloat();
                    Dust flame = Dust.NewDustPerfect(initSpawnPos, Main.rand.NextFloat() >= height ? HigherDust : BottemDust);
                    flame.noGravity = true;
                    flame.position += new Vector2(Main.rand.NextFloat(height - 1, 1 - height) * 5 * scale.X, -height * 1 * scale.Y);
                    flame.velocity.Y -= 10f;
                    if (Main.rand.NextBool(4))
                    {
                        flame.position.X -= 12;
                        flame.velocity.X += 0.02f;
                        flame.scale *= 2f;
                    }
                    else 
                        flame.velocity.Y -= Main.rand.NextFloat(5f, 12f);
                }
            }
        }
        private Vector2 GetScale
        {
            get
            {
                Vector2 scale;
                Vector2 start = new(30f, 10f);
                Vector2 middle = new(60f, 20f);
                Vector2 late = new(15f, 5f);
                if (Timer < InitPhase)
                    scale = Vector2.SmoothStep(start, middle, Timer / InitPhase);
                else 
                    scale = Vector2.SmoothStep(middle, late, (Timer - InitPhase) / EarlyPhase);
                return scale;
            }
        }
        #endregion

        #region 控制玩家状态
        /// <summary>
        /// 锤子砸地时，震飞附近的所有玩家
        /// </summary>
        private void SmashPlayerToAir()
        {
            foreach (var plr in Main.player)
            {
                float distance = (Projectile.Center - plr.Center).Length();
                if (!plr.active || plr.dead)
                    continue;

                if (distance > 1000f)
                    continue;
                
                Vector2 vel = -(Projectile.Center - plr.Center).SafeNormalize(Vector2.UnitX);
                //用-log函数控制每个玩家与锤子距离的大小引起的震击力度差异
                distance = (distance / 1000f).ToClamp(0.01f, distance);
                float velY = (1 / distance).ToClamp(0.01f, 4.5f) * 3.2f;
                plr.velocity = vel * 5f - new Vector2(-vel.X * velY/4, velY);
            }
        }
        /// <summary>
        /// 查看御主状态（过远或者死亡）
        /// </summary>
        private void CheckPlayerStatus()
        {
            //如果御主距离锤子过远，处死锤子，并返还原本的弑神锤
            if ((Projectile.Center - Owner.Center).Length() > 3600f)
            {
                Owner.QuickSpawnItem(Projectile.GetSource_FromThis(), ModContent.ItemType<DivineHammer>());
                Projectile.Kill();
            }
            //如果御主死亡，也处死这把锤子
            if (Owner.dead && !Main.zenithWorld)
            {
                Projectile.Kill();
                //启用这个字段，让御主复活后返还弑神锤
                Owner.UCA().ShouldGiveSpareGodsHammer = true;
            }
        }
        /// <summary>
        /// 总控所有玩家状态
        /// </summary>
        private const float ArmOrPushawayDistance = 200f;
        private void HandleAllPlayer()
        {
            foreach (var tar in Main.player)
            {
                float dist = (Projectile.Center - Owner.Center).Length();

                if (!tar.active || tar.dead)
                    continue;
                if (dist > ArmOrPushawayDistance)
                    continue;
                Vector2 pushDir = (Projectile.Center - Owner.Center).SafeNormalize(Vector2.UnitX);
                if (Projectile.owner != tar.whoAmI)
                    HandlePushAway(pushDir, dist);
                else
                    HandleOwnerAnimation(pushDir, dist);
            }
        }
        /// <summary>
        /// 总控非御主的所有玩家，这里会将所有玩家推开
        /// </summary>
        private void HandlePushAway(Vector2 dir, float dist)
        {
            if(dist < ArmOrPushawayDistance)
                Owner.velocity = Owner.velocity - dir * 1 / (dist / ArmOrPushawayDistance); 
        }

        /// <summary>
        /// 总控御主靠近动作，包括手臂动画
        /// </summary>
        private void HandleOwnerAnimation(Vector2 dir, float dist)
        {
            //判定是不是面朝锤子，如果是，则不让手臂有动画
            if (dir.X * Owner.direction < 0)
                return;
            //满足一定距离，让御主伸手
            if (dist < ArmOrPushawayDistance)
            {
                Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, dir.ToRotation() - MathHelper.PiOver2);
                Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, dir.ToRotation() - MathHelper.PiOver2);
                //叠加速度向量，让锤子以一定程度上推开御主，但不影响获取
                Owner.velocity = Owner.velocity - dir * 1 / (dist / 92f); 
            }

        }
        #endregion
        #region 绳子实例控制
        public PixelationPrimitiveLayer LayerToRenderTo => PixelationPrimitiveLayer.AfterPlayers;
        public struct TrailDrawDate(Vector2 drawPos, Color drawColor, Vector2 primitivesHeight, float primitivesHeightRot)
        {
            /// <summary>
            /// 传入的世界坐标
            /// </summary>
            public Vector2 PosDate = drawPos;
            /// <summary>
            /// 传入每个点的颜色
            /// </summary>
            public Color DrawColor = drawColor;
            /// <summary>
            /// 顶点的偏移
            /// </summary>
            public Vector2 PrimitivesOffset = primitivesHeight;
            /// <summary>
            /// 顶点偏移的整体旋转
            /// </summary>
            public float PrimitivesHeightRot = primitivesHeightRot;
        }
        public struct DrawSetting(Texture2D texture, bool usePosTransformation, bool usePixelTransformation)
        {
            public Texture2D texture = texture;
            public bool usePosTransformation = usePosTransformation;
            public bool usePixelTransformation = usePixelTransformation;
        }
        /// <summary>
        /// 绳子的起点
        /// </summary>
        public Vector2 RopStartPoint
        {
            get
            {
                Vector2 baseProjCenter = Projectile.Center + Projectile.velocity * Projectile.scale * Projectile.width * 1.1f;
                Vector2 offsetDir = Projectile.rotation.ToRotationVector2().RotatedBy(-MathHelper.PiOver2) * -19f;
                Vector2 offset = Projectile.rotation.ToRotationVector2() * -1.34f;
                return baseProjCenter + (offsetDir + offset) * Projectile.scale;
            }
        }
        /// <summary>
        /// 绳子实例
        /// </summary>
        public RopeHandle? Rope;

        /// <summary>
        /// 初始化绳子实例
        /// </summary>
        public void InitializeRope()
        {
            //多少个体节
            int ribbonSegmentCount = 20;
            //长度
            float Length = 100f;
            //体节之间的距离
            float distancePerSegment = Length / ribbonSegmentCount;
            RopeSettings ribbonSettings = new RopeSettings()
            {
                StartIsFixed = true,
                Mass = 0.5f,
                RespondToEntityMovement = true,
                RespondToWind = true,
                TileColliderArea = Collision.TileCollision(Projectile.Center, Projectile.velocity, Projectile.width, Projectile.height)

            };
            Vector2 dir;
            if (CurRotation > 0)
                dir = -CurRotation.ToRotationVector2().SafeNormalize(Vector2.UnitX);
            else
                dir = (CurRotation - MathHelper.PiOver2).ToRotationVector2().SafeNormalize(Vector2.UnitX);
            Vector2 gravity = dir;
            Rope = ModContent.GetInstance<RopeManagerSystem>().RequestNew(RopStartPoint, Projectile.Center, ribbonSegmentCount, distancePerSegment, gravity, ribbonSettings, 100);
        }
        /// <summary>
        /// 更新绳子
        /// </summary>
        public void UpdateRibbon()
        {
            //确保绳子需要先行一步的初始化。
            if (Rope is not RopeHandle rope)
                return;
            rope.Start = RopStartPoint;
            Vector2 dir = -CurRotation.ToRotationVector2().SafeNormalize(Vector2.UnitX);
            //我曹，这里太tm小众变态了
            if (CurRotation < 0)
            {
                Vector2 baseProjCenter = Projectile.Center + Projectile.velocity * Projectile.scale * Projectile.width * 1.1f;
                Vector2 offsetDir = Projectile.rotation.ToRotationVector2().RotatedBy(-MathHelper.PiOver2) * 5f;
                Vector2 offset = Projectile.rotation.ToRotationVector2() * -21f;
                rope.Start = baseProjCenter + (offsetDir + offset) * Projectile.scale;
            }
            else
                rope.Gravity = dir * new Vector2(1, -1f);

        }
        SpriteBatch SB { get => Main.spriteBatch; }
        /// <summary>
        /// 管理绳子实例与进行像素渲染
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="layer"></param>
        public void RenderPixelatedPrimitives(SpriteBatch spriteBatch, PixelationPrimitiveLayer layer)
        {
            //实际进行之前不要绘制
            if (Rope is not RopeHandle rope)
                return;
            SB.End();
            SB.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null);

            var instance = ModContent.GetInstance<ThunderHandler>();
            string path = $"{instance.GetType().Namespace}.{instance.GetType().Name}Rope".Replace(".", "/");
            //获取这个绳子示例。
            Texture2D texture = ModContent.Request<Texture2D>(path).Value;
            Vector2[] ribbonPositions = rope.Positions.ToArray();
            DrawSetting drawSetting = new(texture, true, true);
            List<TrailDrawDate> trailDrawDate = [];
            List<TrailDrawDate> trailDrawDate1 = [];
            List<TrailDrawDate> trailDrawDate2 = [];
            int positionCount = ribbonPositions.Length;
            for (int i = 0; i < positionCount - 1; i++)
            {
                // 这个顶点的旋转，从这个位置指向下一个位置
                Vector2 Position = ribbonPositions[i];
                Vector2 NextPosition = ribbonPositions[i + 1];
                float rot = (NextPosition - Position).ToRotation();
                float height = 2.5f;
                if (i < 8)
                {
                    rot -= MathHelper.PiOver4;
                }

                Vector2 dir1 = Position.SafeNormalize(Vector2.UnitX);
                Vector2 pos1 = Position + dir1 * 4.5f;
                Vector2 pos2 = Position - dir1 * 4.5f;
                trailDrawDate.Add(new(Position, Color.White, new Vector2(0, height), rot));
            }
            DrawTrail([.. trailDrawDate], drawSetting);
            SB.End();
            SB.BeginDefault();
        }
        /// <summary>
        /// 实际绘制绳子
        /// </summary>
        /// <param name="DrawDate"></param>
        /// <param name="drawSetting"></param>
        public static void DrawTrail(TrailDrawDate[] DrawDate, DrawSetting drawSetting)
        {
            List<VertexPosition2DColorTexture> Vertexlist = new List<VertexPosition2DColorTexture>();

            for (int i = 0; i < DrawDate.Length; i++)
            {
                float progress = (float)i / DrawDate.Length;
                //绘制位置
                Vector2 DrawPos = DrawDate[i].PosDate - (drawSetting.usePosTransformation ? Main.screenPosition : Vector2.Zero);

                if (drawSetting.usePixelTransformation)
                    DrawPos = DrawPos / 2;

                //每个片的高度与旋转
                Vector2 PrimitivesHeight = DrawDate[i].PrimitivesOffset;
                float PrimitivesHeightRot = DrawDate[i].PrimitivesHeightRot;
                Color DrawColor = DrawDate[i].DrawColor;

                Vertexlist.Add(new VertexPosition2DColorTexture(DrawPos - PrimitivesHeight.RotatedBy(PrimitivesHeightRot), DrawColor, new Vector2(progress, 0), 0));
                Vertexlist.Add(new VertexPosition2DColorTexture(DrawPos + PrimitivesHeight.RotatedBy(PrimitivesHeightRot), DrawColor, new Vector2(progress, 1), 0));
            }

            Main.graphics.GraphicsDevice.Textures[0] = drawSetting.texture;
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, Vertexlist.ToArray(), 0, Vertexlist.Count - 2);
        }
        #endregion

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            //后续的情况下不再进入下方的所有状态
            if (Projectile.velocity == Vector2.Zero)
                return false;
            //设置为零速度，且后续我们不在更新速度
            Projectile.velocity = Vector2.Zero;
            //略微嵌入进去
            Projectile.position += Vector2.UnitY * 45f;
            CanSmashDust = true;
            //天顶世界下变成……钢管。
            SoundEngine.PlaySound(Main.zenithWorld ? SoundsMenu.Pipes : SoundsMenu.Smash_GroundHeavy, Projectile.Center);
            Timer = 1;
            CurRotation = Owner.direction > 0 ? MathHelper.PiOver4 : -(MathHelper.Pi + MathHelper.PiOver4);
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.QuickDrawBloomEdge(Color.White, 16, CurRotation, MathHelper.Lerp(2f, 4f, MathF.Sin(Timer / 20f)).ToClamp(2f, 4f));
            Projectile.QuickDrawWithTrailing(0.7f, Color.White, CurRotation);
            return false;
        }
    }
}
