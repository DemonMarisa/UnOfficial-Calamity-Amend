using CalamityMod.Cooldowns;
using LAP.Core.LAPUI.CustomCD;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Localization;
using UCA.Content.Particiles;

namespace UCA.Content.UCACooldowns
{
    public class ShadowBotlStaffDodge : BaseCD
    {
        public override LocalizedText DisplayName()
        {
            return Language.GetOrRegister($"Mods.UCA.Cooldowns.UCAShadowBotlStaffDodgeCooldown");
        }
        public override void Update(Player player)
        {
            for (int i = 0; i < 2; i++)
            {
                Vector2 GenPos = player.Center + new Vector2(Main.rand.Next(-16, 16), Main.rand.Next(-24, 24));
                Color Firecolor = LAPUtilities.LerpColor(Color.Black, Color.DarkViolet);
                Vector2 fireVel = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.2f, 1.2f);
                new Fire(GenPos, fireVel, Firecolor, 90, Main.rand.NextFloat(MathHelper.TwoPi), 1f, 0.2f).SpawnToPriorityNonPreMult();
            }
        }
        public override void PostDraw()
        {
            Texture2D texture = CustomCDManger.CDTexture[Type].Value;
            Main.spriteBatch.Draw(texture, DrawPosition, null, Color.White, 0f, texture.Size() / 2, 1f, SpriteEffects.None, 0f);
        }
    }
}
