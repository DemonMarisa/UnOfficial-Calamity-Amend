using LAP.Assets.TextureRegister;
using LAP.Core.AnimationHandle;
using LAP.Core.BaseClass.Projectiles;
using LAP.Core.Enums;
using LAP.Core.Graphics.PixelatedRender;
using LAP.Core.IDSets;
using LAP.Core.NetCode.NetUtilities;
using LAP.Core.StateMachine.SynedHitEffect;
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
using UCA.Assets.Sounds;
using UCA.Content.HitEffect;
using UCA.Content.Items.Weapons.Magic.Ray;
using UCA.Content.Particiles;
using UCA.Content.Projectiles.Magic.Ray;
using UCA.Core.GlobalInstance.Players;
using UCA.Core.Presets;
using UCA.Core.Utilities;

namespace UCA.Content.Projectiles.HeldProj.Magic.NightRatHeld
{
    public class NightRayHeldProjMelee : BaseHeldProj, IPixelatedRenderer
    {
        public DrawLayer drawLayer = DrawLayer.BeforeDusts;
        public override LocalizedText DisplayName => LAPUtilities.GetItemName<NightsRayAlt>();
        public override string Texture => GetInstance<NightRayHeldProj>().Texture;
        public Vector2 RotVector => new Vector2(12 * Owner.direction, 7).BetterRotatedBy(Owner.GetPlayerToMouseVector2().ToRotation(), default, 0.5f, 1f);
        public override Vector2 PositionOffset => RotVector * Owner.direction;
        public Vector2 FireOffset => new Vector2(26, 0.8f * Owner.direction).RotatedBy(Projectile.rotation);
        public bool ShouldSpawnFullChargeDust;
        public bool BeginFadeOut;
        public float XScale;
        public float OpacityOffset;
        public AniHelper AniProgress = new AniHelper(3);
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
            Projectile.Opacity = 0f;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.Opacity);
            writer.Write(OpacityOffset);
            writer.Write(XScale);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.Opacity = reader.ReadSingle();
            OpacityOffset = reader.ReadSingle();
            XScale = reader.ReadSingle();
        }
        #region 主AI
        public override void Initialize()
        {
            RotAmount = 0.25f;
        }
        public override bool ExPreAI()
        {
            BeginFadeOut = true;
            return true;
        }
        public override void ExAI()
        {
            Projectile.netSpam = 0;
            Projectile.netUpdate = true;
            if (Projectile.IsLocalPlayer())
            {
                float opacityOffset = MathHelper.Lerp(1f, 0f, Owner.UCA().NightShieldHP / (float)UCAPlayer.NightShieldMaxHP);
                OpacityOffset = MathHelper.Clamp(opacityOffset, 0f, 0.8f);
                if (Owner.LAP().MouseRight || Active)
                {
                    BeginFadeOut = false;
                    XScale = MathHelper.Lerp(XScale, 1f, 0.1f);
                    Projectile.Opacity = MathHelper.Lerp(Projectile.Opacity, 1f, 0.1f);
                }
                else
                {
                    XScale = MathHelper.Lerp(XScale, 0, 0.12f);
                    Projectile.Opacity = MathHelper.Lerp(Projectile.Opacity, 0f, 0.12f);
                }
            }
            UpdateShield();
            UpdateFire();
        }
        public void UpdateShield()
        {
            if (Projectile.Opacity < 0.5f)
                return;
            if (Owner.UCA().NightShieldHP < UCAPlayer.NightShieldMaxHP && Owner.miscCounter % 4 == 0)
            {
                int cost = 1;
                if (Owner.manaCost == 0)
                    cost = 0;
                if (Owner.CheckMana(Owner.ActiveItem(), cost, true, false))
                    Owner.UCA().NightShieldHP += 2;
            }
            if (Owner.UCA().NightShieldHP == UCAPlayer.NightShieldMaxHP)
                Owner.UCA().NightShieldCanBlock = true;
            // 护盾效果
            ProtectPlayer();
            UpdateChargeDust();
        }
        public void UpdateFire()
        {
            if (Projectile.Opacity < 0.5f)
                return;
            // 按住左键不会开火
            if (!Owner.LAP().MouseLeft && Owner.LAP().MouseRight)
            {
                if (UseDelay <= 0 && Owner.CheckMana(Owner.ActiveItem(), (int)(Owner.HeldItem.mana * Owner.manaCost), true, false))
                {
                    Vector2 firePos = Projectile.Center + new Vector2(90, 0).RotatedBy(Projectile.rotation);
                    SoundEngine.PlaySound(SoundsMenu.NightRayHeavyAttack, Projectile.Center);
                    UCAParticlePreset.GenUnDeathSign(firePos);
                    for (int i = 0; i < 30; i++)
                    {
                        Color color = Color.Lerp(Color.DarkOrchid, Color.DarkViolet, Main.rand.NextFloat(0, 1f));
                        new Line(firePos, Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(3, 7), color, Main.rand.Next(60, 90), 0, 1, 0.2f, false, firePos).Spawn();
                    }
                    if (Projectile.IsLocalPlayer())
                    {
                        NightsRayAlt.UseCount++;
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), firePos, Projectile.rotation.ToRotationVector2() * 1.8f, ModContent.ProjectileType<NightEnergyHeavy>(), Projectile.damage, Projectile.knockBack, Owner.whoAmI, 1);
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
                            NPC npc = LAPUtilities.FindClosestTarget(Projectile.Center, 1500, false);
                            if (npc != null)
                            {
                                float DistanceToNPC = Vector2.Distance(SpawnPos, npc.Center);
                                float PredictMult = DistanceToNPC / 48;
                                Vector2 ToNPCVel = (npc.Center - SpawnPos + npc.velocity * PredictMult).SafeNormalize(Projectile.rotation.ToRotationVector2());
                                Projectile.NewProjectile(Projectile.GetSource_FromThis(), SpawnPos, ToNPCVel * 4, ModContent.ProjectileType<NightEnergySplit>(), Projectile.damage, Projectile.knockBack, Owner.whoAmI, 0.5f);
                            }
                            else
                            {
                                Projectile.NewProjectile(Projectile.GetSource_FromThis(), SpawnPos, Projectile.rotation.ToRotationVector2() * 4, ModContent.ProjectileType<NightEnergySplit>(), Projectile.damage, Projectile.knockBack, Owner.whoAmI, 0.5f);
                            }
                        }
                    }
                    Projectile.velocity -= Projectile.velocity.RotatedBy(Projectile.spriteDirection * MathHelper.PiOver2) * 0.15f;
                    UseDelay = Owner.HeldItem.useTime * 2;
                }
            }
        }
        #endregion
        #region 常驻AI
        public override void ExPostAI()
        {
            float baseRotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            float directionVerticality = MathF.Abs(Projectile.velocity.X);
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, baseRotation + Owner.direction * directionVerticality * 1.5f);
            Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, baseRotation + Owner.direction * directionVerticality * 1.2f);
        }
        #endregion
        public override bool PreKill()
        {
            return BeginFadeOut && XScale < 0.1f;
        }
        #region 绘制
        public override bool PreDraw(ref Color lightColor)
        {
            PixelatedRenderManger.BeginDrawProj = true;
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D Weapontexture = TextureAssets.Projectile[Type].Value;

            Main.graphics.GraphicsDevice.Textures[0] = Weapontexture;
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

            Main.graphics.GraphicsDevice.Textures[1] = LAPTextureRegister.Noise.Value;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;

            LAPUtilities.FastApplyEdgeMeltsShader(1 - Projectile.Opacity, Weapontexture.Size(), Color.DarkViolet, 0.01f, 0);

            Projectile.GetProjDrawInfo_Staff(out Texture2D _, out Vector2 drawPosition, out float drawRotation, out Vector2 rotationPoint, out SpriteEffects flipSprite);

            Main.spriteBatch.Draw(Weapontexture, drawPosition, null, Projectile.GetAlpha(lightColor), drawRotation, rotationPoint, Projectile.scale * Main.player[Projectile.owner].gravDir, flipSprite, default);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
        #endregion
        #region 护盾碰撞
        // 只在本地判定反弹，其它客户端只判定特效
        public void ProtectPlayer()
        {
            if (Projectile.owner != Main.myPlayer)
                return;

            if (Owner.UCA().NightShieldHP <= 0 || !Owner.UCA().NightShieldCanBlock)
                return;

            Vector2 perpendicular = Projectile.velocity.RotatedBy(MathHelper.PiOver2);
            Vector2 forcefieldStart = Projectile.Center + Projectile.rotation.ToRotationVector2() * 70;

            // 获取护盾的法向量用于镜面反射
            Vector2 shieldNormal = Projectile.rotation.ToRotationVector2();
            foreach (Projectile projectile in Main.ActiveProjectiles)
            {
                // 需要：是敌对弹幕，活跃，不是无限穿
                if (!projectile.hostile || !projectile.active || projectile.UCA().NightShieldBeBlock || projectile.velocity == Vector2.Zero)
                    continue;
                if (ProjectileID.Sets.DrawScreenCheckFluff[projectile.type] > 500)
                    continue;
                if (LAPIDSet.HeldProj.Contains(projectile.type))
                    continue;
                if (LAPIDSet.ProtectedProj.Contains(projectile.type))
                    continue;
                bool movingTowardsForcefield = Vector2.Dot(projectile.velocity, Projectile.rotation.ToRotationVector2()) < 0f;
                if (!movingTowardsForcefield)
                    continue;
                float RealSpeed = projectile.velocity.Length() * (projectile.extraUpdates + 1);
                if (RealSpeed > 150)
                    return;
                bool collidingWithForcefield = false;

                // 根据extraUpdates分段回溯弹幕在这一帧内的运动轨迹
                int steps = projectile.extraUpdates + 1;
                for (int i = 0; i < steps; i++)
                {
                    // 计算回溯位置
                    Vector2 checkPos = projectile.position - projectile.velocity * i;
                    Rectangle hitbox = projectile.Hitbox;
                    hitbox.X = (int)checkPos.X;
                    hitbox.Y = (int)checkPos.Y;
                    // 使用回溯的 Hitbox 进行碰撞判定
                    if (projectile.Colliding(hitbox, Utils.CenteredRectangle(forcefieldStart - perpendicular * 40, Vector2.One * 45)) ||
                        projectile.Colliding(hitbox, Utils.CenteredRectangle(forcefieldStart + perpendicular * 40, Vector2.One * 45)) ||
                        projectile.Colliding(hitbox, Utils.CenteredRectangle(forcefieldStart, Vector2.One * 60)))
                    {
                        collidingWithForcefield = true;
                        break;
                    }
                }

                if (collidingWithForcefield)
                {
                    Vector2 impactPoint = Projectile.Center + LAPUtilities.GetVector2(Projectile.Center, projectile.Center) * 75f;
                    Vector2 TangentVector = (impactPoint - (Projectile.Center + FireOffset)) * 1.5f;

                    HitEffectManager.SpawnHitEffect(HitEffectManager.HEType<NightRayShieldHit>(), Projectile.owner, Projectile.GetSource_FromThis(), impactPoint, TangentVector);

                    // 这一块的逻辑是
                    // 弹幕击中后，在这里处理伤害吸收和反弹
                    // 在对应全局射弹中处理反弹后的伤害变化
                    int realDamage = (int)LAPUtilities.PostModeBoostProjDamage(projectile.damage);
                    if ((int)Owner.ApplyPlayerDefAndDR(realDamage, false) < Owner.UCA().NightShieldHP)
                    {
                        // V_new = V_old - 2 * (V_old · Normal) * Normal
                        projectile.velocity = projectile.velocity - 2f * Vector2.Dot(projectile.velocity, shieldNormal) * shieldNormal;
                        projectile.velocity += Main.rand.NextVector2Circular(1f, 1f);
                        int Finaldamage = (int)MathHelper.Clamp(Owner.ApplyPlayerDefAndDR(realDamage, false), 0, 401);
                        Owner.UCA().NightShieldHP -= Finaldamage;
                        projectile.UCA().NightShieldBeBlock = true;
                        projectile.damage = 0;
                    }
                    else
                    {
                        HitEffectManager.SpawnHitEffect(HitEffectManager.HEType<NightRayShieldBreakHit>(), Projectile.owner, Projectile.GetSource_FromThis(), impactPoint, TangentVector);

                        projectile.UCA().NightShieldBeBlock = true;
                        projectile.UCA().NightShieldFallBlock = true;
                        projectile.UCA().DamageDefence = Owner.UCA().NightShieldHP;
                        Owner.UCA().NightShieldHP = 0;
                        Owner.UCA().NightShieldCanBlock = false;
                    }

                    projectile.UCA().NightShieldBeBlock = true;
                    projectile.netSpam = 0;
                    projectile.netUpdate = true;

                    projectile.SyncedReflectProj();
                }
            }
        }
        #endregion
        #region 更新满充能的粒子
        public void UpdateChargeDust()
        {
            if (!Owner.UCA().NightShieldCanBlock)
            {
                Vector2 SpawnPos = Projectile.Center + FireOffset + Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.Next(25, 75);
                Vector2 SpawnPosToMouseWorld = (Projectile.Center + FireOffset - SpawnPos).SafeNormalize(Vector2.UnitX);
                float rot = SpawnPosToMouseWorld.ToRotation() + 3;
                Color color = Color.Lerp(Color.DarkOrchid, Color.DarkViolet, Main.rand.NextFloat(0, 1f));
                new Line(SpawnPos, Vector2.Zero, color, Main.rand.Next(45, 70), rot, 1, 0.15f, true, Projectile.Center + FireOffset).Spawn();
                ShouldSpawnFullChargeDust = true;
            }

            if (ShouldSpawnFullChargeDust && Owner.UCA().NightShieldCanBlock)
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
            LAPContent.ReSetToBeginShader_Pixel(BlendState.AlphaBlend);

            Main.graphics.GraphicsDevice.Textures[0] = UCATextureRegister.NightRayShield.Value;
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

            Main.graphics.GraphicsDevice.Textures[1] = LAPTextureRegister.Noise.Value;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;

            Texture2D Weapontexture = TextureAssets.Projectile[Type].Value;

            LAPUtilities.FastApplyEdgeMeltsShader((1 - Projectile.Opacity) + OpacityOffset * 0.5f, Weapontexture.Size(), Color.DarkViolet, 0.01f, 0);

            Projectile.GetProjDrawInfo_Staff(out Texture2D _, out Vector2 drawPosition, out float drawRotation, out Vector2 rotationPoint, out SpriteEffects flipSprite);

            Vector2 ShieledPos = drawPosition + new Vector2(60, 0).RotatedBy(Projectile.rotation);

            Main.spriteBatch.Draw(UCATextureRegister.NightRayShield.Value, ShieledPos, null, new Color(255, 0, 255, 255), drawRotation + MathHelper.PiOver4 * Projectile.spriteDirection,
                UCATextureRegister.NightRayShield.Size() / 2, new Vector2(XScale, 1) * Projectile.scale * Main.player[Projectile.owner].gravDir * 0.35f, flipSprite, default);
            Main.spriteBatch.Draw(UCATextureRegister.NightRayShield.Value, ShieledPos, null, new Color(255, 0, 255, 155), drawRotation + MathHelper.PiOver4 * Projectile.spriteDirection,
                UCATextureRegister.NightRayShield.Size() / 2, new Vector2(XScale, 1) * Projectile.scale * Main.player[Projectile.owner].gravDir * 0.4f, flipSprite, default);  

            LAPContent.ReSetToBeginShader_Pixel(BlendState.Additive);

            Vector2 SpreadLinePos = drawPosition + FireOffset;
            Main.spriteBatch.Draw(UCATextureRegister.SpreadLine.Value, SpreadLinePos, null, new Color(185, 0, 204, 155) * Projectile.Opacity, drawRotation + MathHelper.PiOver4 * Projectile.spriteDirection,
                UCATextureRegister.SpreadLine.Size() / 2, new Vector2(XScale * 1.2f * (1 - OpacityOffset * 0.7f), 1) * 0.35f, flipSprite, default);

            LAPContent.ReSetToBeginShader_Pixel(BlendState.AlphaBlend);
        }
    }
}
