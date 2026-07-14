using LAP.Assets.TextureRegister;
using LAP.Content.Particles;
using LAP.Content.Particles_ECS;
using LAP.Core.Graphics.DeepGlow;
using LAP.Core.Graphics.VFX;
using LAP.Core.Presets.Content;
using LAP.Core.SystemsLoader;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using UCA.Assets;
using UCA.Assets.Effects;

namespace UCA.Content.VFXs
{
    public class VGSChargeBlast : VFXBehavior
    {
        public static VFXInstance Spawn(Vector2 position, float rot, float targetScale)
        {
            VFXInstance vfx = LAPContent.SpawnVFX(LAPContent.VFXType<VGSChargeBlast>(), position, Vector2.Zero, Color.White, rot, 0, targetScale);
            return vfx;
        }
        public ref float MaxScale => ref VFXInstance.AiFloat[0];
        public bool FullCharge => VFXInstance.AiBool[0];
        public override void OnSpawn()
        {
            VFXInstance.Lifetime = 25;
            VFXInstance.Scale = 0f;
            for (int i = 0; i < 32;i++)
            {
                new TrailGlowBall(VFXInstance.Position, Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * 5f * Main.rand.NextFloat(0.4f, 1f) * MaxScale, Color.White, 25, 0.3f).Spawn();
            }
            for (int i = 0; i < 16; i++)
            {
                ParticlePreset.NewDustGlow(VFXInstance.Position, Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * 24f * Main.rand.NextFloat(0.4f, 1f) * MaxScale, 0, Color.Gray * 1.3f, 45, 0.3f, 0);
            }
            for (int i = 0; i < 19; i++)
            {
                ParticlePreset.NewTOFL(VFXInstance.Position, Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * 6f * Main.rand.NextFloat(0.4f, 1f) * MaxScale, Color.White, 60, 0.2f, (9f * MaxScale) + 3f, 1.3f);
            }
            for (int i =0; i < 25;i++)
            {
                Vector2 BeginPos = VFXInstance.Position;
                Vector2 EndPos = VFXInstance.Position + VFXInstance.Rotation.ToRotationVector2() * 500 * MaxScale;
                Vector2 SpawnPos = Vector2.Lerp(BeginPos, EndPos, i / 25f);
                ParticlePreset.NewTOFL(SpawnPos, Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * 6f * Main.rand.NextFloat(0.4f, 1f) * MaxScale, Color.White, 60, 0.15f, (3f * MaxScale) + 1f, 1.1f);
            }
            for (int i = 0; i < 16; i++)
            {
                Vector2 BeginPos = VFXInstance.Position;
                Vector2 EndPos = VFXInstance.Position + VFXInstance.Rotation.ToRotationVector2() * 570 * MaxScale;
                Vector2 SpawnPos = Vector2.Lerp(BeginPos, EndPos, Main.rand.NextFloat());
                ParticlePreset.NewDustGlow(SpawnPos, (VFXInstance.Rotation - MathHelper.PiOver2).ToRotationVector2() * Main.rand.NextFloat(3f, 12f), 0, Color.Gray * 1.3f, 45, 0.3f, 0);
            }
            for (int i = 0; i < 16; i++)
            {
                Vector2 BeginPos = VFXInstance.Position;
                Vector2 EndPos = VFXInstance.Position + VFXInstance.Rotation.ToRotationVector2() * 570 * MaxScale;
                Vector2 SpawnPos = Vector2.Lerp(BeginPos, EndPos, Main.rand.NextFloat());
                ParticlePreset.NewDustGlow(SpawnPos, -(VFXInstance.Rotation - MathHelper.PiOver2).ToRotationVector2() * Main.rand.NextFloat(3f, 12f), 0, Color.Gray * 1.3f, 45, 0.3f, 0);
            }
        }
        public override void Update()
        {
            if (FullCharge)
                VFXInstance.Lifetime = 60;

            VFXInstance.Scale = MathHelper.Lerp(0f, MaxScale, EasingHelper.EaseOutExpo(VFXInstance.LifetimeRatio));
            VFXInstance.Opacity = MathHelper.Lerp(1f, 0f, EasingHelper.EaseInCubic(VFXInstance.LifetimeRatio));
            if (VFXInstance.Time < 26)
            {
                Vector2 xoffset = Vector2.UnitX.RotatedBy(VFXInstance.Rotation - MathHelper.PiOver2) * Main.rand.NextFloat(-80, 80);
                Vector2 yoffset = -VFXInstance.Rotation.ToRotationVector2() * 120;
                AuraTrail.Spawn(VFXInstance.Position + xoffset + yoffset, VFXInstance.Rotation.ToRotationVector2() * 9f, Color.White, Main.rand.Next(5, 110), 10, true, 12, 1f, 30);
            }
        }
        public override void Draw()
        {
            DrawNoiseRing();
            DrawCoreLight();
            DrawNoiseCirclle();
        }
        public void DrawNoiseRing()
        {
            Effect shader = UCAShaderRegister.PolarDistortShaderWithR.Value;
            shader.Parameters["uWidthMult"].SetValue(4f);
            shader.Parameters["uRingMult"].SetValue(4f);
            shader.Parameters["uYTime"].SetValue(Main.GlobalTimeWrappedHourly);
            shader.CurrentTechnique.Passes[0].Apply();

            Main.instance.GraphicsDevice.Textures[1] = UCATextureRegister.BloomRing.Value;

            Texture2D texture = LAPTextureRegister.Aura_01.Value;
            Vector2 orig = texture.Size() / 2;
            Main.spriteBatch.Draw(texture, VFXInstance.Position - Main.screenPosition, null, VFXInstance.DrawColor * VFXInstance.Opacity, VFXInstance.Rotation, orig, VFXInstance.Scale * 1.3f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(texture, VFXInstance.Position - Main.screenPosition, null, VFXInstance.DrawColor * VFXInstance.Opacity, VFXInstance.Rotation, orig, VFXInstance.Scale * 1.3f, SpriteEffects.None, 0);
            Texture2D Aura2 = LAPTextureRegister.Aura_02.Value;
            Vector2 Aura2orig = Aura2.Size() / 2;
            Main.spriteBatch.Draw(Aura2, VFXInstance.Position - Main.screenPosition, null, VFXInstance.DrawColor * VFXInstance.Opacity, VFXInstance.Rotation, Aura2orig, VFXInstance.Scale * 1.1f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(Aura2, VFXInstance.Position - Main.screenPosition, null, VFXInstance.DrawColor * VFXInstance.Opacity, VFXInstance.Rotation, Aura2orig, VFXInstance.Scale * 1.1f, SpriteEffects.None, 0);

            DeepGlow.SubmitCustomGlow(() =>
            {
                LAPUtilities.ReSetToBeginShader();

                Effect shader = UCAShaderRegister.PolarDistortShaderWithR.Value;
                shader.Parameters["uWidthMult"].SetValue(4f);
                shader.Parameters["uRingMult"].SetValue(4f);
                shader.Parameters["uYTime"].SetValue(Main.GlobalTimeWrappedHourly);
                shader.CurrentTechnique.Passes[0].Apply();

                Main.instance.GraphicsDevice.Textures[1] = UCATextureRegister.BloomRing.Value;

                Main.spriteBatch.Draw(texture, VFXInstance.Position - Main.screenPosition, null, VFXInstance.DrawColor * VFXInstance.Opacity, VFXInstance.Rotation, orig, VFXInstance.Scale * 1.3f, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(texture, VFXInstance.Position - Main.screenPosition, null, VFXInstance.DrawColor * VFXInstance.Opacity, VFXInstance.Rotation, orig, VFXInstance.Scale * 1.3f, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(Aura2, VFXInstance.Position - Main.screenPosition, null, VFXInstance.DrawColor * VFXInstance.Opacity, VFXInstance.Rotation, Aura2orig, VFXInstance.Scale * 1.1f, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(Aura2, VFXInstance.Position - Main.screenPosition, null, VFXInstance.DrawColor * VFXInstance.Opacity, VFXInstance.Rotation, Aura2orig, VFXInstance.Scale * 1.1f, SpriteEffects.None, 0);

                LAPUtilities.ReSetToEndShader();
            });
            LAPUtilities.ApplyDefaultShader();
        }
        public void DrawCoreLight()
        {
            Texture2D CrossGlow = UCATextureRegister.CrossGlow.Value;
            LAPUtilities.Draw(CrossGlow, VFXInstance.Position - Main.screenPosition, null, VFXInstance.DrawColor * VFXInstance.Opacity, 0, CrossGlow.Size() / 2, VFXInstance.Scale * 0.6f, SpriteEffects.None, 0);
            Texture2D OfLine = LAPTextureRegister.OpticalFlaresLine.Value;
            LAPUtilities.Draw(OfLine, VFXInstance.Position - Main.screenPosition, null, VFXInstance.DrawColor * VFXInstance.Opacity, 0, OfLine.Size() / 2, VFXInstance.Scale * 0.8f, SpriteEffects.None, 0);
            Texture2D flash = LAPTextureRegister.Flash_01.Value;
            LAPUtilities.Draw(flash, VFXInstance.Position - Main.screenPosition, null, VFXInstance.DrawColor * VFXInstance.Opacity, VFXInstance.Rotation, flash.Size() / 2, VFXInstance.Scale * 0.8f, SpriteEffects.None, 0);

        }
        public void DrawNoiseCirclle()
        {
            Effect shader = UCAShaderRegister.PolarDistortShaderWithR.Value;
            shader.Parameters["uWidthMult"].SetValue(4f);
            shader.Parameters["uRingMult"].SetValue(1f);
            shader.Parameters["uYTime"].SetValue(Main.GlobalTimeWrappedHourly);
            shader.CurrentTechnique.Passes[0].Apply();

            Main.instance.GraphicsDevice.Textures[1] = UCATextureRegister.FusableBall.Value;

            Texture2D texture = LAPTextureRegister.Aura_01.Value;
            Vector2 orig = texture.Size() / 2;
            Main.spriteBatch.Draw(texture, VFXInstance.Position - Main.screenPosition, null, VFXInstance.DrawColor * VFXInstance.Opacity, VFXInstance.Rotation, orig, VFXInstance.Scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(texture, VFXInstance.Position - Main.screenPosition, null, VFXInstance.DrawColor * VFXInstance.Opacity, VFXInstance.Rotation, orig, VFXInstance.Scale, SpriteEffects.None, 0);
        }
    }
}
