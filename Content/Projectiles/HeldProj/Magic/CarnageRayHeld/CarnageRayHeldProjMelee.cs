using LAP.Core.AnimationHandle;
using LAP.Core.BaseClass.Projectiles;
using LAP.Core.Enums;
using LAP.Core.Graphics.PixelatedRender;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Assets.Sounds;
using UCA.Content.Items.Weapons.Magic.Ray;
using UCA.Content.MetaBalls;
using UCA.Content.Particiles;
using UCA.Content.Projectiles.Magic.Ray;

namespace UCA.Content.Projectiles.HeldProj.Magic.CarnageRayHeld
{
    public class CarnageRayHeldProjMelee : BaseHeldProj, IPixelatedRenderer
    {
        public DrawLayer LayerToRenderTo => DrawLayer.BeforeDusts;
        public override LocalizedText DisplayName => LAPUtilities.GetItemName<CarnageRay>();
        public override string Texture => GetInstance<CarnageRayHeldProj>().Texture;
        public Vector2 RotVector => new Vector2((12 + XOffset) * Owner.direction, YOffset).BetterRotatedBy(Owner.GetPlayerToMouseVector2().ToRotation()) * Owner.direction;
        public override Vector2 PositionOffset => RotVector;
        public AniHelper AniHelper = new AniHelper(3);
        public float XOffset = -8;
        public int YOffset = 7;
        public int StabsFrame;
        public override void ExSD()
        {
            Projectile.width = 66;
            Projectile.height = 66;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.netImportant = true;
            RotAmount = 0.25f;
        }
        public override bool? CanHitNPC(NPC target)
        {
            return null;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (targetHitbox.Intersects(projHitbox))
                return true;
            float _ = float.NaN;
            bool c = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * 55, 33f, ref _);
            return c;
        }
        public override void Initialize()
        {
            AniHelper.MaxAniProgress[AniState.Begin] = 10;
            RotAmount = 0.25f;
        }
        public override void ExAI()
        {
            Projectile.Opacity = MathHelper.Lerp(Projectile.Opacity, 1f, 0.14f);
            if (Owner.LAP().MouseRight)
            {
                if (UseDelay <= 0)
                {
                    Projectile.LAP().OnceHitEffect = true;
                    StabsFrame = 0;
                    UseDelay = 45;
                    AniHelper.ResetAni(AniState.Begin);
                }
            }
            if (!AniHelper.HasFinish[AniState.Begin])
            {
                AniHelper.UpDateAni(AniState.Begin);
                float progress = AniHelper.GetProgress(AniState.Begin);
                XOffset = MathHelper.Lerp(-8, 35, EasingHelper.EaseInBack(progress));

                if (AniHelper.AniProgress[AniState.Begin] == 9)
                {
                    SoundEngine.PlaySound(SoundsMenu.CarnageRightUse, Projectile.Center);
                    SpawnDust();
                    if (Projectile.IsLocalPlayer() && !Owner.LAP().MouseLeft)
                    {
                        for (int i = 0; i < 5; i++)
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity.RotatedByRandom(MathHelper.PiOver4 * 0.7f) * 9 * Main.rand.NextFloat(0.3f, 1.1f), ModContent.ProjectileType<CarnageBall>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 1);
                    }
                }
            }
            else
            {
                XOffset = MathHelper.Lerp(XOffset, -8, 0.12f);
            }
            if (UseDelay == 35)
            {
                Projectile.ResetLocalNPCHitImmunity();
            }

            float baseRotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            float directionVerticality = MathF.Abs(Projectile.velocity.X);
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, baseRotation + Owner.direction * directionVerticality * 1.5f);
            Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, baseRotation + Owner.direction * directionVerticality * 1.2f);
        }
        public override void PostAI()
        {
            base.PostAI();
            if (StabsFrame < 20)
                StabsFrame++;
        }
        public void SpawnDust()
        {
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
                new BloodDrop(Projectile.Center,  shootVel,  color, Main.rand.Next(60, 90), 0, 1, 0.1f).Spawn();
            }
            for (int i = 0; i < 10; i++)
                CarnageMetaBall.SpawnParticle(Projectile.Center + Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.2f, 0.55f) * 24f,
                    Projectile.rotation.ToRotationVector2(), Main.rand.NextFloat(0.4f, 1f), Projectile.rotation);
            SoundEngine.PlaySound(SoundsMenu.CarnageBallSpawn, Projectile.Center);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage *= 3;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.LAP().OnceHitEffect)
            {
                if (Owner.CheckMana(Owner.ActiveItem(), (int)(Owner.HeldItem.mana * Owner.manaCost), true, false))
                {
                    for (int i = 0; i < Main.rand.Next(5, 9); i++)
                    {
                        Vector2 SpawnPos = Owner.Center + new Vector2(Main.rand.Next(300, 500), 0).RotatedByRandom(MathHelper.TwoPi);
                        int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), SpawnPos, Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * 3f, ModContent.ProjectileType<CarnageBall>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 1);
                        Main.projectile[p].tileCollide = false;
                    }
                }
            }
        }
        public void RenderPixelated(SpriteBatch spriteBatch)
        {
            if (StabsFrame > 19)
                return;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null);

            Main.graphics.GraphicsDevice.Textures[0] = UCATextureRegister.CarnageStabs.Value;
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;

            Main.graphics.GraphicsDevice.Textures[1] = UCATextureRegister.Noise.Value;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;

            Rectangle frame = UCATextureRegister.CarnageStabs.Frame(19, 1, StabsFrame, 0);

            LAPUtilities.FastApplyEdgeMeltsShader(1 - Projectile.Opacity, frame.Size(), Color.Red, 0.01f, 0);

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

            Main.spriteBatch.Draw(texture, drawPosition, frame, Color.White, drawRotation, origin, Projectile.scale * Main.player[Projectile.owner].gravDir * 0.15f, flipSprite, default);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            PixelatedRenderManger.BeginDrawProj = true;
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D Weapontexture = TextureAssets.Projectile[Type].Value;

            Main.graphics.GraphicsDevice.Textures[0] = Weapontexture;
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

            Main.graphics.GraphicsDevice.Textures[1] = UCATextureRegister.Noise.Value;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;

            LAPUtilities.FastApplyEdgeMeltsShader(1 - Projectile.Opacity, Weapontexture.Size(), Color.Red, 0.01f, 0);

            Projectile.GetProjDrawInfo_Staff(out Texture2D texture,out Vector2 drawPosition, out float drawRotation, out Vector2 rotationPoint, out SpriteEffects flipSprite);

            Main.spriteBatch.Draw(texture, drawPosition, null, lightColor, drawRotation, rotationPoint, Projectile.scale * Main.player[Projectile.owner].gravDir, flipSprite, default);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }
}
