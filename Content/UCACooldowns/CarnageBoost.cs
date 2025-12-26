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

namespace UCA.Content.UCACooldowns
{
    public class CarnageBoost : BaseCD
    {
        public override LocalizedText DisplayName()
        {
            return Language.GetOrRegister($"Mods.UCA.Cooldowns.UCACarnageBoostCooldown");
        }
        public override void Update(Player player)
        {
            player.GetCritChance<GenericDamageClass>() += 15;
            player.manaCost *= 0.95f;

            if (player.miscCounter % 2 == 0)
                CarnageMetaBall.SpawnParticle(player.Center - new Vector2(Main.rand.Next(-25, 25), 0),
                    Vector2.UnitY * Main.rand.NextFloat(2, 6f) * -1f,
                    Main.rand.NextFloat(0.3f, 0.5f), MathHelper.PiOver2);

            if (player.miscCounter % 9 == 0)
            {
                player.NCHeal(1);
            }
        }
        public override void PostDraw()
        {
            Texture2D texture = CustomCDManger.CDTexture[Type].Value;
            Main.spriteBatch.Draw(texture, DrawPosition, null, Color.White, 0f, texture.Size() / 2, 1f, SpriteEffects.None, 0f);
        }
    }
}
