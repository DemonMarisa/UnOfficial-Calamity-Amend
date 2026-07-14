using LAP.Assets.TextureRegister;
using LAP.Core.Graphics.Primitives.Trail;
using LAP.Core.Graphics.VFX;
using LAP.Core.SystemsLoader;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using LAP.Core.Enums;
using Microsoft.Xna.Framework.Graphics;
using LAP.Core.Utilities;
using LAP.Core.Graphics.DeepGlow;

namespace UCA.Content.VFXs
{
    public class AuraTrail : VFXBehavior
    {
        public static DrawSetting setting = new DrawSetting(LAPTextureRegister.Aura_01.Value, false, -1, TrailEffects.None, SamplerState.PointWrap);
        public bool UseBloom => VFXInstance.AiBool[0];
        public int Length => VFXInstance.AiInt[0];
        public float Height => VFXInstance.AiFloat[0];
        public float TurSpeed => VFXInstance.AiFloat[1];
        public float TurAngle => VFXInstance.AiFloat[2];
        public static void Spawn(Vector2 position, Vector2 vel, Color color, int life, float height, bool useBloom = false, float tspeed = 0, float tAngle = 0, int length = 20)
        {
            VFXInstance vfx = LAPContent.SpawnVFX(LAPContent.VFXType<AuraTrail>(), position, vel, color);
            vfx.Lifetime = life;
            vfx.AiInt[0] = length;

            vfx.AiBool[0] = useBloom;

            vfx.AiFloat[0] = height;
            vfx.AiFloat[1] = tspeed;
            vfx.AiFloat[2] = tAngle;
        }
        public float TurToward;
        public int SeedOffset;
        public bool FadeOut;
        public override void OnSpawn()
        {
            VFXInstance.ExtraUpdate = 2;
            SeedOffset = Main.rand.Next(0, 100000);
            TurToward = VFXInstance.Velocity.ToRotation();
        }
        public override void Update()
        {
            if (!FadeOut && VFXInstance.Lifetime - VFXInstance.Time < 15)
            {
                VFXInstance.Lifetime = 15;
                VFXInstance.Time = 0;
                FadeOut = true;
            }

            if (FadeOut)
            {
                VFXInstance.Opacity = MathHelper.Lerp(1f, 0f, VFXInstance.LifetimeRatio);
            }

            if (TurSpeed != 0)
            {
                float sin = (float)Math.Sin(VFXInstance.Time / 12f + SeedOffset) + 1f;
                Vector2 idealVelocity = Vector2.UnitX.RotatedBy(Utils.AngleLerp(TurToward - TurAngle, TurToward + TurAngle, sin * 0.5f)) * TurSpeed;
                float movementInterpolant = MathHelper.Lerp(0.01f, 0.1f, Utils.GetLerpValue(0, VFXInstance.Lifetime, VFXInstance.Time, true));
                VFXInstance.Velocity = Vector2.Lerp(VFXInstance.Velocity, idealVelocity, movementInterpolant);
                VFXInstance.Velocity = VFXInstance.Velocity.SafeNormalize(-Vector2.UnitY) * TurSpeed;
            }

            VFXInstance.Rotation = VFXInstance.Velocity.ToRotation();

            VFXInstance.OldRot.Add(VFXInstance.Rotation);
            if (VFXInstance.OldRot.Count > Length)
                VFXInstance.OldRot.RemoveAt(0);

            VFXInstance.OldPos.Add(VFXInstance.Position);
            if (VFXInstance.OldPos.Count > Length)
                VFXInstance.OldPos.RemoveAt(0);
        }
        public override void OnKill()
        {
        }
        public override void Draw()
        {
            Vector4 uvFade = new Vector4(0.2f, 0.2f, 0.2f, 0.2f);
            Vector2 uvoffset = new Vector2(Main.GlobalTimeWrappedHourly, 0);
            Vector2 uvmult = new Vector2(1.5f, 1);
            LAPUtilities.ApplyAlphaCut(uvFade, uvoffset, uvmult, VFXInstance.DrawColor);
            LAPContent.DrawTrail(VFXInstance.OldPos, VFXInstance.OldRot, Vector2.Zero, VFXInstance.DrawColor * VFXInstance.Opacity, Height, setting);
            if (UseBloom)
            {
                DeepGlow.SubmitCustomGlow(() =>
                {
                    LAPUtilities.ApplyAlphaCut(uvFade, uvoffset, uvmult, VFXInstance.DrawColor);
                    LAPContent.DrawTrail(VFXInstance.OldPos, VFXInstance.OldRot, Vector2.Zero, VFXInstance.DrawColor * VFXInstance.Opacity, Height, setting);
                    LAPUtilities.ApplyDefaultShader();
                });
            }
        }
    }
}
