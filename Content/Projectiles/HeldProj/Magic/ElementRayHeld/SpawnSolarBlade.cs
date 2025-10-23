using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using UCA.Assets;
using UCA.Assets.Effects;
using UCA.Assets.Sounds;
using UCA.Content.DrawNodes;
using UCA.Content.Particiles;
using UCA.Core.Enums;
using UCA.Core.Graphics.Primitives.Trail;
using UCA.Core.Utilities;

namespace UCA.Content.Projectiles.HeldProj.Magic.ElementRayHeld
{
    public partial class ElementRaySpecialHeldProj
    {
        public float SolarBladeXScale = 0;
        public int SolarBladeXOffset = 128;
        public void InitializeSolarBlade()
        {
            MainFragmentOffset = new Vector2(0, 0);
            AuxFragmentOffset = new Vector2(0, -0);
            FilpAuxFragmentOffset = new Vector2(0, 0);
            SolarBladeXOffset = 128;
            Projectile.damage *= 10;

            RelativeOwnerPos = new Vector2(10, 0);
            animationHelper.MaxAniProgress[AnimationState.Begin] = 30;
            animationHelper.MaxAniProgress[AnimationState.Middle] = 25;
            animationHelper.MaxAniProgress[AnimationState.End] = 100;
            SoundEngine.PlaySound(SoundsMenu.MAGNOLIASPRelease, Projectile.Center);
            SoundEngine.PlaySound(SoundsMenu.MagicStaffCharge, Projectile.Center);
        }
        public void UpdateSolarBlade()
        {
            if (!animationHelper.HasFinish[AnimationState.Begin])
            {
                animationHelper.UpDateAni(AnimationState.Begin, 35);
                HandleSolorBeginAni();
            }
            else if (!animationHelper.HasFinish[AnimationState.Middle])
            {
                if (animationHelper.AniProgress[AnimationState.Middle] < animationHelper.MaxAniProgress[AnimationState.Middle])
                    animationHelper.AniProgress[AnimationState.Middle]++;

                HandleSolorMiddleAni();

                if (animationHelper.AniProgress[AnimationState.Middle] >= animationHelper.MaxAniProgress[AnimationState.Middle] && !Owner.UCA().MouseRight)
                {
                    animationHelper.HasFinish[AnimationState.Middle] = true;
                    BeginRot = ToMouseVector;
                    CanChangeDir = false;
                    candamage = true;
                    SoundStyle sound = SoundsMenu.NightRayHit;
                    sound.Pitch = 2f;
                    SoundEngine.PlaySound(sound, Projectile.Center);
                    SoundEngine.PlaySound(SoundsMenu.SoulGreatSwordSwimg, Projectile.Center);
                }
            }
            else if (!animationHelper.HasFinish[AnimationState.End])
            {
                animationHelper.AniProgress[AnimationState.End]++;
                Projectile.extraUpdates = 10;
                HandleSolorEndAni();
                if (animationHelper.AniProgress[AnimationState.End] >= animationHelper.MaxAniProgress[AnimationState.End])
                {
                    SpawnDust();
                    animationHelper.HasFinish[AnimationState.End] = true;
                }
            }
            else
            {
                FollowOwner = false;
                CanDraw = false;
                Projectile.Kill();
                SoundStyle sound = SoundsMenu.FireBlast;
                sound.Volume = 0.4f;
                sound.Pitch = 0.2f;
                SoundEngine.PlaySound(sound, Projectile.Center);
            }
            if (animationHelper.HasFinish[AnimationState.Begin] && Time % 20 == 0)
                SoundEngine.PlaySound(SoundsMenu.Fire, Projectile.Center);

            if (animationHelper.HasFinish[AnimationState.Begin] && Time % 2 == 0 && CanDraw)
            {
                Vector2 beginSpawnPos = Projectile.Center + new Vector2(64, Main.rand.Next(-100, 100)).RotatedBy(Projectile.rotation);
                Vector2 EndSpawnPos = Projectile.Center + new Vector2(SolarBladeXOffset + 720, Main.rand.Next(-100, 100)).RotatedBy(Projectile.rotation);
                Color DrawColor = Color.Lerp(Color.Orange, Color.OrangeRed, Main.rand.NextFloat());
                new MediumGlowBall(Vector2.Lerp(beginSpawnPos, EndSpawnPos, Main.rand.NextFloat()), Vector2.Zero, DrawColor, Main.rand.Next(100, 200), Main.rand.NextFloat(MathHelper.TwoPi), 1f, 0.2f, Main.rand.NextFloat(2f, 4f)).Spawn();
            }
        }
        #region 处理开始动画
        public void HandleSolorBeginAni()
        {
            int MaxAni = animationHelper.MaxAniProgress[AnimationState.Begin];
            int CurAni = animationHelper.AniProgress[AnimationState.Begin];
            // 使用缓动函数让动画更自然
            float easedProgress = EasingHelper.EaseOutCubic(CurAni / (float)MaxAni);
            // 设置起始与结束角度
            float startAngleOffset = MathHelper.ToRadians(45);
            float endAngleOffset = MathHelper.ToRadians(-145);
            // 计算基础旋转角度
            float baseRotation = MathHelper.Lerp(startAngleOffset, endAngleOffset, easedProgress);
            // 根据玩家方向进行镜像处理
            if (Owner.direction == -1)// 水平镜像
                baseRotation = baseRotation * Owner.direction;

            RelativeOwnerPosRot = baseRotation + ToMouseVector;
            Projectile.rotation = RelativeOwnerPosRot;

            if (CurAni < MaxAni / 2)
            {
                float beginrot = Main.rand.NextFloat(0, MathHelper.TwoPi);
                float rotSpeed = Main.rand.NextBool() ? 0.07f : -0.07f;
                int length = Main.rand.Next(250, 500);
                int LifeTime = Main.rand.Next(30, 60);
                Vector2 offset = new Vector2(50, 0);
                new ProjAbsorbGlowBall(Owner.Center, Color.OrangeRed, LifeTime, 0.1f, beginrot, rotSpeed, Projectile.whoAmI, length, offset).Spawn();
            }

            if (CurAni == 1)
            {
                int LifeTime = 60;
                Vector2 offset = new Vector2(50, 0);
                new FollowProjCrossGlow(Owner.Center, Color.OrangeRed, LifeTime, 0.8f, Projectile.whoAmI, offset).Spawn();
                new FollowProjCrossGlow(Owner.Center, Color.Orange, LifeTime, 0.4f, Projectile.whoAmI, offset).Spawn();
            }
        }
        #endregion
        #region 处理中间的动画
        public void HandleSolorMiddleAni()
        {
            int MaxAni = animationHelper.MaxAniProgress[AnimationState.Middle];
            int CurAni = animationHelper.AniProgress[AnimationState.Middle];
            // 使用缓动函数让动画更自然
            float easedProgress = EasingHelper.EaseOutCubic(CurAni / (float)MaxAni);

            SolarBladeXScale = easedProgress;
            if (CurAni == 1)
            {
                SoundStyle sound = SoundsMenu.FireBlast;
                sound.Volume = 1f;
                sound.Pitch = 1f;
                SoundEngine.PlaySound(sound, Projectile.Center);
                SpawnDust();
                int LifeTime = 60;
                Vector2 offset = new Vector2(96, 0);
                new FollowProjCrossGlow(Owner.Center, Color.OrangeRed, LifeTime, 1.4f, Projectile.whoAmI, offset).Spawn();
                new FollowProjCrossGlow(Owner.Center, Color.Orange, LifeTime, 1f, Projectile.whoAmI, offset).Spawn();
            }
            float baseRotation = MathHelper.ToRadians(-145);
            if (Owner.direction == -1)// 水平镜像
                baseRotation = baseRotation * Owner.direction;
            RelativeOwnerPosRot = baseRotation + ToMouseVector;
            Projectile.rotation = RelativeOwnerPosRot;
        }
        #endregion
        #region 处理结束的动画
        public void HandleSolorEndAni()
        {
            int MaxAni = animationHelper.MaxAniProgress[AnimationState.End];
            int CurAni = animationHelper.AniProgress[AnimationState.End];
            // 使用缓动函数让动画更自然
            float easedProgress = EasingHelper.EaseInCubic(CurAni / (float)MaxAni);
            // 设置起始与结束角度
            float startAngleOffset = MathHelper.ToRadians(-145);
            float endAngleOffset = MathHelper.ToRadians(145);
            // 计算基础旋转角度
            float baseRotation = MathHelper.Lerp(startAngleOffset, endAngleOffset, easedProgress);
            // 根据玩家方向进行镜像处理
            if (Owner.direction == -1)// 水平镜像
                baseRotation = baseRotation * Owner.direction;

            RelativeOwnerPosRot = baseRotation + BeginRot;
            Projectile.rotation = RelativeOwnerPosRot;

            if (Time % 2 == 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    Vector2 beginSpawnPos = Projectile.Center + new Vector2(64, 0).RotatedBy(Projectile.rotation);
                    Vector2 EndSpawnPos = Projectile.Center + new Vector2(SolarBladeXOffset + 720, 0).RotatedBy(Projectile.rotation);
                    float Progress = i / 10f;
                    Vector2 firVel = Vector2.UnitX.RotatedBy(Projectile.rotation + MathHelper.PiOver2) * 9 * Main.rand.NextFloat(0, 2f) * Owner.direction;
                    Color DrawColor = Color.Lerp(Color.Orange, Color.OrangeRed, Main.rand.NextFloat());
                    new Fire(Vector2.Lerp(beginSpawnPos, EndSpawnPos, Progress), firVel, DrawColor, Main.rand.Next(45, 55), Main.rand.NextFloat(MathHelper.TwoPi), 1f, 0.8f).Spawn();
                }
                for (int i = 0; i < 10; i++)
                {
                    Vector2 beginSpawnPos = Projectile.Center + new Vector2(64, 0).RotatedBy(Projectile.rotation);
                    Vector2 EndSpawnPos = Projectile.Center + new Vector2(SolarBladeXOffset + 720, 0).RotatedBy(Projectile.rotation);
                    Vector2 firVel = Vector2.UnitX.RotatedBy(Projectile.rotation + MathHelper.PiOver2) * 6 * Main.rand.NextFloat(0, 2f) * Owner.direction;
                    Color DrawColor = Color.Lerp(Color.Orange, Color.OrangeRed, Main.rand.NextFloat());
                    new TrailGlowBall(Vector2.Lerp(beginSpawnPos, EndSpawnPos, Main.rand.NextFloat()), firVel, DrawColor, Main.rand.Next(45, 65), 0.4f).Spawn();
                }
            }
            Vector2 beginSpawnPos2 = Projectile.Center + new Vector2(64, Main.rand.Next(-100, 100)).RotatedBy(Projectile.rotation);
            Vector2 EndSpawnPos2 = Projectile.Center + new Vector2(SolarBladeXOffset + 720, Main.rand.Next(-100, 100)).RotatedBy(Projectile.rotation);
            Color DrawColor2 = Color.Lerp(Color.Orange, Color.OrangeRed, Main.rand.NextFloat());
            new MediumGlowBall(Vector2.Lerp(beginSpawnPos2, EndSpawnPos2, Main.rand.NextFloat()), Vector2.Zero, DrawColor2, Main.rand.Next(100, 200), Main.rand.NextFloat(MathHelper.TwoPi), 1f, 0.2f, Main.rand.NextFloat(2f, 4f)).Spawn();
        }
        #endregion
        #region 更新耀斑模式的碎片位置
        public void UpdateSolarFragmentOffset()
        {
            if (!animationHelper.HasFinish[AnimationState.Begin])
            {
                UpdateFragContractile();
            }
            else
            {
                UpDateFragRelease();
            }
        }
        public void UpdateFragContractile()
        {
            int MaxAni = animationHelper.MaxAniProgress[AnimationState.Begin];
            int CurAni = animationHelper.AniProgress[AnimationState.Begin];
            float Progress = CurAni / (float)MaxAni;
            // 更新主碎片
            Vector2 MainFragTarget = Vector2.Lerp(new Vector2(260, 0), new Vector2(48, 0), EasingHelper.EaseOutCubic(Progress)).RotatedBy(Projectile.rotation);
            MainFragmentOffset = Vector2.Lerp(MainFragmentOffset, MainFragTarget, 0.2f);
            MainFragmentRot = Projectile.rotation + (Projectile.spriteDirection == -1 ? MathHelper.PiOver2 + MathHelper.PiOver4 : MathHelper.PiOver4);
            // 通过这个限制高度乘数，营造出螺旋的效果
            float HeightCap = (float)Math.Sin(Progress * MathHelper.Pi);
            float SinProgress = (float)Math.Sin(Progress * MathHelper.TwoPi * 1.5f) * 2 * HeightCap;
            // 更新左侧碎片
            Vector2 AuxFragTarget = Vector2.Lerp(new Vector2(220, 0), new Vector2(48, SinProgress * -36), Progress).RotatedBy(Projectile.rotation);
            AuxFragmentOffset = Vector2.Lerp(AuxFragmentOffset, AuxFragTarget, 0.3f);
            Vector2 AuxFragWorldPos = Projectile.Center + AuxFragmentOffset;
            AuxFragmentRot = UCAUtilities.GetVector2(AuxFragWorldPos, Projectile.Center).ToRotation() - MathHelper.PiOver4;
            // 更新右侧碎片
            Vector2 FilpAuxFragTarget = Vector2.Lerp(new Vector2(220, 0), new Vector2(48, SinProgress * 36), Progress).RotatedBy(Projectile.rotation);
            FilpAuxFragmentOffset = Vector2.Lerp(FilpAuxFragmentOffset, FilpAuxFragTarget, 0.3f);
            Vector2 FilpAuxFragWorldPos = Projectile.Center + FilpAuxFragmentOffset;
            FilpAuxFragmentRot = UCAUtilities.GetVector2(FilpAuxFragWorldPos, Projectile.Center).ToRotation() + MathHelper.PiOver4;
        }
        public void UpDateFragRelease()
        {
            // 更新主碎片
            Vector2 MainFragTarget = new Vector2(96, 0).RotatedBy(Projectile.rotation);
            MainFragmentOffset = Vector2.Lerp(MainFragmentOffset, MainFragTarget, 0.2f);
            MainFragmentRot = Projectile.rotation + (Projectile.spriteDirection == -1 ? MathHelper.PiOver2 + MathHelper.PiOver4 : MathHelper.PiOver4);
            // 更新左侧碎片
            Vector2 AuxFragTarget = new Vector2(68, -36).RotatedBy(Projectile.rotation);
            AuxFragmentOffset = Vector2.Lerp(AuxFragmentOffset, AuxFragTarget, 0.2f);
            Vector2 AuxFragWorldPos = Projectile.Center + AuxFragmentOffset;
            AuxFragmentRot = UCAUtilities.GetVector2(AuxFragWorldPos, Projectile.Center).ToRotation() - MathHelper.PiOver4;
            // 更新右侧碎片
            Vector2 FilpAuxFragTarget = new Vector2(68, 36).RotatedBy(Projectile.rotation);
            FilpAuxFragmentOffset = Vector2.Lerp(FilpAuxFragmentOffset, FilpAuxFragTarget, 0.2f);
            Vector2 FilpAuxFragWorldPos = Projectile.Center + FilpAuxFragmentOffset;
            FilpAuxFragmentRot = UCAUtilities.GetVector2(FilpAuxFragWorldPos, Projectile.Center).ToRotation() + MathHelper.PiOver4;
        }
        #endregion
        #region 绘制耀斑大剑
        public void DrawSolarBlade(Vector2 DrawPos, Vector2 offset, float DrawRot, Vector2 scale)
        {
            UCAUtilities.ReSetToBeginShader(BlendState.Additive);
            Main.graphics.GraphicsDevice.Textures[1] = UCATextureRegister.FireNoise.Value;
            PrepareShader(Color.Red, Color.OrangeRed, 0.08f, false);
            Vector2 GlowScale = new Vector2(1f * SolarBladeXScale, 1f) * scale;
            DrawBladeGlowSource(DrawPos + offset.RotatedBy(DrawRot), DrawRot, GlowScale);
            UCAUtilities.ReSetToEndShader();
            
            UCAUtilities.ReSetToBeginShader(BlendState.Additive);
            offset.Y = offset.Y + 56;
            PrepareShader(Color.OrangeRed, Color.Orange, 0.15f, true);
            Main.graphics.GraphicsDevice.Textures[1] = UCATextureRegister.FireNoise.Value;
            Vector2 MiddleBladeScale = new Vector2(0.5f * SolarBladeXScale, 1f) * scale;
            DrawMainBladeSource(DrawPos + offset.RotatedBy(DrawRot), DrawRot, MiddleBladeScale);
            UCAUtilities.ReSetToEndShader();

            UCAUtilities.ReSetToBeginShader(BlendState.Additive);
            // 准备shader
            PrepareShader(Color.Red, Color.OrangeRed, 0.15f, true);
            // 设置材质和偏移
            Main.graphics.GraphicsDevice.Textures[1] = UCATextureRegister.Wood.Value;
            Vector2 OutSideBladeScale = new Vector2(0.85f * SolarBladeXScale, 1f) * scale;
            DrawMainBladeSource(DrawPos + offset.RotatedBy(DrawRot), DrawRot, OutSideBladeScale);
            // 改一下置换材质
            Main.graphics.GraphicsDevice.Textures[1] = UCATextureRegister.FireNoise.Value;
            Vector2 InSideBladeScale = new Vector2(0.4f * SolarBladeXScale, 0.79f) * scale;
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
            Main.spriteBatch.Draw(SolarBlade, drawPosition, null, Color.White * 0.7f, DrawRot, origin, scale, SpriteEffects.None, 0);
        }
        #endregion
        #region 发射粒子
        public void SpawnDust()
        {
            for (int i = 0; i < 15; i++)
            {
                Vector2 beginSpawnPos = Projectile.Center + new Vector2(64, 0).RotatedBy(Projectile.rotation);
                Vector2 firVel = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * 9 * Main.rand.NextFloat(0, 2f);
                Color DrawColor = Color.Lerp(Color.Orange, Color.OrangeRed, Main.rand.NextFloat());
                new PoisonSmoke(beginSpawnPos, firVel + Owner.velocity, DrawColor, Main.rand.Next(45, 55), Main.rand.NextFloat(MathHelper.TwoPi), 1f, 1f).Spawn();
            }
            for (int i = 0; i < 50; i++)
            {
                Vector2 beginSpawnPos = Projectile.Center + new Vector2(64, 0).RotatedBy(Projectile.rotation);
                Color DrawColor = Color.Lerp(Color.Orange, Color.OrangeRed, Main.rand.NextFloat());
                new MediumGlowBall(beginSpawnPos, Vector2.Zero, DrawColor, Main.rand.Next(100, 200), Main.rand.NextFloat(MathHelper.TwoPi), 1f, 0.2f, Main.rand.NextFloat(2f, 12f)).Spawn();
            }
            for (int i = 0; i < 25; i++)
            {
                Vector2 beginSpawnPos = Projectile.Center + new Vector2(64, 0).RotatedBy(Projectile.rotation);
                Vector2 EndSpawnPos = Projectile.Center + new Vector2(SolarBladeXOffset + 720, 0).RotatedBy(Projectile.rotation);
                float Progress = i / 25f;
                Vector2 firVel = Vector2.UnitX.RotatedBy(Projectile.rotation + MathHelper.PiOver2) * 9 * Main.rand.NextFloat(0, 2f);
                Color DrawColor = Color.Lerp(Color.Orange, Color.OrangeRed, Main.rand.NextFloat());
                new PoisonSmoke(Vector2.Lerp(beginSpawnPos, EndSpawnPos, Progress), firVel + Owner.velocity, DrawColor, Main.rand.Next(45, 55), Main.rand.NextFloat(MathHelper.TwoPi), 1f, 1f).Spawn();
            }
            for (int i = 0; i < 25; i++)
            {
                Vector2 beginSpawnPos = Projectile.Center + new Vector2(64, 0).RotatedBy(Projectile.rotation);
                Vector2 EndSpawnPos = Projectile.Center + new Vector2(SolarBladeXOffset + 640, 0).RotatedBy(Projectile.rotation);
                float Progress = i / 25f;
                Vector2 firVel = Vector2.UnitX.RotatedBy(Projectile.rotation + MathHelper.PiOver2) * -9 * Main.rand.NextFloat(0, 2f);
                Color DrawColor = Color.Lerp(Color.Orange, Color.OrangeRed, Main.rand.NextFloat());
                new PoisonSmoke(Vector2.Lerp(beginSpawnPos, EndSpawnPos, Progress), firVel + Owner.velocity, DrawColor, Main.rand.Next(45, 55), Main.rand.NextFloat(MathHelper.TwoPi), 1f, 1f).Spawn();
            }
            for (int i = 0; i < 200; i++)
            {
                Vector2 beginSpawnPos = Projectile.Center + new Vector2(64, Main.rand.Next(-100, 100)).RotatedBy(Projectile.rotation);
                Vector2 EndSpawnPos = Projectile.Center + new Vector2(SolarBladeXOffset + 640, Main.rand.Next(-100, 100)).RotatedBy(Projectile.rotation);
                float Progress = i / 200f;
                Color DrawColor = Color.Lerp(Color.Orange, Color.OrangeRed, Main.rand.NextFloat());
                new MediumGlowBall(Vector2.Lerp(beginSpawnPos, EndSpawnPos, Progress), Vector2.Zero, DrawColor, Main.rand.Next(100, 200), Main.rand.NextFloat(MathHelper.TwoPi), 1f, 0.2f, Main.rand.NextFloat(2f, 12f)).Spawn();
            }
        }
        #endregion
    }
}
