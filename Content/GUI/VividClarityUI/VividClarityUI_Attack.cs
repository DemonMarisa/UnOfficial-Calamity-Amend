using LAP.Core.BaseClass.UIs;
using LAP.Core.SystemsLoader;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using UCA.Assets;
using UCA.Content.Projectiles.HeldProj.Magic.ElementRayHeld;
using UCA.Core.Enum;
using UCA.Core.Utilities;

namespace UCA.Content.GUI.VividClarityUI
{
    public class VividClarityUI_Attack : RouletteUIPart
    {
        public override void PostSetUpContent()
        {
            Parent = LAPContent.UIType<VividClarityUI>();
        }
        public override void MouseHover(bool isHover)
        {
            if (isHover)
                Scale = Vector2.Lerp(Scale, Vector2.One * 1.1f, 0.2f);
            else
                Scale = Vector2.Lerp(Scale, Vector2.One * 1f, 0.2f);
        }
        public override void MouseLeft()
        {
            Player player = Main.LocalPlayer;
            player.UCA().VividClarityStates = VividClarityState.Attack;
            LAPContent.DeActive(Parent);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D Misc = UCATextureRegister.VividClarityAttack.Value;
            Vector2 origin = Misc.Size() / 2;
            float Offset = 150 * Scale2;
            Vector2 DrawPos = LAPUtilities.ScreenCenter() + new Vector2(Offset, 0).RotatedBy(SectorCenterRot);
            Main.spriteBatch.Draw(Misc, DrawPos, null, Color.White * Opacity, 0, origin, 0.25f * Scale * Scale2, SpriteEffects.None, 0f);
        }
    }
}
