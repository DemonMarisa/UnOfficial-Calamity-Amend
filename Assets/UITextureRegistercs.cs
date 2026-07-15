using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

namespace UCA.Assets
{
    public partial class UCATextureRegister : ModSystem
    {
        public static Asset<Texture2D> VividClarityAttack { get; private set; }
        public static Asset<Texture2D> VividClarityDefense { get; private set; }
        public static Asset<Texture2D> VividClaritySupport { get; private set; }
        public static void LoadUITexture()
        { 
            VividClarityAttack = Request<Texture2D>($"UCA/Assets/Textures/UIs/VividClarityAttack");
            VividClarityDefense = Request<Texture2D>($"UCA/Assets/Textures/UIs/VividClarityDefense");
            VividClaritySupport = Request<Texture2D>($"UCA/Assets/Textures/UIs/VividClaritySupport");
        }
        public static void UnLoadUITexture()
        {
            VividClarityAttack = null;
            VividClarityDefense = null;
            VividClaritySupport = null;
        }
    }
}
