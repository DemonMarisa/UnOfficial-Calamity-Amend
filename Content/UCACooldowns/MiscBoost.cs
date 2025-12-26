using CalamityMod.Cooldowns;
using LAP.Core.LAPUI.CustomCD;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using UCA.Content.MetaBalls;
using UCA.Content.Particiles;

namespace UCA.Content.UCACooldowns
{
    public class MiscBoost : BaseCD
    {
        public override LocalizedText DisplayName()
        {
            return Language.GetOrRegister($"Mods.UCA.Cooldowns.UCAMiscBoostCooldown");
        }
        public override void Update(Player player)
        {
            if (player.miscCounter % 20 == 0)
            {
                player.NCHeal(5);
            }
            if (player.miscCounter % 4 == 0)
            {
                Vector2 pos = player.Center - new Vector2(Main.rand.Next(-50, 50), -player.height / 2);
                Color RandomColor = Color.Lerp(Color.White, Color.WhiteSmoke, Main.rand.NextFloat(0, 1));
                new MediumGlowBall(pos, Vector2.Zero, RandomColor, 120, 0, 1, 0.2f, Main.rand.NextFloat(1f, 2f)).Spawn();
            }
        }
        public override void PostDraw()
        {
            Texture2D texture = CustomCDManger.CDTexture[Type].Value;
            Main.spriteBatch.Draw(texture, DrawPosition, null, Color.White, 0f, texture.Size() / 2, 1f, SpriteEffects.None, 0f);
        }
    }
}
