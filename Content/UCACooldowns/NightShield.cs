using LAP.Core.LAPUI.CustomCD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI.Chat;
using UCA.Content.Items.Weapons.Magic.Ray;
using UCA.Core.GlobalInstance.Players;
using UCA.Core.Utilities;

namespace UCA.Content.UCACooldowns
{
    public class NightShield : BaseCD
    {
        public int CurShieldHP => Main.LocalPlayer.UCA().NightShieldHP;
        public override Rectangle OverLayerRec => new Rectangle(0, 0, CDTexture_OverLayer.Width, 30 + (int)((CDTexture_OverLayer.Height - 30) * (1 - (CurShieldHP / (float)MaxTime))));
        public override LocalizedText DisplayName()
        {
            return Language.GetOrRegister($"Mods.UCA.Cooldowns.UCANightShield");
        }
        public override void OnRegister()
        {
            Buff = false;
            Info = true;
        }
        public override void OnSpawn(Player player)
        {
            MaxTime = UCAPlayer.NightShieldMaxHP;
        }
        public override void Update(Player player)
        {
            if (player.HeldItem.type == ModContent.ItemType<NightsRayAlt>())
            {
                Time = 2;
                MaxTime = UCAPlayer.NightShieldMaxHP;
            }
        }
        public override void PostDraw()
        {
            Texture2D texture = CustomCDManger.CDTexture[Type].Value;
            Main.spriteBatch.Draw(texture, DrawPosition, null, Color.White, 0f, texture.Size() / 2, 1f, SpriteEffects.None, 0f);
        }
        public override bool PreDrawTime(DynamicSpriteFont MGRFont)
        {
            Player player = Main.LocalPlayer;
            int thisCdRemin = CurShieldHP;
            string Count = $"{thisCdRemin}";
            Vector2 stringsize = ChatManager.GetStringSize(MGRFont, Count, Vector2.One);
            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, MGRFont, Count, DrawPosition + new Vector2(0, 24), Color.White, 0f, stringsize / 2, new Vector2(0.4f));
            return false;
        }
    }
}
