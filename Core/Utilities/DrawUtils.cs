using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using UCA.Assets.Effects;
using UCA.Content.Particiles;

namespace UCA.Core.Utilities
{
    public static partial class UCAUtilities
    {
        public static void GenStarLine(Vector2 BeginPos, Vector2 EndPos, float GenStep, Color color)
        {
            for (int i = 0; i < GenStep; i++)
            {
                Vector2 SpawnVector = Vector2.Lerp(BeginPos, EndPos, i / GenStep);
                new MediumGlowBall(SpawnVector, Vector2.Zero, color, 60, 0, 1f, 0.1f, 0).Spawn();
            }
        }
        public static void ApplySolarBladeShader(Color beginColor, Color endColor, float uIntensity = 0.15f, bool useColor = true, float Opacity = 0.5f)
        {
            UCAShaderRegister.SolarBladeShader.Parameters["uTime"].SetValue(-Main.GlobalTimeWrappedHourly);
            UCAShaderRegister.SolarBladeShader.Parameters["uIntensity"].SetValue(uIntensity);
            UCAShaderRegister.SolarBladeShader.Parameters["ubeginColor"].SetValue(beginColor.ToVector4());
            UCAShaderRegister.SolarBladeShader.Parameters["uendColor"].SetValue(endColor.ToVector4());
            UCAShaderRegister.SolarBladeShader.Parameters["UseColor"].SetValue(useColor);
            UCAShaderRegister.SolarBladeShader.Parameters["Opacity"].SetValue(Opacity);
            UCAShaderRegister.SolarBladeShader.CurrentTechnique.Passes[0].Apply();
        }
        public static void QuickDrawWithTrailing(this Projectile proj, float offset, Color color, float rotFix = 0) => QuickDrawWithTrailing(proj, offset, color, proj.Center, proj.scale, 4, rotFix);
        public static void QuickDrawWithTrailing(this Projectile proj, float offset, Color color, int drawTime, float rotFix = 0) => QuickDrawWithTrailing(proj, offset, color, proj.Center, proj.scale, drawTime, rotFix);
        public static void QuickDrawWithTrailing(this Projectile proj, float offset, Color color, int drawTime, Vector2 drawCenter, float rotFix = 0) => QuickDrawWithTrailing(proj, offset, color, drawCenter, proj.scale, drawTime, rotFix);
        public static void QuickDrawWithTrailing(this Projectile proj, float offset, Color color, int drawTime, Vector2 drawCenter, float scale, float rotFix = 0) => QuickDrawWithTrailing(proj, offset, color, drawCenter, scale, drawTime, rotFix);
        public static void QuickDrawWithTrailing(this Projectile proj, float offset, Color color, Vector2 drawCenter, float scale, int drawTime = 4, float rotFix = 0)
        {
            Texture2D tex = TextureAssets.Projectile[proj.type].Value;
            Vector2 orig = tex.Size() / 2;
            Vector2 drawPos = drawCenter - Main.screenPosition;
            for (int i = 1; i < drawTime; i++)
            {
                Vector2 trailingDrawPos = drawPos - proj.velocity * i * offset;
                float faded = 1 - i / (float)drawTime;
                //平方放缩
                faded = MathF.Pow(faded, 2);
                Color trailColor = color * faded;
                Main.spriteBatch.Draw(tex, trailingDrawPos, null, trailColor, proj.oldRot[i] + rotFix, orig, scale, 0, 0);
            }
            //直接绘制主射弹位于最顶层
            Main.spriteBatch.Draw(tex, drawPos, null, color, proj.rotation + rotFix, orig, scale, 0, 0.1f);
        }
        /// <summary>
        /// 以最快的方法为物品创建一个发光遮罩+描边
        /// </summary>
        /// <param name="item"></param>
        /// <param name="SB"></param>
        /// <param name="scale"></param>
        public static void QuickDrawItemWithBloomToWorld(this Item item, SpriteBatch SB, Color color, ref float scale)
        {
            Texture2D tex = TextureAssets.Item[item.type].Value;
            Vector2 position = item.position - Main.screenPosition + tex.Size() / 2;
            Rectangle iFrame = tex.Frame();
            //为物品添加描边，并时刻更新大小
            //如果你要是有能力做渐变的话，so be it
            for (int i = 0; i < 16; i++)
                SB.Draw(tex, position + MathHelper.ToRadians(i * 60f).ToRotationVector2() * 2.4f, null, color with { A = 0 }, 0f, tex.Size() / 2, scale, 0, 0f);
            //然后绘制锤子本身。
            SB.Draw(tex, position, iFrame, Color.White, 0f, tex.Size() / 2, scale, 0f, 0f);
        }
        public static SpriteEffects FlipHorizonHandler(this Projectile projectile)
        {
            return projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        }
        /// <summary>
        /// 为你的射弹绘制一个发光描边。基于射弹本体颜色
        /// </summary>
        /// <param name="proj"></param>
        /// <param name="totalDrawTime"></param>
        /// <param name="posMove"></param>
        public static void QuickDrawBloomEdge(this Projectile proj, int totalDrawTime = 8, float rotOffset = 0, float posMove = 2f)
        {
            QuickDrawBloomEdge(proj, Color.White, totalDrawTime, rotOffset, posMove);
        }
        /// <summary>
        /// 为你的射弹绘制一个发光描边。基于射弹本体，重载输入颜色
        /// </summary>
        /// <param name="proj"></param>
        /// <param name="totalDrawTime"></param>
        /// <param name="posMove"></param>
        public static void QuickDrawBloomEdge(this Projectile proj, Color color, int totalDrawTime = 8, float rotOffset = 0, float posMove = 2f)
        {
            QuickDrawBloomEdge(proj, Color.White, proj.scale, totalDrawTime, rotOffset, posMove);
        }
        /// <summary>
        /// 为你的射弹绘制一个发光描边。基于射弹本体，重载输入颜色
        /// </summary>
        /// <param name="proj"></param>
        /// <param name="totalDrawTime"></param>
        /// <param name="posMove"></param>
        public static void QuickDrawBloomEdge(this Projectile proj, Color color, float scale, int totalDrawTime = 8, float rotOffset = 0, float posMove = 2f)
        {
            for (int i = 0; i < totalDrawTime; i++)
                Main.spriteBatch.Draw(proj.GetTexture(), proj.Center - Main.screenPosition + MathHelper.ToRadians(i * 60f).ToRotationVector2() * posMove, null, color with { A = 0 }, proj.rotation + rotOffset, proj.GetTexture().Size() / 2, scale, 0, 0f);
        }
        public static Texture2D GetTexture(this Projectile proj) => TextureAssets.Projectile[proj.type].Value;  
        /// <summary>
        /// 为顶点数据清理一些可能存在的无效定点。专门处理射弹情况
        /// </summary>
        /// <param name="proj"></param>
        /// <param name="validPos"></param>
        /// <param name="validRot"></param>
        /// <param name="rawPos"></param>
        /// <param name="rawRot"></param>
        public static void ClearInvaidData(this Projectile proj, out List<Vector2> validPos, out List<float> validRot, Vector2[] rawPos = null, float[] rawRot = null)
        {
            validPos = new List<Vector2>();
            validRot = new List<float>();
            Vector2[] posList = rawPos ?? proj.oldPos;
            float[] rotList = rawRot ?? proj.oldRot;
            ClearInvaidData(out validPos, out validRot, posList, rotList);
        }
        /// <summary>
        /// 为顶点数据清理一些可能存在的无效顶点（主要是零向量
        /// </summary>
        /// <param name="validPos"></param>
        /// <param name="validRot"></param>
        /// <param name="rawPos"></param>
        /// <param name="rawRot"></param>
        public static void ClearInvaidData(out List<Vector2> validPos, out List<float> validRot, Vector2[] rawPos, float[] rawRot)
        {
            validPos = [];
            validRot = [];
            for (int i = 0; i < rawPos.Length; i++)
            {
                if (rawPos[i] == Vector2.Zero)
                    continue;
                validPos.Add(rawPos[i]);
                validRot.Add(rawRot[i]);
            }
        }
    }
}
