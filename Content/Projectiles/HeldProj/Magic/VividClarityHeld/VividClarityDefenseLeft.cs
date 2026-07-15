using LAP.Assets.TextureRegister;
using LAP.Core.BaseClass.Projectiles;
using LAP.Core.Graphics.DeepGlow;
using LAP.Core.Graphics.PixelatedRender;
using LAP.Core.SystemsLoader;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Content.Items.Weapons.Magic.Ray;
using UCA.Content.Projectiles.Magic.Ray;

namespace UCA.Content.Projectiles.HeldProj.Magic.VividClarityHeld
{
    public class VividClarityDefenseLeft : BaseHeldProj
    {
        public override string Texture => GetInstance<VividClarityAlt>().Texture;
        public override LocalizedText DisplayName => LAPUtilities.GetItemName<VividClarityAlt>();
        public override Vector2 PositionOffset => RotVector * Owner.direction;
        public Vector2 RotVector => new Vector2(-10, -10).BetterRotatedBy(Owner.GetPlayerToMouseVector2().ToRotation());
        public int RecoilTimer;
        public const int MaxRecoilTimer = 30;
        public int MaxTimer = 30;
        public float RecoilRotOffset;

        public float ShieldScale;
        public override void SetDefaults()
        {
            Projectile.width = 142;
            Projectile.height = 142;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.netImportant = true;
        }
        public override void Initialize()
        {
            Projectile.Opacity = 0f;
            RotAmount = 0.2f;
        }
        public override void ExAI()
        {
            if (Owner.LAP().MouseLeft && UseDelay <= 0 && Owner.CheckMana(Owner.ActiveItem(), (int)(Owner.HeldItem.mana * Owner.manaCost * 2), true, false))
            {
                Fire();
                RecoilTimer = 0;
                UseDelay = 60;
            }
            UpdateOpacity();
            HandleRecoil();
            if (Owner.LAP().MouseRight && !Owner.HasProj<VividWeakParryProj>())
            {
                if (Owner.CheckFocus(Owner.HeldItem.LAP().WeaponSkillRealFocusCost, true))
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center,Projectile.velocity, ProjectileType<VividWeakParryProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
        }
        public override void ExPostAI()
        {
            if (!Owner.LAP().MouseLeft && Owner.LAP().MouseRight && UseDelay == 0)
            {
                Projectile.Kill();
            }
        }
        public void Fire()
        {
            Vector2 Fireoffset = Vector2.UnitX.RotatedBy(Projectile.rotation) * 96f;
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + Fireoffset, Projectile.velocity.SafeNormalize(Vector2.One) * 12f, ProjectileType<VividPowerfulBeam>(), 
                Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
        public void HandleRecoil()
        {
            if (RecoilTimer < MaxTimer)
                RecoilTimer++;
            float progress = RecoilTimer / (float)MaxTimer;
            float End = BezierEaseHelper.BezierSmooth(Vector2.UnitY, Vector2.One, progress);
            DrawRotOffset = MathHelper.Lerp(0.3f, 0f, End) * -Owner.direction;
            DrawPosOffset = Vector2.Lerp(new Vector2(-24, 0), Vector2.Zero, End).RotatedBy(Projectile.rotation);
        }
        public void UpdateOpacity()
        {
            if (Owner.LAP().MouseLeft || UseDelay > 10)
                Projectile.Opacity = MathHelper.Lerp(Projectile.Opacity, 1f, 0.12f);
            else
                Projectile.Opacity = MathHelper.Lerp(Projectile.Opacity, 0f, 0.12f);
        }
        public override void SetPlayerVisuals()
        {
            Projectile.SetHeldProj(Owner, false);
            Owner.SetArmRot(LAPUtilities.GetVector2(Owner.Center, Owner.LocalMouseWorld()).ToRotation());
        }
        public override void AimToMouse()
        {
            Projectile.spriteDirection = Owner.direction;
            Projectile.Center = Vector2.Lerp(Projectile.Center, PositionOffset + Owner.Center, 0.4f) + Vector2.UnitY * MathF.Sin(Main.GlobalTimeWrappedHourly);
            Projectile.timeLeft = 2;
            Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            Vector2 target = LAPUtilities.GetVector2(Projectile.Center, Owner.LocalMouseWorld());
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, target, RotAmount);
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
        public override void OnKill(int timeLeft)
        {
            Owner.SetItemAnimation(0);
            Owner.SetItemTime(0);
            Owner.SetDummyItemTime(0);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            PixelatedRenderManger.BeginDrawProj = true;

            LAPUtilities.ReSetToBeginShader(BlendState.Additive, SamplerState.PointClamp);

            Vector4 cut = new Vector4(0.5f, 1f, 0f, 1f);
            LAPUtilities.ApplyUVRot(cut, 1f);
            DrawControlCircle(0.2f, 20);
            LAPUtilities.ApplyUVRot(cut, -1f);
            DrawControlCircle(0.15f, -15);

            LAPUtilities.ReSetToBeginShader(BlendState.AlphaBlend);

            Projectile.GetProjDrawInfo_Staff(out Texture2D texture, out Vector2 drawPosition, out float drawRotation, out Vector2 rotationPoint, out SpriteEffects flipSprite);
            LAPUtilities.SetTexture(LAPTextureRegister.Noise.Value, SamplerState.PointWrap, 1);
            LAPUtilities.FastApplyEdgeMeltsShader(1 - Projectile.Opacity, texture.Size(), Color.GhostWhite, 0.01f, 0);
            Main.spriteBatch.Draw(texture, drawPosition + DrawPosOffset, null, lightColor, drawRotation + DrawRotOffset, rotationPoint, Projectile.scale, flipSprite, 0);

            LAPUtilities.ReSetToBeginShader(BlendState.Additive, SamplerState.PointClamp);

            Vector4 Frontcut = new Vector4(0f, 0.5f, 0f, 1f);
            LAPUtilities.ApplyUVRot(Frontcut, 1f);
            DrawControlCircle(0.2f, 20);
            LAPUtilities.ApplyUVRot(Frontcut, -1f);
            DrawControlCircle(0.15f, -15);

            LAPUtilities.ReSetToEndShader();


            DeepGlow.SubmitCustomGlow(() =>
            {
                LAPUtilities.ReSetToBeginShader(BlendState.AlphaBlend);

                LAPUtilities.SetTexture(LAPTextureRegister.Noise.Value, SamplerState.PointWrap, 1);
                LAPUtilities.FastApplyEdgeMeltsShader(1 - Projectile.Opacity, texture.Size(), Color.GhostWhite, 0.01f, 0);
                Main.spriteBatch.Draw(texture, drawPosition + DrawPosOffset, null, Color.Transparent, drawRotation + DrawRotOffset, rotationPoint, Projectile.scale, flipSprite, 0);

                LAPUtilities.ReSetToBeginShader(BlendState.Additive, SamplerState.PointClamp);

                Vector4 Frontcut = new Vector4(0f, 0.5f, 0f, 1f);
                LAPUtilities.ApplyUVRot(Frontcut, 1f);
                DrawControlCircle(0.2f, 20);
                LAPUtilities.ApplyUVRot(Frontcut, -1f);
                DrawControlCircle(0.15f, -15);

                LAPUtilities.ReSetToEndShader();
            });
            return false;
        }
        public void DrawControlCircle(float scale = 1f, float YOffset = 24f)
        {
            Vector2 offset = new Vector2(YOffset, 0).RotatedBy(Projectile.rotation) + new Vector2(0, 12).RotatedBy(Projectile.rotation - MathHelper.PiOver2);
            Texture2D circle = UCATextureRegister.TechCircle.Value;
            LAPUtilities.Draw(circle, Projectile.Center - Main.screenPosition + offset + DrawPosOffset, null, Color.White * Projectile.Opacity, Projectile.rotation + DrawRotOffset, circle.Size() / 2, new Vector2(0.25f, 1f) * scale, 0);
        }
    }
}
