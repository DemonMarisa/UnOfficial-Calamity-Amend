using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;
using UCA.Content.Particiles;

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
        public static Asset<Texture2D> Slash01 { get; private set; }
        public static Asset<Texture2D> Ring04 { get; private set; }
        public static Asset<Texture2D> Butterfly { get; private set; }
        public static Asset<Texture2D> Petal { get; private set; }
        public static Asset<Texture2D> Tornado { get; private set; }
        public static Asset<Texture2D> Mowa11 { get; private set; }
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
        #endregion
        public static void LoadENDERTextures()
        {
            LilyLiquid = ModContent.Request<Texture2D>($"UCA/Assets/LILES/LilyLiquid");
            LilySmoke = ModContent.Request<Texture2D>($"UCA/Assets/LILES/LilySmoke");
            Flame = ModContent.Request<Texture2D>($"UCA/Assets/LILES/Flame");
            Spirit = ModContent.Request<Texture2D>($"UCA/Assets/LILES/Spirit");
            BallSoft = ModContent.Request<Texture2D>($"UCA/Assets/LILES/BallSoft");
            Slash01 = ModContent.Request<Texture2D>($"UCA/Assets/LILES/Slash01");
            Ring04 = ModContent.Request<Texture2D>($"UCA/Assets/LILES/Ring04");
            Butterfly = ModContent.Request<Texture2D>($"UCA/Assets/LILES/Butterfly");
            Petal = ModContent.Request<Texture2D>($"UCA/Assets/LILES/Petal");
            Tornado = ModContent.Request<Texture2D>($"UCA/Assets/LILES/Tornado");
            Mowa11 = ModContent.Request<Texture2D>($"UCA/Assets/LILES/Mowa11");

            CollectableLight = ModContent.Request<Texture2D>($"UCA/Assets/MAGNOLIA/CollectableLight");
            BladeM = ModContent.Request<Texture2D>($"UCA/Assets/MAGNOLIA/BladeM");
            BladeAura = ModContent.Request<Texture2D>($"UCA/Assets/MAGNOLIA/BladeAura");
            Fire = ModContent.Request<Texture2D>($"UCA/Assets/MAGNOLIA/Fire");
            Flower = ModContent.Request<Texture2D>($"UCA/Assets/MAGNOLIA/Flower");
            LilyFlower = ModContent.Request<Texture2D>($"UCA/Assets/MAGNOLIA/LilyFlower");
            PoisonSmoke = ModContent.Request<Texture2D>($"UCA/Assets/MAGNOLIA/PoisonSmoke");
        }
        public static void UnLoadENDERTextures()
        {
            LilyLiquid = null;
            LilySmoke = null;
            Flame = null;
            Spirit = null;
            BallSoft = null;
            Slash01 = null;
            Butterfly = null;
            Petal = null;
            Tornado = null;

            CollectableLight = null;
            BladeM = null;
            BladeAura = null;
            Fire = null;
            Flower = null;
            LilyFlower = null;
            PoisonSmoke = null;
        }
    }
}
