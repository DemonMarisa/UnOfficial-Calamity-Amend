using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI.Chat;
using UCA.Assets;
using UCA.Content.Items.Weapons.Magic.Ray;
using UCA.Content.Paths;
using UCA.Content.Projectiles.HeldProj.Magic.ElementRayHeld;
using UCA.Core.Utilities;

namespace UCA.Content.GUI
{
    public class ElementalRayUI
    {
        public static bool Active = false;

        public static int HoverID = -1;
        public static bool IsHover = false;
        public static float Angle = MathHelper.ToRadians(15);
        public static float Scale = 0f;
        public static bool BeginFadeOut = false;
        public static int FadeProgress = 0;
        public static int FadeMaxProgress = 15;
        public static float Progress => EasingHelper.EaseOutCubic(FadeProgress / (float)FadeMaxProgress);

        public static float MiscScale = 1f;
        public static float SolorScale = 1f;
        public static float VortexScale = 1f;
        public static float NebulaScale = 1f;
        public static float StarDustScale = 1f;
        public static float Opacity = 1f;
        public static void Update()
        {
            if (Main.LocalPlayer.HeldItem.type != ModContent.ItemType<ElementRayAlt>())
                BeginFadeOut = true;
            IsHover = false;
            UpdateFadeIn();
            if (!BeginFadeOut)
                UpdateHover();
        }
        public static void UpdateFadeIn()
        {
            FadeMaxProgress = 10;
            if (BeginFadeOut)
            {
                if (FadeProgress > 0)
                    FadeProgress--;
                if (FadeProgress == 0)
                    Active = false;
            }
            else if (FadeProgress < FadeMaxProgress)
                FadeProgress++;

            Opacity = MathHelper.Lerp(0f, 1f, Progress);
            Scale = MathHelper.Lerp(0f, 1f, Progress);
        }
        public static void UpdateHover()
        {
            ResetScales();

            Vector2 mousePosition = Main.MouseWorld - Main.screenPosition;
            Vector2 vectorToMouse = LAPUtilities.ScreenCenter() - mousePosition;

            // Atan2 的结果范围是 (-π, π]
            // 我们将其规范化到 [0, 2π) 以方便比较
            float mousetoCenterAngle = vectorToMouse.ToRotation();
            if (mousetoCenterAngle < 0)
            {
                mousetoCenterAngle += MathHelper.TwoPi;
            }
            // BaseRot 是 DrawLine() 中使用的起始角度 MathHelper.PiOver2 (90度)
            float BaseRot = MathHelper.PiOver2;
            // 元素扇形的宽度
            float AngleAdd = MathHelper.ToRadians(72.2f); // 361度 / 5，略大于 72度
            float HalfAngleAdd = AngleAdd / 2f;// 每个扇形有一个小的偏移，以确保它覆盖 72.2 度
            // 遍历并检测悬停
            for (int i = 0; i < 5; i++)
            {
                // 扇形的中线角度
                float elementCenterAngle = BaseRot + i * AngleAdd;
                // 规范化到 [0, 2π)
                if (elementCenterAngle >= MathHelper.TwoPi)
                {
                    elementCenterAngle -= MathHelper.TwoPi;
                }
                // 检查角度是否在当前扇形区域内
                if (LAPUtilities.IsAngleInSector(mousetoCenterAngle, elementCenterAngle, HalfAngleAdd))
                {
                    IsHover = true;
                    // 根据 i 值设置对应的 Scale
                    SetHoverScale(i);
                    // 找到后设置并关闭UI
                    if (Main.mouseLeft)
                    {
                        Player player = Main.LocalPlayer;
                        player.UCA().ElementalRayStates = HoverID;
                        BeginFadeOut = true;
                    }
                    return;
                }
            }

        }
        public static void SetHoverScale(int index)
        {
            switch (index)
            {
                case 0:
                    MiscScale = MathHelper.Lerp(MiscScale, 1.4f, 0.2f);
                    HoverID = ElementalRayState.Misc;
                    break;
                case 1: 
                    SolorScale = MathHelper.Lerp(SolorScale, 1.4f, 0.2f);
                    HoverID = ElementalRayState.Solar;
                    break;
                case 2:
                    NebulaScale = MathHelper.Lerp(NebulaScale, 1.4f, 0.2f);
                    HoverID = ElementalRayState.Nebula;
                    break;
                case 3:
                    StarDustScale = MathHelper.Lerp(StarDustScale, 1.4f, 0.2f);
                    HoverID = ElementalRayState.StarDust;
                    break;
                case 4:
                    VortexScale = MathHelper.Lerp(VortexScale, 1.4f, 0.2f);
                    HoverID = ElementalRayState.Vortex;
                    break;
            }
        }
        public static void ResetScales()
        {
            MiscScale = MathHelper.Lerp(MiscScale, 1f, 0.2f);
            SolorScale = MathHelper.Lerp(SolorScale, 1f, 0.2f);
            NebulaScale = MathHelper.Lerp(NebulaScale, 1f, 0.2f);
            StarDustScale = MathHelper.Lerp(StarDustScale, 1f, 0.2f);
            VortexScale = MathHelper.Lerp(VortexScale, 1f, 0.2f);
        }
        public static void OnKill()
        {
            Player player = Main.LocalPlayer;
            player.UCA().ElementalRayStates = HoverID;
        }
        #region 绘制
        public static void Draw()
        {
            DrawBG();
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.UIScaleMatrix);
            DrawRing();
            DrawLine();
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.UIScaleMatrix);
            DrawElementalRay();
        }
        public static void DrawBG()
        {
            Texture2D texture = UCATextureRegister.BloomBlackCircle.Value;
            Vector2 origin = new Vector2(texture.Width / 2, texture.Height / 2);
            Main.spriteBatch.Draw(texture, LAPUtilities.ScreenCenter(), null, Color.White * 0.6f * Opacity, 0, origin, 1.5f * Scale, SpriteEffects.None, 0f);
        }
        public static void DrawRing()
        {
            Texture2D texture = UCATextureRegister.Ring.Value;
            Vector2 origin = new Vector2(texture.Width / 2, texture.Height / 2);
            Main.spriteBatch.Draw(texture, LAPUtilities.ScreenCenter(), null, Color.White * Opacity, -Main.GlobalTimeWrappedHourly, origin, 0.2f * Scale, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(texture, LAPUtilities.ScreenCenter(), null, Color.White * Opacity, Main.GlobalTimeWrappedHourly, origin, 0.62f * Scale, SpriteEffects.None, 0f);
        }
        public static void DrawLine()
        {
            Texture2D texture = UCATextureRegister.BloomLine.Value;
            Vector2 origin = new Vector2(-280, texture.Height / 2);
            float BaseRot = MathHelper.PiOver2;
            for (int i = 0; i < 5; i++)
            {
                float DrawRot = BaseRot + i * MathHelper.ToRadians(72.2f);
                Main.spriteBatch.Draw(texture, LAPUtilities.ScreenCenter(), null, Color.White * Opacity, DrawRot, origin, 0.17f * Scale, SpriteEffects.None, 0f);
            }
        }
        public static void DrawElementalRay()
        {
            Texture2D Misc = UCATextureRegister.ElementalRayMisc.Value;
            Texture2D Solor = UCATextureRegister.ElementalRaySolor.Value;
            Texture2D Nebula = UCATextureRegister.ElementalRayNebula.Value;
            Texture2D StarDust = UCATextureRegister.ElementalRayStarDust.Value;
            Texture2D Vortex = UCATextureRegister.ElementalRayVortex.Value;

            Vector2 origin = Misc.Size() / 2;

            Texture2D outLine = UCATextureRegister.ElementalRayOutLine.Value;
            Vector2 outLineorigin = outLine.Size() / 2;

            int DrawCount = 0;
            float Offset = -150 * Progress;
            Vector2 DrawPos = LAPUtilities.ScreenCenter() + new Vector2(0, Offset).RotatedBy(DrawCount * MathHelper.ToRadians(72.2f));
            float DrawRot = (DrawPos - LAPUtilities.ScreenCenter()).ToRotation();
            Main.spriteBatch.Draw(Misc, DrawPos, null, Color.White * Opacity, DrawRot - MathHelper.PiOver4 * 3, origin, 1f * Scale * MiscScale, SpriteEffects.None, 0f);
            if (HoverID == ElementalRayState.Misc)
            {
                Main.spriteBatch.Draw(outLine, DrawPos, null, Color.White * Opacity, DrawRot - MathHelper.PiOver4 * 3, outLineorigin, 1f * Scale * MiscScale, SpriteEffects.None, 0f);
                // 获取字体引用
                DynamicSpriteFont font = FontAssets.MouseText.Value;
                string text = LocalizedPath.ElementalRayMisc;
                // 计算文本尺寸
                Vector2 textSize = ChatManager.GetStringSize(font, text, new Vector2(1f));
                Vector2 orig = new Vector2(textSize.X / 2, textSize.Y);
                TextSnippet[] snippets = ChatManager.ParseMessage(text, Color.White).ToArray();
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, snippets, Main.MouseWorld - Main.screenPosition, 0, orig, new Vector2(1), out _);
            }
            DrawCount++;

            DrawPos = LAPUtilities.ScreenCenter() + new Vector2(0, Offset).RotatedBy(DrawCount * MathHelper.ToRadians(72.2f));
            DrawRot = (DrawPos - LAPUtilities.ScreenCenter()).ToRotation();
            Main.spriteBatch.Draw(Solor, DrawPos, null, Color.White * Opacity, DrawRot - MathHelper.PiOver4 * 3, origin, 1f * Scale * SolorScale, SpriteEffects.None, 0f);
            if (HoverID == ElementalRayState.Solar)
            {
                Main.spriteBatch.Draw(outLine, DrawPos, null, Color.White * Opacity, DrawRot - MathHelper.PiOver4 * 3, outLineorigin, 1f * Scale * SolorScale, SpriteEffects.None, 0f);
                // 获取字体引用
                DynamicSpriteFont font = FontAssets.MouseText.Value;
                string text = LocalizedPath.ElementalRaySolor;
                // 计算文本尺寸
                Vector2 textSize = ChatManager.GetStringSize(font, text, new Vector2(1f));
                Vector2 orig = new Vector2(textSize.X / 2, textSize.Y);
                TextSnippet[] snippets = ChatManager.ParseMessage(text, Color.White).ToArray();
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, snippets, Main.MouseWorld - Main.screenPosition, 0, orig, new Vector2(1), out _);
            }
            DrawCount++;

            DrawPos = LAPUtilities.ScreenCenter() + new Vector2(0, Offset).RotatedBy(DrawCount * MathHelper.ToRadians(72.2f));
            DrawRot = (DrawPos - LAPUtilities.ScreenCenter()).ToRotation();
            Main.spriteBatch.Draw(Nebula, DrawPos, null, Color.White * Opacity, DrawRot - MathHelper.PiOver4 * 3, origin, 1f * Scale * NebulaScale, SpriteEffects.None, 0f);
            if (HoverID == ElementalRayState.Nebula)
            {
                Main.spriteBatch.Draw(outLine, DrawPos, null, Color.White * Opacity, DrawRot - MathHelper.PiOver4 * 3, outLineorigin, 1f * Scale * NebulaScale, SpriteEffects.None, 0f);
                // 获取字体引用
                DynamicSpriteFont font = FontAssets.MouseText.Value;
                string text = LocalizedPath.ElementalRayNebula;
                // 计算文本尺寸
                Vector2 textSize = ChatManager.GetStringSize(font, text, new Vector2(1f));
                Vector2 orig = new Vector2(textSize.X / 2, textSize.Y);
                TextSnippet[] snippets = ChatManager.ParseMessage(text, Color.White).ToArray();
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, snippets, Main.MouseWorld - Main.screenPosition, 0, orig, new Vector2(1), out _);
            }
            DrawCount++;

            DrawPos = LAPUtilities.ScreenCenter() + new Vector2(0, Offset).RotatedBy(DrawCount * MathHelper.ToRadians(72.2f));
            DrawRot = (DrawPos - LAPUtilities.ScreenCenter()).ToRotation();
            Main.spriteBatch.Draw(StarDust, DrawPos, null, Color.White * Opacity, DrawRot - MathHelper.PiOver4 * 3, origin, 1f * Scale * StarDustScale, SpriteEffects.None, 0f);
            if (HoverID == ElementalRayState.StarDust)
            {
                Main.spriteBatch.Draw(outLine, DrawPos, null, Color.White * Opacity, DrawRot - MathHelper.PiOver4 * 3, outLineorigin, 1f * Scale * StarDustScale, SpriteEffects.None, 0f);
                // 获取字体引用
                DynamicSpriteFont font = FontAssets.MouseText.Value;
                string text = LocalizedPath.ElementalRayStarDust;
                // 计算文本尺寸
                Vector2 textSize = ChatManager.GetStringSize(font, text, new Vector2(1f));
                Vector2 orig = new Vector2(textSize.X / 2, textSize.Y);
                TextSnippet[] snippets = ChatManager.ParseMessage(text, Color.White).ToArray();
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, snippets, Main.MouseWorld - Main.screenPosition, 0, orig, new Vector2(1), out _);
            }
            DrawCount++;

            DrawPos = LAPUtilities.ScreenCenter() + new Vector2(0, Offset).RotatedBy(DrawCount * MathHelper.ToRadians(72.2f));
            DrawRot = (DrawPos - LAPUtilities.ScreenCenter()).ToRotation();
            Main.spriteBatch.Draw(Vortex, DrawPos, null, Color.White * Opacity, DrawRot - MathHelper.PiOver4 * 3, origin, 1f * Scale * VortexScale, SpriteEffects.None, 0f);
            if (HoverID == ElementalRayState.Vortex)
            {
                Main.spriteBatch.Draw(outLine, DrawPos, null, Color.White * Opacity, DrawRot - MathHelper.PiOver4 * 3, outLineorigin, 1f * Scale * VortexScale, SpriteEffects.None, 0f);
                // 获取字体引用
                DynamicSpriteFont font = FontAssets.MouseText.Value;
                string text = LocalizedPath.ElementalRayVortexr;
                // 计算文本尺寸
                Vector2 textSize = ChatManager.GetStringSize(font, text, new Vector2(1f));
                Vector2 orig = new Vector2(textSize.X / 2, textSize.Y);
                TextSnippet[] snippets = ChatManager.ParseMessage(text, Color.White).ToArray();
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, snippets, Main.MouseWorld - Main.screenPosition, 0, orig, new Vector2(1), out _);
            }
            DrawCount++;
        }
        #endregion
    }
}
