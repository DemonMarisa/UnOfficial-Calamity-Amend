using CalamityMod;
using CalamityMod.Graphics.Primitives;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Assets.Effects;
using UCA.Content.ItemOverride.Magic;
using UCA.Content.Particiles;
using UCA.Content.Paths;
using UCA.Content.Projectiles.Magic.Ray;
using UCA.Core.BaseClass;
using UCA.Core.GlobalInstance.Players;
using UCA.Core.Utilities;

namespace UCA.Content.Projectiles.HeldProj.Magic
{
    public class NightRayHeldProjMelee : BaseHeldProj, IPixelatedPrimitiveRenderer
    {
        public PixelationPrimitiveLayer LayerToRenderTo => PixelationPrimitiveLayer.AfterPlayers;

        public override LocalizedText DisplayName => CalamityUtils.GetItemName<NightsRay>();
        public override string Texture => $"{ProjPath.HeldProjPath}" + "Magic/NightRayHeldProj";
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
        }

        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }
        #region 主AI
        public override void HoldoutAI()
        {
            if (AniProgress < InToAni)
                return;

            if (Owner.miscCounter % 4 == 0)
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
            // 按住左键不会开火
            if (Main.mouseLeft)
            {
                DelTimer = Owner.HeldItem.useTime * 2;
                return;
            }

            if (UseDelay <= 0 && Owner.CheckMana(Owner.ActiveItem(), (int)(Owner.HeldItem.mana * Owner.manaCost), true, false))
            {
                Vector2 firePos = Projectile.Center + new Vector2(90, 0).RotatedBy(Projectile.rotation);

                SoundEngine.PlaySound(SoundsMenu.NightRayHeavyAttack, Projectile.Center);
                int a = Projectile.NewProjectile(Projectile.GetSource_FromThis(), firePos, Projectile.rotation.ToRotationVector2() * 1.8f, ModContent.ProjectileType<NightEnergy>(), Projectile.damage, Projectile.knockBack, Owner.whoAmI, 1);
                Main.projectile[a].timeLeft = 99;

                Projectile.velocity -= Projectile.velocity.RotatedBy(Projectile.spriteDirection * MathHelper.PiOver2) * 0.12f;
                
                NightRayHeldProj.GenUnDeathSign(firePos);

                for (int i = 0; i < 30; i++)
                {
                    Color color = Color.Lerp(Color.DarkOrchid, Color.DarkViolet, Main.rand.NextFloat(0, 1f));
                    new Line(firePos, Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(3, 7), color, Main.rand.Next(60, 90), 0, 1, 0.2f, false, firePos).Spawn();
                }

                NightsRayOverride.UseCount++;
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
                    NPC npc = Projectile.FindClosestTarget(1500, true);
                    if (npc != null)
                    {
                        float DistanceToNPC = Vector2.Distance(SpawnPos, npc.Center);
                        float PredictMult = DistanceToNPC / 48;
                        Vector2 ToNPCVel = (npc.Center - SpawnPos + npc.velocity * PredictMult).SafeNormalize(Projectile.rotation.ToRotationVector2());
                        NightRayHeldProj.GenUnDeathSign(SpawnPos, 0.4f);
                        int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), SpawnPos, ToNPCVel * 4, ModContent.ProjectileType<NightEnergySplit>(), Projectile.damage, Projectile.knockBack, Owner.whoAmI);
                        Main.projectile[p].penetrate = 1;
                    }
                    else
                    {
                        NightRayHeldProj.GenUnDeathSign(SpawnPos, 0.4f);
                        int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), SpawnPos, Projectile.rotation.ToRotationVector2() * 4, ModContent.ProjectileType<NightEnergySplit>(), Projectile.damage, Projectile.knockBack, Owner.whoAmI);
                        Main.projectile[p].penetrate = 1;
                    }
                }
                UseDelay = Owner.HeldItem.useTime * 2;
            }
        }
        #endregion
        public override void InDel()
        {
            if (DelTimer > 0)
                DelTimer--;

            if (UseDelay <= 0 && DelTimer <= 0)
                Projectile.Kill();
        }
        #region 常驻AI
        public override void PostAI()
        {
            float baseRotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            float directionVerticality = MathF.Abs(Projectile.velocity.X);
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, baseRotation + Owner.direction * directionVerticality * 1.5f);
            Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, baseRotation + Owner.direction * directionVerticality * 1.2f);
            
            if (AniProgress < InToAni)
                AniProgress++;

            if (Main.mouseRight)
            {
                // UseDelay = Owner.HeldItem.useTime * 2;
                XScale = MathHelper.Lerp(XScale, 1f, 0.1f);
                ShaderOpacity = MathHelper.Lerp(ShaderOpacity, 0f, 0.1f);
            }

            if (!Main.mouseRight)
            {
                XScale = MathHelper.Lerp(XScale, 0, 0.064f);
                ShaderOpacity = MathHelper.Lerp(ShaderOpacity, 1f, 0.064f);
            }
        }
        #endregion
        #region 绘制
        public override bool ExtraPreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.graphics.GraphicsDevice.Textures[0] = ModContent.Request<Texture2D>(Texture).Value;
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

            Main.graphics.GraphicsDevice.Textures[1] = UCATextureRegister.Noise.Value;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;

            UCAShaderRegister.EdgeMeltsShader.Parameters["progress"].SetValue(ShaderOpacity);
            UCAShaderRegister.EdgeMeltsShader.Parameters["InPutTextureSize"].SetValue(ModContent.Request<Texture2D>(Texture).Size());
            UCAShaderRegister.EdgeMeltsShader.Parameters["EdgeColor"].SetValue(Color.DarkViolet.ToVector4());
            UCAShaderRegister.EdgeMeltsShader.Parameters["EdgeWidth"].SetValue(0.01f);
            UCAShaderRegister.EdgeMeltsShader.CurrentTechnique.Passes[0].Apply();

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

        void IPixelatedPrimitiveRenderer.RenderPixelatedPrimitives(SpriteBatch spriteBatch, PixelationPrimitiveLayer layer)
        {
            float OpacityOffset = MathHelper.Lerp(1f, 0f, Owner.UCA().NightShieldHP / (float)UCAPlayer.NightShieldMaxHP);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null);
            
            Main.graphics.GraphicsDevice.Textures[0] = UCATextureRegister.NightRayShield.Value;
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

            Main.graphics.GraphicsDevice.Textures[1] = UCATextureRegister.Noise.Value;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;

            UCAShaderRegister.EdgeMeltsShader.Parameters["progress"].SetValue(ShaderOpacity + OpacityOffset * 0.5f);
            UCAShaderRegister.EdgeMeltsShader.Parameters["InPutTextureSize"].SetValue(ModContent.Request<Texture2D>(Texture).Size());
            UCAShaderRegister.EdgeMeltsShader.Parameters["EdgeColor"].SetValue(Color.DarkViolet.ToVector4());
            UCAShaderRegister.EdgeMeltsShader.Parameters["EdgeWidth"].SetValue(0.01f);
            UCAShaderRegister.EdgeMeltsShader.CurrentTechnique.Passes[0].Apply();

            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 ShieledPos = drawPosition + new Vector2(60, 0).RotatedBy(Projectile.rotation);
            float drawRotation = Projectile.rotation + (Owner.direction == -1 ? MathHelper.Pi : 0f) + RotOffset * Owner.direction;

            SpriteEffects flipSprite = Owner.direction * Main.player[Projectile.owner].gravDir == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Main.spriteBatch.Draw(UCATextureRegister.NightRayShield.Value, ShieledPos / 2, null, new Color(255, 0, 255, 255), drawRotation + MathHelper.PiOver4 * Owner.direction,
                UCATextureRegister.NightRayShield.Size() / 2, new Vector2(XScale, 1) * Projectile.scale * Main.player[Projectile.owner].gravDir * 0.175f, flipSprite, default);

            Main.spriteBatch.Draw(UCATextureRegister.NightRayShield.Value, ShieledPos / 2, null, new Color(255, 0, 255, 155), drawRotation + MathHelper.PiOver4 * Owner.direction,
                UCATextureRegister.NightRayShield.Size() / 2, new Vector2(XScale, 1) * Projectile.scale * Main.player[Projectile.owner].gravDir * 0.2f, flipSprite, default);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null);

            Vector2 SpreadLinePos = drawPosition + FireOffset;

            Main.spriteBatch.Draw(UCATextureRegister.SpreadLine.Value, SpreadLinePos / 2, null, new Color(185, 0, 204, 255) * (1 - ShaderOpacity), drawRotation + MathHelper.PiOver4 * Owner.direction,
                UCATextureRegister.SpreadLine.Size() / 2, new Vector2(XScale * 1.2f * (1 - OpacityOffset * 0.7f), 1) * Projectile.scale * Main.player[Projectile.owner].gravDir * 0.175f, flipSprite, default);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null);
        }
        #endregion
        #region 护盾碰撞
        public void ProtectPlayer()
        {
            if (Owner.UCA().NightShieldHP <= 0 || !Owner.UCA().NightShieldCanDefense)
                return;

            Vector2 perpendicular = Projectile.velocity.RotatedBy(MathHelper.PiOver2);
            Vector2 forcefieldStart = Projectile.Center + Projectile.rotation.ToRotationVector2() * 70;
            foreach (Projectile projectile in Main.ActiveProjectiles)
            {
                // 需要：是敌对弹幕，活跃，伤害不超过100，不是无限穿，可以被反弹
                if (!projectile.hostile || !projectile.active || projectile.reflected || projectile.UCA().HasThroughNightShield)
                    continue;

                bool movingTowardsForcefield = Vector2.Dot(projectile.velocity, Projectile.rotation.ToRotationVector2()) < 0f;
                bool collidingWithForcefield =
                    projectile.Colliding(projectile.Hitbox, Utils.CenteredRectangle(forcefieldStart - perpendicular * 40, Vector2.One * 45)) ||
                    projectile.Colliding(projectile.Hitbox, Utils.CenteredRectangle(forcefieldStart + perpendicular * 40, Vector2.One * 45)) ||
                    projectile.Colliding(projectile.Hitbox, Utils.CenteredRectangle(forcefieldStart, Vector2.One * 60));

                if (collidingWithForcefield && movingTowardsForcefield)
                {
                    Vector2 impactPoint = Projectile.Center + Projectile.SafeDirectionTo(projectile.Center) * 75f;

                    float bloomScaleFactor = Main.rand.NextFloat(0.6f, 0.95f) * 0.4f;

                    for (int i = 0; i < 3; i++)
                    {
                        StrongBloom bloom = new(impactPoint, Vector2.Zero, Color.DeepPink, bloomScaleFactor * 0.56f, 9);
                        GeneralParticleHandler.SpawnParticle(bloom);
                        
                        StrongBloom glow = new(impactPoint, Vector2.Zero, Color.MediumPurple * 0.6f, bloomScaleFactor * 0.95f, 12);
                        GeneralParticleHandler.SpawnParticle(glow);

                        StrongBloom outerGlow = new(impactPoint, Vector2.Zero, Color.White * 0.35f, bloomScaleFactor * 1.5f, 14);
                        GeneralParticleHandler.SpawnParticle(outerGlow);
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
                    int realDamage = (int)UCAUtilities.PostModeBoostProjDamage(projectile.damage);

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
                    projectile.Calamity().DealsDefenseDamage = false;

                    projectile.UCA().HasThroughNightShield = true;

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
    }
}
