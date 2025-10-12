using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Content.DrawNodes;
using UCA.Content.Particiles;
using UCA.Content.Projectiles.HealPRoj;
using UCA.Core.AnimationHandle;
using UCA.Core.BaseClass;
using UCA.Core.Enums;
using UCA.Core.Graphics;
using UCA.Core.Utilities;

namespace UCA.Content.Projectiles.Magic.Ray
{
    public class TerrarTornado : BaseMagicProj
    {
        public override string Texture => UCATextureRegister.InvisibleTexturePath;
        public int FrameX;
        public int FrameY;
        public int MaxTime = 128;
        public AnimationHelper AnimationHelper;
        public SpriteEffects filp = SpriteEffects.None;
        public NPC Target;
        public bool CanShootLance => Projectile.ai[1] == 0;
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
            Projectile.extraUpdates = 1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 5;
            Projectile.Opacity = 0;
        }
        public override void OnSpawn(IEntitySource source)
        {
            AnimationHelper = new AnimationHelper(3);
            AnimationHelper.MaxAniProgress[AnimationState.Begin] = 16;
            AnimationHelper.MaxAniProgress[AnimationState.End] = 32;
            FrameX = Main.rand.Next(0, 7);
            FrameY = Main.rand.Next(0, 7);
            int filps = -1;
            if (Main.rand.NextBool())
            {
                filps = 1;
                filp = SpriteEffects.FlipHorizontally;
            }

            for (int i = 0; i < 1; i++)
            {
                Color RandomColor2 = Color.Lerp(Color.Pink, Color.Green, Main.rand.NextFloat(0, 1));
                Vector2 pos = Projectile.position + new Vector2(Main.rand.Next(-32, Projectile.width + 32),0);
                new Petal(pos, -Vector2.UnitY, RandomColor2, 360, 0, 1, 0.1f, Main.rand.NextFloat(0.5f, 0.7f)).Spawn();
            }

            for (int i = 0; i < 5; i++)
            {
                Color color = Color.Lerp(Color.LightGreen, Color.LawnGreen, Main.rand.NextFloat(0, 1f));
                new TurbulenceGlowBall(Projectile.position + new Vector2(Main.rand.Next(0, Projectile.width), Projectile.height * 0.75f),
                    Main.rand.NextFloat(4f, 6f), color, Main.rand.Next(90, 120), 0.2f, MathHelper.PiOver2).Spawn();
            }
            
            for (int i = 0; i < 1; i++)
            {
                Color color = Color.Lerp(Color.DarkGreen, Color.LightGreen, Main.rand.NextFloat(0, 1f));
                Vector2 pos = Projectile.position + new Vector2(Main.rand.Next(0, Projectile.width), Projectile.height);
                new TerraTree(pos, -Vector2.UnitY.RotatedBy(MathHelper.PiOver4 * filps) * Main.rand.NextFloat(1, 2), color, 0, DrawLayer.AfterDust, Main.rand.NextFloat(12, 15), -1, Main.rand.NextFloat(9, 18f)).Spawn();
                Vector2 pos2 = Projectile.position + new Vector2(Main.rand.Next(0, Projectile.width), Projectile.height);
                new TerraTree(pos2, -Vector2.UnitY.RotatedBy(-MathHelper.PiOver4 * filps) * Main.rand.NextFloat(1, 2), Color.SaddleBrown, 0, DrawLayer.AfterDust, Main.rand.NextFloat(12, 17), 1, Main.rand.NextFloat(11, 22)).Spawn();
            }
            
            for (int i = 0; i < 2; i++)
            {
                Vector2 pos = Projectile.position + new Vector2(Main.rand.Next(0, Projectile.width), Projectile.height);
                Color RandomColor = Color.Lerp(Color.LightGreen, Color.ForestGreen, Main.rand.NextFloat(0, 1));
                new Butterfly(pos, Vector2.Zero, RandomColor, 120, 0, 1, 0.2f, Main.rand.NextFloat(2f, 4f)).Spawn();
            }
        }
        public override void AI()
        {
            Target = Projectile.FindClosestTarget(1500);
            if (Target is not null)
            {
                ShootLance();
            }

            UpDateFrame();
            UpDateFade();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            if (CanShootLance)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, player.GetPlayerToMouseVector2().RotatedByRandom(MathHelper.TwoPi) * -6f, ModContent.ProjectileType<TerraHeal>(), 0, 0, Projectile.owner);
            // 生成枝条
            Vector2 firPos = target.Center;
            for (int i = 0; i < 3; i++)
            {
                float rot = MathHelper.TwoPi / 3;
                float XScale = Main.rand.NextFloat(9, 12);
                float Height = Main.rand.NextFloat(4f, 9f);

                Vector2 firVec = Vector2.UnitX.RotatedBy(rot * i).RotatedByRandom(MathHelper.TwoPi);
                Color color = Main.rand.NextBool() ? Color.ForestGreen : Color.SaddleBrown;
                new TerraTree(firPos, firVec * Main.rand.NextFloat(0.8f, 1.4f), color, 0, DrawLayer.BeforeDust, XScale, Main.rand.NextBool() ? 1 : -1, Height).Spawn();
            }
        }
        public override void OnKill(int timeLeft)
        {
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = UCATextureRegister.Tornado.Value;
            Rectangle frame = UCATextureRegister.Tornado.Frame(8, 8, FrameX, FrameY);
            Vector2 origin = frame.Size() * 0.5f;
            Color DrawColor = Color.LawnGreen;
            DrawColor = new Color(DrawColor.R, DrawColor.G, DrawColor.B, 0);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, frame, DrawColor * Projectile.Opacity, Projectile.rotation, origin, new Vector2(1.5f, 1f), filp, 0);
            return false;
        }
        #region 更新帧数
        public void UpDateFrame()
        {
            if (FrameX < 7)
                FrameX++;
            else
            {
                FrameX = 0;
                if (FrameY < 7)
                    FrameY++;
                else
                    FrameY = 0;
            }
        }
        #endregion
        #region 更新淡入淡出
        public void UpDateFade()
        {
            if (!AnimationHelper.HasFinish[AnimationState.Begin])
            {
                float maxAni = AnimationHelper.MaxAniProgress[AnimationState.Begin];
                float curAni = AnimationHelper.AniProgress[AnimationState.Begin];

                Projectile.Opacity = MathHelper.Lerp(0f, 1f, curAni / maxAni);
                AnimationHelper.AniProgress[AnimationState.Begin]++;

                if (AnimationHelper.AniProgress[AnimationState.Begin] >= AnimationHelper.MaxAniProgress[AnimationState.Begin])
                    AnimationHelper.HasFinish[AnimationState.Begin] = true;
            }
            else if (Projectile.timeLeft < AnimationHelper.MaxAniProgress[AnimationState.End])
            {
                float maxAni = AnimationHelper.MaxAniProgress[AnimationState.End];
                float curAni = AnimationHelper.AniProgress[AnimationState.End];

                Projectile.Opacity = MathHelper.Lerp(1f, 0f, curAni / maxAni);
                AnimationHelper.AniProgress[AnimationState.End]++;

                if (AnimationHelper.AniProgress[AnimationState.End] >= AnimationHelper.MaxAniProgress[AnimationState.End])
                    AnimationHelper.HasFinish[AnimationState.End] = true;
            }
        }
        #endregion
        #region 更新射弹发射
        public void ShootLance()
        {
            if (!CanShootLance)
                return;
            Projectile.ai[0]--;
            if (Projectile.ai[0] == 0 && Projectile.timeLeft > AnimationHelper.MaxAniProgress[AnimationState.End])
            {
                Vector2 shootVel = UCAUtilities.GetVector2(Projectile.Center, Target.Center).SafeNormalize(Vector2.UnitX).RotatedByRandom(MathHelper.PiOver4);
                Projectile.ai[0] = 64;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + Main.rand.NextVector2Circular(64, 32), shootVel * 6, ModContent.ProjectileType<TerraLance>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
        }
        #endregion
    }
}
