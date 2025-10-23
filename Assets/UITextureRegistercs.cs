using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

namespace UCA.Assets
{
    public partial class UCATextureRegister : ModSystem
    {
        public static Asset<Texture2D> BloomBlackCircle { get; private set; }
        public static Asset<Texture2D> Ring { get; private set; }
        public void LoadUITexture()
        {
            BloomBlackCircle = ModContent.Request<Texture2D>($"UCA/Assets/Textures/UI/BloomBlackCircle");
            Ring = ModContent.Request<Texture2D>($"UCA/Assets/Textures/UI/Ring");
        }
        public void UnLoadUITexture()
        {
            BloomBlackCircle = null;
            Ring = null;
        }
    }
}
