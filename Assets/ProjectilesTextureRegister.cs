using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

namespace UCA.Assets
{
    public partial class UCATextureRegister : ModSystem
    {
        public static string MagicProjectilesTexturePath => "UCA/Assets/Textures/Projectiles/Magic";
        public static Asset<Texture2D> Crystal { get; private set; }
        public void LoadProjectilesTexture()
        {
            Crystal = ModContent.Request<Texture2D>($"{MagicProjectilesTexturePath}/Crystal");
        }
        public void UnLoadProjectilesTexture()
        {
            Crystal = null;
        }
    }
}
