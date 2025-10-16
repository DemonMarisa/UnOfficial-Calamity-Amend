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
        public void LoadWeaponsTexture()
        {
            MainElementalFragments = ModContent.Request<Texture2D>($"{MagicWeaponsTexturePath}/Ray/MainElementalFragments");
            AuxElementalFragments = ModContent.Request<Texture2D>($"{MagicWeaponsTexturePath}/Ray/AuxElementalFragments");
            ElementalRayBase = ModContent.Request<Texture2D>($"{MagicWeaponsTexturePath}/Ray/ElementalRayBase");
        }
        public void UnLoadWeaponsTexture()
        {
            MainElementalFragments = null;
            AuxElementalFragments = null;
            ElementalRayBase = null;
        }
    }
}
