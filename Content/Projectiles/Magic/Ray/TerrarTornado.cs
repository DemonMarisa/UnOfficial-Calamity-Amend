using LAP.Assets.TextureRegister;
using LAP.Core.AnimationHandle;
using LAP.Core.Enums;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Content.Particiles;
using UCA.Content.Projectiles.HealPRoj;
using UCA.Content.VFXs;
using UCA.Core.BaseClass;
using UCA.Core.Utilities;

namespace UCA.Content.Projectiles.Magic.Ray
{
    public class TerrarTornado : BaseMagicProj
    {
        public override string Texture => LAPTextureRegister.InvisibleTexturePath;
        public int FrameX;
        public int FrameY;
        public int MaxTime = 128;
        public AniHelper AniHelper = new AniHelper(3);
        public SpriteEffects filp = SpriteEffects.None;
        public NPC Target;
        public bool CanShootLance => Projectile.ai[1] != 0;
        public bool CanShootHealLance => Projectile.ai[2] != 0;
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
        }
        public override void AI()
        {
            if (Projectile.LAP().FirstFrame)
            {
                AniHelper = new AniHelper(3);
                AniHelper.MaxAniProgress[AniState.Begin] = 16;
                AniHelper.MaxAniProgress[AniState.End] = 32;
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
                    Vector2 pos = Projectile.position + new Vector2(Main.rand.Next(-32, Projectile.width + 32), 0);
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
                    Vector2 pos = Projectile.position + new Vector2(Main.rand.NextFloat(0, Projectile.width * 0.8f), Projectile.height);
                    TerraVine.Spawn(pos, -Vector2.UnitY.RotatedBy(MathHelper.PiOver4 * filps) * Main.rand.NextFloat(1, 2), color, -1, 2f, Main.rand.NextFloat(12, 15), Main.rand.NextFloat(2f, 3.5f));

                    Vector2 pos2 = Projectile.position + new Vector2(Main.rand.NextFloat(0, Projectile.width * 0.8f), Projectile.height);
                    TerraVine.Spawn(pos2, -Vector2.UnitY.RotatedBy(MathHelper.PiOver4 * filps) * Main.rand.NextFloat(1, 2), Color.SaddleBrown, -1, 2f, Main.rand.NextFloat(12, 15), Main.rand.NextFloat(2f, 3.5f));
                }

                for (int i = 0; i < 2; i++)
                {
                    Vector2 pos = Projectile.position + new Vector2(Main.rand.Next(0, Projectile.width), Projectile.height);
                    Color RandomColor = Color.Lerp(Color.LightGreen, Color.ForestGreen, Main.rand.NextFloat(0, 1));
                    new Butterfly(pos, Vector2.Zero, RandomColor, 120, 0, 1, 0.2f, Main.rand.NextFloat(2f, 4f)).Spawn();
                }
            }
            Target = LAPUtilities.FindClosestTarget(Projectile.Center, 1500, true);
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
            if (player.UCA().TerraRayHealCD > 0)
                return;
            if (CanShootHealLance)
            {
                player.UCA().TerraRayHealCD = 5;
                Projectile.Owner().SpawnLifeStealProj(target, Projectile.GetSource_FromThis(), ProjectileType<TerraHeal>(), target.Center, player.GetPlayerToMouseVector2().RotatedByRandom(MathHelper.TwoPi) * -6f);
            }
            if (!Projectile.LAP().OnceHitEffect)
                return;
            // 生成枝条
            //Vector2 firPos = target.Center;
            //for (int i = 0; i < 3; i++)
            //{
            //    float rot = MathHelper.TwoPi / 3;
            //    float XScale = Main.rand.NextFloat(9, 12);
            //    float Height = Main.rand.NextFloat(4f, 9f);

            //    Vector2 firVec = Vector2.UnitX.RotatedBy(rot * i).RotatedByRandom(MathHelper.TwoPi);
            //    Color color = Main.rand.NextBool() ? Color.ForestGreen : Color.SaddleBrown;
            //    new TerraTree(firPos, firVec * Main.rand.NextFloat(0.8f, 1.4f), color, 0, XScale, Main.rand.NextBool() ? 1 : -1, Height).Spawn();
            //}
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
            if (!AniHelper.HasFinish[AniState.Begin])
            {
                float maxAni = AniHelper.MaxAniProgress[AniState.Begin];
                float curAni = AniHelper.AniProgress[AniState.Begin];

                Projectile.Opacity = MathHelper.Lerp(0f, 1f, curAni / maxAni);
                AniHelper.AniProgress[AniState.Begin]++;

                if (AniHelper.AniProgress[AniState.Begin] >= AniHelper.MaxAniProgress[AniState.Begin])
                    AniHelper.HasFinish[AniState.Begin] = true;
            }
            else if (Projectile.timeLeft < AniHelper.MaxAniProgress[AniState.End])
            {
                float maxAni = AniHelper.MaxAniProgress[AniState.End];
                float curAni = AniHelper.AniProgress[AniState.End];

                Projectile.Opacity = MathHelper.Lerp(1f, 0f, curAni / maxAni);
                AniHelper.AniProgress[AniState.End]++;

                if (AniHelper.AniProgress[AniState.End] >= AniHelper.MaxAniProgress[AniState.End])
                    AniHelper.HasFinish[AniState.End] = true;
            }
        }
        #endregion
        #region 更新射弹发射
        public void ShootLance()
        {
            if (!LAPUtilities.IsLocalPlayer(Projectile.owner))
                return;
            if (!CanShootLance)
                return;
            Projectile.ai[0]--;
            if (Projectile.ai[0] == 0 && Projectile.timeLeft > AniHelper.MaxAniProgress[AniState.End])
            {
                Vector2 shootVel = LAPUtilities.GetVector2(Projectile.Center, Target.Center).SafeNormalize(Vector2.UnitX).RotatedByRandom(MathHelper.PiOver4);
                Projectile.ai[0] = 64;
                Projectile p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center + Main.rand.NextVector2Circular(64, 32), shootVel * 6, ProjectileType<TerraLance>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                if (Projectile.LAP().isWeaponSkillProj)
                    p.LAP().isWeaponSkillProj = true;
            }
        }
        #endregion
    }
}
