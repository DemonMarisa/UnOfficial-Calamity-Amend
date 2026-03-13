using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Diagnostics.Metrics;
using Terraria.ModLoader;

namespace UCA.Assets
{
    public partial class UCATextureRegister : ModSystem
    {
        public static Asset<Texture2D> WhiteCube { get; private set; }
        public static Asset<Texture2D> WhiteCircle { get; private set; }
        public static Asset<Texture2D> SmallIcon { get; private set; }
        public override void Load()
        {
            WhiteCube = ModContent.Request<Texture2D>($"UCA/Assets/Textures/WhiteCube");
            WhiteCircle = ModContent.Request<Texture2D>($"UCA/Assets/Textures/WhiteCircle");
            SmallIcon = ModContent.Request<Texture2D>($"UCA/icon_small");
            LoadExtraTextures();
            LoadParticileTextures();
            LoadENDERTextures();
            LoadWeaponsTexture();
            LoadProjectilesTexture();
            LoadUITexture();
        }

        public override void Unload()
        {
            WhiteCube = null;
            WhiteCircle = null;
            SmallIcon = null;
            UnLoadExtraTextures();
            UnLoadParticileTextures();
            UnLoadENDERTextures();
            UnLoadWeaponsTexture();
            UnLoadProjectilesTexture();
            UnLoadUITexture();
        }
    }
}
