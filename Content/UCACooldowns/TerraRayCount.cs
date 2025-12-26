using LAP.Core.LAPUI.CustomCD;
using Microsoft.Xna.Framework;
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
    public class TerraRayCount : BaseCD
    {
        public int TerraRayRestore => Main.LocalPlayer.UCA().TerraRayRestore;
        public int TerraRayCharge => Main.LocalPlayer.UCA().TerraRayCharge;
        public override Rectangle OverLayerRec => TerraRayRestore == 3 ? 
            new Rectangle(0, 0, CDTexture_OverLayer.Width, 0) :
            new Rectangle(0, 0, CDTexture_OverLayer.Width, 30 + (int)((CDTexture_OverLayer.Height - 30) * (1 - (TerraRayCharge / (float)UCAPlayer.TerraRayChargeCD))));
        public override LocalizedText DisplayName()
        {
            return Language.GetOrRegister($"Mods.UCA.Cooldowns.UCATerraRestoreCount");
        }
        public override void OnRegister()
        {
            Buff = false;
            Info = true;
        }
        public override void OnSpawn(Player player)
        {
            MaxTime = UCAPlayer.MaxTerraRayRestore;
        }
        public override void Update(Player player)
        {
            if (player.HeldItem.type == ModContent.ItemType<TerraRay>())
            {
                Time = 2;
                MaxTime = UCAPlayer.MaxTerraRayRestore;
            }
        }
        public override bool PreDrawTime(DynamicSpriteFont MGRFont)
        {
            int thisCdRemin = TerraRayRestore;
            string Count = $"{thisCdRemin}";
            Vector2 stringsize = ChatManager.GetStringSize(MGRFont, Count, Vector2.One);
            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, MGRFont, Count, DrawPosition + new Vector2(0, 24), Color.White, 0f, stringsize / 2, new Vector2(0.4f));
            return false;
        }
    }
}
