using LAP.Assets.Sounds;
using LAP.Assets.TextureRegister;
using LAP.Core.Graphics.ScreenCaustics;
using LAP.Core.Graphics.VFX;
using LAP.Core.Presets.Content;
using LAP.Core.SpecificEffectManagers;
using LAP.Core.SystemsLoader;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Content.Particiles;
using UCA.Content.Projectiles.Misc;
using UCA.Content.VFXs;
using UCA.Core.BaseClass;

namespace UCA.Content.Projectiles.Magic.Ray
{
    public class VividPowerfulBeam : BaseMagicProj
    {
        public override string Texture => LAPTextureRegister.InvisibleTexturePath;
        public List<Vector2> FirePos = new List<Vector2>(8);
        public Vector2 EndPos;
        public float LaserLength;
        public int LengthCount = 180;
        public CausticsEntity caustics;
        public int MaxTime = 35;
        public bool BeginFadeOut;
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
            Projectile.localNPCHitCooldown = 10;
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
            SpawnDust();
            FadeOut();
        }
        public void Init()
        {
            if (Projectile.LAP().FirstFrame)
            {
                EndPos = Projectile.velocity * LengthCount;
                LaserLength = Projectile.velocity.Length() * LengthCount;
                Projectile.rotation = Projectile.velocity.ToRotation();
            }
            SpawnProj();
        }
        public void SpawnDust()
        {
            if (Projectile.LAP().FirstFrame)
            {
                ScreenShakeSystem.AddScreenShakes(Projectile.Center, 10, 15, 0);
                SoundEngine.PlaySound(LAPSoundsMenu.CarianGreatswordCharage with { Pitch = Main.rand.NextFloat(0.3f, 0.7f)}, Projectile.Center);
                #region 核心高光
                // 冲击波与高光
                Color color = LAPUtilities.LerpColor(Color.WhiteSmoke, Color.LightGreen, 0.5f);
                new CrossGlow(Projectile.Center, Vector2.Zero, color, 60, 1f, 0.4f).Spawn();
                new CrossGlow(Projectile.Center, Vector2.Zero, color, 60, 1f, 0.4f).Spawn();
                new NoiseShockRing(Projectile.Center, Vector2.Zero, color, 60, 1f, 1f, Projectile.whoAmI, Vector2.Zero).Spawn();
                new NoiseShockRing(Projectile.Center, Vector2.Zero, color, 60, 1f, 1f, Projectile.whoAmI, Vector2.Zero).Spawn();
                ParticlePreset.NewTOFL(Projectile.Center, Vector2.Zero, color, 45, 1.5f);
                caustics = LAPContent.AddScreenCaustics(25, Projectile.Center, 0.1f, 0.1f, 0.02f, 1f, false);
                #endregion
                #region 生成环绕粒子
                for (int i = 0; i < 25; i++)
                {
                    Color RandomColor = LAPUtilities.LerpColor(Color.WhiteSmoke, Color.LightGreen);
                    ParticlePreset.NewTGlowBall(Projectile.Center, Vector2.Zero, RandomColor, 75, 0.4f, Main.rand.NextFloat(4f, 9f));
                }
                for (int i = 0; i < LengthCount; i++)
                {
                    color = LAPUtilities.LerpColor(Color.WhiteSmoke, Color.LightGreen);
                    int time = MaxTime - i;
                    for (int d = 0; d < 2; d++)
                    {
                        Vector2 offset = -Vector2.UnitY.RotatedBy((time * MathHelper.Pi / 24f + d * MathHelper.Pi), default) * new Vector2(5f, 18f) - Projectile.rotation.ToRotationVector2() * 10f;
                        ParticlePreset.NewTGlowBall(Projectile.Center + Projectile.velocity * i + offset, Vector2.Zero, color, 60, 0.2f, 0.2f);
                    }
                }
                #endregion
                #region 生成枝条
                // 枝条
                color = LAPUtilities.LerpColor(Color.WhiteSmoke, Color.LightGreen);
                VFXInstance vfx = TerraVine.Spawn(Projectile.Center, Projectile.velocity * 2f, color, 1, 2f, Main.rand.NextFloat(12, 15), Main.rand.NextFloat(0.6f, 1f));
                vfx.ExtraUpdate *= 5;
                vfx.Lifetime = 150;
                color = LAPUtilities.LerpColor(Color.WhiteSmoke, Color.LightGreen);
                VFXInstance vfx2 = TerraVine.Spawn(Projectile.Center, Projectile.velocity * 2f, color, -1, 2f, Main.rand.NextFloat(12, 15), Main.rand.NextFloat(0.6f, 1f));
                vfx2.ExtraUpdate *= 5;
                vfx2.Lifetime = 150;
                color = LAPUtilities.LerpColor(Color.WhiteSmoke, Color.LightGreen);
                VFXInstance vfx3 = TerraVine.Spawn(Projectile.Center, Projectile.velocity * 2f, color, -1, 2f, Main.rand.NextFloat(12, 15), Main.rand.NextFloat(0.6f, 1f));
                vfx3.ExtraUpdate *= 5;
                vfx3.Lifetime = 150;
                color = LAPUtilities.LerpColor(Color.WhiteSmoke, Color.LightGreen);
                VFXInstance vfx4 = TerraVine.Spawn(Projectile.Center, Projectile.velocity * 2f, color, -1, 2f, Main.rand.NextFloat(12, 15), Main.rand.NextFloat(0.6f, 1f));
                vfx4.ExtraUpdate *= 5;
                vfx4.Lifetime = 150;
                #endregion
                #region 生成冲击环
                // 生成前进环
                float rotArgTotal = 15f;
                float rotArg = MathHelper.TwoPi / rotArgTotal;
                for (int i = 1; i < 10; i++)
                {
                    for (int j = 0; j < rotArgTotal; j++)
                    {
                        float rorate = j * rotArg;
                        Vector2 dustVelocity = new Vector2(4f, 0).BetterRotatedBy(rorate, default, 0.35f);
                        dustVelocity = dustVelocity.RotatedBy(Projectile.rotation);
                        ParticlePreset.NewTGlowBall(Projectile.Center + Projectile.velocity * 16 * i, dustVelocity, color, 60, 0.3f, 0f);
                    }
                }
                float rotArgTotal2 = 30f;
                float rotArg2 = MathHelper.TwoPi / rotArgTotal2;
                // 生成点
                for (int j = 0; j < rotArgTotal2; j++)
                {
                    float rorate = j * rotArg2;
                    Vector2 dustVelocity = new Vector2(12f, 0).BetterRotatedBy(rorate, default, 0.35f);
                    dustVelocity = dustVelocity.RotatedBy(Projectile.rotation);
                    ParticlePreset.NewTGlowBall(Projectile.Center, dustVelocity, color, 60, 0.3f, 0f);
                }
                for (int j = 0; j < rotArgTotal2; j++)
                {
                    float rorate = j * rotArg2;
                    Vector2 dustVelocity = new Vector2(8f, 0).BetterRotatedBy(rorate, default, 0.35f);
                    dustVelocity = dustVelocity.RotatedBy(Projectile.rotation);
                    ParticlePreset.NewTGlowBall(Projectile.Center + Projectile.velocity * 7, dustVelocity, color, 60, 0.3f, 0f);
                }
                for (int j = 0; j < rotArgTotal2; j++)
                {
                    float rorate = j * rotArg2;
                    Vector2 dustVelocity = new Vector2(5f, 0).BetterRotatedBy(rorate, default, 0.35f);
                    dustVelocity = dustVelocity.RotatedBy(Projectile.rotation);
                    ParticlePreset.NewTGlowBall(Projectile.Center + Projectile.velocity * 12, dustVelocity, color, 60, 0.3f, 0f);
                }
                for (int j = 0; j < rotArgTotal2; j++)
                {
                    float rorate = j * rotArg2;
                    Vector2 dustVelocity = new Vector2(8f, 0).BetterRotatedBy(rorate, default, 0.35f);
                    dustVelocity = dustVelocity.RotatedBy(Projectile.rotation);
                    ParticlePreset.NewTGlowBall(Projectile.Center - Projectile.velocity * 7, dustVelocity, color, 60, 0.3f, 0f);
                }
                #endregion
                #region 生成后坐力粒子
                for (int j = 0; j < 15; j++)
                {
                    Vector2 vel = -Projectile.velocity.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.2f, 1f) * 2f;
                    ParticlePreset.NewGlowLozenge(Projectile.Center, vel, color, 45, 0.5f);
                }
                for (int j = 0; j < 15; j++)
                {
                    Vector2 vel = -Projectile.velocity.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.2f, 1f) * 1.6f;
                    ParticlePreset.NewGlowLozenge_FastF(Projectile.Center, vel, color, 10, Vector2.One * 0.5f);
                }
                for (int j = 0; j < 10; j++)
                {
                    Vector2 vel = -Projectile.velocity.RotatedByRandom(MathHelper.PiOver4 * 0.8f) * Main.rand.NextFloat(0.2f, 1f) * 2f;
                    ParticlePreset.NewGlowLozenge(Projectile.Center + Main.rand.NextVector2Circular(15, 15), vel, color, 45, 0.5f);
                }
                for (int j = 0; j < 10; j++)
                {
                    Vector2 vel = -Projectile.velocity.RotatedByRandom(MathHelper.PiOver4 * 0.6f) * Main.rand.NextFloat(0.2f, 1f) * 4f;
                    ParticlePreset.NewGlowLozenge(Projectile.Center + Main.rand.NextVector2Circular(15, 15), vel, color, 45, 0.5f);
                }
                for (int j = 0; j < 10; j++)
                {
                    Vector2 vel = -Projectile.velocity.RotatedByRandom(MathHelper.PiOver4 * 0.3f) * Main.rand.NextFloat(0.2f, 1f) * 6f;
                    ParticlePreset.NewGlowLozenge(Projectile.Center + Main.rand.NextVector2Circular(15, 15), vel, color, 45, 0.5f);
                }
                #endregion
                #region 生成星星
                for (int i = 0; i < LengthCount; i+= 3)
                {
                    color = LAPUtilities.LerpColor(Color.WhiteSmoke, Color.LightGreen);
                    int time = MaxTime - i;
                    for (int d = 0; d < 2; d++)
                    {
                        Vector2 offset = -Vector2.UnitY.RotatedBy((time * MathHelper.Pi / 24f + d * MathHelper.Pi), default) * new Vector2(5f, 10f) - Projectile.rotation.ToRotationVector2() * 10f;
                        ParticlePreset.NewDustGlow(Projectile.Center + Projectile.velocity * i + offset, Main.rand.NextVector2CircularEdge(3, 3), 0, color, 45, 0.1f, 0);
                    }
                }
                for (int i = 0; i < 10; i ++)
                {
                    color = LAPUtilities.LerpColor(Color.WhiteSmoke, Color.LightGreen);
                    ParticlePreset.NewDustGlow(Projectile.Center, Main.rand.NextVector2CircularEdge(12, 12) * Main.rand.NextFloat(0.5f, 1f), 0, color, 45, 0.15f, 0);
                }
                #endregion
            }
        }
        public void SpawnProj()
        {
            if (Projectile.LAP().FirstFrame)
            {
                for (int i = 0; i < 6; i++)
                {
                    Vector2 pos = Projectile.Center + Projectile.velocity * 35 * i;
                    if (Projectile.IsLocalPlayer())
                    {
                        NPC npc = LAPUtilities.FindClosestTarget(pos, 1500);
                        if (npc is not null)
                        {
                            Vector2 toTarget = LAPUtilities.GetVector2(pos, npc.Center) * 12f;
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), pos, toTarget, ProjectileType<VividSmallBeam>(), Projectile.damage,
                                Projectile.knockBack, Projectile.owner, npc.Center.X, npc.Center.Y);
                        }
                    }
                    FirePos.Add(pos);
                }
            }
            if (Projectile.timeLeft == 2)
            {
                for (int i = 0; i < FirePos.Count; i++)
                {
                    Vector2 pos = FirePos[i];
                    Vector2 fireVel = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * 6f;
                    if (Projectile.IsLocalPlayer())
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), pos, fireVel, ProjectileType<ExoEnergy>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 1f);
                    }

                    Color color = Color.Lerp(Color.DarkBlue, Color.DeepSkyBlue, Main.rand.NextFloat());
                    new CrossGlow(pos, Vector2.Zero, color, 30, 1f, 0.2f).Spawn();
                    new CrossGlow(pos, Vector2.Zero, Color.White, 30, 1f, 0.2f).Spawn();
                    for (int j = 0; j < 5; j++)
                    {
                        Color RandomColor = LAPUtilities.LerpColor(Color.WhiteSmoke, Color.SkyBlue);
                        ParticlePreset.NewTGlowBall(pos, Vector2.Zero, RandomColor, 75, 0.2f, Main.rand.NextFloat(2f, 5f));
                    }
                }
            }
        }
        public void FadeOut()
        {
            Projectile.Opacity = MathHelper.Lerp(0f, 1f, EasingHelper.EaseOutCubic(Projectile.timeLeft / (float)MaxTime));
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.LAP().OnceHitEffect)
            {
                caustics = LAPContent.AddScreenCaustics(25, target.Center, 0.1f, 0.1f, 0.02f, 1f, false);

                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Projectile.velocity.RotatedBy(MathHelper.PiOver2), ProjectileType<ExoLightning>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Projectile.velocity.RotatedBy(MathHelper.PiOver2), ProjectileType<ExoLightning>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ProjectileType<ExoBlast>(), (int)(Projectile.damage * 5), Projectile.knockBack, Projectile.owner);
                float spread = MathHelper.TwoPi / 6f;
                float add = Main.rand.NextFloat(MathHelper.TwoPi);
                for (int i = 0; i < 6; i++)
                {
                    float fireAngle = spread * i;
                    Vector2 fireVel = Vector2.UnitX.RotatedBy(fireAngle + add);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, fireVel * 6f, ProjectileType<ExoEnergy>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            LAPUtilities.ReSetToBeginShader();
            Texture2D texture = LAPTextureRegister.StandardFlow3.Value;
            Vector4 uvFade = new Vector4(0.1f, 0.1f, 0, 0);
            Vector2 uvAdd = new Vector2(Main.GlobalTimeWrappedHourly * -0.5f, 0);
            Vector2 uvMult = new Vector2(3f, 1f);
            LAPUtilities.ApplyAlphaCut(uvFade, uvAdd, uvMult);
            DrawLaser(Color.Green, 1f);
            LAPUtilities.ApplyAlphaCut(uvFade, uvAdd * 0.75f, uvMult);
            DrawLaser(Color.SkyBlue, 1f);
            LAPUtilities.ApplyAlphaCut(uvFade, uvAdd * 0.5f, uvMult);
            DrawLaser(Color.Orange, 1f);
            Vector4 CoreFade = new Vector4(0.03f, 0.03f, 0, 0);
            LAPUtilities.ApplyAlphaCut(CoreFade, uvAdd * 0.5f, uvMult);
            DrawLaser(Color.White, 0.2f);

            Texture2D Lightning = UCATextureRegister.Lightning.Value;
            Vector2 LightninguvAdd = new Vector2(Main.GlobalTimeWrappedHourly * -0.5f + 0.3f, 0);
            Vector2 LightninguvMult = new Vector2(2f, 1f);
            LAPUtilities.ApplyAlphaCut(uvFade, LightninguvAdd * 0.25f, LightninguvMult);
            DrawLightning(Color.Green, 1f);
            LightninguvAdd.X += 0.4f;
            LAPUtilities.ApplyAlphaCut(uvFade, LightninguvAdd * 0.75f, LightninguvMult);
            DrawLightning(Color.SkyBlue, 1f);
            LightninguvAdd.X += 0.3f;
            LAPUtilities.ApplyAlphaCut(uvFade, LightninguvAdd * 0.5f, LightninguvMult);
            DrawLightning(Color.Orange, 1f);

            LAPUtilities.ReSetToEndShader();
            void DrawLaser(Color color, float height)
            {
                Vector2 orig = new(0, texture.Height / 2);
                float xScale = LaserLength / texture.Width;
                Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, color * Projectile.Opacity, Projectile.rotation, orig, new Vector2(xScale, 0.25f * height * Projectile.scale), SpriteEffects.None, 0);
            }
            void DrawLightning(Color color, float height)
            {
                Vector2 orig = new(0, Lightning.Height / 2);
                float xScale = LaserLength / Lightning.Width;
                Main.spriteBatch.Draw(Lightning, Projectile.Center - Main.screenPosition, null, color * Projectile.Opacity, Projectile.rotation, orig, new Vector2(xScale, 0.35f * height * Projectile.scale), SpriteEffects.None, 0);
            }
            return false;
        }
    }
}
