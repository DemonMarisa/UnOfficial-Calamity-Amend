using LAP.Assets.TextureRegister;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

namespace UCA.Assets
{
    public partial class UCATextureRegister : ModSystem
    {
        public static string MagicWeaponsTexturePath => "UCA/Assets/Textures/Items/Weapons/Magic";
        public static Asset<Texture2D> MainElementalFragments { get; private set; }
        public static Asset<Texture2D> AuxElementalFragments { get; private set; }
        public static Asset<Texture2D> ElementalRayBase { get; private set; }
        public static Asset<Texture2D> ElementalRayNebula { get; private set; }
        public static Asset<Texture2D> ElementalRayMisc { get; private set; }
        public static Asset<Texture2D> ElementalRaySolor { get; private set; }
        public static Asset<Texture2D> ElementalRayStarDust { get; private set; }
        public static Asset<Texture2D> ElementalRayVortex { get; private set; }
        public static Asset<Texture2D> ElementalRayOutLine { get; private set; }
        public static Asset<Texture2D> ShadowBoltStaffLong { get; private set; }
        public static Asset<Texture2D> ShadowBoltStaffOrb { get; private set; }
        public static Asset<Texture2D> ShadowBoltStaffOverLay { get; private set; }
        public static void LoadWeaponsTexture()
        {
            MainElementalFragments = Request<Texture2D>($"{MagicWeaponsTexturePath}/Ray/MainElementalFragments");
            AuxElementalFragments = Request<Texture2D>($"{MagicWeaponsTexturePath}/Ray/AuxElementalFragments");
            ElementalRayBase = Request<Texture2D>($"{MagicWeaponsTexturePath}/Ray/ElementalRayBase");
            ElementalRayNebula = Request<Texture2D>($"{MagicWeaponsTexturePath}/Ray/ElementalRayNebula");
            ElementalRayMisc = Request<Texture2D>($"{MagicWeaponsTexturePath}/Ray/ElementalRayMisc");
            ElementalRaySolor = Request<Texture2D>($"{MagicWeaponsTexturePath}/Ray/ElementalRaySolor");
            ElementalRayStarDust = Request<Texture2D>($"{MagicWeaponsTexturePath}/Ray/ElementalRayStarDust");
            ElementalRayVortex = Request<Texture2D>($"{MagicWeaponsTexturePath}/Ray/ElementalRayVortex");
            ElementalRayOutLine = Request<Texture2D>($"{MagicWeaponsTexturePath}/Ray/ElementalRayOutLine");
            ShadowBoltStaffLong = Request<Texture2D>($"{MagicWeaponsTexturePath}/Ray/ShadowBoltStaffLong");
            ShadowBoltStaffOrb = Request<Texture2D>($"{MagicWeaponsTexturePath}/Ray/ShadowBoltStaffOrb");
            ShadowBoltStaffOverLay = Request<Texture2D>($"{MagicWeaponsTexturePath}/Ray/ShadowBoltStaffOverLay");
            LoadMeleeWeaponsTexture();
        }
        public static void UnLoadWeaponsTexture()
        {
            MainElementalFragments = null;
            AuxElementalFragments = null;
            ElementalRayBase = null;
            ElementalRayNebula = null;
            ElementalRayMisc = null;
            ElementalRaySolor = null;
            ElementalRayStarDust = null;
            ElementalRayVortex = null;
            ElementalRayOutLine = null;
            ShadowBoltStaffLong = null;
            ShadowBoltStaffOrb = null;
            ShadowBoltStaffOverLay = null;
            UnLoadMeleeWeaponsTexture();
        }
    }
}
