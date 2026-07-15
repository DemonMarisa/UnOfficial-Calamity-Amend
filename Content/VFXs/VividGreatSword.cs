using LAP.Assets.TextureRegister;
using LAP.Core.Enums;
using LAP.Core.Graphics.DeepGlow;
using LAP.Core.Graphics.Primitives.Trail;
using LAP.Core.Graphics.VFX;
using LAP.Core.Presets.Content;
using LAP.Core.SystemsLoader;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using UCA.Assets;
using UCA.Assets.Effects;
using UCA.Content.Projectiles.HeldProj.Magic.VividClarityHeld;

namespace UCA.Content.VFXs
{
    public class VividGreatSword : VFXBehavior
    {
        public static VFXInstance Spawn(int father)
        {
            VFXInstance vfx = LAPContent.SpawnVFX(LAPContent.VFXType<VividGreatSword>(), Vector2.Zero, Vector2.Zero, Color.White);
            vfx.AiInt[0] = father;
            return vfx;
        }
        public Projectile Father => Main.projectile[VFXInstance.AiInt[0]];
        public float TargetScale => VFXInstance.AiFloat[0];
        public ref float DrawScale => ref VFXInstance.AiFloat[1];
        public ref float BladeXScale => ref VFXInstance.AiFloat[2];
        public ref bool SpawmCharge => ref VFXInstance.AiBool[0];
        public ref bool FullCharge => ref VFXInstance.AiBool[1];
        public ref bool Follow => ref VFXInstance.AiBool[2];
        public int Time;
        public float InhaleOpacity = 1f;
        public List<VFXInstance> blast = [];
        public override void OnSpawn()
        {
            blast = new List<VFXInstance>(4);
            VFXInstance.Lifetime = 2;
            VFXInstance.Time = 0;
            VFXInstance.Scale = 0;
            InhaleOpacity = 1f;
            DrawScale = 1f;
            BladeXScale = 1f;
        }
        public override void Update()
        {
            UpdateFollow();
            Time++;
            if (Time % 2 == 0)
                SpawnInhaleDust();
            HandleChargeOrder();
            SpawnIdleDust();
        }
        public void UpdateFollow()
        {
            VFXInstance.Lifetime = 2;
            VFXInstance.Time = 0;
            VFXInstance.Position = Father.Center;
            VFXInstance.Rotation = Father.rotation;
            VFXInstance.Scale = MathHelper.Lerp(VFXInstance.Scale, TargetScale, 0.16f);

            for (int i = 0; i < blast.Count; i++)
            {
                if (blast[i].Behavior.Type == LAPContent.VFXType<VGSChargeBlast>())
                {
                    Vector2 InhaleOffset = new Vector2(42, 0).RotatedBy(VFXInstance.Rotation);
                    blast[i].Position = VFXInstance.Position + InhaleOffset;
                    blast[i].Velocity = VFXInstance.Velocity;
                    blast[i].Rotation = VFXInstance.Rotation;
                }
            }
        }
        public void SpawnInhaleDust()
        {
            if (VFXInstance.Scale == 0 || InhaleOpacity < 0.2f)
                return;
            Vector2 InhaleOffset = new Vector2(42, 0).RotatedBy(VFXInstance.Rotation);
            Vector2 SpawnPos = VFXInstance.Position + InhaleOffset + Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(50, 200) * VFXInstance.Scale;
            Vector2 Vel = LAPUtilities.GetVector2(SpawnPos, VFXInstance.Position + InhaleOffset);
            ParticlePreset.NewTOFL(SpawnPos, Vel * 3, Color.White, 45, 0.1f);
        }
        public void HandleChargeOrder()
        {
            if (SpawmCharge)
            {
                SpawnChargeDust();
                SpawmCharge = false;
            }
            if (FullCharge)
            {
                InhaleOpacity = MathHelper.Lerp(InhaleOpacity, 0f, 0.04f);
            }
        }
        public void SpawnChargeDust()
        {
            Vector2 InhaleOffset = new Vector2(42, 0).RotatedBy(VFXInstance.Rotation);
            VFXInstance vfx = VGSChargeBlast.Spawn(VFXInstance.Position + InhaleOffset, VFXInstance.Rotation, TargetScale + 0.2f);
            vfx.AiBool[0] = FullCharge;
            blast.Add(vfx);
        }
        public void SpawnIdleDust()
        {
            if (VFXInstance.Scale == 0 || FullCharge)
                return;
            Vector2 RandomAdd = new Vector2(Main.rand.NextFloat(-200, 800), Main.rand.NextFloat(-150, 150)) * VFXInstance.Scale;
            Vector2 SpawnPos = VFXInstance.Position + RandomAdd.RotatedBy(VFXInstance.Rotation);
            Vector2 Vel = VFXInstance.Rotation.ToRotationVector2();
            ParticlePreset.NewTOFL(SpawnPos, Vel * 3, Color.White, 45, 0.1f, 2f);
        }
        public override void OnKill()
        {
            base.OnKill();
        }
        public override void Draw()
        {
            if ((Father.type != ProjectileType<VividClarityGreatSword>() && Father.type != ProjectileType<VividClaritySupportMinion>()) || !Father.active)
            {
                VFXInstance.Kill();
            }
            DrawBlade();
            DrawInhale();
            DrawCoreLight();
        }
        public void DrawBlade()
        {
            Effect shader = UCAShaderRegister.SolarBladeShader.Value;
            shader.Parameters["uTime"].SetValue(-Main.GlobalTimeWrappedHourly);
            shader.Parameters["uIntensity"].SetValue(0.12f);
            shader.Parameters["ubeginColor"].SetValue(Color.White.ToVector4());
            shader.Parameters["uendColor"].SetValue(Color.White.ToVector4());
            shader.Parameters["UseColor"].SetValue(true);
            shader.Parameters["Opacity"].SetValue(1f);
            shader.CurrentTechnique.Passes[0].Apply();
            Main.graphics.GraphicsDevice.Textures[1] = LAPTextureRegister.Wood.Value;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;

            Texture2D blade = Request<Texture2D>($"UCA/Assets/ExtraTextures/VividClarityBlade").Value; //UCATextureRegister.VividClarityBlade.Value;
            Vector2 orig = new Vector2(blade.Size().X / 2, 800);
            LAPUtilities.Draw(blade, VFXInstance.Position - Main.screenPosition, null, Color.LightGreen * 1.3f, VFXInstance.Rotation + MathHelper.PiOver2, orig, VFXInstance.Scale * DrawScale * new Vector2(BladeXScale, 1f), 0);

            DeepGlow.SubmitCustomGlow(() =>
            {
                LAPUtilities.ReSetToBeginShader(BlendState.Additive);
                Effect shader = UCAShaderRegister.SolarBladeShader.Value;
                shader.Parameters["uTime"].SetValue(-Main.GlobalTimeWrappedHourly);
                shader.Parameters["uIntensity"].SetValue(0.12f);
                shader.Parameters["ubeginColor"].SetValue(Color.White.ToVector4());
                shader.Parameters["uendColor"].SetValue(Color.White.ToVector4());
                shader.Parameters["UseColor"].SetValue(true);
                shader.Parameters["Opacity"].SetValue(0.6f);
                shader.CurrentTechnique.Passes[0].Apply();
                Main.graphics.GraphicsDevice.Textures[1] = LAPTextureRegister.Wood.Value;
                Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
                LAPUtilities.Draw(blade, VFXInstance.Position - Main.screenPosition, null, Color.LightGreen, VFXInstance.Rotation + MathHelper.PiOver2, orig, VFXInstance.Scale * DrawScale * new Vector2(BladeXScale , 1f), 0);
                LAPUtilities.ReSetToEndShader();
            });
        }
        public void DrawInhale()
        {
            Vector2 InhaleOffset = new Vector2(42, 0).RotatedBy(VFXInstance.Rotation);
            Vector2 drawPos = VFXInstance.Position - Main.screenPosition + InhaleOffset;
            Effect shader = UCAShaderRegister.PolarDistortShader_Rot.Value;
            shader.Parameters["uWidthMult"].SetValue(4f);
            shader.Parameters["uRingMult"].SetValue(0.6f);
            shader.Parameters["uYTime"].SetValue(Main.GlobalTimeWrappedHourly * -0.3f);
            shader.Parameters["uTwist"].SetValue(2f);
            shader.CurrentTechnique.Passes[0].Apply();

            Main.instance.GraphicsDevice.Textures[1] = UCATextureRegister.FusableBall.Value;

            Texture2D texture = LAPTextureRegister.Aura_02.Value;
            Vector2 orig = texture.Size() / 2;
            Main.spriteBatch.Draw(texture, drawPos, null, Color.White * InhaleOpacity * VFXInstance.Opacity, 0, orig, VFXInstance.Scale * 1.2f * DrawScale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(texture, drawPos, null, Color.White * InhaleOpacity * VFXInstance.Opacity, 0, orig, VFXInstance.Scale * 1.2f * DrawScale, SpriteEffects.None, 0);

            Main.spriteBatch.Draw(texture, drawPos, null, Color.Gray * InhaleOpacity * VFXInstance.Opacity, 0, orig, VFXInstance.Scale * 1.2f * DrawScale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(texture, drawPos, null, Color.Gray * InhaleOpacity * VFXInstance.Opacity, 0, orig, VFXInstance.Scale * 1.2f * DrawScale, SpriteEffects.None, 0);
            shader.Parameters["uWidthMult"].SetValue(2f);
            Texture2D Aura_01 = LAPTextureRegister.Aura_01.Value;
            Main.spriteBatch.Draw(Aura_01, drawPos, null, Color.Gray * InhaleOpacity * VFXInstance.Opacity, 0, orig, VFXInstance.Scale * 1.2f * DrawScale, SpriteEffects.None, MathHelper.PiOver4);
            Main.spriteBatch.Draw(Aura_01, drawPos, null, Color.Gray * InhaleOpacity * VFXInstance.Opacity, 0, orig, VFXInstance.Scale * 1.2f * DrawScale, SpriteEffects.None, MathHelper.PiOver4);

            Main.spriteBatch.Draw(texture, drawPos, null, Color.Gray * InhaleOpacity * VFXInstance.Opacity, 0, orig, VFXInstance.Scale * 1.2f * DrawScale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(texture, drawPos, null, Color.Gray * InhaleOpacity * VFXInstance.Opacity, 0, orig, VFXInstance.Scale * 1.2f * DrawScale, SpriteEffects.None, 0);
        }
        public void DrawCoreLight()
        {
            LAPUtilities.ApplyDefaultShader();
            Texture2D texture = UCATextureRegister.CrossGlow.Value;
            Vector2 InhaleOffset = new Vector2(42, 0).RotatedBy(VFXInstance.Rotation);
            Vector2 drawPos = VFXInstance.Position - Main.screenPosition + InhaleOffset;
            LAPUtilities.Draw(texture, drawPos, null, Color.White * VFXInstance.Opacity, 0, texture.Size() / 2, VFXInstance.Scale * 0.5f, 0);
            Texture2D OFLine = LAPTextureRegister.OpticalFlaresLine.Value;
            LAPUtilities.Draw(OFLine, drawPos, null, Color.White * VFXInstance.Opacity, 0, texture.Size() / 2, VFXInstance.Scale * 0.7f, 0);
        }
    }
}
