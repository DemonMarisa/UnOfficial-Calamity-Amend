using CalamityMod;
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
using UCA.Assets.Effects;
using UCA.Content.MetaBalls;
using UCA.Content.Particiles;
using UCA.Content.Paths;
using UCA.Content.UCACooldowns;
using UCA.Core.GlobalInstance.Players;
using UCA.Core.Utilities;

namespace UCA.Content.Projectiles.HeldProj.Magic
{
    public class NightRaySkillProj : ModProjectile, ILocalizedModType
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<NightsRay>();

        public Player Owner => Main.player[Projectile.owner];
        public override string Texture => $"{ProjPath.HeldProjPath}" + "Magic/NightRayHeldProj";

        public Vector2 BeginPos => Owner.Center + new Vector2(45 * Owner.direction, -10);
        public Vector2 MedPos => Owner.Center + new Vector2(45 * Owner.direction, -35);
        public Vector2 EndPos => Owner.Center + new Vector2(45 * Owner.direction, -50);

        // 控制动画进度
        public int AniProgress = 0;

        public float OwnervelocityMult = 1;

        public float Opacity = 1;
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
        public override void OnSpawn(IEntitySource source)
        {
        }
        public override void AI()
        {
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;

            Owner.ChangeDir(Main.MouseWorld.X > Owner.Center.X ? 1 : -1);
            float TargetRot = (Owner.Center - Projectile.Center).ToRotation() + MathHelper.PiOver2;
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, TargetRot + Owner.direction * 0.1f * 1.5f);
            Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, TargetRot + Owner.direction * -0.1f * 1.2f);

            // 基础信息
            Projectile.velocity = Vector2.Zero;
            AniProgress++;
            Projectile.timeLeft = 2;
            float IntoAni = 75;
            float OutAni = 20;

            if (AniProgress == 27)
            {
                SoundEngine.PlaySound(SoundsMenu.NightRayCharge, Projectile.Center);
            }
            if (AniProgress < IntoAni)
            {
                if (AniProgress  < IntoAni * 0.5f)
                {
                    // 生成粒子
                    Vector2 SpawnPos = MedPos + Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(100, 200);
                    Vector2 SpawnPosToMouseWorld = (MedPos - SpawnPos).SafeNormalize(Vector2.UnitX);
                    float rot = SpawnPosToMouseWorld.ToRotation() + 1;
                    Color color = Color.Lerp(Color.DarkOrchid, Color.DarkViolet, Main.rand.NextFloat(0, 1f));
                    new Line(SpawnPos, Vector2.Zero, color, Main.rand.Next(60, 90), rot, 1, 0.15f, true, MedPos).Spawn();
                }
                // 设置玩家减速
                Owner.velocity.X *= MathHelper.Lerp(OwnervelocityMult, 0.1f, 0.16f);
                // 设置shader淡入
                Opacity = MathHelper.Lerp(1, 0, EasingHelper.EaseOutExpo(AniProgress / IntoAni));
                // 设置弹幕位置
                Projectile.Center = Vector2.Lerp(BeginPos, MedPos, (float)Math.Pow(EasingHelper.EaseInBack(AniProgress / IntoAni), 2D));
            }
            else
            {
                Opacity = MathHelper.Lerp(Opacity, 0.92f, 0.08f);
                Projectile.Center = Vector2.Lerp(MedPos, EndPos, EasingHelper.EaseOutExpo((AniProgress - IntoAni) / OutAni));

                if (AniProgress == 80)
                {
                    Owner.AddCooldown(NightBoost.ID, CalamityUtils.SecondsToFrames(30));

                    for (int i = 0; i < 25; i++)
                    {
                        ShadowMetaBall.SpawnParticle(Owner.Center - new Vector2(0, -Owner.height / 2) + new Vector2(Main.rand.Next(-10, 10), Main.rand.Next(-2, 2)), Vector2.UnitX  * i * 0.3f, Main.rand.NextFloat(0.1f, 0.15f));
                    }

                    for (int i = 0; i < 25; i++)
                    {
                        ShadowMetaBall.SpawnParticle(Owner.Center - new Vector2(0, -Owner.height / 2) + new Vector2(Main.rand.Next(-10, 10), Main.rand.Next(-2, 2)), Vector2.UnitX * i * 0.3f * -1, Main.rand.NextFloat(0.1f, 0.15f));
                    }

                    for (int i = 0; i < 30; i++)
                    {
                        ShadowMetaBall.SpawnParticle(Owner.Center - new Vector2(Main.rand.Next(-50, 50), -Owner.height / 2), Vector2.UnitY * i * 0.4f * -1, Main.rand.NextFloat(0.1f, 0.15f));
                    }
                }

                if (AniProgress == 76)
                {
                    for (int i = 0; i < 25; i++)
                    {
                        Color color = Color.Lerp(Color.DarkOrchid, Color.DarkViolet, Main.rand.NextFloat(0, 1f));
                        new Line(EndPos, Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(3, 7), color, Main.rand.Next(60, 90), 0, 1, 0.1f, false, EndPos).Spawn();
                    }

                    Owner.UCA().NightShieldHP = UCAPlayer.NightShieldMaxHP;
                }
            }

            if (AniProgress >= 120)
                Projectile.Kill();
        }

        public override void OnKill(int timeLeft)
        {
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            
            Main.graphics.GraphicsDevice.Textures[0] = ModContent.Request<Texture2D>(Texture).Value;
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

            Main.graphics.GraphicsDevice.Textures[1] = UCATextureRegister.Noise.Value;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;

            UCAShaderRegister.EdgeMeltsShader.Parameters["progress"].SetValue(Opacity);
            UCAShaderRegister.EdgeMeltsShader.Parameters["InPutTextureSize"].SetValue(ModContent.Request<Texture2D>(Texture).Size());
            UCAShaderRegister.EdgeMeltsShader.Parameters["EdgeColor"].SetValue(Color.DarkViolet.ToVector4());
            UCAShaderRegister.EdgeMeltsShader.Parameters["EdgeWidth"].SetValue(0.01f);
            UCAShaderRegister.EdgeMeltsShader.CurrentTechnique.Passes[0].Apply();
            
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            float drawRotation = Projectile.rotation + (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0f);
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
