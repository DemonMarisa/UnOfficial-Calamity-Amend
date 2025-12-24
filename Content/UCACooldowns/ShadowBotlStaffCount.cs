using LAP.Core.LAPUI.CustomCD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;

namespace UCA.Content.UCACooldowns
{
    public class ShadowBotlStaffCount : BaseCD
    {
        public override bool DeBuff => true;
        public override bool Buff => false;
        public override LocalizedText DisplayName()
        {
            return Language.GetOrRegister($"Mods.UCA.Cooldowns.UCAShadowBotlStaffCountCooldown");
        }
        public override void PostDraw()
        {
            Texture2D texture = CustomCDManger.CDTexture[Type].Value;
            Main.spriteBatch.Draw(texture, DrawPosition, null, Color.White, 0f, texture.Size() / 2, 1f, SpriteEffects.None, 0f);
        }
    }
}
