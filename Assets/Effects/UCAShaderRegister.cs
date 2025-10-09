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
        public static Effect ColorShader;
        public static Effect TerraRayVinesShader;
        public static Effect TerrarRayLaser;
        public override void Load()
        {
            if (Main.dedServ)
                return;

            static Effect LoadShader(string path)
            {
                return ModContent.Request<Effect>($"{ShaderPath}{path}", AssetRequestMode.ImmediateLoad).Value;
            }

            MetaballShader = LoadShader("MetaBallShader");
            RegisterMiscShader(MetaballShader, "UCAMetalBallPass", "MetaBallShader");

            EdgeMeltsShader = LoadShader("EdgeMeltsShader");
            RegisterMiscShader(EdgeMeltsShader, "UCAEdgeMeltsPass", "EdgeMeltsShader");

            ColorShader = LoadShader("ColorShader");
            RegisterMiscShader(ColorShader, "UCAColorPass", "ColorShader");

            TerraRayVinesShader = LoadShader("TerraRayVinesShader");
            RegisterMiscShader(TerraRayVinesShader, "UCATerraRayVinesPass", "TerraRayVinesShader");

            TerrarRayLaser = LoadShader("TerrarRayLaser");
            RegisterMiscShader(TerrarRayLaser, "UCATerrarRayLaserPass", "TerrarRayLaser");
        }

        public static void RegisterMiscShader(Effect shader, string passName, string registrationName)
        {
            Ref<Effect> shaderPointer = new(shader);
            MiscShaderData passParamRegistration = new(shaderPointer, passName);
            GameShaders.Misc[$"{ShaderPrefix}{registrationName}"] = passParamRegistration;
        }
    }
}
