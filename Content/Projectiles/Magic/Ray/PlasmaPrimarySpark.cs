using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Assets.Sounds;
using UCA.Content.Particiles;
using UCA.Core.BaseClass;
using LAP.Core.Graphics.Primitives.Trail;
using LAP.Core.Utilities;
using LAP.Core.Graphics.PixelatedRender;
using LAP.Core.Enums;

namespace UCA.Content.Projectiles.Magic.Ray
{
    public class PlasmaPrimarySpark : BaseMagicProj, IPixelatedRenderer
    {
        public DrawLayer LayerToRenderTo => DrawLayer.BeforePlayer;
        public Player Owner => Main.player[Projectile.owner];
        public override string Texture => UCATextureRegister.InvisibleTexturePath;
        public NPC Target = null;
        public float DrawRot = 0;
        public bool FadeOut = false;
        public int FadeOutCount = 60;
        public float VelocityLength = 0;
        public override void SetStaticDefaults()
        {
            // 保存旧朝向与旧位置
            ProjectileID.Sets.TrailingMode[Type] = 2;
            // 一共爆粗20个数据
            ProjectileID.Sets.TrailCacheLength[Type] = 15;
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 240;
            Projectile.extraUpdates = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void OnSpawn(IEntitySource source)
        {
        }

        public override void AI()
        {
            if (Projectile.LAP().FirstFrame)
            {
                Projectile.rotation = Projectile.velocity.ToRotation();
                VelocityLength = Projectile.velocity.Length();
            }
            if (Projectile.timeLeft > 160)
            {
                Projectile.velocity *= 1.02f;
                if (VelocityLength < 9)
                VelocityLength *= 1.02f;
            }
            for (int i = 0; i < 2; i++)
            {
                Color RandomColor = Color.Lerp(Color.Violet, Color.Purple, Main.rand.NextFloat(0, 1));
                new MediumGlowBall(Projectile.Center + Main.rand.NextVector2Circular(18, 18) + Projectile.velocity / 2 * i,
                    -Projectile.velocity, RandomColor, 120, 0, 1, 0.12f, Main.rand.NextFloat(0.5f, 1f)).Spawn();
            }
            TrackTarget();
        }
        public void TrackTarget()
        {
            Target = Projectile.FindClosestTarget(2000, false, false);
            if (Target is not null)
            {
                Vector2 home = Target.Center - Projectile.Center;
                VelocityLength = MathHelper.Lerp(VelocityLength, 9, 0.02f);
                Projectile.rotation = Projectile.rotation.AngleTowards(home.ToRotation(), MathHelper.ToRadians(1.5f));
                Projectile.velocity = Projectile.rotation.ToRotationVector2() * VelocityLength;
            }
        }

        public void RenderPixelated(SpriteBatch spriteBatch)
        {
            Vector2 HalfProj = new Vector2(Projectile.width / 2, Projectile.height / 2);
            List<TrailDrawDate> trailDrawDate = [];
            List<TrailDrawDate> SecondtrailDrawDate = [];
            List<TrailDrawDate> ThirdtrailDrawDate = [];
            DrawSetting drawSetting = new(UCATextureRegister.HoodTrail.Value, false, false);

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                if (Projectile.oldPos[i] != Vector2.Zero)
                {
                    Vector2 DrawPos = Projectile.oldPos[i] - Main.screenPosition + HalfProj - new Vector2(-18, 0).RotatedBy(Projectile.oldRot[i]);

                    TrailDrawDate TrailDrawDate = new(DrawPos, new Color(208, 0, 255, 0), new Vector2(0, 24), Projectile.oldRot[i]);
                    trailDrawDate.Add(TrailDrawDate);

                    TrailDrawDate TrailDrawDate2 = new(DrawPos, new Color(255, 255, 255, 0), new Vector2(0, 8), Projectile.oldRot[i]);
                    SecondtrailDrawDate.Add(TrailDrawDate2);

                    TrailDrawDate TrailDrawDate3 = new(DrawPos, new Color(186, 50, 205, 0), new Vector2(0, 40), Projectile.oldRot[i]);
                    ThirdtrailDrawDate.Add(TrailDrawDate3);
                }
            }

            TrailRender.RenderTrail(trailDrawDate.ToArray(), drawSetting);
            TrailRender.RenderTrail(SecondtrailDrawDate.ToArray(), drawSetting);
            TrailRender.RenderTrail(ThirdtrailDrawDate.ToArray(), drawSetting);

            Vector2 DrawTexPos = Projectile.Center - Main.screenPosition;
            Texture2D texture = UCATextureRegister.GlowBall.Value;
            spriteBatch.Draw(texture, DrawTexPos, null, new Color(0, 0, 0, 255), 0, texture.Size() / 2, Projectile.scale * 0.6f, SpriteEffects.None, 0);
            spriteBatch.Draw(UCATextureRegister.BallSoft.Value, DrawTexPos, null, new Color(148, 0, 255, 0), 0, UCATextureRegister.BallSoft.Size() / 2, Projectile.scale * 0.8f, SpriteEffects.None, 0);
            spriteBatch.Draw(UCATextureRegister.Spirit.Value, DrawTexPos, null, new Color(255, 255, 255, 0), Main.GlobalTimeWrappedHourly * 2, UCATextureRegister.Spirit.Size() / 2, Projectile.scale * 0.4f, SpriteEffects.None, 0);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            PixelatedRenderManger.BeginDrawProj = true;
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 45; i++)
            {
                float offset = MathHelper.TwoPi / 45;
                Color RandomColor = Color.Lerp(Color.DarkViolet, Color.LightPink, Main.rand.NextFloat(0, 1));
                new MediumGlowBall(Projectile.Center + Projectile.velocity.ToRotation().ToRotationVector2() * 12f, Projectile.velocity.RotatedBy(offset * i), RandomColor, 180, 0, 1, 0.2f, Main.rand.NextFloat(3f, 5f)).Spawn();
            }
            SoundEngine.PlaySound(SoundsMenu.PlasmaBlastBomb, Projectile.Center);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<PlasmaBlast>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.ShadowFlame, 300);
        }
    }
}
