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
        public static Asset<Texture2D> BloodDrop { get; private set; }
        public static Asset<Texture2D> BloodStain { get; private set; }
        public static Asset<Texture2D> BloodSplash { get; private set; }
        public static void LoadParticileTextures()
        {
            BloodDrop = ModContent.Request<Texture2D>($"UCA/Assets/ParticilesTextures/BloodDrop");
            BloodStain = ModContent.Request<Texture2D>($"UCA/Assets/ParticilesTextures/BloodStain");
            BloodSplash = ModContent.Request<Texture2D>($"UCA/Assets/ParticilesTextures/BloodSplash");
        }
        public static void UnLoadParticileTextures()
        {
            BloodDrop = null;
            BloodStain = null;
            BloodSplash = null;
        }
    }
}
