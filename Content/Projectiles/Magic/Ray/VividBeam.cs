using LAP.Assets.Effects;
using LAP.Assets.TextureRegister;
using LAP.Content.Configs;
using LAP.Core.Presets.Content;
using LAP.Core.SystemsLoader;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Assets.Sounds;
using UCA.Content.Particiles;
using UCA.Core.BaseClass;

namespace UCA.Content.Projectiles.Magic.Ray
{
    public class VividBeam : BaseMagicProj
    {
        public override string Texture => LAPTextureRegister.InvisibleTexturePath;
        public Vector2 BeginPos;
        public float LaserLength => Vector2.Distance(BeginPos, Projectile.Center);
        public int MaxTime = 360;
        public Color StartColor;
        public bool BeginFadeOut;
        public float Opacity;
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
            Projectile.extraUpdates = 55;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Opacity = 1f;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(BeginFadeOut);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            BeginFadeOut = reader.ReadBoolean();
        }
        public override bool? CanHitNPC(NPC target)
        {
            if (BeginFadeOut) return false;
            else return null;
        }
        public override void AI()
        {
            if (Projectile.LAP().FirstFrame)
            {
                Init();
            }
            if (BeginFadeOut)
            {
                Projectile.extraUpdates = 0;
                Projectile.velocity *= 0.8f;
                Opacity = MathHelper.Lerp(Opacity, 0f, 0.04f);
                Projectile.scale = MathHelper.Lerp(Projectile.scale, 0f, 0.04f);
                if (Opacity < 0.01f)
                    Projectile.Kill();
            }
            else if (Projectile.timeLeft < 120)
            {
                Projectile.extraUpdates = 0;
                Projectile.numUpdates = -1;
                BeginFadeOut = true;
            }
            else
            {
                if (LAPConfig.Instance.PerformanceMode)
                {
                    Color color = LAPUtilities.LerpColor(Color.WhiteSmoke, Color.LightGreen);
                    for (int i = 0; i < 3; i++)
                    {
                        Vector2 finalPos = (Projectile.velocity / 3) * i;
                        ParticlePreset.NewTGlowBall(Projectile.Center - finalPos, Vector2.Zero, color, 60, 0.1f);
                    }
                    int time = MaxTime - Projectile.timeLeft;
                    for (int d = 0; d < 2; d++)
                    {
                        Vector2 offset = -Vector2.UnitY.RotatedBy((double)(time * MathHelper.Pi / 24f + d * MathHelper.Pi), default) * new Vector2(5f, 10f) - Projectile.rotation.ToRotationVector2() * 10f;
                        ParticlePreset.NewTGlowBall(Projectile.Center + offset, Vector2.Zero, color, 60, 0.1f, 0.2f);
                    }
                }
                else
                {
                    Color color = LAPUtilities.LerpColor(Color.WhiteSmoke, Color.LightGreen);
                    new TrailGlowBall(Projectile.Center + Main.rand.NextVector2Circular(9, 9), Projectile.velocity * 0.25f, color * 0.7f, Main.rand.Next(45, 65), 0.08f, true).Spawn();
                    int time = MaxTime - Projectile.timeLeft;
                    for (int d = 0; d < 2; d++)
                    {
                        Vector2 offset = -Vector2.UnitY.RotatedBy((double)(time * MathHelper.Pi / 24f + d * MathHelper.Pi), default) * new Vector2(5f, 10f) - Projectile.rotation.ToRotationVector2() * 10f;
                        ParticlePreset.NewTGlowBall(Projectile.Center + offset, Vector2.Zero, color, 60, 0.1f, 0.2f);
                    }
                    if (Projectile.timeLeft % 20 == 0 && Projectile.timeLeft < MaxTime - 10)
                    {
                        float rotArg2 = MathHelper.TwoPi / 20f;
                        for (int i = 0; i < 20; i++)
                        {
                            float rorate = i * rotArg2;
                            Vector2 dustVelocity = new Vector2(2f, 0).BetterRotatedBy(rorate, default, 0.35f);
                            dustVelocity = dustVelocity.RotatedBy(Projectile.rotation);
                            ParticlePreset.NewTGlowBall(Projectile.Center + Projectile.velocity * 3, dustVelocity, color, 60, 0.2f, 0f);
                        }
                    }
                }
            }
        }
        public void Init()
        {
            BeginPos = Projectile.Center;
            Projectile.rotation = Projectile.velocity.ToRotation();
            float rotArg = MathHelper.TwoPi / 40f;
            float rotArg2 = MathHelper.TwoPi / 20f;
            Color color = LAPUtilities.LerpColor(Color.WhiteSmoke, Color.LightGreen);
            SoundEngine.PlaySound(SoundsMenu.VividClarityShoot);
            if (LAPConfig.Instance.PerformanceMode)
            {
                for (int i = 0; i < 40; i++)
                {
                    float rorate = i * rotArg;
                    Vector2 dustVelocity = new Vector2(3, 0).BetterRotatedBy(rorate, default, 0.35f);
                    dustVelocity = dustVelocity.RotatedBy(Projectile.rotation);
                    ParticlePreset.NewTGlowBall(Projectile.Center, dustVelocity, color, 60, 0.2f, 0f);
                }
                for (int j = 0; j < 30; j++)
                {
                    Vector2 vel = Projectile.velocity.RotatedByRandom(MathHelper.PiOver4 * 0.2f) * Main.rand.NextFloat(0.2f, 1f) * 3f;
                    ParticlePreset.NewGlowLozenge(Projectile.Center + Main.rand.NextVector2Circular(9, 9), vel, color, 45, 0.2f);
                }
                new CrossGlow(Projectile.Center, Vector2.Zero, color, 60, 1f, 0.23f).Spawn();
                new CrossGlow(Projectile.Center, Vector2.Zero, color, 60, 1f, 0.23f).Spawn();
                return;
            }
            LAPContent.AddScreenDistortion(30, Projectile.Center, 4f, 0.015f);
            for (int i = 0; i < 20; i++)
            {
                float rorate = i * rotArg2;
                Vector2 dustVelocity = new Vector2(3.5f, 0).BetterRotatedBy(rorate, default, 0.35f);
                dustVelocity = dustVelocity.RotatedBy(Projectile.rotation);
                ParticlePreset.NewTGlowBall(Projectile.Center - Projectile.velocity * 3, dustVelocity, color, 60, 0.2f, 0f);
            }
            for (int i = 0; i < 40; i++)
            {
                float rorate = i * rotArg;
                Vector2 dustVelocity = new Vector2(5, 0).BetterRotatedBy(rorate, default, 0.35f);
                dustVelocity = dustVelocity.RotatedBy(Projectile.rotation);
                ParticlePreset.NewTGlowBall(Projectile.Center, dustVelocity, color, 60, 0.2f, 0f);
            }
            for (int i = 0; i < 20; i++)
            {
                float rorate = i * rotArg2;
                Vector2 dustVelocity = new Vector2(3.5f, 0).BetterRotatedBy(rorate, default, 0.35f);
                dustVelocity = dustVelocity.RotatedBy(Projectile.rotation);
                ParticlePreset.NewTGlowBall(Projectile.Center + Projectile.velocity * 3, dustVelocity, color, 60, 0.2f, 0f);
            }
            for (int i = 0; i < 20; i++)
            {
                float rorate = i * rotArg2;
                Vector2 dustVelocity = new Vector2(2.5f, 0).BetterRotatedBy(rorate, default, 0.35f);
                dustVelocity = dustVelocity.RotatedBy(Projectile.rotation);
                ParticlePreset.NewTGlowBall(Projectile.Center + Projectile.velocity * 6, dustVelocity, color, 60, 0.2f, 0f);
            }
            for (int i = 0; i < 20; i++)
            {
                float rorate = i * rotArg2;
                Vector2 dustVelocity = new Vector2(2f, 0).BetterRotatedBy(rorate, default, 0.35f);
                dustVelocity = dustVelocity.RotatedBy(Projectile.rotation);
                ParticlePreset.NewTGlowBall(Projectile.Center + Projectile.velocity * 9, dustVelocity, color, 60, 0.2f, 0f);
            }
            for (int j = 0; j < 20; j++)
            {
                ParticlePreset.NewTGlowBall(Projectile.Center, Vector2.Zero, color, 90, 0.1f, 2.5f);
            }
            for (int j = 0; j < 15; j++)
            {
                Vector2 vel = -Projectile.velocity.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.7f, 1f) * 1.6f;
                ParticlePreset.NewGlowLozenge(Projectile.Center + Main.rand.NextVector2Circular(9, 9), vel, color, 45, 0.2f);
            }
            for (int j = 0; j < 30; j++)
            {
                Vector2 vel = Projectile.velocity.RotatedByRandom(MathHelper.PiOver4 * 0.2f) * Main.rand.NextFloat(0.2f, 1f) * 3f;
                ParticlePreset.NewGlowLozenge(Projectile.Center + Main.rand.NextVector2Circular(9, 9), vel, color, 45, 0.2f);
            }
            new CrossGlow(Projectile.Center, Vector2.Zero, color, 60, 1f, 0.23f).Spawn();
            new CrossGlow(Projectile.Center, Vector2.Zero, color, 60, 1f, 0.23f).Spawn();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.timeLeft = 120;
            BeginFadeOut = true;
            Projectile.extraUpdates = 0;
            Projectile.numUpdates = -1;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (LAPConfig.Instance.PerformanceMode)
                return false;
            LAPUtilities.ReSetToBeginShader();
            Texture2D texture = UCATextureRegister.LaserHighContrast.Value;
            DrawLaser(texture, Color.Green, 0.13f, 0.15f, 0.75f);
            DrawLaser(texture, Color.SkyBlue, 0.13f, 0.2f, 0.25f);
            DrawLaser(texture, Color.Orange, 0.13f, 0.1f, 0.5f);
            Texture2D aura = UCATextureRegister.Lightning.Value;
            DrawAura(aura, Color.Green, 0.4f, 0.1f, 0.25f);
            DrawAura(aura, Color.SkyBlue, 0.4f, 0.1f, 0.15f);
            DrawAura(aura, Color.Orange, 0.4f, 0.1f, 0.05f);
            LAPUtilities.ReSetToEndShader();
            return false;
        }
        public void DrawLaser(Texture2D texture, Color color, float height = 0.2f, float op = 0.1f, float Speed = 1f)
        {
            float TextureWidth = texture.Width;
            float UVMult = LaserLength / TextureWidth;

            float FadeInProgress = 50f / LaserLength;

            Vector4 opacity = new Vector4(FadeInProgress, FadeInProgress, 0f, 0f);
            Vector2 uv = new Vector2(-Main.GlobalTimeWrappedHourly * Speed, 0);
            Vector2 uvmult = new Vector2(UVMult, 1);

            Effect effect2 = LAPShaderRegister.AlphaFade.Value;
            effect2.Parameters["uFadeoutLeftLength"].SetValue(opacity.X);
            effect2.Parameters["uFadeinRigtLength"].SetValue(opacity.Y);
            effect2.Parameters["uFadeinTopLength"].SetValue(opacity.Z);
            effect2.Parameters["uFadeinBottomLength"].SetValue(opacity.W);
            effect2.Parameters["UVOffset"].SetValue(uv);
            effect2.Parameters["UVMult"].SetValue(uvmult);
            effect2.CurrentTechnique.Passes[0].Apply();

            Vector2 orig = new(0, TextureWidth / 2);
            Main.spriteBatch.Draw(texture, BeginPos - Main.screenPosition, null, color * Opacity, Projectile.rotation, orig, new Vector2(UVMult, height * Projectile.scale), SpriteEffects.None, 0);
        }
        public void DrawAura(Texture2D texture, Color color, float height = 0.2f, float op = 0.1f, float Speed = 1f)
        {
            float TextureWidth = texture.Width;
            float UVMult = LaserLength / TextureWidth;

            float FadeInProgress = 50f / LaserLength;

            Vector4 opacity = new Vector4(FadeInProgress, FadeInProgress, 0.2f, 0.2f);
            Vector2 uv = new Vector2(-Main.GlobalTimeWrappedHourly * Speed, 0);
            Vector2 uvmult = new Vector2(UVMult / 5, 1);

            LAPUtilities.ApplyAlphaCut(opacity, uv, uvmult, color);

            Vector2 orig = new(0, TextureWidth / 2);
            Main.spriteBatch.Draw(texture, BeginPos - Main.screenPosition, null, color * Opacity, Projectile.rotation, orig, new Vector2(UVMult, height * Projectile.scale), SpriteEffects.None, 0);
        }
    }
}
