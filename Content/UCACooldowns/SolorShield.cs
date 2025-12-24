using CalamityMod.Cooldowns;
using LAP.Core.LAPUI.CustomCD;
using LAP.Core.ParticleSystem;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Localization;
using UCA.Content.MetaBalls;
using UCA.Content.Particiles;

namespace UCA.Content.UCACooldowns
{
    public class SolorShield : BaseCD
    {
        public override LocalizedText DisplayName()
        {
            return Language.GetOrRegister($"Mods.UCA.Cooldowns.UCASolorShieldCooldown");
        }
        public override void Update(Player player)
        {
            player.LAP().ExternalDR += 0.15f;
            player.statDefense += 30;
            Vector2 GenPos = player.Center + -new Vector2 (Main.rand.Next(-25, 25), -player.height / 2);
            Vector2 fireVel = Vector2.UnitY * Main.rand.NextFloat(2, 6f) * -1f;
            Color Firecolor = LAP.Core.Utilities.LAPUtilities.LerpColor(Color.Orange, Color.OrangeRed);
            new Fire(GenPos, fireVel, Firecolor, 90, Main.rand.NextFloat(MathHelper.TwoPi), 1f, 0.2f).AddToADD(); 
        }
        public override void PostDraw()
        {
            Texture2D texture = CustomCDManger.CDTexture[Type].Value;
            Main.spriteBatch.Draw(texture, DrawPosition, null, Color.White, 0f, texture.Size() / 2, 1f, SpriteEffects.None, 0f);
        }
    }
}