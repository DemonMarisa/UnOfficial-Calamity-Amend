using CalamityMod;
using CalamityMod.Graphics.Primitives;
using CalamityMod.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Assets.Effects;
using UCA.Content.Items.Magic.Ray;
using UCA.Content.MetaBalls;
using UCA.Content.Particiles;
using UCA.Content.Paths;
using UCA.Content.Projectiles.Magic.Ray;
using UCA.Core.BaseClass;
using UCA.Core.Utilities;

namespace UCA.Content.Projectiles.HeldProj.Magic
{
    public class CarnageRayHeldProjMelee : BaseHeldProj, IPixelatedPrimitiveRenderer
    {
        public PixelationPrimitiveLayer LayerToRenderTo => PixelationPrimitiveLayer.AfterPlayers;
        public int YOffset = 7;
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<CarnageRay>();
        public override string Texture => $"{ProjPath.HeldProjPath}" + "Magic/CarnageRayHeldProj";
        public Vector2 RotVector => new Vector2((12 + XOffset) * Owner.direction, YOffset).BetterRotatedBy(Owner.GetPlayerToMouseVector2().ToRotation(), default, 0.5f, 1f);

        public override Vector2 RotPoint => TextureAssets.Projectile[Type].Size() / 2;

        public override Vector2 Posffset => new Vector2(RotVector.X, RotVector.Y) * Owner.direction;

        public override float RotAmount => 0.25f;

        public override float RotOffset => MathHelper.PiOver4;

        // 控制动画进度
        public int AniProgress = 0;

        // 这里0才是完全出现
        public float ShaderOpacity = 1f;

        public float XOffset = -8;

        // 帧图的索引
        public int StabsFrame = 0;

        public float IinToAni = 10;

        public bool CanHit = false;
        public override void SetDefaults()
        {
            Projectile.width = 66;
            Projectile.height = 66;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 45;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (target.friendly)
                return false;
            else if (CanHit)
                return true;
            else
                return false;
        }

        public override bool StillInUse()
        {
            return Active && UCAUtilities.JustPressRightClick();
        }
        public override void HoldoutAI()
        {
            if (AniProgress == IinToAni && Owner.CheckMana(Owner.ActiveItem(), (int)(Owner.HeldItem.mana * Owner.manaCost), true, false))
            {
                #region 处理常规发射
                for (int i = 0; i < 4; i++)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity.RotatedByRandom(MathHelper.PiOver4 * 0.7f) * 9 * Main.rand.NextFloat(0.3f, 1.1f), ModContent.ProjectileType<CarnageBall>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 1);

                for (int i = 0; i < 35; i++)
                {
                    new LilyLiquid(Projectile.Center, Projectile.velocity.RotatedByRandom(MathHelper.PiOver4 * 0.6f) * Main.rand.NextFloat(0f, 1.2f) * -18f, Color.Red, 64, 0, 1, 1.5f).Spawn();
                }
                for (int i = 0; i < 25; i++)
                {
                    new LilyLiquid(Projectile.Center, Projectile.velocity.RotatedByRandom(MathHelper.PiOver4 * 0.6f) * Main.rand.NextFloat(0f, 1.2f) * -18f, Color.Black, 64, 0, 1, 1.5f).Spawn();
                }
                for (int i = 0; i < 25; i++)
                {
                    Vector2 shootVel = Projectile.velocity.RotatedByRandom(MathHelper.PiOver4 * 0.7f) * Main.rand.NextFloat(0.2f, 1.2f) * -18f;
                    if (shootVel.ToRotation() > 0)
                        shootVel.Y *= 0.15f;
                    Color color = Main.rand.NextBool(3) ? Color.Black : Color.DarkRed;
                    new BloodDrop(Projectile.Center,
                        shootVel,
                        color,
                        Main.rand.Next(60, 90), 0, 1, 0.1f).Spawn();
                }

                for (int i = 0; i < 10; i++)
                    CarnageMetaBall.SpawnParticle(Projectile.Center + Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.2f, 0.55f) * 24f,
                        Projectile.rotation.ToRotationVector2(), 
                        Main.rand.NextFloat(0.4f, 1f),
                        Projectile.rotation);
                #endregion

                SoundEngine.PlaySound(SoundsMenu.CarnageBallSpawn, Projectile.Center);
                SoundEngine.PlaySound(SoundsMenu.CarnageRightUse, Projectile.Center);
                #region 处理额外弹幕

                Vector2 SpawnPos = Owner.Center + new Vector2(Main.rand.Next(100, 300), 0).RotatedByRandom(MathHelper.TwoPi);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), SpawnPos, Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * 3f, ModContent.ProjectileType<CarnageBall>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 1);
           
                #endregion
            }
        }
        #region AI
        
        public override void PostAI()
        {
            if (StabsFrame < 20)
                StabsFrame++;
            StabsAI();
        }
        
        public void StabsAI()
        {
            // 基础信息
            AniProgress++;
            Projectile.timeLeft = 2;

            // 设置玩家手持效果
            float baseRotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            float directionVerticality = MathF.Abs(Projectile.velocity.X);
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, baseRotation + Owner.direction * directionVerticality * 1.5f);
            Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, baseRotation + Owner.direction * directionVerticality * 1.2f);
            if (UseDelay <= 0)
            {
                CanHit = true;
                StabsFrame = 0;
                UseDelay = 45;
                AniProgress = 0;
            }

            if (AniProgress < IinToAni)
            {
                XOffset = MathHelper.Lerp(-8, 50, EasingHelper.EaseInBack(AniProgress / IinToAni));
            }
            else
            {
                XOffset = MathHelper.Lerp(XOffset, -8, 0.12f);
            }

            if (Active)
                ShaderOpacity = MathHelper.Lerp(ShaderOpacity, 0, 0.12f);
            else
                ShaderOpacity = MathHelper.Lerp(ShaderOpacity, 1, 0.08f);
        }

        public override bool CanDel()
        {
            return AniProgress == 0 && !UCAUtilities.PressLeftAndRightClick();
        }

        public override void InDel()
        {
            Projectile.Kill();
        }
        #endregion
        void IPixelatedPrimitiveRenderer.RenderPixelatedPrimitives(SpriteBatch spriteBatch, PixelationPrimitiveLayer layer)
        {
            if (StabsFrame > 19)
                return;
            
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null);

            Main.graphics.GraphicsDevice.Textures[0] = UCATextureRegister.CarnageStabs.Value;
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

            Main.graphics.GraphicsDevice.Textures[1] = UCATextureRegister.Noise.Value;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;

            Rectangle frame = UCATextureRegister.CarnageStabs.Frame(19, 1, StabsFrame, 0);

            UCAShaderRegister.EdgeMeltsShader.Parameters["progress"].SetValue(ShaderOpacity);
            UCAShaderRegister.EdgeMeltsShader.Parameters["InPutTextureSize"].SetValue(frame.Size());
            UCAShaderRegister.EdgeMeltsShader.Parameters["EdgeColor"].SetValue(Color.Red.ToVector4());
            UCAShaderRegister.EdgeMeltsShader.Parameters["EdgeWidth"].SetValue(0.01f);
            UCAShaderRegister.EdgeMeltsShader.CurrentTechnique.Passes[0].Apply();

            DrawStabs();

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null);
        
        }
        public void DrawStabs()
        {
            Texture2D texture = UCATextureRegister.CarnageStabs.Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            float drawRotation = Projectile.rotation + (Owner.direction == -1 ? MathHelper.Pi : 0f) * Owner.direction + MathHelper.PiOver2 * Owner.direction;

            Rectangle frame = texture.Frame(19, 1, StabsFrame, 0);
            Vector2 origin = frame.Size() * 0.5f;

            SpriteEffects flipSprite = Owner.direction * Main.player[Projectile.owner].gravDir == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.spriteBatch.Draw(texture, drawPosition / 2, frame, Color.White, drawRotation, origin, Projectile.scale * Main.player[Projectile.owner].gravDir * 0.5f * 0.15f, flipSprite, default);

        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.graphics.GraphicsDevice.Textures[0] = ModContent.Request<Texture2D>(Texture).Value;
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

            Main.graphics.GraphicsDevice.Textures[1] = UCATextureRegister.Noise.Value;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;

            UCAShaderRegister.EdgeMeltsShader.Parameters["progress"].SetValue(ShaderOpacity);
            UCAShaderRegister.EdgeMeltsShader.Parameters["InPutTextureSize"].SetValue(ModContent.Request<Texture2D>(Texture).Size());
            UCAShaderRegister.EdgeMeltsShader.Parameters["EdgeColor"].SetValue(Color.Red.ToVector4());
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
        #region 对原的覆写
        public override void UpdateAim(Vector2 source)
        {
            Vector2 aim = Vector2.Normalize(Main.MouseWorld - (Owner.Center));
            if (aim.HasNaNs())
            {
                aim = -Vector2.UnitY;
            }

            aim = Vector2.Normalize(Vector2.Lerp(Vector2.Normalize(Projectile.velocity), aim, RotAmount));

            if (aim != Projectile.velocity)
            {
                Projectile.netUpdate = true;
            }

            Projectile.velocity = aim;
        }
        #endregion

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage *= 3;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            CanHit = false;
            if (Owner.CheckMana(Owner.ActiveItem(), (int)(Owner.HeldItem.mana * Owner.manaCost), true, false))
            {
                for (int i = 0; i < Main.rand.Next(5, 9); i++)
                {
                    Vector2 SpawnPos = Owner.Center + new Vector2(Main.rand.Next(300, 500), 0).RotatedByRandom(MathHelper.TwoPi);

                    for (int j = 0; j < 10; j++)
                    {
                        new LilyLiquid(SpawnPos, Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0f, 1.2f) * -6f, Color.Red, 64, 0, 1, 1.5f).Spawn();
                    }
                    for (int x = 0; x < 5; x++)
                    {
                        new LilyLiquid(SpawnPos, Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0f, 1.2f) * -6f, Color.Black, 64, 0, 1, 1.5f).Spawn();
                    }

                    int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), SpawnPos, Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * 3f, ModContent.ProjectileType<CarnageBall>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 1);
                    Main.projectile[p].tileCollide = false;
                }
            }
            
            for (int i = 0; i < 15; i++)
            {
                Vector2 spawnVec = Projectile.velocity.RotateRandom(0.3f) * Main.rand.NextFloat(0.1f, 1.6f) * 24f;
                CarnageMetaBall.SpawnParticle(Projectile.Center, spawnVec, Main.rand.NextFloat(0.4f, 0.6f), 0, true);
            }

        }
    }
}
