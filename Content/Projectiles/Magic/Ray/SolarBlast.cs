using CalamityMod;
using CalamityMod.Graphics.Primitives;
using LAP.Content.Configs;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Assets.Effects;
using UCA.Assets.Sounds;
using UCA.Content.Configs;
using UCA.Content.Particiles;
using UCA.Content.Projectiles.HeldProj.Magic.ElementRayHeld;
using UCA.Core.BaseClass;
using UCA.Core.Utilities;

namespace UCA.Content.Projectiles.Magic.Ray
{
    public class SolarBlast : BaseMagicProj
    {
        public override string Texture => UCATextureRegister.ShockWavePath;
        public float Scale = 0f;
        public float Opacity = 1f;
        public float Rot = 0;
        public float Rot2 = 0;
        public ref float GlowBallCount => ref Projectile.ai[0];
        public ref float AllScale => ref Projectile.ai[1];
        public ref float StrikeCount => ref Projectile.ai[2];
        public override void SetDefaults()
        {
            Projectile.width = 200;
            Projectile.height = 200;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 30;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 45;
        }
        public override void OnSpawn(IEntitySource source)
        {
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float scale = 1 + AllScale;
            return CalamityUtils.CircularHitboxCollision(Projectile.Center, 150 * scale, targetHitbox);
        }

        public override void AI()
        {
            float scale = 1 + AllScale;
            if (Projectile.LAP().FirstFrame)
            {
                SoundEngine.PlaySound(SoundsMenu.FireBallBlast, Projectile.Center);
                Rot = Main.rand.NextFloat(0, MathHelper.TwoPi);
                Rot2 = Main.rand.NextFloat(0, MathHelper.TwoPi);
                int glowBallCount = 45 + (int)GlowBallCount;
                for (int j = 0; j < glowBallCount; j++)
                {
                    Color RandomColor = Color.Lerp(Color.Orange, Color.OrangeRed, Main.rand.NextFloat(0, 1));
                    new MediumGlowBall(Projectile.Center, RandomColor, 180, 0.2f, Main.rand.NextFloat(3f, 4f)).Spawn();
                }
                int strikeCount = 9 + (int)StrikeCount;
                for (int j = 0; j < strikeCount; j++)
                {
                    float rot = MathHelper.TwoPi / strikeCount;
                    new FireStrike(Projectile.Center, Vector2.Zero, Color.White, 30, 1f, rot * j + Main.rand.NextFloat(-0.3f, 0.3f), Main.rand.NextFloat(0.3f, 0.35f) * scale).SpawnToPriority();
                }
                if (!LAPConfig.Instance.PerformanceMode)
                    new CrossGlow(Projectile.Center, Vector2.Zero, Color.Orange, 60, 1f, 0.4f).Spawn();
                Projectile.netUpdate = true;
            }

            Scale = MathHelper.Lerp(0f, 1f, 1 - EasingHelper.EaseInCubic(Projectile.timeLeft / 30f)) * scale;

            if (Projectile.timeLeft < 15)
                Opacity = MathHelper.Lerp(1f, 0f, 1 - EasingHelper.EaseInCubic(Projectile.timeLeft / 15f));
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Daybreak, 180);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            LAPUtilities.ReSetToBeginShader(BlendState.Additive);
            Effect shader = UCAShaderRegister.SolarBlastShader.Value;
            shader.Parameters["uTime"].SetValue(-Main.GlobalTimeWrappedHourly);
            shader.Parameters["uIntensity"].SetValue(0.2f);
            shader.Parameters["ubeginColor"].SetValue(Color.Orange.ToVector4() * Opacity);
            shader.Parameters["uendColor"].SetValue(Color.OrangeRed.ToVector4() * Opacity);
            shader.Parameters["UseColor"].SetValue(true);
            shader.CurrentTechnique.Passes[0].Apply();
            Main.graphics.GraphicsDevice.Textures[1] = UCATextureRegister.FireNoise.Value;
            Vector2 DrawPos = Projectile.Center - Main.screenPosition;
            Vector2 orig = new Vector2(UCATextureRegister.ShockWave.Size().X / 2, UCATextureRegister.ShockWave.Size().Y / 2 - 40);
            Main.spriteBatch.Draw(UCATextureRegister.ShockWave.Value, DrawPos, null, Color.Orange, 0, orig, Scale * 0.8f, SpriteEffects.None, 0);
            if (!LAPConfig.Instance.PerformanceMode)
                Main.spriteBatch.Draw(UCATextureRegister.ShockWave.Value, DrawPos, null, Color.Orange, 0, orig, Scale * 0.8f, SpriteEffects.None, 0);

            shader.Parameters["uTime"].SetValue(-Main.GlobalTimeWrappedHourly);
            shader.Parameters["uIntensity"].SetValue(0.2f);
            shader.Parameters["ubeginColor"].SetValue(Color.OrangeRed.ToVector4() * Opacity);
            shader.Parameters["uendColor"].SetValue(Color.OrangeRed.ToVector4() * Opacity);
            shader.Parameters["UseColor"].SetValue(true);
            shader.CurrentTechnique.Passes[0].Apply();
            Main.graphics.GraphicsDevice.Textures[1] = UCATextureRegister.FireNoise.Value;
            Main.spriteBatch.Draw(UCATextureRegister.ShockWave.Value, DrawPos, null, Color.Orange, 0, orig, Scale * 0.6f, SpriteEffects.None, 0);
            if (!LAPConfig.Instance.PerformanceMode)
                Main.spriteBatch.Draw(UCATextureRegister.ShockWave.Value, DrawPos, null, Color.Orange, 0, orig, Scale * 0.6f, SpriteEffects.None, 0);
            LAPUtilities.ReSetToEndShader();
            return false;
        }
    }
}
