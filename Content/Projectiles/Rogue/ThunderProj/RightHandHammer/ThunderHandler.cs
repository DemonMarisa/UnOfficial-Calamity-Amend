using CalamityMod.Graphics.Primitives;
using CalamityMod.Physics;
using LAP.Core.Graphics.Primitives.Trail;
using LAP.Core.Keybind;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Core.BaseClass;
using UCA.Core.GlobalInstance.Projectiles;
using UCA.Core.Keybinds;
using UCA.Core.Utilities;

namespace UCA.Content.Projectiles.Rogue.ThunderProj.RightHandHammer
{
    public partial class ThunderHandler : RogueProjClass, IPixelatedPrimitiveRenderer
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
        /// <summary>
        /// 枚举 - 脱手状态下的攻击
        /// </summary>
        public enum DoType
        {
            IsIdle,
            IsRotatedTo,
            IsChasing,
            IsStop
        }
        /// <summary>
        /// 枚举 - 战技攻击
        /// </summary>
        public enum DoStrike
        {
            IsFlyingUp,
            IsStrikingDown,
            IsHandleUp
        }
        /// <summary>
        /// 枚举 - 脱手与战技状态的切换
        /// </summary>
        public enum AttackMode
        {
            ModeStrike,
            ModeGeneral
        }
        private UCAGlobalProj ModProj => Projectile.UCA();
        private int TargetIndex
        {
            get => (int)Projectile.ai[2];
            set => Projectile.ai[2] = value;
        }
        private ref float AttackTimer => ref Projectile.ai[1];
        public DoType AttackType
        {
            get => (DoType)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }
        public DoStrike StrikeType
        {
            get => (DoStrike)ModProj.ExtraAI[0];
            set => ModProj.ExtraAI[0] = (float)value;
        }
        public bool IdleHammer_CanStrike
        {
            get => ModProj.ExtraAI[1] == 1f;
            set => ModProj.ExtraAI[1] = value ? 1f : 0f;
        }
        public bool CanStrike = false;
        /// <summary>
        /// 绳子的起点
        /// </summary>
        public Vector2 RopStartPoint
        {
            get
            {
                Vector2 baseProjCenter = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.UnitX) * Projectile.scale * 1.1f;
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
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
            Projectile.netImportant = true;
            Projectile.timeLeft = 114514;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if(AttackType == DoType.IsChasing)
                return GetRectCentered(Projectile.Center + Projectile.rotation.ToRotationVector2(), Projectile.width, Projectile.height).Intersects(targetHitbox);

            return base.Colliding(projHitbox, targetHitbox);
        }
        public static Rectangle GetRectCentered(Vector2 center, float w, float h)
        {
            return new Rectangle((int)(center.X - w / 2), (int)(center.Y - h / 2), (int)w, (int)h);
        }
        public override bool? CanDamage() => true;
        float Timer = 0f;
        public override void AI()
        {
            if (!InitProj)
            {
                InitializeRope();
                InitProj = true;
            }
            HandleHeldProjBase();
            UpdateRibbon();
            Timer += 1;
            if (Timer % (Projectile.extraUpdates + 8) == 0)
                DrawGroundIdleDust();
                UpdateMode_General();
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
            rope.Gravity = Vector2.UnitY;

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
            int positionCount = ribbonPositions.Length;
            for (int i = 0; i < positionCount - 1; i++)
            {
                // 这个顶点的旋转，从这个位置指向下一个位置
                Vector2 Position = ribbonPositions[i];
                Vector2 NextPosition = ribbonPositions[i + 1];
                float rot = (NextPosition - Position).ToRotation();
                float height = 2.5f;
                if (i < 8)
                    rot -= MathHelper.PiOver4;
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
}
