using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Diagnostics.Metrics;
using Terraria.ModLoader;

namespace UCA.Assets
{
    public partial class UCATextureRegister : ModSystem
    {
        public static string InvisibleTexturePath => "UCA/Assets/Textures/InvisibleProj";
        public static Asset<Texture2D> WhiteCube { get; private set; }
        public static Asset<Texture2D> WhiteCircle { get; private set; }
        public static Asset<Texture2D> InvisibleProj { get; private set; }
        public static Asset<Texture2D> SmallIcon { get; private set; }
        public override void Load()
        {
            WhiteCube = ModContent.Request<Texture2D>($"UCA/Assets/Textures/WhiteCube");
            WhiteCircle = ModContent.Request<Texture2D>($"UCA/Assets/Textures/WhiteCircle");
            InvisibleProj = ModContent.Request<Texture2D>($"UCA/Assets/Textures/InvisibleProj");
            SmallIcon = ModContent.Request<Texture2D>($"UCA/icon_small");
            LoadExtraTextures();
            LoadParticileTextures();
            LoadENDERTextures();
        }

        public override void Unload()
        {
            WhiteCube = null;
            WhiteCircle = null;
            InvisibleProj = null;
            SmallIcon = null;
            UnLoadExtraTextures();
            UnLoadParticileTextures();
            UnLoadENDERTextures();
        }
    }
}
