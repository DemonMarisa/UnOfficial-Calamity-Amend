using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

namespace UCA.Assets
{
    public partial class UCATextureRegister : ModSystem
    {
        #region 来自莉莉的
        public static Asset<Texture2D> LilyLiquid { get; private set; }
        public static Asset<Texture2D> LilySmoke { get; private set; }
        public static Asset<Texture2D> Flame { get; private set; }
        public static Asset<Texture2D> Spirit { get; private set; }
        public static Asset<Texture2D> BallSoft { get; private set; }
        public static Asset<Texture2D> HoodTrail { get; private set; }
        public static Asset<Texture2D> Ring04 { get; private set; }
        public static Asset<Texture2D> Butterfly { get; private set; }
        public static Asset<Texture2D> Petal { get; private set; }
        public static Asset<Texture2D> Tornado { get; private set; }
        public static Asset<Texture2D> Mowa11 { get; private set; }
        public static Asset<Texture2D> Shine { get; private set; }
        public static Asset<Texture2D> Thrust01 { get; private set; }
        public static Asset<Texture2D> Thrust02 { get; private set; }
        public static Asset<Texture2D> Lightning01 { get; private set; }
        public static Asset<Texture2D> Lightning02 { get; private set; }
        public static Asset<Texture2D> Lightning03 { get; private set; }
        public static Asset<Texture2D> Hahen01 { get; private set; }
        public static Asset<Texture2D> Hahen02 { get; private set; }
        #endregion
        #region 来自马格诺利亚的
        public static string CollectableLightPath = "UCA/Assets/MAGNOLIA/CollectableLight";
        public static Asset<Texture2D> CollectableLight { get; private set; }
        public static Asset<Texture2D> BladeM { get; private set; }
        public static Asset<Texture2D> BladeAura { get; private set; }
        public static Asset<Texture2D> Fire { get; private set; }
        public static Asset<Texture2D> Flower { get; private set; }
        public static Asset<Texture2D> LilyFlower { get; private set; }
        public static Asset<Texture2D> PoisonSmoke { get; private set; }
        public static Asset<Texture2D> Slash { get; private set; }
        public static Asset<Texture2D> Slash_Wrap { get; private set; }
        public static Asset<Texture2D> Slash_Wrap2 { get; private set; }
        public static Asset<Texture2D> Tornade_Fire { get; private set; }
        public static Asset<Texture2D> MiscNoise01 { get; private set; }
        public static Asset<Texture2D> MiscNoise02 { get; private set; }
        public static Asset<Texture2D> Aura_01 { get; private set; }
        public static Asset<Texture2D> Aura_02 { get; private set; }
        public static Asset<Texture2D> Flash_01 { get; private set; }
        public static Asset<Texture2D> BladeTrail { get; private set; }
        public static Asset<Texture2D> Slash2 { get; private set; }
        #endregion
        public static void LoadENDERTextures()
        {
            LilyLiquid = ModContent.Request<Texture2D>($"UCA/Assets/LILES/LilyLiquid");
            LilySmoke = ModContent.Request<Texture2D>($"UCA/Assets/LILES/LilySmoke");
            Flame = ModContent.Request<Texture2D>($"UCA/Assets/LILES/Flame");
            Spirit = ModContent.Request<Texture2D>($"UCA/Assets/LILES/Spirit");
            BallSoft = ModContent.Request<Texture2D>($"UCA/Assets/LILES/BallSoft");
            HoodTrail = ModContent.Request<Texture2D>($"UCA/Assets/LILES/HoodTrail");
            Ring04 = ModContent.Request<Texture2D>($"UCA/Assets/LILES/Ring04");
            Butterfly = ModContent.Request<Texture2D>($"UCA/Assets/LILES/Butterfly");
            Petal = ModContent.Request<Texture2D>($"UCA/Assets/LILES/Petal");
            Tornado = ModContent.Request<Texture2D>($"UCA/Assets/LILES/Tornado");
            Mowa11 = ModContent.Request<Texture2D>($"UCA/Assets/LILES/Mowa11");
            Shine = ModContent.Request<Texture2D>($"UCA/Assets/LILES/Shine");
            Thrust01 = ModContent.Request<Texture2D>($"UCA/Assets/LILES/Thrust01");
            Thrust02 = ModContent.Request<Texture2D>($"UCA/Assets/LILES/Thrust02");
            Hahen01 = ModContent.Request<Texture2D>($"UCA/Assets/LILES/Hahen01");
            Hahen02 = ModContent.Request<Texture2D>($"UCA/Assets/LILES/Hahen02");

            CollectableLight = ModContent.Request<Texture2D>($"UCA/Assets/MAGNOLIA/CollectableLight");
            BladeM = ModContent.Request<Texture2D>($"UCA/Assets/MAGNOLIA/BladeM");
            BladeAura = ModContent.Request<Texture2D>($"UCA/Assets/MAGNOLIA/BladeAura");
            Fire = ModContent.Request<Texture2D>($"UCA/Assets/MAGNOLIA/Fire");
            Flower = ModContent.Request<Texture2D>($"UCA/Assets/MAGNOLIA/Flower");
            LilyFlower = ModContent.Request<Texture2D>($"UCA/Assets/MAGNOLIA/LilyFlower");
            PoisonSmoke = ModContent.Request<Texture2D>($"UCA/Assets/MAGNOLIA/PoisonSmoke");
            Lightning01 = ModContent.Request<Texture2D>($"UCA/Assets/MAGNOLIA/Lightning01");
            Lightning02 = ModContent.Request<Texture2D>($"UCA/Assets/MAGNOLIA/Lightning02");
            Lightning03 = ModContent.Request<Texture2D>($"UCA/Assets/MAGNOLIA/Lightning03");
            Slash = ModContent.Request<Texture2D>($"UCA/Assets/MAGNOLIA/Slash");
            Slash_Wrap = ModContent.Request<Texture2D>($"UCA/Assets/MAGNOLIA/Slash_Wrap");
            Slash_Wrap2 = ModContent.Request<Texture2D>($"UCA/Assets/MAGNOLIA/Slash_Wrap2");
            Tornade_Fire = ModContent.Request<Texture2D>($"UCA/Assets/MAGNOLIA/Tornade_Fire");
            MiscNoise01 = ModContent.Request<Texture2D>($"UCA/Assets/MAGNOLIA/MiscNoise01");
            MiscNoise02 = ModContent.Request<Texture2D>($"UCA/Assets/MAGNOLIA/MiscNoise02");
            Aura_01 = ModContent.Request<Texture2D>($"UCA/Assets/MAGNOLIA/Aura_01");
            Aura_02 = ModContent.Request<Texture2D>($"UCA/Assets/MAGNOLIA/Aura_02");
            Flash_01 = ModContent.Request<Texture2D>($"UCA/Assets/MAGNOLIA/flash_01");
            BladeTrail = ModContent.Request<Texture2D>($"UCA/Assets/MAGNOLIA/BladeTrail");
            Slash2 = ModContent.Request<Texture2D>($"UCA/Assets/MAGNOLIA/Slash2");
        }
        public static void UnLoadENDERTextures()
        {
            LilyLiquid = null;
            LilySmoke = null;
            Flame = null;
            Spirit = null;
            BallSoft = null;
            HoodTrail = null;
            Butterfly = null;
            Petal = null;
            Tornado = null;
            Shine = null;
            Thrust01 = null;
            Thrust02 = null;
            Hahen01 = null;
            Hahen02 = null;

            CollectableLight = null;
            BladeM = null;
            BladeAura = null;
            Fire = null;
            Flower = null;
            LilyFlower = null;
            PoisonSmoke = null;
            Lightning01 = null;
            Lightning02 = null;
            Lightning03 = null;
            Slash = null;
            Slash_Wrap = null;
            Slash_Wrap2 = null;
            Tornade_Fire = null;
            MiscNoise01 = null;
            MiscNoise02 = null;
            Aura_01 = null;
            Aura_02 = null;
            Flash_01 = null;
            BladeTrail = null;
            Slash2 = null;
        }
    }
}
