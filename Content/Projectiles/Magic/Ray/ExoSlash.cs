using LAP.Assets.TextureRegister;
using LAP.Core.Graphics.DeepGlow;
using LAP.Core.Presets.Content;
using LAP.Core.SystemsLoader;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Core.BaseClass;

namespace UCA.Content.Projectiles.Magic.Ray
{
    public class ExoSlash : BaseMagicProj
    {
        public override string Texture => LAPTextureRegister.InvisibleTexturePath;
        public int LaserLength;
        public Vector2 BeginPos;
        public Vector2 EndPos;
        public int Time;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 4400;
            Projectile.AddProtectedProj();
        }
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.extraUpdates = 0;
            Projectile.friendly = true;
            Projectile.timeLeft = 40;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 40;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Projectile.Opacity < 0.1f) return false;
            float _ = float.NaN;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), BeginPos, EndPos, 128f, ref _);
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override void AI()
        {
            Time++;
            if (Projectile.LAP().FirstFrame)
            {
                BeginPos = Projectile.Center - Projectile.velocity.SafeNormalize(Vector2.UnitX) * 300;
                EndPos = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.UnitX) * 2100;
                LaserLength = (int)(EndPos - BeginPos).Length();
                Projectile.rotation = Projectile.velocity.ToRotation();
                for (int i = 0;i < 100; i++)
                {
                    Vector2 vel = Projectile.velocity.RotatedBy(Main.rand.NextBool() ? MathHelper.PiOver2 :- MathHelper.PiOver2) * Main.rand.NextFloat(3f, 9f);
                    Vector2 SpawnPos = Vector2.Lerp(BeginPos, EndPos, i / 100f) + Main.rand.NextVector2Circular(24, 24);
                    Color color = LAPUtilities.LerpColor(Color.WhiteSmoke, Color.LightGreen);
                    ParticlePreset.NewDustGlow(SpawnPos, vel, 0, color, 45, 0.2f, 0);

                    ParticlePreset.NewTGlowBall(SpawnPos, Vector2.Zero, color, 70, 0.25f, 4f);
                }
            }
            if (Time < 5)
                Projectile.scale = MathHelper.Lerp(0f, 1f, EasingHelper.EaseOutCubic(Time / 5f));
            else if (Time > 15 && Time < 40)
            {
                Projectile.scale = MathHelper.Lerp(1f, 0f, EasingHelper.EaseInCubic((Time - 15) / 24f));
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            LAPUtilities.ReSetToBeginShader();
            Texture2D texture = LAPTextureRegister.Aura_01.Value;
            DrawAura(texture, Color.White);
            DrawAura(texture, Color.White, 0.7f);
            DrawAura(texture, Color.White, 0.5f);
            Texture2D Laser = LAPTextureRegister.StandardFlow3.Value;
            DrawLaser(Laser, Color.Orange);
            DrawLaser(Laser, Color.SkyBlue, 0.7f);
            DrawLaser(Laser, Color.LightGreen, 0.4f);
            LAPUtilities.ReSetToEndShader();
            DeepGlow.SubmitCustomGlow(() =>
            {
                LAPUtilities.ReSetToBeginShader();
                Texture2D texture = LAPTextureRegister.Aura_01.Value;
                DrawAura(texture, Color.White);
                LAPUtilities.ReSetToEndShader();
            });
            return false;
        }
        public void DrawAura(Texture2D texture, Color color, float uvAdd = 0f, float height = 0.2f, float Speed = 1f)
        {
            float TextureWidth = texture.Width;
            float UVMult = LaserLength / TextureWidth;

            float FadeInProgress = 200f / LaserLength;

            Vector4 opacity = new Vector4(FadeInProgress, FadeInProgress, 0.2f, 0.2f);
            Vector2 uv = new Vector2(-Main.GlobalTimeWrappedHourly * Speed + uvAdd, 0);
            Vector2 uvmult = new Vector2(UVMult / 10, 1);

            LAPUtilities.ApplyAlphaCut(opacity, uv, uvmult, color);

            Vector2 orig = new(0, TextureWidth / 2);
            Main.spriteBatch.Draw(texture, BeginPos - Main.screenPosition, null, color * Projectile.Opacity, Projectile.rotation, orig, new Vector2(UVMult, height * Projectile.scale), SpriteEffects.None, 0);
        }
        public void DrawLaser(Texture2D texture, Color color, float uvAdd = 0f, float height = 0.2f, float Speed = 1f)
        {
            float TextureWidth = texture.Width;
            float UVMult = LaserLength / TextureWidth;

            float FadeInProgress = 200f / LaserLength;

            Vector4 opacity = new Vector4(FadeInProgress, FadeInProgress, 0.2f, 0.2f);
            Vector2 uv = new Vector2(-Main.GlobalTimeWrappedHourly * Speed + uvAdd, 0);
            Vector2 uvmult = new Vector2(UVMult, 1);

            LAPUtilities.ApplyAlphaCut(opacity, uv, uvmult);

            Vector2 orig = new(0, TextureWidth / 2);
            Main.spriteBatch.Draw(texture, BeginPos - Main.screenPosition, null, color * Projectile.Opacity, Projectile.rotation, orig, new Vector2(UVMult, height * Projectile.scale), SpriteEffects.None, 0);
        }
    }
}
