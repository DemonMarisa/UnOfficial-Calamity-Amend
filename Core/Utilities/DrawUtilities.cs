using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace UCA.Core.Utilities
{
    public static partial class UCAUtilities
    {
        /// <summary>
        /// 将当前渲染目标设置为提供的渲染目标。
        /// </summary>
        /// <param name="rt">要交换到的渲染目标</param>
        public static bool SwapToTarget(RenderTarget2D rt)
        {
            GraphicsDevice gD = Main.graphics.GraphicsDevice;
            SpriteBatch spriteBatch = Main.spriteBatch;

            if (Main.gameMenu || Main.dedServ || spriteBatch is null || rt is null || gD is null)
                return false;

            gD.SetRenderTarget(rt);
            gD.Clear(Color.Transparent);
            return true;
        }

        public static void BaseProjPreDraw(this Projectile proj, Texture2D texture, Color lightColor, float rotOffset = 0f, float scale = 1f)
        {
            Vector2 drawPosition = proj.Center - Main.screenPosition;
            float drawRotation = proj.rotation + (proj.spriteDirection == -1 ? MathHelper.Pi : 0f);
            Vector2 rotationPoint = texture.Size() / 2f;
            SpriteEffects flipSprite = (proj.spriteDirection * Main.player[proj.owner].gravDir == -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Main.spriteBatch.Draw(texture, drawPosition, null, lightColor, drawRotation + rotOffset, rotationPoint, proj.scale * Main.player[proj.owner].gravDir * scale, flipSprite, 0f);
        }
    }
}
