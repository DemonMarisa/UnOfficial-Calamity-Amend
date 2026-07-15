using LAP.Core.BaseClass.UIs;
using LAP.Core.SystemsLoader;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.UI.Chat;
using UCA.Assets;
using UCA.Content.Paths;
using UCA.Content.Projectiles.HeldProj.Magic.ElementRayHeld;
using UCA.Core.Utilities;

namespace UCA.Content.GUI.ERayUI
{
    public class ERayMisc : RouletteUIPart
    {
        public override void PostSetUpContent()
        {
            Parent = LAPContent.UIType<ERayUI>();
        }
        public override void MouseHover(bool isHover)
        {
            if (isHover)
                Scale = Vector2.Lerp(Scale, Vector2.One * 1.2f, 0.2f);
            else
                Scale = Vector2.Lerp(Scale, Vector2.One * 1f, 0.2f);
        }
        public override void MouseLeft()
        {
            Player player = Main.LocalPlayer;
            player.UCA().ElementalRayStates = ElementalRayState.Misc;
            LAPContent.DeActive(Parent);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D Misc = UCATextureRegister.ElementalRayMisc.Value;
            Vector2 origin = Misc.Size() / 2;
            Texture2D outLine = UCATextureRegister.ElementalRayOutLine.Value;
            Vector2 outLineorigin = outLine.Size() / 2;
            float Offset = 150 * Scale2;
            Vector2 DrawPos = LAPUtilities.ScreenCenter() + new Vector2(Offset, 0).RotatedBy(SectorCenterRot);
            float DrawRot = (DrawPos - LAPUtilities.ScreenCenter()).ToRotation();
            Main.spriteBatch.Draw(Misc, DrawPos, null, Color.White * Opacity, DrawRot - MathHelper.PiOver4 * 3, origin, 1f * Scale * Scale2, SpriteEffects.None, 0f);
            if (IsHover)
            {
                Main.spriteBatch.Draw(outLine, DrawPos, null, Color.White * Opacity, DrawRot - MathHelper.PiOver4 * 3, outLineorigin, 1f * Scale * Scale2, SpriteEffects.None, 0f);
                DynamicSpriteFont font = FontAssets.MouseText.Value;
                string text = LocalizedPath.ElementalRayMisc;
                // 计算文本尺寸
                Vector2 textSize = ChatManager.GetStringSize(font, text, new Vector2(1f));
                Vector2 orig = new Vector2(textSize.X / 2, textSize.Y);
                TextSnippet[] snippets = ChatManager.ParseMessage(text, Color.White).ToArray();
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, snippets, Main.MouseWorld - Main.screenPosition, 0, orig, new Vector2(1), out _);
            }
        }
    }
}
