using LAP.Assets.TextureRegister;
using LAP.Content.Configs;
using LAP.Core.Graphics.DeepGlow;
using LAP.Core.Graphics.Primitives.Trail;
using LAP.Core.Graphics.VFX;
using LAP.Core.Presets.Content;
using LAP.Core.SystemsLoader;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Assets.Effects;
using UCA.Content.Particiles;
using UCA.Core.BaseClass;

namespace UCA.Content.Projectiles.Magic.Ray
{
    public class ExoEnergy : BaseMagicProj
    {
        public override string Texture => LAPTextureRegister.InvisibleTexturePath;
        public bool BeginHit => Projectile.ai[0] != 0;
        public int Time;
        public bool BeginFadeOut;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 4400;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 25;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            Projectile.AddProtectedProj();
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
            Projectile.timeLeft = 240;
            Projectile.extraUpdates = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.Opacity = 1f;
            Projectile.scale = 0.25f;
        }
        public override bool? CanHitNPC(NPC target)
        {
            if (BeginFadeOut)
                return false;
            if (BeginHit)
                return base.CanHitNPC(target);
            if (Time < 30)
                return false;
            else
                return base.CanHitNPC(target);
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(BeginFadeOut);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            BeginFadeOut = reader.ReadBoolean();
        }
        public override void AI()
        {
            if (BeginFadeOut)
            {
                Projectile.Opacity = MathHelper.Lerp(Projectile.Opacity, 0f, 0.12f);
                return;
            }
            else if (Projectile.timeLeft < 30)
            {
                BeginFadeOut = true;
                Projectile.velocity = Vector2.Zero;
                Projectile.numUpdates = -1;
                Projectile.extraUpdates = 0;
                Projectile.timeLeft = 30;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            Time++;
            if (Time > 30)
            {
                Color color = LAPUtilities.LerpColor(new Color(57, 46, 115), Color.SkyBlue);
                if (Projectile.timeLeft % 3 == 0)
                {
                    ParticlePreset.NewTOFL(Projectile.Center, Vector2.Zero, color, 15, 0.1f, 0);
                }
                NPC npc = LAPUtilities.FindClosestTarget(Projectile.Center, 1500, true);
                if (npc is not null)
                    Projectile.HomingTarget(npc.Center, 1500, 18f, 35f);
                else
                    Projectile.velocity *= 0.97f;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.velocity = Vector2.Zero;
            BeginFadeOut = true;
            Projectile.numUpdates = -1;
            Projectile.extraUpdates = 0;
            Projectile.timeLeft = 30;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Time < 10)
                return false;
            List<Vector2> list = [];
            List<float> rot = [];
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                if (Projectile.oldPos[i] != Vector2.Zero)
                {
                    list.Add(Projectile.oldPos[i] + Projectile.Size / 2);
                    rot.Add(Projectile.oldRot[i]);
                }
            }
            Color exoBlueColor = new Color(57, 46, 115);

            LAPUtilities.ReSetToBeginShader(BlendState.Additive, SamplerState.PointClamp);
            Effect shader = UCAShaderRegister.PolarDistortShaderWithR.Value;
            shader.Parameters["uWidthMult"].SetValue(4f);
            shader.Parameters["uRingMult"].SetValue(1f);
            shader.Parameters["uYTime"].SetValue(Main.GlobalTimeWrappedHourly * 0.3f);
            shader.CurrentTechnique.Passes[0].Apply();

            Main.instance.GraphicsDevice.Textures[1] = UCATextureRegister.FusableBall.Value;

            Texture2D texture = LAPTextureRegister.Aura_02.Value;
            Vector2 orig = texture.Size() / 2;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.DeepSkyBlue * Projectile.Opacity, 0, orig, Projectile.scale * 0.8f, SpriteEffects.FlipVertically, 0);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.DeepSkyBlue * Projectile.Opacity, 0, orig, Projectile.scale * 0.8f, SpriteEffects.FlipVertically, 0);

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, exoBlueColor * Projectile.Opacity, 0, orig, Projectile.scale * 0.8f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, exoBlueColor * Projectile.Opacity, 0, orig, Projectile.scale * 0.8f, SpriteEffects.None, 0);

            Effect shader_Rot = UCAShaderRegister.PolarDistortShader_Rot.Value;
            shader_Rot.Parameters["uWidthMult"].SetValue(2f);
            shader_Rot.Parameters["uRingMult"].SetValue(4f);
            shader_Rot.Parameters["uYTime"].SetValue(Main.GlobalTimeWrappedHourly * -0.3f);
            shader_Rot.Parameters["uTwist"].SetValue(10f);
            shader_Rot.CurrentTechnique.Passes[0].Apply();

            Main.instance.GraphicsDevice.Textures[1] = UCATextureRegister.BloomRing.Value;

            Texture2D Aura01texture = LAPTextureRegister.Aura_01.Value;
            Vector2 Auraorig = Aura01texture.Size() / 2;
            Main.spriteBatch.Draw(Aura01texture, Projectile.Center - Main.screenPosition, null, Color.DeepSkyBlue * Projectile.Opacity, 0, Auraorig, Projectile.scale * 0.9f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(Aura01texture, Projectile.Center - Main.screenPosition, null, exoBlueColor * Projectile.Opacity, 0, Auraorig, Projectile.scale * 0.9f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.DeepSkyBlue * Projectile.Opacity, 0, Auraorig, Projectile.scale * 0.9f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, exoBlueColor * Projectile.Opacity, 0, Auraorig, Projectile.scale * 0.9f, SpriteEffects.None, 0);

            Vector4 uvfade = new Vector4(0.05f, 0.3f, 0.2f, 0.2f);
            Vector2 uvmult = new Vector2(2f, 1f);
            Vector2 uvadd = new Vector2(-Main.GlobalTimeWrappedHourly, 0f);
            DrawSetting setting = new DrawSetting(LAPTextureRegister.Aura_01.Value, true, -1, LAP.Core.Enums.TrailEffects.None);

            if (!LAPConfig.Instance.PerformanceMode)
            {
                LAPUtilities.ApplyAlphaCut(uvfade, uvadd, uvmult, Color.SkyBlue * Projectile.Opacity);
                LAPContent.DrawTrail(list, rot, Vector2.Zero, Color.White * Projectile.Opacity, 8, setting);
            }

            DrawSetting setting2 = new DrawSetting(LAPTextureRegister.StandardFlow2.Value, true, -1, LAP.Core.Enums.TrailEffects.None);
            Vector4 uvfade2 = new Vector4(0.05f, 0.8f, 0.2f, 0.2f);
            LAPUtilities.ApplyAlphaCut(uvfade2, uvadd, uvmult);

            LAPContent.DrawTrail(list, rot, Vector2.Zero, Color.SkyBlue * Projectile.Opacity, 15, setting2);

            DeepGlow.SubmitCustomGlow(() =>
            {
                LAPUtilities.ReSetToBeginShader();

                LAPUtilities.ApplyAlphaCut(uvfade, uvadd, uvmult, Color.DeepSkyBlue);

                DrawSetting setting3 = new DrawSetting(LAPTextureRegister.Aura_01.Value, true, -1, LAP.Core.Enums.TrailEffects.None);
                LAPContent.DrawTrail(list, rot, Vector2.Zero, Color.Blue * Projectile.Opacity, 12, setting3);

                LAPUtilities.ReSetToEndShader();
            });

            LAPUtilities.ApplyDefaultShader();

            Texture2D core = LAPTextureRegister.OpticalFlaresLine.Value;
            Vector2 coreorig = core.Size() / 2;
            Main.spriteBatch.Draw(core, Projectile.Center - Main.screenPosition, null, Color.DeepSkyBlue * Projectile.Opacity,0, coreorig, Projectile.scale * 0.3f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(core, Projectile.Center - Main.screenPosition, null, Color.White * Projectile.Opacity, 0, coreorig, Projectile.scale * 0.15f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(core, Projectile.Center - Main.screenPosition, null, exoBlueColor * Projectile.Opacity, 0, coreorig, Projectile.scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(core, Projectile.Center - Main.screenPosition, null, exoBlueColor * Projectile.Opacity, 0, coreorig, Projectile.scale, SpriteEffects.None, 0);

            LAPUtilities.ReSetToEndShader();
            return false;
        }
    }
}
