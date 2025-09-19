
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
        public static void LoadExtraTextures()
        {
            Noise = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/Noise");
            ShabowBackGround = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/Shadow");
            ShadowNebulaBackGround = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/ShadowNebula");
            NightRayShield = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/NightShield");
            SpreadLine = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/SpreadLine");
        }
        public static void UnLoadExtraTextures()
        {
            Noise = null;
            ShabowBackGround = null;
            ShadowNebulaBackGround = null;
            NightRayShield = null;
            SpreadLine = null;
        }
    }
}
