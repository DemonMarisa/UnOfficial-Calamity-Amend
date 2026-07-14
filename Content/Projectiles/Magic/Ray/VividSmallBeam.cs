using LAP.Assets.TextureRegister;
using LAP.Core.Presets.Content;
using LAP.Core.SystemsLoader;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Content.Particiles;
using UCA.Content.Projectiles.Misc;

namespace UCA.Content.Projectiles.Magic.Ray
{
    public class VividSmallBeam : ModProjectile
    {
        public override string Texture => LAPTextureRegister.InvisibleTexturePath;
        public Vector2 EndPos => new Vector2(Projectile.ai[0], Projectile.ai[1]);
        public int LengthCount;
        public float LaserLength;
        public int MaxTime = 35;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 4400;
            Projectile.AddProtectedProj();
        }
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = MaxTime;
            Projectile.extraUpdates = 0;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float _ = float.NaN;
            bool c = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * LaserLength, 64f, ref _);
            return c;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override void AI()
        {
            Init();
            FadeOut();
        }
        public void FadeOut()
        {
            Projectile.Opacity = MathHelper.Lerp(0f, 1f, EasingHelper.EaseOutCubic(Projectile.timeLeft / (float)MaxTime));
        }
        public void Init()
        {
            if (Projectile.LAP().FirstFrame)
            {
                LaserLength = Vector2.Distance(Projectile.Center, EndPos);
                LengthCount = (int)(LaserLength / Projectile.velocity.Length());

                Projectile.rotation = Projectile.velocity.ToRotation();

                Color color = LAPUtilities.LerpColor(Color.WhiteSmoke, Color.LightGreen, 0.5f);
                new CrossGlow(Projectile.Center, Vector2.Zero, color, 60, 1f, 0.2f).Spawn();
                new CrossGlow(Projectile.Center, Vector2.Zero, color, 60, 1f, 0.2f).Spawn();
                new NoiseShockRing(Projectile.Center, Vector2.Zero, color, 60, 1f, 0.4f, Projectile.whoAmI, Vector2.Zero).Spawn();
                new NoiseShockRing(Projectile.Center, Vector2.Zero, color, 60, 1f, 0.4f, Projectile.whoAmI, Vector2.Zero).Spawn();

                for (int i = 0; i < 10; i++)
                {
                    Color RandomColor = LAPUtilities.LerpColor(Color.WhiteSmoke, Color.LightGreen);
                    ParticlePreset.NewTGlowBall(Projectile.Center, Vector2.Zero, RandomColor, 75, 0.4f, Main.rand.NextFloat(2f, 4f));
                }

                for (int i = 0; i < LengthCount; i++)
                {
                    color = LAPUtilities.LerpColor(Color.WhiteSmoke, Color.LightGreen);
                    int time = MaxTime - i;
                    for (int d = 0; d < 2; d++)
                    {
                        Vector2 offset = -Vector2.UnitY.RotatedBy((time * MathHelper.Pi / 24f + d * MathHelper.Pi), default) * new Vector2(5f, 8f) - Projectile.rotation.ToRotationVector2() * 10f;
                        ParticlePreset.NewTGlowBall(Projectile.Center + Projectile.velocity * i + offset, Vector2.Zero, color, 60, 0.2f, 0.2f);
                    }
                }

                for (int i = 0; i < LengthCount; i += 3)
                {
                    if (Main.rand.NextBool())
                        continue;
                    color = LAPUtilities.LerpColor(Color.WhiteSmoke, Color.LightGreen);
                    int time = MaxTime - i;
                    for (int d = 0; d < 2; d++)
                    {
                        Vector2 offset = -Vector2.UnitY.RotatedBy((time * MathHelper.Pi / 24f + d * MathHelper.Pi), default) * new Vector2(5f, 5f) - Projectile.rotation.ToRotationVector2() * 10f;
                        ParticlePreset.NewDustGlow(Projectile.Center + Projectile.velocity * i + offset, Main.rand.NextVector2CircularEdge(3, 3), 0, color, 45, 0.1f, 0);
                    }
                }

                for (int i = 0; i < 5; i++)
                {
                    Color RandomColor = LAPUtilities.LerpColor(Color.WhiteSmoke, Color.LightGreen);
                    ParticlePreset.NewTGlowBall(EndPos, Vector2.Zero, RandomColor, 75, 0.4f, Main.rand.NextFloat(2f, 4f));
                }
                new NoiseShockRing(EndPos, Vector2.Zero, color, 60, 1f, 0.4f * Main.rand.NextFloat(0.8f, 1.8f), -1, Vector2.Zero).Spawn();
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
        }
        public override bool PreDraw(ref Color lightColor)
        {
            LAPUtilities.ReSetToBeginShader();

            Texture2D texture = LAPTextureRegister.StandardFlow3.Value;
            Vector4 uvFade = new Vector4(0.1f, 0.1f, 0, 0);
            Vector2 uvAdd = new Vector2(Main.GlobalTimeWrappedHourly * -0.5f, 0);
            Vector2 uvMult = new Vector2(2f, 1f);
            LAPUtilities.ApplyAlphaCut(uvFade, uvAdd, uvMult);
            DrawLaser(Color.Green, 0.5f);
            LAPUtilities.ApplyAlphaCut(uvFade, uvAdd * 0.75f, uvMult);
            DrawLaser(Color.SkyBlue, 0.5f);
            LAPUtilities.ApplyAlphaCut(uvFade, uvAdd * 0.5f, uvMult);
            DrawLaser(Color.Orange, 0.5f);

            void DrawLaser(Color color, float height)
            {
                Vector2 orig = new(0, texture.Height / 2);
                float xScale = LaserLength / texture.Width;
                Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, color * Projectile.Opacity, Projectile.rotation, orig, new Vector2(xScale, 0.25f * height * Projectile.scale), SpriteEffects.None, 0);
            }

            LAPUtilities.ReSetToEndShader();
            return false;
        }
    }
}
