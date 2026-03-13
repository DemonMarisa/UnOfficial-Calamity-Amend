using LAP.Content.Particles.CalParticiles;
using LAP.Core.BaseClass.Legacys;
using LAP.Core.Enums;
using LAP.Core.Graphics.PixelatedRender;
using LAP.Core.SystemsLoader;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Assets.Effects;
using UCA.Assets.Sounds;
using UCA.Content.Items.Weapons.Magic.Ray;
using UCA.Content.Particiles;
using UCA.Content.Paths;
using UCA.Content.Projectiles.Magic.Ray;
using UCA.Core.GlobalInstance.Players;
using UCA.Core.Utilities;

namespace UCA.Content.Projectiles.HeldProj.Magic.NightRatHeld
{
    public class NightRayHeldProjMelee : BaseHeldProj, IPixelatedRenderer
    {
        public DrawLayer drawLayer = DrawLayer.BeforeDusts;
        public override LocalizedText DisplayName => LAPUtilities.GetItemName<NightsRayAlt>();
        public override string Texture => $"{ProjPath.HeldProjPath}" + "Magic/NightRatHeld/NightRayHeldProj";
        public Vector2 RotVector => new Vector2(12 * Owner.direction, 7).BetterRotatedBy(Owner.GetPlayerToMouseVector2().ToRotation(), default, 0.5f, 1f);

        public override Vector2 RotPoint => TextureAssets.Projectile[Type].Size() / 2;

        public override Vector2 Posffset => new Vector2(RotVector.X, RotVector.Y) * Owner.direction;

        public override float RotAmount => 0.25f;

        public override float RotOffset => MathHelper.PiOver4;

        // 控制动画进度
        public int AniProgress = 0;

        // 这里0才是完全出现
        public float ShaderOpacity = 1f;

        public float XScale = 0f;

        public Vector2 FireOffset => new Vector2(26, 0.8f * Owner.direction).RotatedBy(Projectile.rotation);

        public bool CanGiveBoost = false;

        public int InToAni = 30;

        public int DelTimer = 0;

        public bool ShouldSpawnFullChargeDust = false;
        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 58;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.netImportant = true;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(ShaderOpacity);
            writer.Write(Projectile.ai[0]);
            writer.Write(Projectile.ai[1]);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            ShaderOpacity = reader.ReadSingle();
            Projectile.ai[0] = reader.ReadSingle();
            Projectile.ai[1] = reader.ReadSingle();
        }
        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }
        #region 主AI
        public override void HoldoutAI()
        {
            PixelatedRenderManger.BeginDrawProj = true;
            if (AniProgress < InToAni)
                return;
            if (Owner.UCA().NightShieldHP != UCAPlayer.NightShieldMaxHP && Owner.miscCounter % 4 == 0)
            {
                if (CanGiveBoost = Owner.CheckMana(Owner.ActiveItem(), 1, true, false))
                    Owner.UCA().NightShieldHP += 2;
            }
            if (CanGiveBoost)
            {
                Owner.UCA().HeldNightShield = true;
            }
            else
            {
                Owner.UCA().HeldNightShield = true;
                Owner.UCA().WeakHeldNightShield = true;
            }
            // 护盾效果
            ProtectPlayer();
            UpdateChargeDust();
        }
        public override void ExtraHoldoutAI()
        {
            PixelatedRenderManger.BeginDrawProj = true;
            if (AniProgress < InToAni)
                return;

            // 按住左键不会开火
            if (Owner.LAP().MouseLeft)
            {
                DelTimer = Owner.HeldItem.useTime;
                return;
            }
            if (UseDelay <= 0 && Owner.CheckMana(Owner.ActiveItem(), (int)(Owner.HeldItem.mana * Owner.manaCost), true, false))
            {
                Vector2 firePos = Projectile.Center + new Vector2(90, 0).RotatedBy(Projectile.rotation);
                if (Projectile.owner == Main.myPlayer)
                {
                    int a = Projectile.NewProjectile(Projectile.GetSource_FromThis(), firePos, Projectile.rotation.ToRotationVector2() * 1.8f, ModContent.ProjectileType<NightEnergy>(), Projectile.damage, Projectile.knockBack, Owner.whoAmI, 1);
                    Main.projectile[a].timeLeft = 99;
                }

                NightsRayAlt.UseCount++;
                SoundEngine.PlaySound(SoundsMenu.NightRayHeavyAttack, Projectile.Center);

                NightRayHeldProj.GenUnDeathSign(firePos);

                for (int i = 0; i < 30; i++)
                {
                    Color color = Color.Lerp(Color.DarkOrchid, Color.DarkViolet, Main.rand.NextFloat(0, 1f));
                    new Line(firePos, Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(3, 7), color, Main.rand.Next(60, 90), 0, 1, 0.2f, false, firePos).Spawn();
                }

                for (int j = 0; j < 2; j++)
                {
                    Vector2 SpawnPos = Owner.Center + new Vector2(Main.rand.Next(100, 200), 0).RotatedByRandom(MathHelper.TwoPi);
                    for (int i = 0; i < 50; i++)
                    {
                        SpawnPos = Owner.Center + new Vector2(Main.rand.Next(100, 200), 0).RotatedByRandom(MathHelper.TwoPi);
                        if (Collision.CanHit(Owner.Center, 0, 0, SpawnPos + (SpawnPos - Owner.Center).SafeNormalize(Vector2.UnitX) * 8f, 0, 0))
                        {
                            break;
                        }
                    }
                    NPC npc = LAPUtilities.FindClosestTarget(Projectile.Center,1500, false);
                    if (npc != null)
                    {
                        float DistanceToNPC = Vector2.Distance(SpawnPos, npc.Center);
                        float PredictMult = DistanceToNPC / 48;
                        Vector2 ToNPCVel = (npc.Center - SpawnPos + npc.velocity * PredictMult).SafeNormalize(Projectile.rotation.ToRotationVector2());
                        if (Projectile.owner == Main.myPlayer)
                        {
                            int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), SpawnPos, ToNPCVel * 4, ModContent.ProjectileType<NightEnergySplit>(), Projectile.damage, Projectile.knockBack, Owner.whoAmI, 0.5f);
                            Main.projectile[p].penetrate = 1;
                        }
                    }
                    else
                    {
                        if (Projectile.owner == Main.myPlayer)
                        {
                            int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), SpawnPos, Projectile.rotation.ToRotationVector2() * 4, ModContent.ProjectileType<NightEnergySplit>(), Projectile.damage, Projectile.knockBack, Owner.whoAmI, 0.5f);
                            Main.projectile[p].penetrate = 1;
                        }
                    }
                }
                Projectile.velocity -= Projectile.velocity.RotatedBy(Projectile.spriteDirection * MathHelper.PiOver2) * 0.12f * Owner.direction;
                UseDelay = Owner.HeldItem.useTime * 2;
            }
        }
        #endregion
        public override void InDel()
        {
            if (DelTimer > 0)
                DelTimer--;

            if (DelTimer <= 0)
                Projectile.Kill();
        }
        #region 常驻AI
        public override void PostAI()
        {
            PixelatedRenderManger.BeginDrawProj = true;
            float baseRotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            float directionVerticality = MathF.Abs(Projectile.velocity.X);
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, baseRotation + Owner.direction * directionVerticality * 1.5f);
            Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, baseRotation + Owner.direction * directionVerticality * 1.2f);
            
            if (AniProgress < InToAni)
                AniProgress++;

            if (Main.mouseRight || Active)
            {
                XScale = MathHelper.Lerp(XScale, 1f, 0.1f);
                ShaderOpacity = MathHelper.Lerp(ShaderOpacity, 0f, 0.1f);
            }
            else
            {
                XScale = MathHelper.Lerp(XScale, 0, 0.02f);
                ShaderOpacity = MathHelper.Lerp(ShaderOpacity, 1f, 0.02f);
            }
        }
        #endregion
        #region 绘制
        public override bool ExtraPreDraw(ref Color lightColor)
        {
            PixelatedRenderManger.BeginDrawProj = true;
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D Weapontexture = TextureAssets.Projectile[Type].Value;

            Main.graphics.GraphicsDevice.Textures[0] = Weapontexture;
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

            Main.graphics.GraphicsDevice.Textures[1] = UCATextureRegister.Noise.Value;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;

            LAPUtilities.FastApplyEdgeMeltsShader(ShaderOpacity, Weapontexture.Size(), Color.DarkViolet, 0.01f, 0);

            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            float drawRotation = Projectile.rotation + (Owner.direction == -1 ? MathHelper.Pi : 0f) + RotOffset * Owner.direction;

            Vector2 rotationPoint = RotPoint;

            SpriteEffects flipSprite = Owner.direction * Main.player[Projectile.owner].gravDir == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Main.spriteBatch.Draw(texture, drawPosition, null, Projectile.GetAlpha(lightColor), drawRotation, rotationPoint, Projectile.scale * Main.player[Projectile.owner].gravDir, flipSprite, default);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
        #endregion
        #region 护盾碰撞
        public void ProtectPlayer()
        {
            if (Projectile.owner != Main.myPlayer)
                return;

            if (Owner.UCA().NightShieldHP <= 0 || !Owner.UCA().NightShieldCanDefense)
                return;

            Vector2 perpendicular = Projectile.velocity.RotatedBy(MathHelper.PiOver2);
            Vector2 forcefieldStart = Projectile.Center + Projectile.rotation.ToRotationVector2() * 70;
            foreach (Projectile projectile in Main.ActiveProjectiles)
            {
                // 需要：是敌对弹幕，活跃，伤害不超过100，不是无限穿
                if (!projectile.hostile || !projectile.active || projectile.UCA().HasThroughNightShield || Projectile.velocity == Vector2.Zero)
                    continue;

                if (ProjectileID.Sets.DrawScreenCheckFluff[projectile.type] > 500)
                    continue;

                bool movingTowardsForcefield = Vector2.Dot(projectile.velocity, Projectile.rotation.ToRotationVector2()) < 0f;
                bool collidingWithForcefield =
                    projectile.Colliding(projectile.Hitbox, Utils.CenteredRectangle(forcefieldStart - perpendicular * 40, Vector2.One * 45)) ||
                    projectile.Colliding(projectile.Hitbox, Utils.CenteredRectangle(forcefieldStart + perpendicular * 40, Vector2.One * 45)) ||
                    projectile.Colliding(projectile.Hitbox, Utils.CenteredRectangle(forcefieldStart, Vector2.One * 60));

                if (collidingWithForcefield && movingTowardsForcefield)
                {
                    Vector2 impactPoint = Projectile.Center + LAPUtilities.GetVector2(Projectile.Center, projectile.Center) * 75f;

                    float bloomScaleFactor = Main.rand.NextFloat(0.6f, 0.95f) * 0.4f;

                    for (int i = 0; i < 3; i++)
                    {
                        new StrongBloom(impactPoint, Vector2.Zero, Color.DeepPink, bloomScaleFactor * 0.56f, 9).Spawn();
                        new StrongBloom(impactPoint, Vector2.Zero, Color.MediumPurple * 0.6f, bloomScaleFactor * 0.95f, 12).Spawn();
                        new StrongBloom(impactPoint, Vector2.Zero, Color.White * 0.35f, bloomScaleFactor * 1.5f, 14).Spawn();
                    }
                    Vector2 TangentVector = (impactPoint - (Projectile.Center + FireOffset)).RotatedBy(MathHelper.PiOver2);
                    for (int i = 0; i < 10; i++)
                    {
                        Color color = Color.Lerp(Color.LightPink, Color.DarkViolet, Main.rand.NextFloat(0, 1f));
                        new GlowBall(impactPoint, TangentVector.RotatedByRandom(0.1f) * Main.rand.NextFloat(0.1f, 0.3f) * 0.35f, color, Main.rand.Next(30, 60), 0, 1, 0.1f).Spawn();
                    }
                    for (int i = 0; i < 10; i++)
                    {
                        Color color = Color.Lerp(Color.LightPink, Color.DarkViolet, Main.rand.NextFloat(0, 1f));
                        new GlowBall(impactPoint,- TangentVector.RotatedByRandom(0.1f) * Main.rand.NextFloat(0.1f, 0.3f) * 0.35f, color, Main.rand.Next(30, 60), 0, 1, 0.1f).Spawn();
                    }
                    // 这一块的逻辑是
                    // 弹幕击中后，在这里处理伤害吸收和反弹
                    // 在对应全局射弹中处理反弹后的伤害变化
                    int realDamage = (int)LAPUtilities.PostModeBoostProjDamage(projectile.damage);

                    if ((int)Owner.ApplyPlayerDefAndDR(realDamage, false) < Owner.UCA().NightShieldHP)
                    {
                        projectile.velocity *= -0.7f;
                        projectile.velocity += Main.rand.NextVector2Circular(2f, 2f);
                        Owner.UCA().NightShieldHP -= (int)Owner.ApplyPlayerDefAndDR(realDamage, false);
                        projectile.UCA().HasThroughNightShield = true;
                    }
                    else
                    {
                        SoundEngine.PlaySound(SoundsMenu.NightRayShieldBreak, Projectile.Center);
                        projectile.UCA().HasThroughNightShield = true;
                        projectile.UCA().HasThroughNightShieldOverMax = true;
                        projectile.UCA().DamageDefence = Owner.UCA().NightShieldHP;
                        Owner.UCA().NightShieldHP = 0;
                        for (int i = 0; i < 50; i++)
                        {
                            Color color = Color.Lerp(Color.LightPink, Color.Purple, Main.rand.NextFloat(0, 1f));
                            new GlowBall(impactPoint, Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(2f, 10f), color, Main.rand.Next(90, 120), 0, 1, 0.1f, true).Spawn();
                        }
                    }

                    SoundEngine.PlaySound(SoundsMenu.NightShieldHit, impactPoint);

                    projectile.UCA().HasThroughNightShield = true;
                    projectile.netSpam = 0;
                    projectile.netUpdate = true;
                }
            }
        }
        #endregion
        #region 更新满充能的粒子
        public void UpdateChargeDust()
        {
            if (!Owner.UCA().NightShieldCanDefense)
            {
                Vector2 SpawnPos = Projectile.Center + FireOffset + Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.Next(25, 75);
                Vector2 SpawnPosToMouseWorld = (Projectile.Center + FireOffset - SpawnPos).SafeNormalize(Vector2.UnitX);
                float rot = SpawnPosToMouseWorld.ToRotation() + 3;
                Color color = Color.Lerp(Color.DarkOrchid, Color.DarkViolet, Main.rand.NextFloat(0, 1f));
                new Line(SpawnPos, Vector2.Zero, color, Main.rand.Next(45, 70), rot, 1, 0.15f, true, Projectile.Center + FireOffset).Spawn();
                ShouldSpawnFullChargeDust = true;
            }

            if (ShouldSpawnFullChargeDust && Owner.UCA().NightShieldCanDefense)
            {
                SoundEngine.PlaySound(SoundsMenu.NightShieldCharge, Projectile.Center);

                for (int i = 0; i < 50; i++)
                {
                    Color color = Color.Lerp(Color.LightPink, Color.Purple, Main.rand.NextFloat(0, 1f));
                    new GlowBall(Projectile.Center + FireOffset, Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(2f, 10f), color, Main.rand.Next(90, 120), 0, 1, 0.1f, true).Spawn();
                }

                ShouldSpawnFullChargeDust = false;
            }
        }
        #endregion
        void IPixelatedRenderer.RenderPixelated(SpriteBatch spriteBatch)
        {
            PixelatedRenderManger.BeginDrawProj = true;
            float OpacityOffset = MathHelper.Lerp(1f, 0f, Owner.UCA().NightShieldHP / (float)UCAPlayer.NightShieldMaxHP);

            LAPContent.ReSetToBeginShader_Pixel(BlendState.AlphaBlend);

            Main.graphics.GraphicsDevice.Textures[0] = UCATextureRegister.NightRayShield.Value;
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

            Main.graphics.GraphicsDevice.Textures[1] = UCATextureRegister.Noise.Value;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;

            Texture2D Weapontexture = TextureAssets.Projectile[Type].Value;

            LAPUtilities.FastApplyEdgeMeltsShader(ShaderOpacity + OpacityOffset * 0.5f, Weapontexture.Size(), Color.DarkViolet, 0.01f, 0);

            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 ShieledPos = drawPosition + new Vector2(60, 0).RotatedBy(Projectile.rotation);
            float drawRotation = Projectile.rotation + (Owner.direction == -1 ? MathHelper.Pi : 0f) + RotOffset * Owner.direction;
            SpriteEffects flipSprite = Owner.direction * Main.player[Projectile.owner].gravDir == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.spriteBatch.Draw(UCATextureRegister.NightRayShield.Value, ShieledPos, null, new Color(255, 0, 255, 255), drawRotation + MathHelper.PiOver4 * Owner.direction,
                UCATextureRegister.NightRayShield.Size() / 2, new Vector2(XScale, 1) * Projectile.scale * Main.player[Projectile.owner].gravDir * 0.35f, flipSprite, default);
            Main.spriteBatch.Draw(UCATextureRegister.NightRayShield.Value, ShieledPos, null, new Color(255, 0, 255, 155), drawRotation + MathHelper.PiOver4 * Owner.direction,
                UCATextureRegister.NightRayShield.Size() / 2, new Vector2(XScale, 1) * Projectile.scale * Main.player[Projectile.owner].gravDir * 0.4f, flipSprite, default);  
            LAPContent.ReSetToBeginShader_Pixel(BlendState.Additive);
            Vector2 SpreadLinePos = drawPosition + FireOffset;
            Main.spriteBatch.Draw(UCATextureRegister.SpreadLine.Value, SpreadLinePos, null, new Color(185, 0, 204, 255) * (1 - ShaderOpacity), drawRotation + MathHelper.PiOver4 * Owner.direction,
                UCATextureRegister.SpreadLine.Size() / 2, new Vector2(XScale * 1.2f * (1 - OpacityOffset * 0.7f), 1) * Projectile.scale * Main.player[Projectile.owner].gravDir * 0.35f, flipSprite, default);
            LAPContent.ReSetToEndShader_Pixel();
        }
    }
}
