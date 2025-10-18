using CalamityMod;
using CalamityMod.Graphics.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Assets.Effects;
using UCA.Content.Particiles;
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
            return CalamityUtils.CircularHitboxCollision(Projectile.Center, 150, targetHitbox);
        }

        public override void AI()
        {
            if (Projectile.UCA().FirstFrame)
            {
                SoundEngine.PlaySound(SoundID.DD2_GoblinBomb);
                Rot = Main.rand.NextFloat(0, MathHelper.TwoPi);
                Rot2 = Main.rand.NextFloat(0, MathHelper.TwoPi);
                for (int j = 0; j < 45; j++)
                {
                    Color RandomColor = Color.Lerp(Color.Orange, Color.OrangeRed, Main.rand.NextFloat(0, 1));
                    new MediumGlowBall(Projectile.Center, RandomColor, 180, 0.2f, Main.rand.NextFloat(3f, 4f)).Spawn();
                }
                for (int j = 0; j < 9; j++)
                {
                    float rot = MathHelper.TwoPi / 9;
                    new FireStrike(Projectile.Center, Vector2.Zero, Color.White, 30, 1f, rot * j + Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextFloat(0.3f, 0.35f)).SpawnToPriority();
                }
                new CrossGlow(Projectile.Center, Vector2.Zero, Color.Orange, 60, 1f, 0.4f).Spawn();
            }
            Scale = MathHelper.Lerp(0f, 1f, 1 - EasingHelper.EaseInCubic(Projectile.timeLeft / 30f));

            if (Projectile.timeLeft < 15)
                Opacity = MathHelper.Lerp(1f, 0f, 1 - EasingHelper.EaseInCubic(Projectile.timeLeft / 15f));
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Daybreak, 180);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            UCAUtilities.ReSetToBeginShader(BlendState.Additive);
            UCAShaderRegister.SolarBlastShader.Parameters["uTime"].SetValue(-Main.GlobalTimeWrappedHourly);
            UCAShaderRegister.SolarBlastShader.Parameters["uIntensity"].SetValue(0.2f);
            UCAShaderRegister.SolarBlastShader.Parameters["ubeginColor"].SetValue(Color.Orange.ToVector4() * Opacity);
            UCAShaderRegister.SolarBlastShader.Parameters["uendColor"].SetValue(Color.OrangeRed.ToVector4() * Opacity);
            UCAShaderRegister.SolarBlastShader.Parameters["UseColor"].SetValue(true);
            UCAShaderRegister.SolarBlastShader.CurrentTechnique.Passes[0].Apply();
            Main.graphics.GraphicsDevice.Textures[1] = UCATextureRegister.FireNoise.Value;
            Vector2 DrawPos = Projectile.Center - Main.screenPosition;
            Vector2 orig = new Vector2(UCATextureRegister.ShockWave.Size().X / 2, UCATextureRegister.ShockWave.Size().Y / 2 - 40);
            Main.spriteBatch.Draw(UCATextureRegister.ShockWave.Value, DrawPos, null, Color.Orange, 0, orig, Scale * 0.8f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(UCATextureRegister.ShockWave.Value, DrawPos, null, Color.Orange, 0, orig, Scale * 0.8f, SpriteEffects.None, 0);

            UCAShaderRegister.SolarBlastShader.Parameters["uTime"].SetValue(-Main.GlobalTimeWrappedHourly);
            UCAShaderRegister.SolarBlastShader.Parameters["uIntensity"].SetValue(0.2f);
            UCAShaderRegister.SolarBlastShader.Parameters["ubeginColor"].SetValue(Color.OrangeRed.ToVector4() * Opacity);
            UCAShaderRegister.SolarBlastShader.Parameters["uendColor"].SetValue(Color.OrangeRed.ToVector4() * Opacity);
            UCAShaderRegister.SolarBlastShader.Parameters["UseColor"].SetValue(true);
            UCAShaderRegister.SolarBlastShader.CurrentTechnique.Passes[0].Apply();
            Main.graphics.GraphicsDevice.Textures[1] = UCATextureRegister.FireNoise.Value;
            Main.spriteBatch.Draw(UCATextureRegister.ShockWave.Value, DrawPos, null, Color.Orange, 0, orig, Scale * 0.6f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(UCATextureRegister.ShockWave.Value, DrawPos, null, Color.Orange, 0, orig, Scale * 0.6f, SpriteEffects.None, 0);
            UCAUtilities.ReSetToEndShader();
            return false;
        }
    }
}
