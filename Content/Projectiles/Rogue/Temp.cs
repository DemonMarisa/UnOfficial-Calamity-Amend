using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using UCA.Core.GlobalInstance.Projectiles;

namespace UCA.Content.Projectiles.Rogue
{
    public static class Temp
    {
        public static float ToClamp(this float value, float min = 0f, float max = 1f) => MathHelper.Clamp(value, min, max);
        /// <summary>
        /// 让任意”实体“安全转向至你需要的位置。如果实体位置不存在，会默认处置为Vector0避免崩溃
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static Vector2 DirectionToSafe(this Entity entity, Vector2 position)
        {
            Vector2 dir = entity.DirectionTo(position);
            if (dir.HasNaNs()) dir = Vector2.Zero;
            return dir;
        }
        public static void BeginDefault(this SpriteBatch SB) =>
            SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        /// <summary>
        /// 快速生成一个简单明了的圆形粒子组
        /// </summary>
        /// <param name="dPos"></param>
        /// <param name="dCounts"></param>
        /// <param name="dScale"></param>
        /// <param name="dType"></param>
        /// <param name="dSpeed"></param>
        /// <param name="dPosOffset"></param>
        /// <param name="dGrav"></param>
        /// <param name="dAlpha"></param>
        public static void CirclrDust(this Vector2 dPos, int dCounts, float dScale, int dType, int dSpeed, float dPosOffset = 0f, bool dGrav = true, int dAlpha = 255)
        {
            float rotArg = 360f / dCounts;
            for (int i = 0; i < dCounts; i++)
            {
                float rot = MathHelper.ToRadians(i * rotArg);
                Vector2 offsetPos = new Vector2(dPosOffset, 0f).RotatedBy(rot);
                Vector2 dVel = new Vector2(dSpeed, 0f).RotatedBy(rot);
                Dust d = Dust.NewDustPerfect(dPos + offsetPos, dType, dVel);
                d.noGravity = dGrav;
                d.velocity = dVel;
                d.scale = dScale;
                d.alpha = dAlpha;
            }
        }
        /// <summary>
        /// 使射弹较为平滑地冲向一个地点。
        /// </summary>
        /// <param name="proj"></param>
        /// <param name="targetPosition"></param>
        /// <param name="speed"></param>
        /// <param name="acceleration"></param>
        /// <param name="killDistance"></param>
        public static void AccelerateToTarget(this Projectile proj, Vector2 targetPosition, float speed, float acceleration, int killDistance = 0)
        {
            Vector2 dist = targetPosition - proj.Center;
            float distLength = dist.Length();
            distLength = speed / distLength;
            dist.X *= distLength;
            dist.Y *= distLength;
            if (proj.velocity.X < dist.X)
            {
                proj.velocity.X += acceleration;
                if (proj.velocity.X < 0f && dist.X > 0f)
                    proj.velocity.X += acceleration;
            }
            else if (proj.velocity.X > dist.X)
            {
                proj.velocity.X -= acceleration;
                if (proj.velocity.X > 0f && dist.X < 0f)
                    proj.velocity.X -= acceleration;
            }
            if (proj.velocity.Y < dist.Y)
            {
                proj.velocity.Y += acceleration;
                if (proj.velocity.Y < 0f && dist.Y > 0f)
                    proj.velocity.Y += acceleration;
            }
            else if (proj.velocity.Y > dist.Y)
            {
                proj.velocity.Y -= acceleration;
                if (proj.velocity.Y > 0f && dist.Y < 0f)
                    proj.velocity.Y -= acceleration;
            }
        }
        /// <summary>
        /// 新的追踪方法，这个会指定一个NPC, 且可以自定义输入额外更新，以及强制速度不受距离影响
        /// 目前没有角度限制等一类的东西，如果需要则可以补上。
        /// </summary>
        /// <param name="proj">射弹</param>
        /// <param name="target">射弹目标</param>
        /// <param name="distRequired">最大范围</param>
        /// <param name="speed">射弹速度</param>
        /// <param name="inertia">惯性</param>
        /// <param name="giveExtraUpdate">给予额外更新，默认1</param>
        /// <param name="forceSpeed">指定射弹无视距离，使射弹使用你输入的速度。这个效果有一个距离特判，即距离比你输入的射弹速度还短的时候才会生效, 一般可无视。</param>
        /// <param name="maxAngleChage">角度限制，默认为空. </param>
        /// <param name="ignoreDist">使这个射弹无视索敌距离(distRequired), 默认取否. </param>
        public static void HomingNPCBetter(this Projectile proj, NPC target, float distRequired, float speed, float inertia, int giveExtraUpdate = 0, float? forceSpeed = null, float? maxAngleChage = null, bool ignoreDist = false)
        {
            //一般来说你用这个方法就说明target理论上应当可以被追，但……just in case
            if (!proj.friendly || target == null || !target.active)
                return;
            bool canHome;

            float curDist = Vector2.Distance(target.Center, proj.Center);
            //存储射弹当前额外更新
            if (proj.GetGlobalProjectile<UCAGlobalProj>().StoredEU == -1)
                proj.GetGlobalProjectile<UCAGlobalProj>().StoredEU = proj.extraUpdates;

            if (!target.chaseable || curDist > distRequired && !ignoreDist)
                canHome = false;
            else
                canHome = true;
            if (canHome)
            {
                //给予额外更新
                proj.extraUpdates = proj.GetGlobalProjectile<UCAGlobalProj>().StoredEU + giveExtraUpdate;
                //开始追踪target
                Vector2 home = (target.Center - proj.Center).SafeNormalize(Vector2.UnitY);
                Vector2 velo = (proj.velocity * inertia + home * speed) / (inertia + 1f);
                //这里给了一个角度限制
                if (maxAngleChage.HasValue)
                {
                    float curAngle = proj.velocity.ToRotation();
                    float tarAngle = velo.ToRotation();
                    float angleDiffer = MathHelper.WrapAngle(tarAngle - curAngle);
                    //转弧度
                    float maxRadians = MathHelper.ToRadians(maxAngleChage.Value);
                    if (Math.Abs(angleDiffer) > maxRadians)
                    {
                        float clampedAngle = curAngle + Math.Sign(angleDiffer) * maxRadians;
                        float setSpeed = velo.Length();
                        velo = new Vector2((float)Math.Cos(clampedAngle), (float)Math.Sin(clampedAngle)) * setSpeed;
                    }
                }
                //除非你当前距离比射弹速度还少, 我们才会重新设定速度
                if (forceSpeed.HasValue && curDist < speed)
                    velo = proj.velocity.SafeNormalize(Vector2.Zero) * home * forceSpeed.Value;
                //设定速度
                proj.velocity = velo;
            }
            //否则返回射弹原本的额外更新
            else
                proj.extraUpdates = proj.GetGlobalProjectile<UCAGlobalProj>().StoredEU;
        }
        /// <summary>
        /// 重载追踪方法，直接快速设定无视距离的追踪
        /// </summary>
        /// <param name="proj"></param>
        /// <param name="target"></param>
        /// <param name="speed"></param>
        /// <param name="inertia"></param>
        /// <param name="giveExtraUpdate"></param>
        /// <param name="forceSpeed"></param>
        /// <param name="maxAngleChage"></param>
        public static void HomingNPCBetter(this Projectile proj, NPC target, float speed, float inertia, int giveExtraUpdate = 0, float? forceSpeed = null, float? maxAngleChage = null) => proj.HomingNPCBetter(target, 1f, speed, inertia, giveExtraUpdate, forceSpeed, maxAngleChage, true);
        /// <summary>
        /// 数学公式：将角度转化为椭圆上的一个点
        /// </summary>
        /// <param name="radians">当前点的弧度</param>
        /// <param name="shortAxis">半短轴长度(短半径)</param>
        /// <param name="longAxis">半长轴长度(长半径)</param>
        /// <param name="rotation">椭圆整体旋转角度(弧度)</param>
        /// <returns>椭圆上相对于原点的点坐标</returns>
        public static Vector2 ToEllipseVector2Edge(this float radians, float shortAxis, float longAxis, float rotation = 0f)
        {
            float x = longAxis * (float)Math.Cos(radians);
            float y = shortAxis * (float)Math.Sin(radians);
            float cosRot = (float)Math.Cos(rotation);
            float sinRot = (float)Math.Sin(rotation);
            float rotX = x * cosRot - y * sinRot;
            float rotY = x * sinRot + y * cosRot;
            return new Vector2(rotX, rotY);
        }
        public static string UCALocalPrefix => "Mods.UCA.";
        /// <summary>
        /// 干翻所有Tooltip，并借助本地化完全重写一次
        /// </summary>
        /// <param name="tooltips"></param>
        /// <param name="replacedTextPath"></param>
        public static void ReplaceAllTooltip(this List<TooltipLine> tooltips, string replacedTextPath) =>
                ReplaceAllTooltip(tooltips, replacedTextPath, Color.White);
        public static void ReplaceAllTooltip(this List<TooltipLine> tooltips, string replacedTextPath, Color textColor)
        {
            tooltips.RemoveAll((line) => line.Mod == "Terraria" && line.Name != "Tooltip0" && line.Name.StartsWith("Tooltip"));
            TooltipLine getTooltip = tooltips.FirstOrDefault((x) => x.Name == "Tooltip0" && x.Mod == "Terraria");
            string formateText = replacedTextPath.ToLangValue();
            if (getTooltip is not null)
            {
                getTooltip.Text = formateText;
                getTooltip.OverrideColor = textColor;
            }
        }
        public static void ReplaceAllTooltip(this List<TooltipLine> tooltips, string replacedTextPath, params object[] args)
            => ReplaceAllTooltip(tooltips, replacedTextPath, Color.White, args);
        /// <summary>
        /// 干翻所有Tooltip，并借助本地化完全重写一次，重载染色，附带键入值
        /// </summary>
        /// <param name="tooltips"></param>
        /// <param name="replacedTextPath"></param>
        /// <param name="args"></param>
        public static void ReplaceAllTooltip(this List<TooltipLine> tooltips, string replacedTextPath, Color textColor, params object[] args)
        {
            tooltips.RemoveAll((line) => line.Mod == "Terraria" && line.Name != "Tooltip0" && line.Name.StartsWith("Tooltip"));
            TooltipLine getTooltip = tooltips.FirstOrDefault((x) => x.Name == "Tooltip0" && x.Mod == "Terraria");
            string formateText = replacedTextPath.ToLangValue().ToFormatValue(args);
            if (getTooltip is not null)
            {
                getTooltip.Text = formateText;
                getTooltip.OverrideColor = textColor;
            }

        }
        /// <summary>
        /// 从最后一行Tooltip后插入值，需填入本地化路径
        /// </summary>
        /// <param name="tooltips"></param>
        /// <param name="textPath"></param>
        /// <param name="mod">该段文本所属的模组，默认值null，将直接选定为本mod</param>
        /// <param name="LineName">为这一行tooltip起名，默认UCA</param>
        public static void QuickAddTooltip(this List<TooltipLine> tooltips, string textPath, Mod mod = null, string LineName = "UCA")
        {
            string text = textPath.ToLangValue();
            Mod tooltipMod = mod ?? UCA.Instance;
            var newLine = new TooltipLine(tooltipMod, LineName, text)
            {
                OverrideColor = tooltips.Count > 0 ? tooltips[^1].OverrideColor : Color.White
            };
            if (tooltips.Count is 0)
                tooltips.Add(newLine);
            else
                tooltips.Insert(tooltips.Count, newLine);
        }
        /// <summary>
        /// 从最后一行Tooltip后插入值，需填入本地化路径，重载传参方法
        /// </summary>
        /// <param name="tooltips"></param>
        /// <param name="textPath"></param>
        /// <param name="mod">该段文本所属的模组，默认值null，将直接选定为本mod</param>
        /// <param name="LineName">为这一行tooltip起名，默认CEMod</param>
        public static void QuickAddTooltip(this List<TooltipLine> tooltips, string textPath, Mod mod = null, string LineName = "UCAMod", params object[] args)
        {
            string text = textPath.ToLangValue().ToFormatValue(args);
            Mod tooltipMod = mod ?? UCA.Instance;
            var newLine = new TooltipLine(tooltipMod, LineName, text)
            {
                OverrideColor = tooltips.Count > 0 ? tooltips[^1].OverrideColor : Color.White
            };
            if (tooltips.Count is 0)
                tooltips.Add(newLine);
            else
                tooltips.Insert(tooltips.Count, newLine);
        }
        /// <summary>
        /// 从最后一行Tooltip后插入值，需填入本地化路径，重载颜色代码
        /// </summary>
        /// <param name="tooltips"></param>
        /// <param name="textPath"></param>
        /// <param name="mod">该段文本所属的模组，默认值null，将直接选定为本mod</param>
        /// <param name="LineName">为这一行tooltip起名，默认CEMod</param>
        public static void QuickAddTooltip(this List<TooltipLine> tooltips, string textPath, Color color, Mod mod = null, string LineName = "UCAMod")
        {
            string text = textPath.ToLangValue();
            Mod tooltipMod = mod ?? UCA.Instance;
            var newLine = new TooltipLine(tooltipMod, LineName, text)
            {
                OverrideColor = color
            };
            if (tooltips.Count is 0)
                tooltips.Add(newLine);
            else
                tooltips.Insert(tooltips.Count, newLine);
        }
        /// <summary>
        /// 从最后一行Tooltip后插入值，需填入本地化路径，重载传参方法，颜色代码
        /// </summary>
        /// <param name="tooltips"></param>
        /// <param name="textPath"></param>
        /// <param name="mod">该段文本所属的模组，默认值null，将直接选定为本mod</param>
        /// <param name="LineName">为这一行tooltip起名，默认CEMod</param>
        public static void QuickAddTooltip(this List<TooltipLine> tooltips, string textPath, Color color, Mod mod = null, string LineName = "UCAMod", params object[] args)
        {
            string text = textPath.ToLangValue().ToFormatValue(args);
            Mod tooltipMod = mod ?? UCA.Instance;
            var newLine = new TooltipLine(tooltipMod, LineName, text)
            {
                OverrideColor = color
            };
            if (tooltips.Count is 0)
                tooltips.Add(newLine);
            else
                tooltips.Insert(tooltips.Count, newLine);
        }
        /// <summary>
        /// 从最后一行Tooltip后插入值，需直接传入需要的文本内容而不是对应的本地化路径
        /// </summary>
        /// <param name="tooltips"></param>
        /// <param name="textValue"></param>
        /// <param name="mod">该段文本所属的模组，默认值null，将直接选定为本mod</param>
        /// <param name="LineName">为这一行tooltip起名，默认CEMod</param>
        public static void QuickAddTooltipDirect(this List<TooltipLine> tooltips, string textValue, Mod mod = null, string LineName = "UCAMod")
        {
            Mod tooltipMod = mod ?? UCA.Instance;
            var newLine = new TooltipLine(tooltipMod, LineName, textValue)
            {
                OverrideColor = tooltips.Count > 0 ? tooltips[^1].OverrideColor : Color.White
            };
            if (tooltips.Count is 0)
                tooltips.Add(newLine);
            else
                tooltips.Insert(tooltips.Count, newLine);
        }
        /// <summary>
        /// 从最后一行Tooltip后插入值，需直接传入需要的文本内容而不是对应的本地化路径，重载传参方法
        /// </summary>
        /// <param name="tooltips"></param>
        /// <param name="textValue"></param>
        /// <param name="mod">该段文本所属的模组，默认值null，将直接选定为本mod</param>
        /// <param name="LineName">为这一行tooltip起名，默认CEMod</param>
        public static void QuickAddTooltipDirect(this List<TooltipLine> tooltips, string textValue, Mod mod = null, string LineName = "UCAMod", params object[] args)
        {
            string text = textValue.ToFormatValue(args);
            Mod tooltipMod = mod ?? UCA.Instance;
            var newLine = new TooltipLine(tooltipMod, LineName, text)
            {
                OverrideColor = tooltips.Count > 0 ? tooltips[^1].OverrideColor : Color.White
            };
            if (tooltips.Count is 0)
                tooltips.Add(newLine);
            else
                tooltips.Insert(tooltips.Count, newLine);
        }
        /// <summary>
        /// 从最后一行Tooltip后插入值，需直接传入需要的文本内容而不是对应的本地化路径，重载颜色代码
        /// </summary>
        /// <param name="tooltips"></param>
        /// <param name="textValue">文本内容</param>
        /// <param name="mod">该段文本所属的模组，默认值null，将直接选定为本mod</param>
        /// <param name="LineName">为这一行tooltip起名，默认CEMod</param>
        public static void QuickAddTooltipDirect(this List<TooltipLine> tooltips, string textValue, Color color, Mod mod = null, string LineName = "UCAMod")
        {
            string text = textValue.ToLangValue();
            Mod tooltipMod = mod ?? UCA.Instance;
            var newLine = new TooltipLine(tooltipMod, LineName, text)
            {
                OverrideColor = color
            };
            if (tooltips.Count is 0)
                tooltips.Add(newLine);
            else
                tooltips.Insert(tooltips.Count, newLine);
        }
        /// <summary>
        /// 从最后一行Tooltip后插入值，需直接传入需要的文本内容而不是对应的本地化路径，需直接传入需要的文本内容而不是对应的本地化路径，重载传参方法，颜色代码
        /// </summary>
        /// <param name="tooltips"></param>
        /// <param name="textValue">文本内容</param>
        /// <param name="mod">该段文本所属的模组，默认值null，将直接选定为本mod</param>
        /// <param name="LineName">为这一行tooltip起名，默认CEMod</param>
        public static void QuickAddTooltipDirect(this List<TooltipLine> tooltips, string textValue, Color color, Mod mod = null, string LineName = "UCAMod", params object[] args)
        {
            string text = textValue.ToFormatValue(args);
            Mod tooltipMod = mod ?? UCA.Instance;
            var newLine = new TooltipLine(tooltipMod, LineName, text)
            {
                OverrideColor = color
            };
            if (tooltips.Count is 0)
                tooltips.Add(newLine);
            else
                tooltips.Insert(tooltips.Count, newLine);
        }
        /// <summary>
        /// 将整型、浮点与双精度直接变成带百分比符号的字符串，用于进行Tooltip的插值。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToPercentReal(this object obj)
        {
            if (obj is int interga)
                return $"{interga}%";
            if (obj is float floatSingle)
                return $"{(int)(floatSingle * 100f)}%";
            if (obj is double doubleSingle)
                return $"{(int)(doubleSingle * 100)}%";
            return "转化出错";

        }
        public static string ToHexColor(this Color color) => $"{color.R:X2}{color.G:X2}{color.B:X2}";

        public static string ToLangValue(this string textPath) => Language.GetTextValue(textPath);

        public static string ToFormatValue(this string baseTextValue, params object[] args)
        {
            try
            {
                return string.Format(baseTextValue, args);
            }
            catch
            {
                return baseTextValue + "格式化出错";
            }
        }
        public static bool GetNPCByWorldPos(this Vector2 searchPos, out NPC target, float halfRadians, bool ignoreTiles = true)
        {
            float distStoraged = halfRadians;
            NPC acceptableTarget = null;
            foreach (NPC npc in Main.ActiveNPCs)
            {
                float exDist = npc.width + npc.height;
                //单位不可被追踪 或者 超出索敌距离则continue
                if (Vector2.Distance(searchPos, npc.Center) > distStoraged + exDist)
                    continue;

                if (!npc.active || npc.friendly || npc.lifeMax < 5)
                    continue;

                //搜索符合条件的敌人, 准备返回这个NPC实例
                float curNpcDist = Vector2.Distance(npc.Center, searchPos);
                if (curNpcDist < distStoraged && (ignoreTiles || Collision.CanHit(searchPos, 1, 1, npc.Center, 1, 1)))
                {
                    distStoraged = curNpcDist;
                    acceptableTarget = npc;
                }
            }
            target = acceptableTarget;
            //返回这个NPC实例
            return acceptableTarget != null;
        }
    }
}
