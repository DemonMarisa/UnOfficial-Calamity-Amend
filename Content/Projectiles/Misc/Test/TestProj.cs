using CalamityMod;
using CalamityMod.Graphics.Primitives;
using CalamityMod.Physics;
using LAP.Core.Graphics.Primitives.Trail;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Assets.Effects;
using UCA.Content.Items;
using UCA.Core.Utilities;

namespace UCA.Content.Projectiles.Misc.Test
{
    public class TestProj : ModProjectile, ILocalizedModType, IPixelatedPrimitiveRenderer
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<Sword>();

        public override string Texture => UCATextureRegister.InvisibleTexturePath;
        public Player Owner => Main.player[Projectile.owner];
        /// <summary>
        /// 绳子的起点
        /// </summary>
        public Vector2 RopStartPoint => Projectile.Center + Projectile.velocity * Projectile.scale * Projectile.width * 0.34f;
        /// <summary>
        /// 绳子实例
        /// </summary>
        public RopeHandle? Rope;
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.timeLeft = 120;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 0;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10 * (Projectile.extraUpdates + 1);
        }
        public override void OnSpawn(IEntitySource source)
        {
            InitializeRope();
        }
        public void InitializeRope()
        {
            // 多少个体节
            int ribbonSegmentCount = 12;
            // 长度
            float Length = 70;
            // 体节之间的距离
            float distancePerSegment = Length / ribbonSegmentCount;
            RopeSettings ribbonSettings = new RopeSettings()
            {
                StartIsFixed = true,
                Mass = 0.72f,
                RespondToEntityMovement = true,
                RespondToWind = true
            };
            Vector2 gravity = Vector2.UnitY;
            Rope = ModContent.GetInstance<RopeManagerSystem>().RequestNew(RopStartPoint, Projectile.Center, ribbonSegmentCount, distancePerSegment, gravity, ribbonSettings, 25);  }
        public override void AI()
        {
            Projectile.timeLeft = 2;
            Projectile.Center = Main.MouseWorld;
            UpdateRibbon();
        }
        /// <summary>
        ///     Updates a given ribbon.
        /// </summary>
        public void UpdateRibbon()
        {
            // Ensure that the handle is properly initialized before proceeding any further.
            if (Rope is not RopeHandle rope)
                return;
            rope.Start = RopStartPoint;
        }
        public override void OnKill(int timeLeft)
        {
            Rope?.Dispose();
        }
        public void RenderPixelatedPrimitives(SpriteBatch spriteBatch, PixelationPrimitiveLayer layer)
        {
            if (Rope is not RopeHandle rope)
                return;
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null);

            Texture2D texture = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/HammerRope").Value;
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
                trailDrawDate.Add(new(Position, Color.White, new Vector2(0, 3), rot));
            }
            TrailRender.RenderTrail(trailDrawDate.ToArray(), drawSetting);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            
            return false;
        }
    }
}
