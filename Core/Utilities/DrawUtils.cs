using Microsoft.Xna.Framework;
using Terraria;
using UCA.Assets.Effects;
using UCA.Content.Particiles;

namespace UCA.Core.Utilities
{
    public static partial class UCAUtilities
    {
        public static void GenStarLine(Vector2 BeginPos, Vector2 EndPos, float GenStep, Color color)
        {
            for (int i = 0; i < GenStep; i++)
            {
                Vector2 SpawnVector = Vector2.Lerp(BeginPos, EndPos, i / GenStep);
                new MediumGlowBall(SpawnVector, Vector2.Zero, color, 60, 0, 1f, 0.1f, 0).Spawn();
            }
        }
        public static void ApplySolarBladeShader(Color beginColor, Color endColor, float uIntensity = 0.15f, bool useColor = true, float Opacity = 0.5f)
        {
            UCAShaderRegister.SolarBladeShader.Parameters["uTime"].SetValue(-Main.GlobalTimeWrappedHourly);
            UCAShaderRegister.SolarBladeShader.Parameters["uIntensity"].SetValue(uIntensity);
            UCAShaderRegister.SolarBladeShader.Parameters["ubeginColor"].SetValue(beginColor.ToVector4());
            UCAShaderRegister.SolarBladeShader.Parameters["uendColor"].SetValue(endColor.ToVector4());
            UCAShaderRegister.SolarBladeShader.Parameters["UseColor"].SetValue(useColor);
            UCAShaderRegister.SolarBladeShader.Parameters["Opacity"].SetValue(Opacity);
            UCAShaderRegister.SolarBladeShader.CurrentTechnique.Passes[0].Apply();
        }
    }
}
