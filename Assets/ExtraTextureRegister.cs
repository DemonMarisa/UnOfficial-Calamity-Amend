
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace UCA.Assets
{
    public partial class UCATextureRegister : ModSystem
    {
        public static Asset<Texture2D> ShadowNebulaBackGround { get; private set; }
        public static Asset<Texture2D> Noise { get; private set; }
        public static Asset<Texture2D> NightRayShield { get; private set; }
        public static Asset<Texture2D> SpreadLine { get; private set; }
        public static Asset<Texture2D> CarnageBackGround { get; private set; }
        public static Asset<Texture2D> CarnageStabs { get; private set; }
        public static Asset<Texture2D> TerrarRayFlow { get; private set; }
        public static Asset<Texture2D> Wood { get; private set; }
        public static Asset<Texture2D> TerraMatrix { get; private set; }
        public static Asset<Texture2D> FireNoise { get; private set; }
        public static Asset<Texture2D> SolarBlade { get; private set; }
        public static Asset<Texture2D> SolarThinBlade { get; private set; }
        public static Asset<Texture2D> SolarBladeGlowMask { get; private set; }
        public static Asset<Texture2D> ElementalRayFlow { get; private set; }
        public static Asset<Texture2D> CrossGlow { get; private set; }
        public static Asset<Texture2D> NebulaBG { get; private set; }
        public static Asset<Texture2D> StarDustBG { get; private set; }
        public static Asset<Texture2D> ShockWave { get; private set; }
        public static Asset<Texture2D> FireStrike { get; private set; }
        public static string ShockWavePath = "UCA/Assets/ExtraTextures/ShockWave";
        public static Asset<Texture2D> BloomShockwave { get; private set; }
        public static Asset<Texture2D> Lightning { get; private set; }
        public static void LoadExtraTextures()
        {
            Noise = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/Noise");
            ShadowNebulaBackGround = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/MetaBall/ShadowNebula");
            NebulaBG = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/MetaBall/NebulaBG");
            StarDustBG = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/MetaBall/StarDustBG");

            NightRayShield = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/NightShield");
            SpreadLine = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/SpreadLine");
            CarnageBackGround = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/Carnage");
            CarnageStabs = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/CarnageStabs");
            TerrarRayFlow = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/TerraRayFlow");
            Wood = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/Wood");
            TerraMatrix = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/TerraMatrix");
            FireNoise = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/FireNoise");
            SolarBlade = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/SolarBlade");
            SolarThinBlade = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/SolarThinBlade");
            SolarBladeGlowMask = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/SolarBladeGlowMask");
            ElementalRayFlow = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/ElementalRayFlow");
            CrossGlow = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/CrossGlow");
            ShockWave = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/ShockWave");
            FireStrike = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/FireStrike");
            BloomShockwave = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/BloomShockwave");
            Lightning = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/Lightning");
        }
        public static void UnLoadExtraTextures()
        {
            Noise = null;
            ShadowNebulaBackGround = null;
            NebulaBG = null;
            StarDustBG = null;

            NightRayShield = null;
            SpreadLine = null;
            CarnageBackGround = null;
            CarnageStabs = null;
            TerrarRayFlow = null;
            Wood = null;
            TerraMatrix = null;
            FireNoise = null;
            SolarBlade = null;
            SolarThinBlade = null;
            SolarBladeGlowMask = null;
            ElementalRayFlow = null;
            CrossGlow = null;
            ShockWave = null;
            FireStrike = null;
            BloomShockwave = null;
        }
    }
}
