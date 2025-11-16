using CalamityMod.Graphics.Primitives;
using CalamityMod.Items.Placeables.Walls;
using CalamityMod.Physics;
using LAP.Core.Graphics.Primitives.Trail;
using LAP.Core.Utilities;
using Microsoft.Build.Evaluation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Content.Items.Weapons.Rogue;
using UCA.Content.Projectiles.Rogue;
using UCA.Core.BaseClass;
using UCA.Core.Keybinds;
using UCA.Core.Utilities;
using static UCA.Content.Projectiles.Rogue.ThunderProj.ThunderHandler;

namespace UCA.Content.Projectiles.Rogue.ThunderProj
{
    public class ThunderHandler : BaseRogueProj, IPixelatedPrimitiveRenderer
    {
        public override string Texture => (GetType().Namespace + "." + GetType().Name).Replace(".","/");
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public bool InitProj = false;
        public float InitRotation = 0f;
        public int OwnerDir = -1;
        public float ProjOpacity = 0f;
        private enum DoType
        {
            IsToTarget,
            IsStrike,
            IsIdle
        }
        private enum DoStrike
        {
            IsFlyingUp,
            IsStrikingDown,
            IsHandleUp
        }
        public enum AttackMode
        {
            ModeStrike,
            ModeGeneral
        }
        private int TargetIndex
        {
            get => (int)Projectile.ai[2];
            set => Projectile.ai[2] = value;
        }
        private ref float AttackTimer => ref Projectile.ai[1];
        private DoType AttackType
        {
            get => (DoType)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }
        private DoStrike StrikeType
        {
            get => (DoStrike)Projectile.UCA().ExtraAI[0];
            set => Projectile.UCA().ExtraAI[0] = (float)value;
        }
        public AttackMode HammerMode
        {
            get => Owner.GetModPlayer<ThunderHandlerSwitchModeRecorder>().StoredMode;
            set => Owner.GetModPlayer<ThunderHandlerSwitchModeRecorder>().StoredMode = value;
        }
        /// <summary>
        /// 绳子的起点
        /// </summary>
        public Vector2 RopStartPoint
        {
            get
            {
                Vector2 baseProjCenter = Projectile.Center + Projectile.velocity * Projectile.scale * 32 * 1.1f;
                Vector2 offsetDir = Projectile.rotation.ToRotationVector2().RotatedBy(-MathHelper.PiOver2) * -19f;
                return baseProjCenter + (offsetDir + Projectile.rotation.ToRotationVector2() * -1.34f) * Projectile.scale;
            }
        }
        /// <summary>
        /// 绳子实例
        /// </summary>
        public RopeHandle? Rope;
        public override void ExSD()
        {
            //这里的大小是无所谓的，因为本身不会造成任何伤害。
            Projectile.width = Projectile.height = 32;
            Projectile.extraUpdates = 0;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.noEnchantments = true;
            Projectile.noEnchantmentVisuals = true;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
            Projectile.timeLeft = 114514;
        }
        public override bool? CanDamage() => false;
        public override void AI()
        {
            if (!InitProj)
            {
                InitializeRope();
                InitProj = true;
            }
            HandleHeldProjBase();
            UpdateRibbon();
            Main.NewText(HammerMode);
            switch (HammerMode)
            {
                case AttackMode.ModeStrike:
                    UpdateMode_Strike();
                    break;
                case AttackMode.ModeGeneral:
                    UpdateMode_General();
                    break;
            }
        }
        private void HandleHeldProjBase()
        {
            Owner.heldProj = Projectile.whoAmI;
            Owner.ChangeDir(Projectile.rotation.ToRotationVector2().X < 0 ? -1 : 1);

            Projectile.timeLeft = 2;
            Projectile.netSpam = 0;
            Projectile.netUpdate = true;
        }
        public override void OnKill(int timeLeft)
        {
            Main.mouseRight = false;
            Owner.itemTime = 0;
            Owner.itemAnimation = 0;
            Rope?.Dispose();
        }
        #region 总控手持AI
        private void UpdateMode_General()
        {
            Oscillation += 0.025f;
            if (Oscillation > 0.125f)
                AttackTimer = 1;
            if (Main.mouseRight)
                HandleToMouse();
            else
                HandleIdle();
        }
        /// <summary>
        /// 总控按住鼠标右键时的AI
        /// </summary>
        private void HandleToMouse()
        {
            UpdateMousePosition();
            UpdateAttackType();
        }

        private void UpdateAttackType()
        {

        }

        #endregion

        #region 控制锤子的悬挂状态
        float Oscillation = 0;
        /// <summary>
        /// 悬挂
        /// </summary>
        private void HandleIdle()
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
            Vector2 targetPos = anchorPos + mousePull;
            Projectile.Center = Vector2.Lerp(Projectile.Center, targetPos, 0.1f);
            //平滑当前角度插值，但是做出一定程度的限制
            float angleToMouse = Projectile.AngleTo(Main.MouseWorld);
            //计算射弹位置与鼠标位置的水平插值。
            bool dir = Projectile.Center.X - Main.MouseWorld.X < 0;
            //为射弹做一定程度的指针角度修正。
            Projectile.rotation = Projectile.rotation.AngleLerp(-MathHelper.PiOver2 + angleToMouse / 45f * dir.ToDirectionInt(), 0.1f);

        }
        /// <summary>
        /// 按住右键过渡至指针位置中心
        /// </summary>
        private void UpdateMousePosition()
        {
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);
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
        private void UpdateIdleHammerAttack(NPC target)
        {
            switch (AttackType)
            {
                case DoType.IsToTarget:
                    DoIdle_ToTarget(target);
                    break;
                case DoType.IsIdle:
                    DoIdle_IsIdle(target);
                    break;
            }
        }
        private void DoIdle_ToTarget(NPC target)
        {

        }

        private void DoIdle_IsIdle(NPC target)
        {
            //计算期望向量值，让他位于玩家与敌对单位的中心点
            Vector2 dir = (Owner.Center - target.Center) / 2;
        }

        #endregion

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

        private void Strike_StrikingDown()
        {
            //将速度飞向过去。用追踪方法
            Projectile.HomingTarget(PlatformPos, 99999, 24f, 20f, MathHelper.ToRadians(30f));
            Projectile platformProj = Main.projectile[PlatformIndex];
            if (Projectile.Hitbox.Intersects(platformProj.Hitbox))
            {
                //将射弹略微嵌入进去
                Projectile.position.Y += 60f;
                Projectile.extraUpdates = 0;
                StrikeType = DoStrike.IsHandleUp;
            }
        }
        private void Strike_HanldeUp()
        {
            //清零当前速度
            Projectile.velocity = Vector2.Zero;
            AttackTimer += 1;
            //时机一旦合适，信号发送给平台处死，并回归常规攻击模组
            if(AttackTimer > 60f)
            {
                Projectile platformProj = Main.projectile[PlatformIndex];
                platformProj.ai[1] = 1f;
                //将总的模式切换回常规模式，并短暂屏蔽战技一段时间
                HammerMode = AttackMode.ModeGeneral;
                //重置AttackTimer并发送数据包
                AttackTimer = 0;
                Projectile.netUpdate = true;
            }
        }

        private void Strike_FlyingUp()
        {
            if (AttackTimer == 0f)
            {
                //给予射弹向上的初速度，并提供额外更新
                Projectile.velocity = new Vector2(0f, -5f);
                Projectile.extraUpdates = 3;
            }
            AttackTimer += 1;
            Projectile.velocity.Y += AttackTimer * 0.05f;
            if ((Projectile.Center - Owner.Center).Length() > 3400f)
            {
                StrikeType = DoStrike.IsStrikingDown;
                Projectile.netUpdate = true;
                Projectile.Center = new(Owner.Center.X, Owner.Center.Y - 1800f);
                Projectile.extraUpdates = 6;
                AttackTimer = 0f;
                //于指针位置处构造假平台
                Projectile platform = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Owner.LocalMouseWorld(), Vector2.Zero, ModContent.ProjectileType<ThunderPlatform>(), 0, 0, Owner.whoAmI);
                PlatformIndex = platform.whoAmI;
                PlatformPos = platform.Center;

            }
        }

        #endregion
        #region 绳子实例控制
        public void InitializeRope()
        {
            //多少个体节
            int ribbonSegmentCount = 20;
            //长度
            float Length = 80f;
            //体节之间的距离
            float distancePerSegment = Length / ribbonSegmentCount;
            RopeSettings ribbonSettings = new RopeSettings()
            {
                StartIsFixed = true,
                Mass = 0.9f,
                RespondToEntityMovement = false,
                RespondToWind = false
                
            };
            Vector2 gravity = Projectile.rotation.ToRotationVector2() * -0.35f;
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
            rope.Gravity = Projectile.rotation.ToRotationVector2() * -0.35f;

        }
        SpriteBatch SB { get => Main.spriteBatch; }
        public void RenderPixelatedPrimitives(SpriteBatch spriteBatch, PixelationPrimitiveLayer layer)
        {
            if (Rope is not RopeHandle rope)
                return;
            SB.End();
            SB.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null);

            string path = $"{GetType().Namespace}.{GetType().Name}Rope".Replace(".", "/");
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
            //绘制这根绳子。
            SB.End();
            SB.BeginDefault();
        }
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
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.QuickDrawBloomEdge(posMove: 2.5f,rotOffset : +MathHelper.PiOver4);
            Projectile.QuickDrawWithTrailing(0.4f, Color.White, +MathHelper.PiOver4);
            return false;
        }
    }
    public class ThunderHandlerSwitchModeRecorder : ModPlayer
    {
        public AttackMode StoredMode = AttackMode.ModeGeneral;
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (UCAKeybind.WeaponSkillHotKey.JustPressed && StoredMode == AttackMode.ModeGeneral)
                StoredMode = AttackMode.ModeStrike;
        }
    }
}
