using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

namespace UCA.Assets
{
    public partial class UCATextureRegister : ModSystem
    {
        public static string InvisibleTexturePath => "UCA/Assets/Textures/InvisibleProj";
        public static Asset<Texture2D> WhiteCube { get; private set; }
        public static Asset<Texture2D> WhiteCircle { get; private set; }
        public static Asset<Texture2D> InvisibleProj { get; private set; }
        public override void Load()
        {
            WhiteCube = ModContent.Request<Texture2D>($"UCA/Assets/Textures/WhiteCube");
            WhiteCircle = ModContent.Request<Texture2D>($"UCA/Assets/Textures/WhiteCircle");
            InvisibleProj = ModContent.Request<Texture2D>($"UCA/Assets/Textures/InvisibleProj");
            LoadExtraTextures();
            LoadParticileTextures();
            LoadENDERTextures();
        }

        public override void Unload()
        {
            WhiteCube = null;
            WhiteCircle = null;
            InvisibleProj = null;
            UnLoadExtraTextures();
            UnLoadParticileTextures();
            UnLoadENDERTextures();
        }
    }
}
