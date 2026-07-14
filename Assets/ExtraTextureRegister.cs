using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

namespace UCA.Assets
{
    public partial class UCATextureRegister : ModSystem
    {
        public static Asset<Texture2D> NightRayShield { get; private set; }
        public static Asset<Texture2D> SpreadLine { get; private set; }
        public static Asset<Texture2D> CarnageBackGround { get; private set; }
        public static Asset<Texture2D> CarnageStabs { get; private set; }
        public static Asset<Texture2D> TerrarRayFlow { get; private set; }
        public static Asset<Texture2D> TerraMatrix { get; private set; }
        public static Asset<Texture2D> FireNoise { get; private set; }
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
        public static Asset<Texture2D> FusableBall { get; private set; }
        public static Asset<Texture2D> BloomRing { get; private set; }
        public static Asset<Texture2D> BloomLine { get; private set; }
        public static Asset<Texture2D> SolarBladeGlowMask_Blue { get; private set; }
        public static Asset<Texture2D> LaserHighContrast { get; private set; }
        public static Asset<Texture2D> CosmicBG { get; private set; }
        public static Asset<Texture2D> SlashLine01 { get; private set; }
        public static Asset<Texture2D> SlashLine02 { get; private set; }
        public static Asset<Texture2D> SlashLine03 { get; private set; }
        public static Asset<Texture2D> SoulGreatSword { get; private set; }
        public static Asset<Texture2D> HarshNoise { get; private set; }
        public static Asset<Texture2D> TechCircle { get; private set; }
        public static Asset<Texture2D> VividClarityBlade { get; private set; }
        public static void LoadExtraTextures()
        {
            NebulaBG = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/MetaBall/NebulaBG");
            StarDustBG = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/MetaBall/StarDustBG");
            CosmicBG = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/MetaBall/CosmicBG");

            NightRayShield = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/NightShield");
            SpreadLine = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/SpreadLine");
            CarnageBackGround = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/Carnage");
            CarnageStabs = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/CarnageStabs");
            TerrarRayFlow = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/TerraRayFlow");
            TerraMatrix = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/TerraMatrix");
            FireNoise = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/FireNoise");
            SolarThinBlade = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/SolarThinBlade");
            SolarBladeGlowMask = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/SolarBladeGlowMask");
            ElementalRayFlow = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/ElementalRayFlow");
            CrossGlow = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/CrossGlow");
            ShockWave = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/ShockWave");
            FireStrike = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/FireStrike");
            BloomShockwave = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/BloomShockwave");
            Lightning = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/Lightning");
            FusableBall = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/FusableBall");
            BloomRing = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/BloomRing");
            BloomLine = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/BloomLine");
            SolarBladeGlowMask_Blue = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/SolarBladeGlowMask_Blue");
            LaserHighContrast = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/LaserHighContrast");
            SlashLine01 = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/SlashLine01");
            SlashLine02 = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/SlashLine02");
            SlashLine03 = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/SlashLine03");
            SoulGreatSword = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/SoulGreatSword");
            HarshNoise = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/HarshNoise");

            TechCircle = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/TechCircle");
            VividClarityBlade = ModContent.Request<Texture2D>($"UCA/Assets/ExtraTextures/VividClarityBlade");
        }
        public static void UnLoadExtraTextures()
        {
            NebulaBG = null;
            StarDustBG = null;

            NightRayShield = null;
            SpreadLine = null;
            CarnageBackGround = null;
            CarnageStabs = null;
            TerrarRayFlow = null;
            TerraMatrix = null;
            FireNoise = null;
            SolarThinBlade = null;
            SolarBladeGlowMask = null;
            ElementalRayFlow = null;
            CrossGlow = null;
            ShockWave = null;
            FireStrike = null;
            BloomShockwave = null;
            Lightning = null;
            FusableBall = null;
            BloomRing = null;
            BloomLine = null;
            SolarBladeGlowMask_Blue = null;
            LaserHighContrast = null;
            SlashLine01 = null;
            SlashLine02 = null;
            SlashLine03 = null;
            SoulGreatSword = null;
            HarshNoise = null;

            TechCircle = null;
            VividClarityBlade = null;
        }
    }
}
