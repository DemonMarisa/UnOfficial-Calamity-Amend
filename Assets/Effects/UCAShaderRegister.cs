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
        public static Effect MetaballShader;
        public static Effect EdgeMeltsShader;
        public static Effect TerraRayVinesShader;
        public static Effect TerrarRayLaser;
        public static Effect SolarBladeShader;
        public static Effect SolarBlastShader;
        public static Effect StandardFlowShader; 
        public static Effect FlowWithAShader;
        public static Effect PolarDistortShader;
        public static Effect PolarDistortShaderWithR;
        public static Effect SoulGreatSwordFlowShader;

        public static Effect VolcanoEruptingShader;
        public override void Load()
        {
            if (Main.dedServ)
                return;

            static Effect LoadShader(string path)
            {
                return ModContent.Request<Effect>($"{ShaderPath}{path}", AssetRequestMode.ImmediateLoad).Value;
            }

            MetaballShader = LoadShader(nameof(MetaballShader));
            RegisterMiscShader(MetaballShader, "UCAMetalBallPass", nameof(MetaballShader));

            EdgeMeltsShader = LoadShader("EdgeMeltsShader");
            RegisterMiscShader(EdgeMeltsShader, "UCAEdgeMeltsPass", nameof(EdgeMeltsShader));

            TerraRayVinesShader = LoadShader("TerraRayVinesShader");
            RegisterMiscShader(TerraRayVinesShader, "UCATerraRayVinesPass", nameof(TerraRayVinesShader));

            TerrarRayLaser = LoadShader("TerrarRayLaser");
            RegisterMiscShader(TerrarRayLaser, "UCATerrarRayLaserPass", nameof(TerrarRayLaser));

            SolarBladeShader = LoadShader("SolarBladeShader");
            RegisterMiscShader(SolarBladeShader, "UCASolarBladePass", nameof(SolarBladeShader));

            StandardFlowShader = LoadShader("StandardFlowShader");
            RegisterMiscShader(StandardFlowShader, "UCAStandardFlowPass", nameof(StandardFlowShader));

            SolarBlastShader = LoadShader("SolarBlastShader");
            RegisterMiscShader(SolarBlastShader, "UCASolarBlastPass", nameof(SolarBlastShader));
            
            FlowWithAShader = LoadShader("FlowWithAShader");
            RegisterMiscShader(StandardFlowShader, "UCAFlowWithAPass", nameof(FlowWithAShader));

            PolarDistortShader = LoadShader("PolarDistortShader");
            RegisterMiscShader(PolarDistortShader, "UCAPolarDistortPass", nameof(PolarDistortShader));

            PolarDistortShaderWithR = LoadShader("PolarDistortShaderWithR");
            RegisterMiscShader(PolarDistortShaderWithR, "UCAPolarDistortPass", nameof(PolarDistortShaderWithR));

            SoulGreatSwordFlowShader = LoadShader("SoulGreatSwordFlowShader");
            RegisterMiscShader(SoulGreatSwordFlowShader, "UCASGSShaderPass", nameof(SoulGreatSwordFlowShader));

            VolcanoEruptingShader = LoadShader(nameof(VolcanoEruptingShader));
            RegisterMiscShader(VolcanoEruptingShader, "UCA" + nameof(VolcanoEruptingShader), nameof(VolcanoEruptingShader));
        }

        public static void RegisterMiscShader(Effect shader, string passName, string registrationName)
        {
            Ref<Effect> shaderPointer = new(shader);
            MiscShaderData passParamRegistration = new(shaderPointer, passName);
            GameShaders.Misc[$"{ShaderPrefix}{registrationName}"] = passParamRegistration;
        }
    }
}
