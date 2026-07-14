using LAP.Assets.TextureRegister;
using LAP.Core.Enums;
using LAP.Core.Graphics.DeepGlow;
using LAP.Core.Graphics.Primitives.Trail;
using LAP.Core.Graphics.VFX;
using LAP.Core.SystemsLoader;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using UCA.Assets;

namespace UCA.Content.VFXs
{
    public class SlashTrail : VFXBehavior
    {
        /// <summary>
        /// 从外向内输入数据进行绘制，不在生成时提供数据
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static VFXInstance Spawn(Color color, int AddTime, int FadeOutTime, int MaxDataCount, bool useBloom = false)
        {
            VFXInstance vfx = LAPContent.SpawnVFX(LAPContent.VFXType<SlashTrail>(), Vector2.Zero, Vector2.Zero, color);
            vfx.Lifetime = FadeOutTime;
            vfx.AiInt[0] = MaxDataCount;
            vfx.AiInt[1] = AddTime;
            vfx.AiBool[2] = useBloom;
            return vfx;
        }
        public float MaxDataCount => VFXInstance.AiInt[0];
        public int AddTime => VFXInstance.AiInt[1];
        public int FadeOutTime => VFXInstance.AiInt[2];
        public ref bool CanFadeOpacity => ref VFXInstance.AiBool[0];
        public bool DeleteData => VFXInstance.AiBool[1];
        public bool UseBloom => VFXInstance.AiBool[2];
        public int Time;
        public override void OnSpawn()
        {
            base.OnSpawn();
        }
        public override void Update()
        {
            Time++;
            if (Time > AddTime)
                CanFadeOpacity = true;

            if (VFXInstance.OldRot.Count > MaxDataCount)
                VFXInstance.OldRot.RemoveAt(0);
            if (VFXInstance.OldPos.Count > MaxDataCount)
                VFXInstance.OldPos.RemoveAt(0);
            if (VFXInstance.Oldfloat.Count > MaxDataCount)
                VFXInstance.Oldfloat.RemoveAt(0);
            if (CanFadeOpacity)
            {
                VFXInstance.Opacity = MathHelper.Lerp(1f, 0f, EasingHelper.EaseOutCubic(VFXInstance.LifetimeRatio));
            }
            if (DeleteData)
            {
                VFXInstance.OldRot.RemoveAt(0);
                VFXInstance.OldPos.RemoveAt(0);
                VFXInstance.Oldfloat.RemoveAt(0);
            }
            if (!CanFadeOpacity && !DeleteData)
            {
                VFXInstance.Time = 0;
            }
        }
        public override void Draw()
        {
            // 基础信息
            Vector4 UVFade = new Vector4(0.1f, 0.1f, 0.2f, 0f);
            Vector2 uvFlow = new Vector2(0, 0);
            Vector2 uvMult = new Vector2(1f, 1f);
            LAPUtilities.ApplyAlphaCut(UVFade, uvFlow, uvMult);
            DrawSetting drawSetting = new(LAPTextureRegister.StandardGradient.Value, smoothUV: true, trailEffect: TrailEffects.None, smoothSegments: 2, samplerState : SamplerState.PointWrap);
            DrawSetting AuraTexture = new(LAPTextureRegister.Aura_01.Value, smoothUV: true, trailEffect: TrailEffects.None, smoothSegments: 2, samplerState: SamplerState.PointWrap);
            DrawSetting LineTexture = new(UCATextureRegister.Slash.Value, smoothUV: true, trailEffect: TrailEffects.None, smoothSegments: 2, samplerState: SamplerState.PointWrap);
            // 绘制底色
            LAPContent.DrawTrail(VFXInstance.OldPos, VFXInstance.OldRot, Vector2.Zero, VFXInstance.DrawColor * VFXInstance.Opacity * 0.7f, VFXInstance.Oldfloat, drawSetting);
            // 绘制流光
            Vector4 AuraUVFade = new Vector4(0.1f, 0.1f, 0.3f, 0.3f);
            Vector2 AurauvFlow = new Vector2(Main.GlobalTimeWrappedHourly * -0.2f, 0);
            Vector2 AurauvMult = new Vector2(3f, 3f);
            LAPUtilities.ApplyAlphaCut(AuraUVFade, AurauvFlow, AurauvMult, Color.White);
            LAPContent.DrawTrail(VFXInstance.OldPos, VFXInstance.OldRot, Vector2.Zero, VFXInstance.DrawColor * VFXInstance.Opacity, VFXInstance.Oldfloat, AuraTexture);
            Vector2 LineuvMult = new Vector2(3f, 3f);
            LAPUtilities.ApplyAlphaCut(AuraUVFade, AurauvFlow, LineuvMult, Color.White);
            LAPContent.DrawTrail(VFXInstance.OldPos, VFXInstance.OldRot, Vector2.Zero, VFXInstance.DrawColor * VFXInstance.Opacity, VFXInstance.Oldfloat, LineTexture);
            // 绘制最外围高光
            Vector4 OutLineUVFade = new Vector4(0.1f, 0.1f, 0f, 1.5f);
            Vector2 OutLineuvFlow = new Vector2(Main.GlobalTimeWrappedHourly * -0.2f, 0);
            Vector2 OutLineuvMult = new Vector2(0.5f, 10f);
            LAPUtilities.ApplyAlphaCut(OutLineUVFade, OutLineuvFlow, OutLineuvMult, Color.White);
            LAPContent.DrawTrail(VFXInstance.OldPos, VFXInstance.OldRot, Vector2.Zero, VFXInstance.DrawColor * VFXInstance.Opacity, VFXInstance.Oldfloat, AuraTexture);
            LAPUtilities.ApplyDefaultShader();

            DeepGlow.SubmitCustomGlow(() =>
            {
                LAPUtilities.ReSetToBeginShader();

                // 绘制流光
                LAPUtilities.ApplyAlphaCut(AuraUVFade, AurauvFlow, AurauvMult, Color.White);
                LAPContent.DrawTrail(VFXInstance.OldPos, VFXInstance.OldRot, Vector2.Zero, VFXInstance.DrawColor * VFXInstance.Opacity, VFXInstance.Oldfloat, AuraTexture);
                LAPUtilities.ApplyAlphaCut(AuraUVFade, AurauvFlow, LineuvMult, Color.White);
                LAPContent.DrawTrail(VFXInstance.OldPos, VFXInstance.OldRot, Vector2.Zero, VFXInstance.DrawColor * VFXInstance.Opacity, VFXInstance.Oldfloat, LineTexture);
                // 绘制最外围高光
                LAPUtilities.ApplyAlphaCut(OutLineUVFade, OutLineuvFlow, OutLineuvMult, Color.White);
                LAPContent.DrawTrail(VFXInstance.OldPos, VFXInstance.OldRot, Vector2.Zero, VFXInstance.DrawColor * VFXInstance.Opacity, VFXInstance.Oldfloat, AuraTexture);

                LAPUtilities.ReSetToEndShader();
            });
        }
    }
}
