using LAP.Assets.TextureRegister;
using LAP.Core.BaseClass.UIs;
using LAP.Core.SystemsLoader;
using LAP.Core.UISystem;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using UCA.Content.GUI.ERayUI;
using UCA.Content.Items.Weapons.Magic.Ray;

namespace UCA.Content.GUI.VividClarityUI
{
    public class VividClarityUI : RouletteUI
    {
        public float Progress => EasingHelper.EaseOutCubic(FadeProgress / (float)MaxFadeProgress);
        public override void PostSetUpContent()
        {
            BeginSctorCenterRot = -MathHelper.PiOver2;
            MaxFadeProgress = 15;
            Active = false;
            FadeProgress = 0;
            if (!Subset.Contains(LAPContent.UIType<VividClarityUI_Attack>()))
                Subset.Add(LAPContent.UIType<VividClarityUI_Attack>());
            if (!Subset.Contains(LAPContent.UIType<VividClarityUI_Defense>()))
                Subset.Add(LAPContent.UIType<VividClarityUI_Defense>());
            if (!Subset.Contains(LAPContent.UIType<VividClarityUI_Support>()))
                Subset.Add(LAPContent.UIType<VividClarityUI_Support>());
        }
        public override void PostOnActive()
        {
            MaxFadeProgress = 15;
            FadeProgress = 0;
            Active = true;
            Scale = Vector2.Zero;
            Scale2 = 0f;
        }
        public override void PostUpdate()
        {
            if (Main.LocalPlayer.HeldItem.type != ItemType<VividClarityAlt>())
                Active = false;
            if (Active && FadeProgress < MaxFadeProgress)
            {
                FadeProgress++;
            }
            else if (!Active && FadeProgress > 0)
            {
                FadeProgress--;
            }
            else if (!Active && FadeProgress == 0)
            {
                LAPContent.DeActive(Type);
            }
            for (int i = 0; i < Subset.Count; i++)
            {
                BaseUI ui = UIManager.UICollection[Subset[i]];
                ui.Scale2 = Progress;
            }
            Scale2 = Progress;
        }
        public override bool PreDeActive()
        {
            Active = false;
            return !Active && FadeProgress <= 0;
        }
        public override bool PreDraw()
        {
            if (Scale2 <= 0)
                return false;
            DrawBG();
            DrawRing();
            DrawLine();
            return true;
        }
        public void DrawBG()
        {
            Texture2D texture = LAPTextureRegister.BloomBlackCircle.Value;
            Vector2 origin = new Vector2(texture.Width / 2, texture.Height / 2);
            Main.spriteBatch.Draw(texture, LAPUtilities.ScreenCenter(), null, Color.White * 0.6f * Opacity, 0, origin, 1.5f * Scale2, SpriteEffects.None, 0f);
        }
        public void DrawRing()
        {
            Texture2D texture = LAPTextureRegister.Ring.Value;
            Vector2 origin = new Vector2(texture.Width / 2, texture.Height / 2);
            Main.spriteBatch.Draw(texture, LAPUtilities.ScreenCenter(), null, Color.White * Opacity, -Main.GlobalTimeWrappedHourly, origin, 0.2f * Scale2, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(texture, LAPUtilities.ScreenCenter(), null, Color.White * Opacity, Main.GlobalTimeWrappedHourly, origin, 0.62f * Scale2, SpriteEffects.None, 0f);
        }
        public void DrawLine()
        {
            Texture2D texture = LAPTextureRegister.BloomLine3.Value;
            Vector2 origin = new Vector2(-280, texture.Height / 2);
            float BaseRot = MathHelper.PiOver2;
            for (int i = 0; i < 5; i++)
            {
                float DrawRot = BaseRot + i * MathHelper.ToRadians(120f);
                Main.spriteBatch.Draw(texture, LAPUtilities.ScreenCenter(), null, Color.White * Opacity, DrawRot, origin, 0.17f * Scale2, SpriteEffects.None, 0f);
            }
        }
    }
}
