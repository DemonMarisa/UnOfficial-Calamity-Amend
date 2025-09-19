using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace UCA.Common.Misc
{
    public class UCAShopValue
    {
        // Base numeric rarity pricing guide.
        private static readonly int Rarity0BuyPrice = Item.buyPrice(0, 0, 50, 0);
        private static readonly int Rarity1BuyPrice = Item.buyPrice(0, 1, 0, 0);
        private static readonly int Rarity2BuyPrice = Item.buyPrice(0, 2, 0, 0);
        private static readonly int Rarity3BuyPrice = Item.buyPrice(0, 4, 0, 0);
        private static readonly int Rarity4BuyPrice = Item.buyPrice(0, 12, 0, 0);
        private static readonly int Rarity5BuyPrice = Item.buyPrice(0, 24, 0, 0);
        private static readonly int Rarity6BuyPrice = Item.buyPrice(0, 36, 0, 0);
        private static readonly int Rarity7BuyPrice = Item.buyPrice(0, 48, 0, 0);
        private static readonly int Rarity8BuyPrice = Item.buyPrice(0, 60, 0, 0);
        private static readonly int Rarity9BuyPrice = Item.buyPrice(0, 80, 0, 0);
        private static readonly int Rarity10BuyPrice = Item.buyPrice(1, 0, 0, 0); // Highest raw rarity used by vanilla items (ML drops)
        private static readonly int Rarity11BuyPrice = Item.buyPrice(1, 20, 0, 0); // End of vanilla rarities
        private static readonly int Rarity12BuyPrice = Item.buyPrice(1, 50, 0, 0);
        private static readonly int Rarity13BuyPrice = Item.buyPrice(1, 75, 0, 0);
        private static readonly int Rarity14BuyPrice = Item.buyPrice(2, 0, 0, 0);
        private static readonly int Rarity15BuyPrice = Item.buyPrice(2, 40, 0, 0);
        private static readonly int Rarity16BuyPrice = Item.buyPrice(2, 80, 0, 0);
        private static readonly int Rarity17BuyPrice = Item.buyPrice(3, 20, 0, 0); // This is Calamity's "plus" rarity (similar to vanilla 11 / Purple). Nothing uses it.

        public static int RarityWhiteBuyPrice => Rarity0BuyPrice;
        public static int RarityBlueBuyPrice => Rarity1BuyPrice;
        public static int RarityGreenBuyPrice => Rarity2BuyPrice;
        public static int RarityOrangeBuyPrice => Rarity3BuyPrice;
        public static int RarityLightRedBuyPrice => Rarity4BuyPrice;
        public static int RarityPinkBuyPrice => Rarity5BuyPrice;
        public static int RarityLightPurpleBuyPrice => Rarity6BuyPrice;
        public static int RarityLimeBuyPrice => Rarity7BuyPrice;
        public static int RarityYellowBuyPrice => Rarity8BuyPrice;
        public static int RarityCyanBuyPrice => Rarity9BuyPrice;
        public static int RarityRedBuyPrice => Rarity10BuyPrice;
        public static int RarityPurpleBuyPrice => Rarity11BuyPrice;
        public static int RarityTurquoiseBuyPrice => Rarity12BuyPrice;
        public static int RarityPureGreenBuyPrice => Rarity13BuyPrice;
        public static int RarityDarkBlueBuyPrice => Rarity14BuyPrice;
        public static int RarityVioletBuyPrice => Rarity15BuyPrice;
        public static int RarityHotPinkBuyPrice => Rarity16BuyPrice;
        public static int RarityCalamityRedBuyPrice => Rarity17BuyPrice;
    }
}
