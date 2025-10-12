using CalamityMod;
using CalamityMod.Cooldowns;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.Localization;
using Terraria.ModLoader;
using UCA.Content.Items.Weapons.Magic.Ray;
using UCA.Core.GlobalInstance.Players;
using UCA.Core.Utilities;

namespace UCA.Content.UCACooldowns
{
    public class TerraRayCount : CooldownHandler
    {
        public static Color ringColorLerpStart = Color.ForestGreen;
        public static Color ringColorLerpEnd = Color.LawnGreen;
        public float AdjustedCompletion => instance.timeLeft / (float)UCAPlayer.TerraRayChargeCD;

        public static new string ID => "UCATerraRestoreCount";
        public override bool CanTickDown => instance.player.HeldItem.type != ModContent.ItemType<TerraRay>() || instance.timeLeft <= 0;
        public override bool ShouldDisplay => instance.player.HeldItem.type == ModContent.ItemType<TerraRay>();
        public override LocalizedText DisplayName => Language.GetOrRegister($"Mods.UCA.Cooldowns.{ID}");
        public override string Texture => "UCA/Content/UCACooldowns/TerraRayCount";
        public override string OutlineTexture => "UCA/Content/UCACooldowns/TerraRayCount_OutLine";
        public override string OverlayTexture => "UCA/Content/UCACooldowns/TerraRayCount_Overlay";
        public override Color OutlineColor => Color.Lerp(ringColorLerpStart, ringColorLerpEnd, (float)Math.Sin(Main.GlobalTimeWrappedHourly) * 0.5f + 0.5f);
        public override Color CooldownStartColor => Color.Lerp(ringColorLerpStart, ringColorLerpEnd, instance.Completion);
        public override Color CooldownEndColor => Color.Lerp(ringColorLerpStart, ringColorLerpEnd, instance.Completion);
        public override bool SavedWithPlayer => false;
        public override bool PersistsThroughDeath => false;
        public override void ApplyBarShaders(float opacity)
        {
            // Use the adjusted completion
            GameShaders.Misc["CalamityMod:CircularBarShader"].UseOpacity(opacity);
            GameShaders.Misc["CalamityMod:CircularBarShader"].UseSaturation(AdjustedCompletion);
            GameShaders.Misc["CalamityMod:CircularBarShader"].UseColor(CooldownStartColor);
            GameShaders.Misc["CalamityMod:CircularBarShader"].UseSecondaryColor(CooldownEndColor);
            GameShaders.Misc["CalamityMod:CircularBarShader"].Apply();
        }
        public override void DrawExpanded(SpriteBatch spriteBatch, Vector2 position, float opacity, float scale)
        {
            base.DrawExpanded(spriteBatch, position, opacity, scale);
            float Xoffset = instance.timeLeft > 9 ? -10f : -5;
            Player player = Main.LocalPlayer;
            CalamityUtils.DrawBorderStringEightWay(spriteBatch, FontAssets.MouseText.Value, player.UCA().TerraRayRestore.ToString(), position + new Vector2(Xoffset, 4) * scale, Color.Lerp(ringColorLerpEnd, Color.Black, 1 - instance.Completion), Color.Black, scale);
        }
        public override void DrawCompact(SpriteBatch spriteBatch, Vector2 position, float opacity, float scale)
        {
            Texture2D sprite = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D outline = ModContent.Request<Texture2D>(OutlineTexture).Value;
            Texture2D overlay = ModContent.Request<Texture2D>(OverlayTexture).Value;
            // Draw the outline
            spriteBatch.Draw(outline, position, null, OutlineColor * opacity, 0, outline.Size() * 0.5f, scale, SpriteEffects.None, 0f);
            // Draw the icon
            spriteBatch.Draw(sprite, position, null, Color.White * opacity, 0, sprite.Size() * 0.5f, scale, SpriteEffects.None, 0f);
            // Draw the small overlay
            int lostHeight = (int)Math.Ceiling(overlay.Height * AdjustedCompletion);
            Rectangle crop = new Rectangle(0, lostHeight, overlay.Width, overlay.Height - lostHeight);
            spriteBatch.Draw(overlay, position + Vector2.UnitY * lostHeight * scale, crop, OutlineColor * opacity * 0.9f, 0, sprite.Size() * 0.5f, scale, SpriteEffects.None, 0f);
            float Xoffset = instance.timeLeft > 9 ? -10f : -5;
            CalamityUtils.DrawBorderStringEightWay(spriteBatch, FontAssets.MouseText.Value, instance.timeLeft.ToString(), position + new Vector2(Xoffset, 4) * scale, Color.Lerp(ringColorLerpStart, Color.OrangeRed, 1 - instance.Completion), Color.Black, scale);
        }
    }
}
