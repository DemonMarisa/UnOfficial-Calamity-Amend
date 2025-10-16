using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.Shaders;
using UCA.Assets;
using UCA.Assets.Effects;
using UCA.Content.Particiles;
using UCA.Core.Enums;
using UCA.Core.Utilities;

namespace UCA.Content.Projectiles.HeldProj.Magic.ElementRayHeld
{
    public partial class ElementRaySpecialHeldProj
    {
        public void InitializeSolarBlade()
        {
            RelativeOwnerPos = new Vector2(10, 0);
            animationHelper.MaxAniProgress[AnimationState.Begin] = 30;
            animationHelper.MaxAniProgress[AnimationState.Middle] = 1000;
            animationHelper.MaxAniProgress[AnimationState.End] = 1000;
        }
        public void UpdateSolarBlade()
        {
            if (!animationHelper.HasFinish[AnimationState.Begin])
            {
                animationHelper.AniProgress[AnimationState.Begin]++;
                HandleBeginAni();
                if (animationHelper.AniProgress[AnimationState.Begin] >= animationHelper.MaxAniProgress[AnimationState.Begin])
                {
                    animationHelper.AniProgress[AnimationState.Begin] = 30;
                    animationHelper.HasFinish[AnimationState.Begin] = false;
                }
            }
            else if (!animationHelper.HasFinish[AnimationState.Middle])
            {
                animationHelper.AniProgress[AnimationState.Middle]++;

                if (animationHelper.AniProgress[AnimationState.Middle] == animationHelper.MaxAniProgress[AnimationState.Middle])
                    animationHelper.HasFinish[AnimationState.Middle] = true;
            }
            else if (!animationHelper.HasFinish[AnimationState.End])
            {
                animationHelper.AniProgress[AnimationState.End]++;

                if (animationHelper.AniProgress[AnimationState.End] == animationHelper.MaxAniProgress[AnimationState.End])
                    animationHelper.HasFinish[AnimationState.End] = true;
            }
            else
            {
                Projectile.Kill();
            }
        }
        public void HandleBeginAni()
        {
            int MaxAni = animationHelper.MaxAniProgress[AnimationState.Begin];
            int CurAni = animationHelper.AniProgress[AnimationState.Begin];
            // 使用缓动函数让动画更自然
            float easedProgress = EasingHelper.EaseOutCubic(CurAni / (float)MaxAni);
            // 设置起始与结束角度
            float startAngleOffset = MathHelper.ToRadians(25);
            float endAngleOffset = MathHelper.ToRadians(-120);
            // 计算基础旋转角度
            float baseRotation = MathHelper.Lerp(startAngleOffset, endAngleOffset, easedProgress);
            // 根据玩家方向进行镜像处理
            if (Owner.direction == -1)// 水平镜像
                baseRotation = baseRotation * Owner.direction;

            RelativeOwnerPosRot = baseRotation + ToMouseVector;
            Projectile.rotation = RelativeOwnerPosRot;
        }
        #region 绘制耀斑大剑
        public void DrawSolarBlade(Vector2 DrawPos, Vector2 offset, float DrawRot, Vector2 scale)
        {
            UCAUtilities.ReSetToBeginShader(BlendState.Additive);
            Main.graphics.GraphicsDevice.Textures[1] = UCATextureRegister.FireNoise.Value;
            PrepareShader(Color.Red, Color.OrangeRed, 0.08f, false);
            Vector2 GlowScale = new Vector2(1f, 1f) * scale;
            DrawBladeGlowSource(DrawPos + offset.RotatedBy(DrawRot), DrawRot, GlowScale);
            UCAUtilities.ReSetToEndShader();

            UCAUtilities.ReSetToBeginShader(BlendState.Additive);
            offset.Y = offset.Y + 56;
            PrepareShader(Color.OrangeRed, Color.Orange, 0.15f, true);
            Main.graphics.GraphicsDevice.Textures[1] = UCATextureRegister.FireNoise.Value;
            Vector2 MiddleBladeScale = new Vector2(0.5f, 1f) * scale;
            DrawMainBladeSource(DrawPos + offset.RotatedBy(DrawRot), DrawRot, MiddleBladeScale);
            UCAUtilities.ReSetToEndShader();

            UCAUtilities.ReSetToBeginShader(BlendState.Additive);
            // 准备shader
            PrepareShader(Color.Red, Color.OrangeRed, 0.15f, true);
            // 设置材质和偏移
            Main.graphics.GraphicsDevice.Textures[1] = UCATextureRegister.Wood.Value;
            Vector2 OutSideBladeScale = new Vector2(0.85f, 1f) * scale;
            DrawMainBladeSource(DrawPos + offset.RotatedBy(DrawRot), DrawRot, OutSideBladeScale);
            // 改一下置换材质
            Main.graphics.GraphicsDevice.Textures[1] = UCATextureRegister.FireNoise.Value;
            Vector2 InSideBladeScale = new Vector2(0.4f, 0.79f) * scale;
            offset.Y = offset.Y - 64;
            DrawAuxBladeSource(DrawPos + offset.RotatedBy(DrawRot), DrawRot, InSideBladeScale);
            UCAUtilities.ReSetToEndShader();
        }
        public void PrepareShader(Color beginColor, Color endColor, float uIntensity = 0.15f, bool useColor = true)
        {
            UCAShaderRegister.SolarBladeShader.Parameters["uTime"].SetValue(-Main.GlobalTimeWrappedHourly);
            UCAShaderRegister.SolarBladeShader.Parameters["uIntensity"].SetValue(uIntensity);
            UCAShaderRegister.SolarBladeShader.Parameters["ubeginColor"].SetValue(beginColor.ToVector4());
            UCAShaderRegister.SolarBladeShader.Parameters["uendColor"].SetValue(endColor.ToVector4());
            UCAShaderRegister.SolarBladeShader.Parameters["UseColor"].SetValue(useColor);
            UCAShaderRegister.SolarBladeShader.CurrentTechnique.Passes[0].Apply();
        }
        public void DrawMainBladeSource(Vector2 DrawPos, float DrawRot, Vector2 scale)
        {
            Texture2D SolarBlade = UCATextureRegister.SolarThinBlade.Value;
            Vector2 drawPosition = DrawPos - Main.screenPosition;
            Vector2 origin = new Vector2(SolarBlade.Size().X / 2, SolarBlade.Size().Y);
            Main.spriteBatch.Draw(SolarBlade, drawPosition, null, Color.White, DrawRot, origin, scale, SpriteEffects.None, 0);
        }

        public void DrawAuxBladeSource(Vector2 DrawPos, float DrawRot, Vector2 scale)
        {
            Texture2D SolarBlade = UCATextureRegister.SolarThinBlade.Value;
            Vector2 drawPosition = DrawPos - Main.screenPosition;
            Vector2 origin = new Vector2(SolarBlade.Size().X / 2, SolarBlade.Size().Y);
            Main.spriteBatch.Draw(SolarBlade, drawPosition, null, Color.White, DrawRot, origin, scale, SpriteEffects.None, 0);
        }

        public void DrawBladeGlowSource(Vector2 DrawPos, float DrawRot, Vector2 scale)
        {
            Texture2D SolarBlade = UCATextureRegister.SolarBladeGlowMask.Value;
            Vector2 drawPosition = DrawPos - Main.screenPosition;
            Vector2 origin = new Vector2(SolarBlade.Size().X / 2, SolarBlade.Size().Y);
            Main.spriteBatch.Draw(SolarBlade, drawPosition, null, Color.White, DrawRot, origin, scale, SpriteEffects.None, 0);
        }
        #endregion
    }
}
