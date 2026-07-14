using LAP.Assets.TextureRegister;
using LAP.Core.Enums;
using LAP.Core.Graphics.Primitives.Trail;
using LAP.Core.Graphics.VFX;
using LAP.Core.SystemsLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using UCA.Assets.Effects;

namespace UCA.Content.VFXs
{
    public class TerraVine : VFXBehavior
    {
        public static VFXInstance Spawn(Vector2 position, Vector2 velocity, Color color, int filp, float LineHight, float VineHeight, float VineWidth)
        {
            int VFXtype = LAPContent.VFXType<TerraVine>();
            VFXInstance vfx = LAPContent.SpawnVFX(VFXtype, position, velocity, color, 0, 1f, LineHight, VineHeight, VineWidth);
            vfx.AiInt[0] = filp;
            return vfx;
        }
        public static int MaxTime = 125;
        public static int FadeOutTime = 35;
        public static int ForwardTime => MaxTime - FadeOutTime;

        public DrawSetting drawSetting;
        public float Progress;
        public bool FadeOut;
        public ref int Filp => ref VFXInstance.AiInt[0];
        public ref float HeightMult => ref VFXInstance.AiFloat[0];
        public ref float VineHeightMult => ref VFXInstance.AiFloat[1];
        public ref float VineWidthMult => ref VFXInstance.AiFloat[2];
        public override void OnSpawn()
        {
            MaxTime = 150;
            FadeOutTime = 60;
            VFXInstance.Lifetime = MaxTime;
            VFXInstance.ExtraUpdate = 4;
            drawSetting = new DrawSetting(LAPTextureRegister.Wood.Value, false, -1, TrailEffects.None);
        }
        public override void Update()
        {
            VFXInstance.Rotation = VFXInstance.Velocity.ToRotation();
            if (!FadeOut)
            {
                float progress2 = Progress * VineWidthMult;
                float yAdd = MathF.Sin(progress2 / 50f);
                Vector2 finalPos = VFXInstance.Position + new Vector2(0, yAdd * VineHeightMult * Filp).RotatedBy(VFXInstance.Rotation);
                VFXInstance.OldPos.Add(finalPos);

                if (VFXInstance.OldPos.Count > ForwardTime)
                {
                    VFXInstance.Time = ForwardTime;
                    FadeOut = true;
                    VFXInstance.ExtraUpdate = 0;
                }
            }
            else
            {
                float progress = (VFXInstance.Time - ForwardTime) / (float)FadeOutTime;
                VFXInstance.Opacity = MathHelper.Lerp(1f, 0f, progress) ;
            }
            Progress += VFXInstance.Velocity.Length();
        }
        public override void Draw()
        {
            float opacity = VFXInstance.Opacity * 0.55f;
            Effect shader = UCAShaderRegister.TerraRayVinesShader.Value;
            shader.Parameters["progress"].SetValue(opacity);
            shader.Parameters["UVMult"].SetValue(new Vector2(0.1f, 0.3f));
            shader.Parameters["UVAdd"].SetValue(new Vector2(-Main.GlobalTimeWrappedHourly * 0.01f, 1));
            shader.CurrentTechnique.Passes[0].Apply();

            float fadeOut = 0;
            List<float> hight = [];
            for (int i = 0; i < VFXInstance.OldPos.Count; i++)
            {
                // 淡入
                float YScale = i / 10f;
                // 淡出
                if (i > VFXInstance.OldPos.Count - 10f)
                {
                    fadeOut++;
                    YScale = 1 - (fadeOut / 10f);
                }
                if (YScale > 1)
                    YScale = 1;
                hight.Add(YScale * HeightMult);
            }

            LAPContent.AutoRotTrail(VFXInstance.OldPos, Vector2.Zero, VFXInstance.DrawColor, hight, drawSetting);
        }
    }
}
