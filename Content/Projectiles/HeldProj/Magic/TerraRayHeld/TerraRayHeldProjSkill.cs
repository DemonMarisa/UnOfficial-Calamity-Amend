using LAP.Assets.TextureRegister;
using LAP.Core.AnimationHandle;
using LAP.Core.Enums;
using LAP.Core.SystemsLoader;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;
using UCA.Assets.Sounds;
using UCA.Content.Items.Weapons.Magic.Ray;
using UCA.Content.Paths;
using UCA.Content.Projectiles.Magic.Ray;
using UCA.Content.UCACooldowns;
using UCA.Content.VFXs;
using UCA.Core.Utilities;

namespace UCA.Content.Projectiles.HeldProj.Magic.TerraRayHeld
{
    public class TerraRayHeldProjSkill : ModProjectile, ILocalizedModType
    {
        public override LocalizedText DisplayName => LAPUtilities.GetItemName<TerraRay>();
        public override string Texture => $"{ProjPath.HeldProjPath}" + "Magic/TerraRayHeld/TerraRayHeldProj";
        public Player Owner => Main.player[Projectile.owner];
        public float Opacity = 1f;
        public AniHelper AniHelper = new AniHelper(4);
        public float HeightOffset;
        public float PosOffsetRot;
        public float RotOffset;
        public int Break;
        public override void SetStaticDefaults()
        {
            // 保存旧朝向与旧位置
            Projectile.AddToSkillProj();
        }
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
            if (Projectile.IsLocalPlayer())
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ProjectileType<TerraMatrix>(),0,0,Projectile.owner);
        }
        public override void AI()
        {
            Owner.SetUseFocus(2);
            if (Projectile.LAP().FirstFrame)
            {
                SoundEngine.PlaySound(SoundsMenu.TerraRestore, Projectile.Center);
                // 初始化效果
                AniHelper.MaxAniProgress[AniState.Begin] = 75;
                AniHelper.MaxAniProgress[AniState.Middle] = 10;
                AniHelper.MaxAniProgress[AniState.End] = 10;
            }
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.ChangeDir(Owner.LocalMouseWorld().X > Owner.Center.X ? 1 : -1);
            Owner.heldProj = Projectile.whoAmI;
            if (!Owner.active || Owner.dead)
                Projectile.Kill();
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
            if (!AniHelper.HasFinish[AniState.Begin])
            {
                if (AniHelper.AniProgress[AniState.Begin] < AniHelper.MaxAniProgress[AniState.Begin])
                    AniHelper.AniProgress[AniState.Begin]++;

                HandleBeginAni();
                Owner.velocity.X *= 0.5f;
                Owner.UCA().TerraRayUseSkillCount = 20;

                if (AniHelper.AniProgress[AniState.Begin] >= AniHelper.MaxAniProgress[AniState.Begin])
                    AniHelper.HasFinish[AniState.Begin] = true;
            }
            else if (!AniHelper.HasFinish[AniState.Middle])
            {
                AniHelper.AniProgress[AniState.Middle]++;
                HandleMiddleAni();
                if (AniHelper.AniProgress[AniState.Middle] >= AniHelper.MaxAniProgress[AniState.Middle])
                {
                    SoundEngine.PlaySound(SoundsMenu.TerraRestoreRelease, Projectile.Center);
                    GenTornado();
                    List<NPC> noUseNPC = [];
                    for (int i = 0; i < 4; i++)
                    {
                        NPC target = LAPUtilities.FindClosestNPCExceptSpecific(Owner.Center, 650, noUseNPC, true);
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
                        foreach (Player playere in Main.ActivePlayers)
                        {
                            if (playere.Distance(Owner.Center) < 650)
                                playere.AddCD(LAPContent.CDType<TerraBoost>(), 30 * 60, false);
                            playere.UCA().TerraRestore = true;
                        }
                        if (Main.LocalPlayer.Center.Distance(Owner.Center) < 650)
                        {
                            Main.LocalPlayer.AddCD(LAPContent.CDType<TerraBoost>(), 30 * 60, false);
                            Owner.UCA().TerraRestore = true;
                        }
                    }
                    AniHelper.HasFinish[AniState.Middle] = true;
                }

                Vector2 firPos = Owner.Center;
                for (int i = 0; i < 6; i++)
                {
                    float rot = MathHelper.TwoPi / 6;

                    Vector2 firVec = Vector2.UnitX.RotatedBy(rot * i).RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(1.6f, 6.4f);
                    Color color = Main.rand.NextBool() ? Color.ForestGreen : Color.SaddleBrown;

                    TerraVine.Spawn(firPos, firVec, color, Main.rand.NextBool() ? -1 : 1, 2.2f, Main.rand.NextFloat(8f, 12f), Main.rand.NextFloat(1f, 2f));
                }
            }
            else if (!AniHelper.HasFinish[AniState.End])
            {
                AniHelper.AniProgress[AniState.End]++;
                HandleEndAni();
                if (AniHelper.AniProgress[AniState.End] >= AniHelper.MaxAniProgress[AniState.End])
                    AniHelper.HasFinish[AniState.End] = true;
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
            if (!Projectile.IsLocalPlayer())
                return;
            for (int j = 0; j < 2; j++)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector2 offset = new Vector2(56 * i * (j == 0 ? 1 : -1), 32 * i);
                    Projectile p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Owner.Center - offset, Vector2.Zero, ModContent.ProjectileType<TerrarTornado>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner, 40 * i, 0, 1);
                    p.LAP().isWeaponSkillProj = true;
                    Vector2 offset2 = new Vector2(56 * i * (j == 0 ? 1 : -1), 48);
                    Projectile p2 = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Owner.Center - offset2, Vector2.Zero, ModContent.ProjectileType<TerrarTornado>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner, 20 * i, 0, 1);
                    p2.LAP().isWeaponSkillProj = true;
                }
            }
            Projectile p3 = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Owner.Center - new Vector2(96, -24), Vector2.Zero, ModContent.ProjectileType<TerrarTornado>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner, 10, 0, 1);
            p3.LAP().isWeaponSkillProj = true;
            Projectile p4 = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Owner.Center - new Vector2(-96, -24), Vector2.Zero, ModContent.ProjectileType<TerrarTornado>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner, 30, 0, 1);
            p4.LAP().isWeaponSkillProj = true;
            Projectile p5 = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Owner.Center - new Vector2(0, -64), Vector2.Zero, ModContent.ProjectileType<TerrarTornado>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner, 10, 0, 1);
            p5.LAP().isWeaponSkillProj = true;
            Projectile p6 = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Owner.Center - new Vector2(0, 102), Vector2.Zero, ModContent.ProjectileType<TerrarTornado>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner, 30, 0, 1);
            p6.LAP().isWeaponSkillProj = true;
        }
        public void GenTrackTornado(Vector2 GenPos)
        {
            if (!Projectile.IsLocalPlayer())
                return;
            for (int j = 0; j < 2; j++)
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 offset = new Vector2(56 * i * (j == 0 ? 1 : -1), 32 * i);
                    Projectile p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), GenPos - offset, Vector2.Zero, ModContent.ProjectileType<TerrarTornado>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner, 40 * i, 0, 0);
                    p.LAP().isWeaponSkillProj = true;
                    Vector2 offset2 = new Vector2(56 * i * (j == 0 ? 1 : -1), 48);
                    Projectile p2 = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), GenPos - offset2, Vector2.Zero, ModContent.ProjectileType<TerrarTornado>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner, 20 * i, 0, 0);
                    p2.LAP().isWeaponSkillProj = true;
                }
            }
            Projectile p3 = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), GenPos - new Vector2(48, -24), Vector2.Zero, ModContent.ProjectileType<TerrarTornado>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner, 10, 0, 0);
            p3.LAP().isWeaponSkillProj = true;
            Projectile p4 = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), GenPos - new Vector2(-48, -24), Vector2.Zero, ModContent.ProjectileType<TerrarTornado>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner, 30, 0, 0);
            p4.LAP().isWeaponSkillProj = true;
        }
        #endregion
        #region 处理动画
        public void HandleBeginAni()
        {
            int MaxAni = AniHelper.MaxAniProgress[AniState.Begin];
            int CurAni = AniHelper.AniProgress[AniState.Begin];
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
            int MaxAni = AniHelper.MaxAniProgress[AniState.Middle];
            int CurAni = AniHelper.AniProgress[AniState.Middle];
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
            int MaxAni = AniHelper.MaxAniProgress[AniState.End];
            int CurAni = AniHelper.AniProgress[AniState.End];
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
            Main.graphics.GraphicsDevice.Textures[1] = LAPTextureRegister.Noise.Value;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            LAPUtilities.FastApplyEdgeMeltsShader(Opacity, texture.Size(), Color.LimeGreen, 0.01f, 0);
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            float drawRotation = Projectile.rotation + (Projectile.spriteDirection == -1 ? MathHelper.PiOver2 + MathHelper.PiOver4 : MathHelper.PiOver4);
            Vector2 rotationPoint = texture.Size() / 2f;
            SpriteEffects flipSprite = Projectile.spriteDirection * Main.player[Projectile.owner].gravDir == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.spriteBatch.Draw(texture, drawPosition, null, lightColor, drawRotation, rotationPoint, Projectile.scale * Main.player[Projectile.owner].gravDir, flipSprite, 0f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }
}
