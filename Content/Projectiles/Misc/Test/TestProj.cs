using CalamityMod;
using CalamityMod.Physics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Content.Items;

namespace UCA.Content.Projectiles.Misc.Test
{
    public class TestProj : ModProjectile, ILocalizedModType
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

        public override bool PreDraw(ref Color lightColor)
        {
            
            return false;
        }
    }
}
