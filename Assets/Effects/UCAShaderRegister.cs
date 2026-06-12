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
            RegisterMiscShader(TerraRayVinesShader, "Pass0", nameof(TerraRayVinesShader));

            TerrarRayLaser = LoadShader("TerrarRayLaser");
            RegisterMiscShader(TerrarRayLaser, "UCATerrarRayLaserPass", nameof(TerrarRayLaser));

            SolarBladeShader = LoadShader("SolarBladeShader");
            RegisterMiscShader(SolarBladeShader, "UCASolarBladePass", nameof(SolarBladeShader));

            SolarBlastShader = LoadShader("SolarBlastShader");
            RegisterMiscShader(SolarBlastShader, "UCASolarBlastPass", nameof(SolarBlastShader));
            
            FlowWithAShader = LoadShader("FlowWithAShader");
            RegisterMiscShader(FlowWithAShader, "UCAFlowWithAPass", nameof(FlowWithAShader));

            PolarDistortShader = LoadShader("PolarDistortShader");
            RegisterMiscShader(PolarDistortShader, "UCAPolarDistortPass", nameof(PolarDistortShader));

            PolarDistortShaderWithR = LoadShader("PolarDistortShaderWithR");
            RegisterMiscShader(PolarDistortShaderWithR, "UCAPolarDistortPass", nameof(PolarDistortShaderWithR));

            SoulGreatSwordFlowShader = LoadShader("SoulGreatSwordFlowShader");
            RegisterMiscShader(SoulGreatSwordFlowShader, "UCASGSShaderPass", nameof(SoulGreatSwordFlowShader));
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
        }
        public static void RegisterMiscShader(Asset<Effect> shader, string passName, string registrationName)
        {
            Asset<Effect> shaderPointer = shader;
            MiscShaderData passParamRegistration = new(shaderPointer, passName);
            GameShaders.Misc[$"{ShaderPrefix}{registrationName}"] = passParamRegistration;
        }
    }
}
