using CalamityMod;
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
using UCA.Content.Items.Magic.Ray;
using UCA.Content.MetaBalls;
using UCA.Content.Particiles;
using UCA.Content.Projectiles.Magic.Ray;
using UCA.Core.BaseClass;
using UCA.Core.Utilities;

namespace UCA.Content.Projectiles.HeldProj.Magic
{
    public class CarnageRayHeldProj : BaseHeldProj
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<CarnageRay>();
        public Vector2 RotVector => new Vector2(16 * Owner.direction, 7).BetterRotatedBy(Owner.GetPlayerToMouseVector2().ToRotation(), default, 0.5f, 1f);

        public override Vector2 RotPoint => TextureAssets.Projectile[Type].Size() / 2;

        public override Vector2 Posffset => new Vector2(RotVector.X, RotVector.Y) * Owner.direction;

        public override float RotAmount => 0.25f;

        public override float RotOffset => MathHelper.PiOver4;

        // 1f是完全透明
        public float ShaderOpacity = 1f;

        public int MaxAni = 10;

        public int AniProgress = 0;

        public override void SetDefaults()
        {
            Projectile.width = 66;
            Projectile.height = 66;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override bool StillInUse()
        {
            return AniProgress == MaxAni && Main.mouseLeft;
        }

        public override void HoldoutAI()
        {
            if (Main.mouseRight)
                return;

            if (AniProgress < MaxAni)
                return;

            if (UseDelay <= 0 && Owner.CheckMana(Owner.ActiveItem(), (int)(Owner.HeldItem.mana * Owner.manaCost), true, false))
            {
                SoundEngine.PlaySound(SoundsMenu.CarnageLeftShoot, Projectile.Center);
                // 常规开火
                // 这里用发射的弹幕AI[0]是否为1来确定是否为主射线
                // ai[0]为1时是主射线
                Vector2 firePos = Projectile.Center + new Vector2(40, 0).RotatedBy(Projectile.rotation);

                SoundEngine.PlaySound(SoundsMenu.NightRayHeavyAttack, Projectile.Center);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), firePos, Projectile.rotation.ToRotationVector2() * 3, ModContent.ProjectileType<CarnageEnergy>(), Projectile.damage, Projectile.knockBack, Owner.whoAmI, 1);
                Projectile.velocity -= Projectile.velocity.RotatedBy(Projectile.spriteDirection * MathHelper.PiOver2) * 0.06f;

                UseDelay = Owner.HeldItem.useTime;

                for (int i = 0; i < 75; i++)
                {
                    new LilyLiquid(Projectile.Center, Projectile.velocity.RotatedByRandom(MathHelper.PiOver4 * 0.6f) * Main.rand.NextFloat(0f, 1.2f) * 24f, Color.Red, 64, 0, 1, 1.5f).Spawn();
                }
                for (int i = 0; i < 35; i++)
                {
                    new LilyLiquid(Projectile.Center, Projectile.velocity.RotatedByRandom(MathHelper.PiOver4 * 0.6f) * Main.rand.NextFloat(0f, 1.2f) * 24f, Color.Black, 64, 0, 1, 1.5f).Spawn();
                }
            }
        }

        public override void PostAI()
        {
            float baseRotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            float directionVerticality = MathF.Abs(Projectile.velocity.X);
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, baseRotation + Owner.direction * directionVerticality * 1.5f);
            Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, baseRotation + Owner.direction * directionVerticality * 1.2f);

            if ((Main.mouseLeft || Active) && !UCAUtilities.JustPressRightClick())
            {
                if (AniProgress < MaxAni)
                    AniProgress++;
            }
            else
            {
                if (AniProgress > 0)
                    AniProgress--;
            }

            ShaderOpacity = MathHelper.Lerp(0.8f, 0f, EasingHelper.EaseInCubic(AniProgress / (float)MaxAni));
        }

        public override bool CanDel()
        {
            return AniProgress == 0 && !Main.mouseLeft;
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
    }
}
