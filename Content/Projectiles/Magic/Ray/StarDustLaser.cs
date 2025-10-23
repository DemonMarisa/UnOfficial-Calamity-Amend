using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Assets.Effects;
using UCA.Assets.Sounds;
using UCA.Content.MetaBalls;
using UCA.Content.Particiles;
using UCA.Core.BaseClass;
using UCA.Core.Utilities;

namespace UCA.Content.Projectiles.Magic.Ray
{
    public class StarDustLaser : BaseMagicProj
    {
        public override string Texture => UCATextureRegister.CollectableLightPath;
        public bool CanHit = true;
        public int MaxLife = 270;
        public Vector2 BeginPos;
        public Vector2 EndPos;
        public float LaserLength => Vector2.Distance(BeginPos, EndPos);
        public float Opacity = 0f;
        public bool BeginFadeOut = false;
        public bool UseFadeIn => Projectile.ai[0] == 0;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 4400;
        }
        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = MaxLife;
            Projectile.extraUpdates = 7;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(CanHit);
            writer.Write(BeginFadeOut);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            CanHit = reader.ReadBoolean();
            BeginFadeOut = reader.ReadBoolean();
        }
        public override bool? CanHitNPC(NPC target)
        {
            if (!CanHit)
                return false;
            else
                return base.CanHitNPC(target);
        }
        public override void AI()
        {
            if (Projectile.UCA().FirstFrame)
            {
                SoundEngine.PlaySound(SoundsMenu.TerraRayLeftFire, Projectile.Center);
                if (UseFadeIn)
                {
                    Projectile.netUpdate = true;
                    for (int j = 0; j < 10; j++)
                    {
                        Color RandomColor = Color.Lerp(Color.SkyBlue, Color.DarkBlue, Main.rand.NextFloat(0, 1));
                        new MediumGlowBall(Projectile.Center, RandomColor, 60, 0.2f, Main.rand.NextFloat(1.6f, 2f)).Spawn();
                    }
                    for (int i = 0; i < 15; i++)
                    {
                        Vector2 spawnVec = Vector2.UnitX.RotateRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.2f, 0.3f) * 12;
                        StarDustMetaBall.SpawnParticle(Projectile.Center, spawnVec, 0.2f, 45);
                    }
                    new CrossGlow(Projectile.Center, Vector2.Zero, Color.SkyBlue, 60, 1f, 0.2f).Spawn();
                    new CrossGlow(Projectile.Center, Vector2.Zero, Color.DeepSkyBlue, 60, 1f, 0.2f).Spawn();
                }
                BeginPos = Projectile.Center;
            }
            EndPos = Projectile.Center;
            Projectile.rotation = (EndPos - BeginPos).ToRotation();
            if (!BeginFadeOut)
            {
                Color color = UCAUtilities.LerpColor(Color.DeepSkyBlue, Color.SkyBlue);
                new TrailGlowBall(Projectile.Center + Main.rand.NextVector2Circular(9, 9), Projectile.velocity * 0.25f, color, Main.rand.Next(45, 65), 0.08f, true).Spawn();
            }
            if (Projectile.timeLeft < MaxLife / 2)
                BeginFadeOut = true;
            UpdateFadeInOut();
        }
        public void UpdateFadeInOut()
        {
            if (BeginFadeOut)
            {
                if (Projectile.ai[1] == 0)
                {
                    for (int i = 0; i < 15; i++)
                    {
                        Vector2 spawnVec = Vector2.UnitX.RotateRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.2f, 0.3f) * 12;
                        StarDustMetaBall.SpawnParticle(Projectile.Center, spawnVec, 0.2f, 45);
                    }
                    Projectile.ai[1]++;
                }

                Opacity = MathHelper.Lerp(Opacity, 0f, 0.01f);
                if (Opacity < 0.05f)
                    Projectile.Kill();
                Projectile.velocity *= 0.8f;
                Projectile.damage = 0;
            }
            else
            {
                Opacity = MathHelper.Lerp(Opacity, 1f, 0.1f);

            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            CanHit = false;
            BeginFadeOut = true;
            Projectile.netUpdate = true;
        }
        public override void OnKill(int timeLeft)
        {

        }
        public override bool PreDraw(ref Color lightColor)
        {
            UCAUtilities.ReSetToBeginShader(BlendState.Additive);
            DrawLaser(Color.DeepSkyBlue, 0.13f);
            DrawLaser(Color.SkyBlue, 0.06f);
            DrawLaser(Color.White, 0.03f);
            UCAUtilities.ReSetToEndShader();
            return false;
        }
        public void DrawLaser(Color colro, float height = 0.2f, float op = 0.1f, int Speed = -50)
        {
            float TextureHeight = UCATextureRegister.ElementalRayFlow.Height();
            float TextureWidth = UCATextureRegister.ElementalRayFlow.Width();

            UCAShaderRegister.StandardFlowShader.Parameters["LaserTextureSize"].SetValue(UCATextureRegister.ElementalRayFlow.Size());
            UCAShaderRegister.StandardFlowShader.Parameters["targetSize"].SetValue(new Vector2(LaserLength, TextureHeight));
            UCAShaderRegister.StandardFlowShader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * Speed);
            UCAShaderRegister.StandardFlowShader.Parameters["uColor"].SetValue(colro.ToVector4() * Opacity);
            UCAShaderRegister.StandardFlowShader.Parameters["uFadeoutLength"].SetValue(op);
            UCAShaderRegister.StandardFlowShader.Parameters["uFadeinLength"].SetValue(op);
            UCAShaderRegister.StandardFlowShader.CurrentTechnique.Passes[0].Apply();

            Vector2 orig = new(0, TextureHeight / 2);
            float xScale = LaserLength / TextureWidth;
            Main.spriteBatch.Draw(UCATextureRegister.TerrarRayFlow.Value, BeginPos - Main.screenPosition, null, Color.White, Projectile.rotation, orig, new Vector2(xScale, height), SpriteEffects.None, 0);
        }
    }
}
