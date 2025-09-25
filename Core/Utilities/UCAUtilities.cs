using System;
using Terraria;
using Terraria.ModLoader;
using UCA.Core.GlobalInstance.Items;
using UCA.Core.GlobalInstance.Players;
using UCA.Core.GlobalInstance.Projectiles;

namespace UCA.Core.Utilities
{
    public static partial class UCAUtilities
    {
        public static string GetNamePlacePath(Type type)
        {
            return (type.GetType().Namespace + ".").Replace('.', '/');
        }

        public static UCAGlobalItem UCA(this Item item)
        {
            return item.GetGlobalItem<UCAGlobalItem>();
        }
        public static UCAGlobalProj UCA(this Projectile proj)
        {
            return proj.GetGlobalProjectile<UCAGlobalProj>();
        }

        public static bool PressLeftAndRightClick()
        {
            return Main.mouseLeft && Main.mouseRight;
        }
        public static bool JustPressLeftClick()
        {
            return Main.mouseLeft && !Main.mouseRight;
        }

        public static bool JustPressRightClick()
        {
            return !Main.mouseLeft && Main.mouseRight;
        }
    }
}
