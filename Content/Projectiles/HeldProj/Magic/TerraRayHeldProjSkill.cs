using CalamityMod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using UCA.Assets;
using UCA.Content.DrawNodes;
using UCA.Content.Items.Weapons.Magic.Ray;
using UCA.Content.Paths;
using UCA.Content.Projectiles.Magic.Ray;
using UCA.Content.UCACooldowns;
using UCA.Core.AnimationHandle;
using UCA.Core.Enums;
using UCA.Core.Graphics;
using UCA.Core.Utilities;
using static System.Net.Mime.MediaTypeNames;

namespace UCA.Content.Projectiles.HeldProj.Magic
{
    public class TerraRayHeldProjSkill : ModProjectile, ILocalizedModType
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<TerraRay>();
        public override string Texture => $"{ProjPath.HeldProjPath}" + "Magic/TerraRayHeldProj";
        public Player Owner => Main.player[Projectile.owner];
        public float Opacity = 1f;
        public AnimationHelper animationHelper;
        public float HeightOffset;
        public float PosOffsetRot;
        public float RotOffset;
        public int Break;
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }
        public override void OnSpawn(IEntitySource source)
        {
            // 初始化效果
            animationHelper = new AnimationHelper(4);
            animationHelper.MaxAniProgress[AnimationState.Begin] = 75;
            animationHelper.MaxAniProgress[AnimationState.Middle] = 5;
            animationHelper.MaxAniProgress[AnimationState.End] = 10;
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<TerraMatrix>(),0,0,Projectile.owner);
        }
        public override void AI()
        {
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.ChangeDir(Owner.LocalMouseWorld().X > Owner.Center.X ? 1 : -1);
            Owner.heldProj = Projectile.whoAmI;
            // 基础信息
            Projectile.velocity = Vector2.Zero;
            Projectile.timeLeft = 2;
            Projectile.Center = Owner.Center + new Vector2(10 * Owner.direction, 0 + HeightOffset).RotatedBy(PosOffsetRot);
            Projectile.rotation = PosOffsetRot + RotOffset;
            HandleAni();
            float TargetRot = (Owner.Center - Projectile.Center).ToRotation() + MathHelper.PiOver2;
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, TargetRot + Owner.direction * -0.1f * 1.5f);
            Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, TargetRot + Owner.direction * 0.1f * 1.2f);
        }
        #region 动画
        public void HandleAni()
        {
            // 处理动画
            if (!animationHelper.HasFinish[AnimationState.Begin])
            {
                if (animationHelper.AniProgress[AnimationState.Begin] < animationHelper.MaxAniProgress[AnimationState.Begin])
                    animationHelper.AniProgress[AnimationState.Begin]++;

                HandleBeginAni();
                Owner.velocity *= 0.75f;
                if (animationHelper.AniProgress[AnimationState.Begin] >= animationHelper.MaxAniProgress[AnimationState.Begin])
                    animationHelper.HasFinish[AnimationState.Begin] = true;
            }
            else if (!animationHelper.HasFinish[AnimationState.Middle])
            {
                animationHelper.AniProgress[AnimationState.Middle]++;
                HandleMiddleAni();
                if (animationHelper.AniProgress[AnimationState.Middle] >= animationHelper.MaxAniProgress[AnimationState.Middle])
                {
                    GenTornado();
                    List<NPC> noUseNPC = [];
                    for (int i = 0; i < 4; i++)
                    {
                        NPC target = UCAUtilities.FindClosestNPCExceptSpecific(Owner.Center, 650, noUseNPC, true);
                        noUseNPC.Add(target);
                    }
                    if (noUseNPC.Count != 0)
                    {
                        foreach (NPC npc in noUseNPC)
                        {
                            if (npc is not null)
                                GenTrackTornado(npc.Center);
                        }
                    }
                    if (Projectile.ai[0] != 0)
                    {
                        Owner.UCA().TerraRestore = true;
                        Owner.AddCooldown(TerraBoost.ID, CalamityUtils.SecondsToFrames(30));
                    }
                    animationHelper.HasFinish[AnimationState.Middle] = true;
                }

                Vector2 firPos = Owner.Center;
                for (int i = 0; i < 6; i++)
                {
                    float rot = MathHelper.TwoPi / 6;
                    float XScale = Main.rand.NextFloat(5, 9);
                    float Height = Main.rand.NextFloat(4f, 12f);

                    Vector2 firVec = Vector2.UnitX.RotatedBy(rot * i).RotatedByRandom(MathHelper.TwoPi);
                    Color color = Main.rand.NextBool() ? Color.ForestGreen : Color.SaddleBrown;
                    new TerraTree(firPos, firVec * Main.rand.NextFloat(1.6f, 6.4f), color, 0, DrawLayer.BeforeDust, XScale, Main.rand.NextBool() ? 1 : -1, Height).Spawn();
                }
            }
            else if (!animationHelper.HasFinish[AnimationState.End])
            {
                animationHelper.AniProgress[AnimationState.End]++;
                HandleEndAni();
                if (animationHelper.AniProgress[AnimationState.End] >= animationHelper.MaxAniProgress[AnimationState.End])
                    animationHelper.HasFinish[AnimationState.End] = true;
            }
            else
            {
                Break++;
                Opacity = MathHelper.Lerp(0f, 1f, Break / 45f);
                if (Break > 45)
                    Projectile.Kill();
            }
        }
        #endregion
        #region 生成龙卷
        public void GenTornado()
        {
            SoundEngine.PlaySound(SoundsMenu.NightRayHit, Projectile.Center);
            for (int j = 0; j < 2; j++)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector2 offset = new Vector2(56 * i * (j == 0 ? 1 : -1), 32 * i);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center - offset, Vector2.Zero, ModContent.ProjectileType<TerrarTornado>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner, 40 * i, 1);
                    Vector2 offset2 = new Vector2(56 * i * (j == 0 ? 1 : -1), 48);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center - offset2, Vector2.Zero, ModContent.ProjectileType<TerrarTornado>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner, 20 * i, 1);
                }
            }
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center - new Vector2(96, -24), Vector2.Zero, ModContent.ProjectileType<TerrarTornado>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner, 10, 1);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center - new Vector2(-96, -24), Vector2.Zero, ModContent.ProjectileType<TerrarTornado>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner, 30, 1);

            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center - new Vector2(0, -64), Vector2.Zero, ModContent.ProjectileType<TerrarTornado>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner, 10, 1);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center - new Vector2(0, 102), Vector2.Zero, ModContent.ProjectileType<TerrarTornado>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner, 30, 1);
        }
        public void GenTrackTornado(Vector2 GenPos)
        {
            for (int j = 0; j < 2; j++)
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 offset = new Vector2(56 * i * (j == 0 ? 1 : -1), 32 * i);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), GenPos - offset, Vector2.Zero, ModContent.ProjectileType<TerrarTornado>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner, 40 * i, 1);
                    Vector2 offset2 = new Vector2(56 * i * (j == 0 ? 1 : -1), 48);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), GenPos - offset2, Vector2.Zero, ModContent.ProjectileType<TerrarTornado>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner, 20 * i, 1);
                }
            }
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), GenPos - new Vector2(48, -24), Vector2.Zero, ModContent.ProjectileType<TerrarTornado>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner, 10, 1);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), GenPos - new Vector2(-48, -24), Vector2.Zero, ModContent.ProjectileType<TerrarTornado>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner, 30, 1);
        }
        #endregion
        #region 处理动画
        public void HandleBeginAni()
        {
            int MaxAni = animationHelper.MaxAniProgress[AnimationState.Begin];
            int CurAni = animationHelper.AniProgress[AnimationState.Begin];
            // 使用缓动函数让动画更自然
            float easedProgress = EasingHelper.EaseOutCubic(CurAni / (float)MaxAni);
            easedProgress = (float)Math.Pow(easedProgress, 0.3f);
            Opacity = MathHelper.Lerp(0.8f, 0f, CurAni / (float)MaxAni);
            // 设置起始与结束角度
            float startAngleOffset = MathHelper.ToRadians(25);
            float endAngleOffset = MathHelper.ToRadians(-20);
            // 计算基础旋转角度
            float baseRotation = MathHelper.Lerp(startAngleOffset, endAngleOffset, easedProgress);
            // 根据玩家方向进行镜像处理
            if (Owner.direction == -1)// 水平镜像
                baseRotation = baseRotation * Owner.direction;

            PosOffsetRot = baseRotation;
            HeightOffset = MathHelper.Lerp(0, -4, easedProgress);
            RotOffset = -MathHelper.PiOver2 * 1.2f;
            if (Owner.direction == -1)// 水平镜像
                RotOffset = -MathHelper.PiOver2 * 0.8f;
        }
        public void HandleMiddleAni()
        {
            int MaxAni = animationHelper.MaxAniProgress[AnimationState.Middle];
            int CurAni = animationHelper.AniProgress[AnimationState.Middle];
            // 使用缓动函数让动画更自然
            float easedProgress = EasingHelper.EaseOutCubic(CurAni / (float)MaxAni);
            // 设置起始与结束角度
            float startAngleOffset = MathHelper.ToRadians(-20);
            float endAngleOffset = MathHelper.ToRadians(35);
            // 计算基础旋转角度
            float baseRotation = MathHelper.Lerp(startAngleOffset, endAngleOffset, easedProgress);
            // 根据玩家方向进行镜像处理
            if (Owner.direction == -1)// 水平镜像
                baseRotation = baseRotation * Owner.direction;

            PosOffsetRot = baseRotation;
            HeightOffset = MathHelper.Lerp(-4, -16, easedProgress);
            RotOffset = MathHelper.Lerp(-MathHelper.PiOver2 * 1.2f, -MathHelper.PiOver2 * 1.1f, easedProgress);
            if (Owner.direction == -1)// 水平镜像
                RotOffset = MathHelper.Lerp(-MathHelper.PiOver2 * 0.8f, -MathHelper.PiOver2 * 0.9f, easedProgress);
        }
        public void HandleEndAni()
        {
            int MaxAni = animationHelper.MaxAniProgress[AnimationState.End];
            int CurAni = animationHelper.AniProgress[AnimationState.End];
            // 使用缓动函数让动画更自然
            float easedProgress = EasingHelper.EaseInCubic(CurAni / (float)MaxAni);
            // 设置起始与结束角度
            float startAngleOffset = MathHelper.ToRadians(35);
            float endAngleOffset = MathHelper.ToRadians(25);
            // 计算基础旋转角度
            float baseRotation = MathHelper.Lerp(startAngleOffset, endAngleOffset, easedProgress);
            // 根据玩家方向进行镜像处理
            if (Owner.direction == -1)// 水平镜像
                baseRotation = baseRotation * Owner.direction;
        }
        #endregion
        public override void OnKill(int timeLeft)
        {
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            Main.graphics.GraphicsDevice.Textures[1] = UCATextureRegister.Noise.Value;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            UCAUtilities.FastApplyEdgeMeltsShader(Opacity, texture.Size(), Color.LimeGreen, 0.01f, 0);
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            float drawRotation = Projectile.rotation + (Projectile.spriteDirection == -1 ? MathHelper.PiOver2 + MathHelper.PiOver4 : MathHelper.PiOver4);
            Vector2 rotationPoint = texture.Size() / 2f;
            SpriteEffects flipSprite = Projectile.spriteDirection * Main.player[Projectile.owner].gravDir == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.spriteBatch.Draw(texture, drawPosition, null, Color.White, drawRotation, rotationPoint, Projectile.scale * Main.player[Projectile.owner].gravDir, flipSprite, 0f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }
}
