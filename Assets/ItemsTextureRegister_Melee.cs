using LAP.Assets.TextureRegister;
using Terraria.ModLoader;

namespace UCA.Assets
{
    public partial class UCATextureRegister : ModSystem
    {
        public static string MeleeWeaponsTexturePath => "UCA/Assets/Textures/Items/Weapons/Melee";
        public static Tex2DWithPath StormRulerAlt { get; private set; }
        public static void LoadMeleeWeaponsTexture()
        {
            StormRulerAlt = new Tex2DWithPath($"{MeleeWeaponsTexturePath}/GreatSwords/StormRulerAlt");
        }
        public static void UnLoadMeleeWeaponsTexture()
        {
            StormRulerAlt = null;
        }
    }
}
