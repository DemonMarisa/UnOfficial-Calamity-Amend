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
        public static Asset<Texture2D> SmallGlowBall { get; private set; }
        public static Asset<Texture2D> MediumGlowBall { get; private set; }
        public static Asset<Texture2D> Line { get; private set; }
        public static Asset<Texture2D> GlowStar { get; private set; }
        public static Asset<Texture2D> GlowBall { get; private set; }
        public static void LoadParticileTextures()
        {
            BloodDrop = ModContent.Request<Texture2D>($"UCA/Assets/ParticilesTextures/BloodDrop");
            BloodStain = ModContent.Request<Texture2D>($"UCA/Assets/ParticilesTextures/BloodStain");
            BloodSplash = ModContent.Request<Texture2D>($"UCA/Assets/ParticilesTextures/BloodSplash");
            MediumGlowBall = ModContent.Request<Texture2D>($"UCA/Assets/ParticilesTextures/MediumGlowBall");
            SmallGlowBall = ModContent.Request<Texture2D>($"UCA/Assets/ParticilesTextures/SmallGlowBall");
            Line = ModContent.Request<Texture2D>($"UCA/Assets/ParticilesTextures/Line");
            GlowStar = ModContent.Request<Texture2D>($"UCA/Assets/ParticilesTextures/GlowStar");
            GlowBall = ModContent.Request<Texture2D>($"UCA/Assets/ParticilesTextures/GlowBall");
        }
        public static void UnLoadParticileTextures()
        {
            BloodDrop = null;
            BloodStain = null;
            BloodSplash = null;
            SmallGlowBall = null;
            MediumGlowBall = null;
            Line = null;
            GlowStar = null;
            GlowBall = null;
        }
    }
}
