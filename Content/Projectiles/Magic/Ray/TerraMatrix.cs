using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Map;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Content.DrawNodes;
using UCA.Content.Particiles;
using UCA.Core.BaseClass;
using UCA.Core.Graphics;
using UCA.Core.Utilities;

namespace UCA.Content.Projectiles.Magic.Ray
{
    public class TerraMatrix : BaseMagicProj
    {
        public override string Texture => UCATextureRegister.InvisibleTexturePath;
        public int StaffBeginAni = 75;
        public int MaxTime = 180;
        public float Height;
        public bool BeginFade; 
        public SpriteEffects filp = SpriteEffects.None;
        public Player Owner => Main.player[Projectile.owner];
        public override void SetDefaults()
        {
            Projectile.width = 128;
            Projectile.height = 128;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = MaxTime;
            Projectile.Opacity = 0;
        }
        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }
        public override void OnSpawn(IEntitySource source)
        {
            if (Main.rand.NextBool())
            {
                filp = SpriteEffects.FlipHorizontally;
            }
            BeginFade = false;
        }
        public override void AI()
        {
            Projectile.Center = Owner.Center + new Vector2(0, 22 + Height);
            ref float AniProgress = ref Projectile.ai[0];
            #region 处理缩放透明度高度
            if (AniProgress < 75f && !BeginFade)
            {
                AniProgress++;
                float progress = EasingHelper.EaseOutCubic(AniProgress / 75f);
                Projectile.scale = MathHelper.Lerp(0.15f, 0.18f, progress);
                Projectile.Opacity = MathHelper.Lerp(0f, 1f, progress);
                if (AniProgress >= 75f)
                {
                    AniProgress = 0;
                    BeginFade = true;
                }
            }
            else
            {
                AniProgress++; 
                float progress = EasingHelper.EaseOutCubic(AniProgress / 75f);
                Projectile.scale = MathHelper.Lerp(0.18f, 0.21f, progress);
                Projectile.Opacity = MathHelper.Lerp(1f, 0f, progress);
                Height = MathHelper.Lerp(0f, -3f, progress);
            }
            #endregion
            #region 处理粒子发射
            if (AniProgress < StaffBeginAni / 2f && !BeginFade)
            {
                if (Projectile.ai[1] > 0)
                    Projectile.ai[1]--;
                if (Projectile.ai[1] == 0)
                {
                    Projectile.ai[1] = 3;
                    SpawnParticlar();
                }
                if (Projectile.timeLeft % 2 == 0)
                {
                    float beginrot = Main.rand.NextFloat(0, MathHelper.TwoPi);
                    float rotSpeed = Main.rand.NextBool() ? 0.07f : -0.07f;
                    int length = Main.rand.Next(250, 500);
                    int LifeTime = Main.rand.Next(120, 150);
                    new AbsorbGlowBall(Owner.Center, Color.LawnGreen, LifeTime, 0.1f, beginrot, rotSpeed, Projectile.owner, length).Spawn();

                    Vector2 pos = Projectile.position + new Vector2(Main.rand.Next(0, Projectile.width), Projectile.height / 2);
                    Color RandomColor = Color.Lerp(Color.LightGreen, Color.ForestGreen, Main.rand.NextFloat(0, 1));
                    new Butterfly(pos, Vector2.Zero, RandomColor, 120, 0, 1, 0.2f, Main.rand.NextFloat(1f, 3f)).Spawn();
                }
            }
            if (AniProgress < StaffBeginAni / 1.15f && !BeginFade)
            {
                if (Projectile.timeLeft % 3 == 0)
                {
                    Color color = Color.Lerp(Color.DarkGreen, Color.LightGreen, Main.rand.NextFloat(0, 1f));
                    Vector2 pos = Projectile.position + new Vector2(Main.rand.Next(0, Projectile.width), Projectile.height / 2);
                    new TerraTree(pos, -Vector2.UnitY.RotatedBy(MathHelper.PiOver4 * 0.5f * (Main.rand.NextBool() ? -1 : 1)) * Main.rand.NextFloat(0.6f, 1.4f), color, 0, DrawLayer.AfterDust, Main.rand.NextFloat(12, 15), (Main.rand.NextBool() ? -1 : 1), Main.rand.NextFloat(9, 18f)).Spawn();

                    Vector2 pos2 = Projectile.position + new Vector2(Main.rand.Next(0, Projectile.width), Projectile.height / 2);
                    new TerraTree(pos2, -Vector2.UnitY.RotatedBy(MathHelper.PiOver4 * 0.5f * (Main.rand.NextBool() ? -1 : 1)) * Main.rand.NextFloat(0.6f, 1.4f), Color.SaddleBrown, 0, DrawLayer.AfterDust, Main.rand.NextFloat(12, 15), (Main.rand.NextBool() ? -1 : 1), Main.rand.NextFloat(9, 18f)).Spawn();
                }
            }
            #endregion
        }
        #region 处理粒子发射
        public void SpawnParticlar()
        {
            int SpawnWidth = 0;
            int SpawnHeight = Main.rand.Next(80, 120);
            int Xscale = Main.rand.Next(10, 14);
            int Life = Main.rand.Next(50, 55);
            int Filp = Main.rand.NextBool() ? -1 : 1;
            int Height = Main.rand.Next(75, 150);
            new TerraGlowBallEmitting(Owner.Center + new Vector2(SpawnWidth, -SpawnHeight), Vector2.UnitY * 2, Life, Xscale, Filp, Height, Projectile.owner).Spawn();
        }
        #endregion
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = UCATextureRegister.TerraMatrix.Value;
            Vector2 origPoint = texture.Size() / 2;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.LawnGreen * Projectile.Opacity, 0, origPoint, new Vector2(1, 0.15f) * Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
