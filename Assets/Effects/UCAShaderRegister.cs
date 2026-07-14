using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace UCA.Assets.Effects
{
    public class UCAShaderRegister : ModSystem
    {        
        // 当未提供特定着色器时，用作基本绘图的默认值。此着色器仅渲染顶点颜色数据，无需修改。
        private const string ShaderPath = "UCA/Assets/Effects/Overlays/";
        internal const string ShaderPrefix = "UCA:";
        public static Asset<Effect> TerraRayVinesShader { get; private set; }
        public static Asset<Effect> TerrarRayLaser { get; private set; }
        public static Asset<Effect> SolarBladeShader { get; private set; }
        public static Asset<Effect> SolarBlastShader { get; private set; }
        public static Asset<Effect> FlowWithAShader { get; private set; }
        public static Asset<Effect> PolarDistortShader { get; private set; }
        public static Asset<Effect> PolarDistortShaderWithR { get; private set; }
        public static Asset<Effect> PolarDistortShader_Rot { get; private set; }
        public static Asset<Effect> SoulGreatSwordFlowShader { get; private set; }
        public override void Load()
        {
            if (Main.dedServ)
                return;

            static Asset<Effect> LoadShader(string path)
            {
                return ModContent.Request<Effect>($"{ShaderPath}{path}");
            }

            TerraRayVinesShader = LoadShader("TerraRayVinesShader");

            TerrarRayLaser = LoadShader("TerrarRayLaser");

            SolarBladeShader = LoadShader("SolarBladeShader");

            SolarBlastShader = LoadShader("SolarBlastShader");

            FlowWithAShader = LoadShader("FlowWithAShader");

            PolarDistortShader = LoadShader("PolarDistortShader");

            PolarDistortShaderWithR = LoadShader("PolarDistortShaderWithR");

            SoulGreatSwordFlowShader = LoadShader("SoulGreatSwordFlowShader");

            PolarDistortShader_Rot = LoadShader("PolarDistortShader_Rot");
        }
        public override void Unload()
        {
            TerraRayVinesShader = null;
            TerrarRayLaser = null;
            SolarBladeShader = null;
            SolarBlastShader = null;
            FlowWithAShader = null;
            PolarDistortShader = null;
            PolarDistortShaderWithR = null;
            SoulGreatSwordFlowShader = null;
            PolarDistortShader_Rot = null;
        }
    }
}
