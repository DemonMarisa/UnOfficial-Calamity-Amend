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

namespace UCA.Content.VFXs.ExoBlasts
{
    public class ExoTrail : VFXBehavior
    {
        public Vector2 TargetCenter => VFXInstance.AiVector2[0];
        public float Rotspeed => VFXInstance.AiFloat[0];
        public float DelDistance => VFXInstance.AiFloat[1];
        public bool fadeout;
        public float UpdateProgress = 0f;
        public static void Spawn(Vector2 pos, Vector2 vel, Vector2 targetCenter, Color color, float rotspeed = 0.25f, float DelDistance = 20f)
        {
            int VFXtype = LAPContent.VFXType<ExoTrail>();
            VFXInstance vfx = LAPContent.SpawnVFX(VFXtype, pos, vel, color, 0, 0, rotspeed, DelDistance);
            vfx.AiVector2[0] = targetCenter;
            vfx.Rotation = vel.ToRotation();
        }
        public override void OnSpawn()
        {
            VFXInstance.Lifetime = 120;
            VFXInstance.ExtraUpdate = 1;
        }
        public override void Update()
        {
            UpdateProgress += 0.25f;
            if (UpdateProgress >= 1f)
                VFXInstance.ExtraUpdate = 1;
            else
                VFXInstance.ExtraUpdate = 0;

            if (!fadeout)
            {
                float targetrot = LAPUtilities.GetVector2(VFXInstance.Position, TargetCenter).ToRotation();
                VFXInstance.Rotation = VFXInstance.Rotation.AngleTowards(targetrot, Rotspeed);
                VFXInstance.Velocity = VFXInstance.Rotation.ToRotationVector2() * 18f;
            }

            VFXInstance.OldRot.Add(VFXInstance.Rotation);
            if (VFXInstance.OldRot.Count > 20)
                VFXInstance.OldRot.RemoveAt(0);

            VFXInstance.OldPos.Add(VFXInstance.Position);
            if (VFXInstance.OldPos.Count > 20)
                VFXInstance.OldPos.RemoveAt(0);

            if (VFXInstance.Position.Distance(TargetCenter) < DelDistance && !fadeout)
            {
                VFXInstance.Velocity = Vector2.Zero;
                fadeout = true;
                VFXInstance.Lifetime = 30;
                VFXInstance.Time = 0;
                for (int i = 2; i < VFXInstance.OldPos.Count; i += 2)
                {
                    Vector2 pos = VFXInstance.OldPos[i];
                    ParticlePreset.NewTOFL(pos, Vector2.Zero, VFXInstance.DrawColor, 45, 0.1f, 1f);
                }
            }
            if (fadeout)
            {
                VFXInstance.Opacity = MathHelper.Lerp(1f, 0f, VFXInstance.LifetimeRatio);
                VFXInstance.OldRot.RemoveAt(0);
                VFXInstance.OldPos.RemoveAt(0);
            }
        }
        public override void Draw()
        {
            Texture2D Aura01 = LAPTextureRegister.Aura_01.Value;
            DrawSetting drawSetting = new(Aura01, true, 2, 0);

            Vector2 uvAdd = new(Main.GlobalTimeWrappedHourly * 0.4f, 0);
            Vector2 uvMult = new(2f, 1f);

            Texture2D Flow1 = LAPTextureRegister.StandardFlow3.Value;
            Vector4 edge2 = new(0.4f, 0.2f, 0.1f, 0.1f);
            DrawSetting drawSetting2 = new(Flow1, true, 2, 0);
            LAPUtilities.ApplyAlphaCut(edge2, uvAdd, uvMult);

            LAPContent.DrawTrail(VFXInstance.OldPos, VFXInstance.OldRot, Vector2.Zero, VFXInstance.DrawColor * 0.9f * VFXInstance.Opacity, 40f, drawSetting2);
            LAPContent.DrawTrail(VFXInstance.OldPos, VFXInstance.OldRot, Vector2.Zero, Color.White * VFXInstance.Opacity, 10f, drawSetting2);

            Texture2D hood = LAPTextureRegister.HoodTrail.Value;
            DrawSetting drawSetting3 = new(hood, true, 2, TrailEffects.None);
            List<float> hoodrot = [];
            List<Vector2> hoodpos = [];
            for (int i = VFXInstance.OldPos.Count - 1; i >= 2; i--)
            {
                hoodpos.Add(VFXInstance.OldPos[i]);
                hoodrot.Add(VFXInstance.OldRot[i]);
            }
            LAPUtilities.ApplyDefaultShader();

            LAPContent.DrawTrail(hoodpos, hoodrot, Vector2.Zero, Color.White, 10f, drawSetting3);

            DeepGlow.SubmitCustomGlow(() =>
            {
                LAPUtilities.ReSetToBeginShader();

                LAPContent.DrawTrail(hoodpos, hoodrot, Vector2.Zero, VFXInstance.DrawColor * 0.8f * VFXInstance.Opacity, 25f, drawSetting3);

                LAPUtilities.ReSetToEndShader();
            }, DrawLayer.EndCapture);
        }
    }
}
