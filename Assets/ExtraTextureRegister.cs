
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

namespace UCA.Assets
{
    public partial class UCATextureRegister : ModSystem
    {
        public static Asset<Texture2D> ShabowBackGround { get; private set; }
        public static Asset<Texture2D> ShadowNebulaBackGround { get; private set; }
        public static Asset<Texture2D> Noise { get; private set; }
        public static Asset<Texture2D> NightRayShield { get; private set; }
        public static Asset<Texture2D> SpreadLine { get; private set; }
        public static Asset<Texture2D> CarnageBackGround { get; private set; }
        public static Asset<Texture2D> CarnageStabs { get; private set; }
        public static Asset<Texture2D> TerrarRayFlow { get; private set; }
        public static Asset<Texture2D> Wood { get; private set; }
        public static Asset<Texture2D> TerraMatrix { get; private set; }
        public static void LoadExtraTextures()
        {
            Noise = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/Noise");
            ShabowBackGround = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/Shadow");
            ShadowNebulaBackGround = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/ShadowNebula");
            NightRayShield = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/NightShield");
            SpreadLine = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/SpreadLine");
            CarnageBackGround = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/Carnage");
            CarnageStabs = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/CarnageStabs");
            TerrarRayFlow = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/TerraRayFlow");
            Wood = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/Wood");
            TerraMatrix = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/TerraMatrix");
        }
        public static void UnLoadExtraTextures()
        {
            Noise = null;
            ShabowBackGround = null;
            ShadowNebulaBackGround = null;
            NightRayShield = null;
            SpreadLine = null;
            CarnageBackGround = null;
            CarnageStabs = null;
            TerrarRayFlow = null;
            Wood = null;
            TerraMatrix = null;
        }
    }
}
