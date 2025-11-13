using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public void LoadWeaponsTexture()
        {
            MainElementalFragments = ModContent.Request<Texture2D>($"{MagicWeaponsTexturePath}/Ray/MainElementalFragments");
            AuxElementalFragments = ModContent.Request<Texture2D>($"{MagicWeaponsTexturePath}/Ray/AuxElementalFragments");
            ElementalRayBase = ModContent.Request<Texture2D>($"{MagicWeaponsTexturePath}/Ray/ElementalRayBase");
            ElementalRayNebula = ModContent.Request<Texture2D>($"{MagicWeaponsTexturePath}/Ray/ElementalRayNebula");
            ElementalRayMisc = ModContent.Request<Texture2D>($"{MagicWeaponsTexturePath}/Ray/ElementalRayMisc");
            ElementalRaySolor = ModContent.Request<Texture2D>($"{MagicWeaponsTexturePath}/Ray/ElementalRaySolor");
            ElementalRayStarDust = ModContent.Request<Texture2D>($"{MagicWeaponsTexturePath}/Ray/ElementalRayStarDust");
            ElementalRayVortex = ModContent.Request<Texture2D>($"{MagicWeaponsTexturePath}/Ray/ElementalRayVortex");
            ElementalRayOutLine = ModContent.Request<Texture2D>($"{MagicWeaponsTexturePath}/Ray/ElementalRayOutLine");
            ShadowBoltStaffLong = ModContent.Request<Texture2D>($"{MagicWeaponsTexturePath}/Ray/ShadowBoltStaffLong");
            ShadowBoltStaffOrb = ModContent.Request<Texture2D>($"{MagicWeaponsTexturePath}/Ray/ShadowBoltStaffOrb");
            ShadowBoltStaffOverLay = ModContent.Request<Texture2D>($"{MagicWeaponsTexturePath}/Ray/ShadowBoltStaffOverLay");
        }
        public void UnLoadWeaponsTexture()
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
        }
    }
}
