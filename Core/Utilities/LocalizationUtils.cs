using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace UCA.Core.Utilities
{
    public static partial class UCAUtilities
    {
        public static LocalizedText GetText(string key)
        {
            return Language.GetOrRegister("Mods.UCA." + key);
        }
        public static string GetTextValue(string key)
        {
            return Language.GetTextValue("Mods.UCA." + key);
        }
        public static void CustomTooltipDraw(this DrawableTooltipLine drawLine, ref int yOffset, Color drawColor, Color innerColor)
        {
            float sine = (float)((1 + Math.Sin(Main.GlobalTimeWrappedHourly * 2.5f)) / 2);
            float sineOffset = MathHelper.Lerp(0.5f, 1f, sine);
            string textValue = drawLine.Text;
            Vector2 textPos = new(drawLine.X, drawLine.Y);
            //绘制发光描边
            for (int i = 0; i < 12; i++)
            {
                Vector2 afterimageOffset = (MathHelper.TwoPi * i / 12f).ToRotationVector2() * (1.8f * sineOffset);
                ChatManager.DrawColorCodedString(Main.spriteBatch, drawLine.Font, textValue, (textPos + afterimageOffset).RotatedBy(MathHelper.TwoPi * (i / 12)), drawColor * 0.9f, drawLine.Rotation, drawLine.Origin, drawLine.BaseScale);
            }
            ChatManager.DrawColorCodedString(Main.spriteBatch, drawLine.Font, textValue, textPos, innerColor, drawLine.Rotation, drawLine.Origin, drawLine.BaseScale);
        }
    }
}
