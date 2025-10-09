using CalamityMod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Content.DrawNodes;
using UCA.Content.Items.Weapons.Magic.Ray;
using UCA.Content.Particiles;
using UCA.Content.Projectiles.Magic.Ray;
using UCA.Core.AnimationHandle;
using UCA.Core.BaseClass;
using UCA.Core.Enums;
using UCA.Core.Graphics;
using UCA.Core.Utilities;

namespace UCA.Content.Projectiles.HeldProj.Magic
{
    public class TerraRayHeldProj : BaseHeldProj
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<TerraRay>();
        public Vector2 RotVector => new Vector2(10 * Owner.direction, 7).BetterRotatedBy(Owner.GetPlayerToMouseVector2().ToRotation(), default, 0.5f, 1f);
        public override Vector2 RotPoint => TextureAssets.Projectile[Type].Size() / 2;
        public override Vector2 Posffset => new Vector2(RotVector.X, RotVector.Y) * Owner.direction;
        public override float RotAmount => 0.25f;
        public override float RotOffset => 0;

        public float Opacity = 1f;

        public AnimationHelper animationHelper;
        public override void SetDefaults()
        {
            Projectile.width = 66;
            Projectile.height = 64;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            animationHelper = new AnimationHelper(3); 
            animationHelper.MaxAniProgress[AnimationState.Begin] = 15;
        }

        public override void HoldoutAI()
        {
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Owner.GetPlayerToMouseVector2().ToRotation() - MathHelper.PiOver2);
            if (UseDelay <= 0 && Owner.CheckMana(Owner.ActiveItem(), (int)(Owner.HeldItem.mana * Owner.manaCost), true, false))
            {
                SoundEngine.PlaySound(SoundsMenu.PlasmaRodAttack, Projectile.Center);
                FirePorj();
                UseDelay = Owner.HeldItem.useTime;
            }
        }

        public void FirePorj()
        {
            SoundEngine.PlaySound(SoundID.Item91, Projectile.Center);
            Vector2 FireOffset = new Vector2(54, 0).RotatedBy(Projectile.rotation);
            GenStar(Projectile.Center + FireOffset, MathHelper.PiOver2 + Projectile.rotation);
            #region 生成环形粒子
            for (int i = 0; i < 30; i++)
            {
                float offset = MathHelper.TwoPi / 30;
                Color RandomColor = Color.Lerp(Color.LightGreen, Color.ForestGreen, Main.rand.NextFloat(0, 1));
                Vector2 firVel = Vector2.UnitX.BetterRotatedBy(offset * i,default, 0.75f, 1f);
                new MediumGlowBall(Projectile.Center + FireOffset, firVel.RotatedBy(Projectile.rotation) * 1.5f, RandomColor, 60, 0, 1, 0.2f, 0).Spawn();
            }
            #endregion
            #region 生成蝴蝶
            for (int i = 0; i < 6; i++)
            {
                float offset = MathHelper.TwoPi / 6;
                Color RandomColor = Color.Lerp(Color.LightGreen, Color.ForestGreen, Main.rand.NextFloat(0, 1));
                Vector2 firVel = Vector2.UnitX.RotatedBy(offset * i).RotatedByRandom(0.3f);
                new Butterfly(Projectile.Center + FireOffset, firVel * Main.rand.NextFloat(0.3f, 0.9f), RandomColor, 120, 0, 1, 0.2f, Main.rand.NextFloat(0.3f, 1.4f)).Spawn();
            }
            #endregion
            #region 生成伴随主弹幕的树
            Vector2 firVec = Projectile.velocity * 3f;
            Vector2 ProjFireOffset = new Vector2(24, 0).RotatedBy(Projectile.rotation);
            Vector2 firPos = Projectile.Center + ProjFireOffset;
            
            for (int i = 0; i < 2; i++)
            {
                float XScale = Main.rand.NextFloat(2, 5);
                float Height = Main.rand.NextFloat(9, 18f);

                new TerraTree(firPos, firVec * Main.rand.NextFloat(6, 6.5f), Color.ForestGreen, 0, DrawLayer.BeforeDust, XScale, 1, Height).Spawn();
                new TerraTree(firPos, firVec * Main.rand.NextFloat(6, 6.5f), Color.LightGreen, 0, DrawLayer.AfterDust, XScale, -1, Height).Spawn();

                float XScale2 = Main.rand.NextFloat(3, 6);
                float Height2 = Main.rand.NextFloat(11, 22);
                new TerraTree(firPos, firVec * Main.rand.NextFloat(6, 6.5f), Color.DarkGreen, 0, DrawLayer.BeforeDust, XScale2, 1, Height2).Spawn();
                new TerraTree(firPos, firVec * Main.rand.NextFloat(6, 6.5f), Color.SaddleBrown, 0, DrawLayer.AfterDust, XScale2, -1, Height2).Spawn();
            }
            #endregion
            // 生成弹幕
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + ProjFireOffset, firVec * 0.0001f, ModContent.ProjectileType<TerraLaser>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            // 后坐力
            Projectile.velocity -= Projectile.velocity.RotatedBy(Projectile.spriteDirection * MathHelper.PiOver2) * 0.1f;
        }
        public static void GenStar(Vector2 pos, float rotoffset, float Xmult = 0.8f)
        {
            // 控制属性分别是：多少个点，生成步进，生成位置
            int PointCount = 9;
            int GenStep = 20;
            float OutSidePoint = 50f;
            float InSidePoint = 35f;
            float RotOffset = rotoffset;
            float FirstRotOffset = MathHelper.ToRadians(12);
            for (int i = 0; i < PointCount; ++i)
            {
                // 生成外部的点
                float OutSideoffset = MathHelper.TwoPi / PointCount;
                float InSideoffset = MathHelper.TwoPi / PointCount;
                float HalfOffsetPoint = MathHelper.TwoPi / PointCount / 2;
                // 先正向生成，再逆向生成
                for (int k = 0; k < 2; k++)
                {
                    int Dir = k == 0 ? 1 : -1;
                    for (float j = 0; j < GenStep; j++)
                    {
                        float Progress = j / GenStep;
                        // 向量计算
                        Vector2 OutSideGenPos = Vector2.UnitX.BetterRotatedBy(OutSideoffset * i + FirstRotOffset, default, 1f, Xmult) * OutSidePoint;
                        // 内部点的位置
                        Vector2 InSideGenPos = Vector2.UnitX.BetterRotatedBy(OutSideoffset * i + HalfOffsetPoint * Dir + FirstRotOffset, default, 1f, Xmult) * InSidePoint;
                        // 整体的旋转
                        OutSideGenPos = OutSideGenPos.RotatedBy(RotOffset);
                        InSideGenPos = InSideGenPos.RotatedBy(RotOffset);
                        // 插值位置，决定最终位置
                        Vector2 LerpPos = Vector2.Lerp(OutSideGenPos, InSideGenPos, Progress);
                        // 插值向量，形成花瓣的形状
                        float LerpVelMult = MathHelper.Lerp(1f, 0.7f, EasingHelper.EaseInCubic(Progress));
                        Vector2 FinalVel = LerpPos * 0.1f * LerpVelMult;
                        // 生成粒子
                        // SpawnParticle(Projectile.Center + FireOffset, FinalVel, 0.1f);
                        Color RandomColor = Color.Lerp(Color.LightGreen, Color.ForestGreen, Main.rand.NextFloat(0, 1));
                        new MediumGlowBall(pos, FinalVel, RandomColor, 70, 0, 1, 0.1f, 0).Spawn();
                    }
                }
            }
            // 生成枝条
            Vector2 firPos = pos;
            for (int i = 0; i <9; i++)
            {
                float rot = MathHelper.TwoPi / 9;
                float XScale = Main.rand.NextFloat(9, 12);
                float Height = Main.rand.NextFloat(4f, 6f);

                Vector2 firVec = Vector2.UnitX.RotatedBy(rot * i);
                Color color = Main.rand.NextBool() ? Color.DarkGreen : Color.SaddleBrown;
                new TerraTree(firPos, firVec * Main.rand.NextFloat(0.3f, 0.6f), color, 0, DrawLayer.BeforeDust, XScale, Main.rand.NextBool() ? 1 : -1, Height).Spawn();
            }
        }
        public override void PostAI()
        {
            // 设置玩家手持效果
            float baseRotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            float directionVerticality = MathF.Abs(Projectile.velocity.X);
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, baseRotation + Owner.direction * directionVerticality * 1.5f);
            Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, baseRotation + Owner.direction * directionVerticality * 1.2f);

            if ((Main.mouseLeft || Active) && !UCAUtilities.JustPressRightClick())
            {
                if (animationHelper.AniProgress[AnimationState.Begin] < animationHelper.MaxAniProgress[AnimationState.Begin])
                    animationHelper.AniProgress[AnimationState.Begin]++;
            }
            else
            {
                if (animationHelper.AniProgress[AnimationState.Begin] > 0)
                    animationHelper.AniProgress[AnimationState.Begin]--;
            }
            int MaxAni = animationHelper.MaxAniProgress[AnimationState.Begin];
            int CurAni = animationHelper.AniProgress[AnimationState.Begin];
            float easedProgress = (CurAni / (float)MaxAni);

            Opacity = MathHelper.Lerp(0.7f, 0f, easedProgress);
        }

        public override bool CanDel()
        {
            return base.CanDel() && Opacity > 0.68f;
        }
        public override void OnKill(int timeLeft)
        {
            Main.mouseRight = false;
            Owner.itemTime = 0;
            Owner.itemAnimation = 0;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.graphics.GraphicsDevice.Textures[1] = UCATextureRegister.Noise.Value;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;

            UCAUtilities.FastApplyEdgeMeltsShader(Opacity, ModContent.Request<Texture2D>(Texture).Size(), Color.LimeGreen, 0.01f, 0);

            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            float drawRotation = Projectile.rotation + (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0f + MathHelper.PiOver4 * (Projectile.spriteDirection + 1));
            Vector2 rotationPoint = ModContent.Request<Texture2D>(Texture).Value.Size() / 2f;
            SpriteEffects flipSprite = Projectile.spriteDirection * Main.player[Projectile.owner].gravDir == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            // spriteBatch会自动把textures0设置为当前使用的材质，所以需要你手动改一下
            Main.spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, drawPosition, null, Color.White, drawRotation - MathHelper.PiOver4, rotationPoint, Projectile.scale * Main.player[Projectile.owner].gravDir, flipSprite, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            
            return false;
        }
    }
}
