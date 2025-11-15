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
        public static Asset<Texture2D> Lozenge { get; private set; }
        public static Asset<Texture2D> GlowBall { get; private set; }
        public static Asset<Texture2D> Star { get; private set; }
        public static Asset<Texture2D> Star_Big { get; private set; }
        public static Asset<Texture2D> Star_Glow { get; private set; }
        public static Asset<Texture2D> Star_Big_Glow { get; private set; }
        public static Asset<Texture2D> Particle_ShinyOrb { get; private set; }
        private static string P_Path = "UCA/Assets/ParticilesTextures/"; 
        public static void LoadParticileTextures()
        {
            BloodDrop = ModContent.Request<Texture2D>($"UCA/Assets/ParticilesTextures/BloodDrop");
            BloodStain = ModContent.Request<Texture2D>($"UCA/Assets/ParticilesTextures/BloodStain");
            BloodSplash = ModContent.Request<Texture2D>($"UCA/Assets/ParticilesTextures/BloodSplash");
            MediumGlowBall = ModContent.Request<Texture2D>($"UCA/Assets/ParticilesTextures/MediumGlowBall");
            SmallGlowBall = ModContent.Request<Texture2D>($"UCA/Assets/ParticilesTextures/SmallGlowBall");
            Line = ModContent.Request<Texture2D>($"UCA/Assets/ParticilesTextures/Line");
            Lozenge = ModContent.Request<Texture2D>($"UCA/Assets/ParticilesTextures/Lozenge");
            GlowBall = ModContent.Request<Texture2D>($"UCA/Assets/ParticilesTextures/GlowBall");
            Star = ModContent.Request<Texture2D>($"UCA/Assets/ParticilesTextures/Star");
            Star_Big = ModContent.Request<Texture2D>($"UCA/Assets/ParticilesTextures/Star_Big");
            Star_Glow = ModContent.Request<Texture2D>($"UCA/Assets/ParticilesTextures/Star_Glow");
            Star_Big_Glow = ModContent.Request<Texture2D>($"UCA/Assets/ParticilesTextures/Star_Big_Glow");
            Particle_ShinyOrb = ModContent.Request<Texture2D>(P_Path + nameof(Particle_ShinyOrb));
        }
        public static void UnLoadParticileTextures()
        {
            BloodDrop = null;
            BloodStain = null;
            BloodSplash = null;
            SmallGlowBall = null;
            MediumGlowBall = null;
            Line = null;
            Lozenge = null;
            GlowBall = null;
            Star = null;
            Star_Big = null;
            Star_Glow = null;
            Star_Big_Glow = null;

            Particle_ShinyOrb = null;
        }
    }
}
