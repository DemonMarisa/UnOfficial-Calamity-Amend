using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Graphics.Primitives;
using CalamityMod.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Content.DrawNodes;
using UCA.Content.Items.Weapons.Magic.Ray;
using UCA.Content.Particiles;
using UCA.Content.Paths;
using UCA.Content.Projectiles.Magic.Ray;
using UCA.Core.AnimationHandle;
using UCA.Core.Enums;
using UCA.Core.Graphics;
using UCA.Core.Utilities;

namespace UCA.Content.Projectiles.HeldProj.Magic
{
    public class TerraRayHeldProjSpecial : ModProjectile, ILocalizedModType
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<TerraRay>();
        public override string Texture => $"{ProjPath.HeldProjPath}" + "Magic/TerraRayHeldProj";
        public AnimationHelper animationHelper;
        public int OwnerDir = 0;
        public float BeginRot = 0;
        public float Opacity = 1f;
        public Vector2 DrawOffset;
        public Vector2 HeldAimPoint;
        // 相对于起始的旋转的偏移
        public float OffsetRot = 0;
        // 设置手臂的旋转
        public float ArmRot = 0;
        // 用于最终阶段动画的延伸效果
        public float LengthOffset;
        public Player Owner => Main.player[Projectile.owner];
        public float ToMouseVector => Owner.GetPlayerToMouseVector2().ToRotation();
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
            animationHelper.MaxAniProgress[AnimationState.Begin] = 30;
            animationHelper.MaxAniProgress[AnimationState.Middle] = 10;
            animationHelper.MaxAniProgress[AnimationState.End] = 40;
        }

        public override void AI()
        {
            OwnerDir = Owner.LocalMouseWorld().X > Owner.Center.X ? 1 : -1;
            BeginRot = UCAUtilities.GetVector2(Owner.Center, Owner.LocalMouseWorld()).ToRotation() + MathHelper.ToRadians(-0 * OwnerDir);
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.ChangeDir(OwnerDir);
            
            if (animationHelper.HasFinish[AnimationState.Begin])
                Owner.heldProj = Projectile.whoAmI;
            
            // 基础信息
            Projectile.velocity = Projectile.rotation.ToRotationVector2();
            Projectile.timeLeft = 2;

            Projectile.rotation = BeginRot + OffsetRot;

            Projectile.Center = Owner.Center + new Vector2(12, 0).RotatedBy(BeginRot + OffsetRot);
            // 这两个都是相对于弹幕中央的偏移
            DrawOffset = new Vector2(LengthOffset, 0).RotatedBy(Projectile.rotation);
            HeldAimPoint = new Vector2(0 + LengthOffset, 0).RotatedBy(Projectile.rotation);

            ArmRot = (Projectile.Center + HeldAimPoint - Owner.Center).ToRotation();

            Projectile.spriteDirection = Owner.direction;
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, ArmRot - MathHelper.PiOver2);
            HandleAni();
        }
        public void HandleAni()
        {
            // 处理动画
            if (!animationHelper.HasFinish[AnimationState.Begin])
            {
                /*
                if (animationHelper.AniProgress[AnimationState.Begin] == 1)
                    SoundEngine.PlaySound(SoundsMenu.NightShieldCharge, Projectile.Center);
                */
                if (animationHelper.AniProgress[AnimationState.Begin] < animationHelper.MaxAniProgress[AnimationState.Begin])
                    animationHelper.AniProgress[AnimationState.Begin]++;


                HandleBeginAni();

                if (animationHelper.AniProgress[AnimationState.Begin] >= animationHelper.MaxAniProgress[AnimationState.Begin])
                {
                    animationHelper.HasFinish[AnimationState.Begin] = true;
                }
            }
            else if (!animationHelper.HasFinish[AnimationState.Middle])
            {
                animationHelper.AniProgress[AnimationState.Middle]++;

                if (animationHelper.AniProgress[AnimationState.Middle] == 1)
                {
                    GenLance();
                }

                HandleMiddleAni();
                if (animationHelper.AniProgress[AnimationState.Middle] >= animationHelper.MaxAniProgress[AnimationState.Middle])
                {
                    animationHelper.HasFinish[AnimationState.Middle] = true;
                }
            }
            else if (!animationHelper.HasFinish[AnimationState.End])
            {
                Break++;
                if (Break > 8)
                {
                    Projectile.extraUpdates = 2;

                    animationHelper.AniProgress[AnimationState.End]++;

                    if (animationHelper.AniProgress[AnimationState.End] == 30)
                    {
                        GenTornado(Owner.Center, false);
                        GenTornado(Owner.LocalMouseWorld(), true);
                    }

                    HandleEndAni();

                    if (animationHelper.AniProgress[AnimationState.End] == animationHelper.MaxAniProgress[AnimationState.End])
                        animationHelper.HasFinish[AnimationState.End] = true;
                }
            }
            else
            {
                Projectile.Kill();
            }
        }
        #region 处理动画
        public void HandleBeginAni()
        {
            int MaxAni = animationHelper.MaxAniProgress[AnimationState.Begin];
            int CurAni = animationHelper.AniProgress[AnimationState.Begin];
            Opacity = MathHelper.Lerp(1f, 0f, CurAni / (float)MaxAni);
            // 使用缓动函数让动画更自然
            float easedProgress = EasingHelper.EaseOutCubic(CurAni / (float)MaxAni);
            // 设置起始与结束角度
            float startAngleOffset = MathHelper.ToRadians(0);
            float endAngleOffset = MathHelper.ToRadians(-165);
            // 计算基础旋转角度
            float baseRotation = MathHelper.Lerp(startAngleOffset, endAngleOffset, easedProgress);
            // 根据玩家方向进行镜像处理
            if (Owner.direction == -1)// 水平镜像
                baseRotation = baseRotation * Owner.direction;

            OffsetRot = baseRotation;

            LengthOffset = MathHelper.Lerp(0, -4, easedProgress);
        }
        public void HandleMiddleAni()
        {
            int MaxAni = animationHelper.MaxAniProgress[AnimationState.Middle];
            int CurAni = animationHelper.AniProgress[AnimationState.Middle];
            // 使用缓动函数让动画更自然
            float easedProgress = EasingHelper.EaseOutCubic(CurAni / (float)MaxAni);
            easedProgress = (float)Math.Pow(easedProgress, 0.1f);
            // 设置起始与结束角度
            float startAngleOffset = MathHelper.ToRadians(-165);
            float endAngleOffset = MathHelper.ToRadians(165);
            // 计算基础旋转角度
            float baseRotation = MathHelper.Lerp(startAngleOffset, endAngleOffset, easedProgress);
            // 根据玩家方向进行镜像处理
            if (Owner.direction == -1)// 水平镜像
                baseRotation = baseRotation * Owner.direction;

            OffsetRot = baseRotation;

            LengthOffset = MathHelper.Lerp(-4, 4, easedProgress);
        }
        public void HandleEndAni()
        {
            int MaxAni = animationHelper.MaxAniProgress[AnimationState.End];
            int CurAni = animationHelper.AniProgress[AnimationState.End];
            // 使用缓动函数让动画更自然
            float easedProgress = EasingHelper.EaseInCubic(CurAni / (float)MaxAni);
            easedProgress = (float)Math.Pow(easedProgress, 2f);
            // 设置起始与结束角度
            float startAngleOffset = MathHelper.ToRadians(165);
            float endAngleOffset = MathHelper.ToRadians(-175);
            // 计算基础旋转角度
            float baseRotation = MathHelper.Lerp(startAngleOffset, endAngleOffset, easedProgress);
            // 根据玩家方向进行镜像处理
            if (Owner.direction == -1)// 水平镜像
                baseRotation = baseRotation * Owner.direction;
            Opacity = MathHelper.Lerp(Opacity, 1f, 0.01f);
            OffsetRot = baseRotation;
            LengthOffset = MathHelper.Lerp(4, 8, easedProgress);
        }
        #endregion
        #region 生成花
        public void GenStar(Vector2 pos, float rotoffset, float Xmult = 0.8f)
        {
            // 控制属性分别是：多少个点，生成步进，生成位置
            int PointCount = 9;
            int GenStep = 5;
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
            for (int i = 0; i < 9; i++)
            {
                float rot = MathHelper.TwoPi / 9;
                float XScale = Main.rand.NextFloat(9, 12);
                float Height = Main.rand.NextFloat(4f, 6f);

                Vector2 firVec = Vector2.UnitX.RotatedBy(rot * i);
                Color color = Main.rand.NextBool() ? Color.DarkGreen : Color.SaddleBrown;
                new TerraTree(firPos, firVec * Main.rand.NextFloat(0.3f, 0.6f), color, 0, DrawLayer.BeforeDust, XScale, Main.rand.NextBool() ? 1 : -1, Height).Spawn();
            }
            #region 生成环形粒子
            for (int i = 0; i < 30; i++)
            {
                float offset = MathHelper.TwoPi / 30;
                Color RandomColor = Color.Lerp(Color.LightGreen, Color.ForestGreen, Main.rand.NextFloat(0, 1));
                Vector2 firVel = Vector2.UnitX.BetterRotatedBy(offset * i, default, 0.75f, 1f);
                new MediumGlowBall(firPos, firVel.RotatedBy(Projectile.rotation) * 1.5f, RandomColor, 60, 0, 1, 0.2f, 0).Spawn();
            }
            #endregion
            #region 生成蝴蝶
            for (int i = 0; i < 3; i++)
            {
                float offset = MathHelper.TwoPi / 3;
                Color RandomColor = Color.Lerp(Color.LightGreen, Color.ForestGreen, Main.rand.NextFloat(0, 1));
                Vector2 firVel = Vector2.UnitX.RotatedBy(offset * i).RotatedByRandom(0.3f);
                new Butterfly(firPos, firVel * Main.rand.NextFloat(0.3f, 0.9f), RandomColor, 120, 0, 1, 0.2f, Main.rand.NextFloat(0.3f, 1.4f)).Spawn();
            }
            #endregion
        }
        #endregion
        #region 生成弹幕
        public void GenLance()
        {
            if (Projectile.owner != Main.myPlayer)
                return;

            SoundEngine.PlaySound(SoundsMenu.PlasmaRodAttack, Projectile.Center);
            
            for (float i = 0; i < 2; i++)
            {
                for (float j = 0; j < 3; j++)
                {
                    float rotOffset = MathHelper.Pi / 4;
                    Vector2 firePos = ToMouseVector.ToRotationVector2() * (i == 0 ? -96 : 96);
                    firePos = firePos.RotatedBy(rotOffset * j - MathHelper.PiOver4 * 1);
                    GenStar(Owner.Center + firePos, ToMouseVector + MathHelper.PiOver2, 0.6f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center + firePos, Owner.GetToMouseVector2(Owner.Center + firePos) * 15f, ModContent.ProjectileType<TerraLance>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
            }
        }
        public void GenTornado(Vector2 genPos, bool fromMouse)
        {
            if (Projectile.owner != Main.myPlayer)
                return;

            int Damage = (int)(Projectile.damage * 2);
            if (fromMouse)
            {
                NPC target = Projectile.FindClosestTarget(250, genPos);
                if (target is not null)
                    genPos = target.Center;
                SoundEngine.PlaySound(SoundsMenu.NightRayHit, Projectile.Center);
                for (int j = 0; j < 2; j++)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Vector2 offset = new Vector2(56 * i * (j == 0 ? 1 : -1), 32 * i);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), genPos - offset, Vector2.Zero, ModContent.ProjectileType<TerrarTornado>(), Damage, Projectile.knockBack, Projectile.owner, 40 * i, 1);
                        Vector2 offset2 = new Vector2(56 * i * (j == 0 ? 1 : -1), 48);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), genPos - offset2, Vector2.Zero, ModContent.ProjectileType<TerrarTornado>(), Damage, Projectile.knockBack, Projectile.owner, 20 * i, 1);
                    }
                }
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), genPos - new Vector2(48, -24), Vector2.Zero, ModContent.ProjectileType<TerrarTornado>(), Damage, Projectile.knockBack, Projectile.owner, 10, 1);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), genPos - new Vector2(-48, -24), Vector2.Zero, ModContent.ProjectileType<TerrarTornado>(), Damage, Projectile.knockBack, Projectile.owner, 30, 1);
                return;
            }
            SoundEngine.PlaySound(SoundsMenu.NightRayHit,Projectile.Center);
            for (int j = 0; j < 2; j++)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector2 offset = new Vector2(56 * i * (j == 0 ? 1 : -1), 32 * i);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), genPos - offset, Vector2.Zero, ModContent.ProjectileType<TerrarTornado>(), Damage, Projectile.knockBack, Projectile.owner, 40 * i);
                    Vector2 offset2 = new Vector2(56 * i * (j == 0 ? 1 : -1), 48);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), genPos - offset2, Vector2.Zero, ModContent.ProjectileType<TerrarTornado>(), Damage, Projectile.knockBack, Projectile.owner, 20 * i);
                }
            }
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), genPos - new Vector2(96, -24), Vector2.Zero, ModContent.ProjectileType<TerrarTornado>(), Damage, Projectile.knockBack, Projectile.owner, 10);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), genPos - new Vector2(-96, -24), Vector2.Zero, ModContent.ProjectileType<TerrarTornado>(), Damage, Projectile.knockBack, Projectile.owner, 30);
        }
        #endregion
        public override void OnKill(int timeLeft)
        {
            Owner.itemTime = 0;
            Owner.itemAnimation = 0;
            if (Main.mouseRight)
            {
                animationHelper = new AnimationHelper();
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, Type, Projectile.damage, Projectile.knockBack, Projectile.owner, 0);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.graphics.GraphicsDevice.Textures[1] = UCATextureRegister.Noise.Value;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;

            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            UCAUtilities.FastApplyEdgeMeltsShader(Opacity, texture.Size(), Color.LimeGreen, 0.01f, 0);

            Vector2 drawPosition = Projectile.Center - Main.screenPosition + DrawOffset;
            float drawRotation = Projectile.rotation + (Projectile.spriteDirection == -1 ? MathHelper.PiOver2 + MathHelper.PiOver4 : MathHelper.PiOver4);
            Vector2 rotationPoint = texture.Size() / 2f;
            SpriteEffects flipSprite = Projectile.spriteDirection * Main.player[Projectile.owner].gravDir == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            // spriteBatch会自动把textures0设置为当前使用的材质，所以需要你手动改一下
            Main.spriteBatch.Draw(texture, drawPosition, null, Color.White, drawRotation, rotationPoint, Projectile.scale * Main.player[Projectile.owner].gravDir, flipSprite, 0f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }
}
