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
    public class NebulaBoost : BaseCD
    {
        public override LocalizedText DisplayName()
        {
            return Language.GetOrRegister($"Mods.UCA.Cooldowns.UCANebulaBoostCooldown");
        }
        public override void Update(Player player)
        {
            if (player.miscCounter % 20 == 0)
            {
                player.NCHeal(5);
            }
            player.GetDamage<MagicDamageClass>() += 0.15f;
            player.GetCritChance<MagicDamageClass>() += 10;
            if (player.miscCounter % 4 == 0)
            {
                NebulaMetaBall.SpawnParticle(player.Center - new Vector2(Main.rand.Next(-25, 25), -player.height / 2), Vector2.UnitY * Main.rand.NextFloat(2, 6f) * -1f, Main.rand.NextFloat(0.1f, 0.15f), 60);

            }
        }
        public override void PostDraw()
        {
            Texture2D texture = CustomCDManger.CDTexture[Type].Value;
            Main.spriteBatch.Draw(texture, DrawPosition, null, Color.White, 0f, texture.Size() / 2, 1f, SpriteEffects.None, 0f);
        }
    }
}
