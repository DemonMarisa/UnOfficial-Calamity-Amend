using LAP.Core.LAPUI.CustomCD;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using UCA.Content.MetaBalls;

namespace UCA.Content.UCACooldowns
{
    public class NightBoost : BaseCD
    {
        public override LocalizedText DisplayName()
        {
            return Language.GetOrRegister($"Mods.UCA.Cooldowns.UCANightBoostCooldown");
        }
        public override void Update(Player player)
        {
            player.GetDamage<GenericDamageClass>() += 0.15f;
            player.manaCost *= 0.95f;
            player.LAP().ExternalDR += 0.05f;
            player.statDefense += 10;
            if (player.miscCounter % 2 == 0)
                ShadowMetaBall.SpawnParticle(player.Center - new Vector2(Main.rand.Next(-25, 25), -player.height / 2), Vector2.UnitY * Main.rand.NextFloat(2, 6f) * -1f, Main.rand.NextFloat(0.1f, 0.15f));
        }
        public override void PostDraw()
        {
            Texture2D texture = CustomCDManger.CDTexture[Type].Value;
            Main.spriteBatch.Draw(texture, DrawPosition, null, Color.White, 0f, texture.Size() / 2, 1f, SpriteEffects.None, 0f);
        }
    }
}
