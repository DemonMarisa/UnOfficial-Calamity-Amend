using Microsoft.Xna.Framework;
using Terraria;
using UCA.Core.GlobalInstance.Items;
using UCA.Core.GlobalInstance.Projectiles;

namespace UCA.Core.Utilities
{
    public static partial class UCAUtilities
    {
        public static UCAGlobalItem UCA(this Item item)
        {
            return item.GetGlobalItem<UCAGlobalItem>();
        }
        public static UCAGlobalProj UCA(this Projectile proj)
        {
            return proj.GetGlobalProjectile<UCAGlobalProj>();
        }
        public static bool LineThroughRect(Vector2 start, Vector2 end, Rectangle rect, int lineWidth = 4, int checkDistance = 8)
        {
            float point = 0f;
            return rect.Contains((int)start.X, (int)start.Y) || rect.Contains((int)end.X, (int)end.Y) || Collision.CheckAABBvLineCollision(rect.TopLeft(), rect.Size(), start, end, lineWidth, ref point);
        }
    }
}
