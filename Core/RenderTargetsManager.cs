using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace UCA.Core
{
    public class RenderTargetsManager : ModSystem
    {
        public static bool ScreenSizeChanged = false;
        public Vector2 oldScreenSize;
        public override void PostUpdateEverything()
        {
            CheckScreenSize();
        }
        // 目前只用于重设像素风格化的屏幕大小
        public void CheckScreenSize()
        {
            if (!Main.dedServ && !Main.gameMenu)
            {
                Vector2 newScreenSize = new Vector2(Main.screenWidth, Main.screenHeight);
                if (oldScreenSize != newScreenSize)
                {
                    ScreenSizeChanged = true;
                }
                oldScreenSize = newScreenSize;
                if (oldScreenSize == newScreenSize)
                {
                    ScreenSizeChanged = false;
                }
            }
        }
    }
}
