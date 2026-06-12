using LAP.Assets.TextureRegister;
using LAP.Core.BaseClass.Projectiles;
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
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Assets.Sounds;
using UCA.Content.Projectiles.Magic.Ray;

namespace UCA.Content.Projectiles.HeldProj.Magic.CarnageRayHeld
{
    public class CarnageRayHeldProj : BaseHeldProj
    {
        public override Vector2 PositionOffset => new Vector2(16 * Owner.direction, 7).BetterRotatedBy(Owner.GetPlayerToMouseVector2().ToRotation(), default, 0.5f, 1f) * Owner.direction;
        public override void ExSD()
        {
            Projectile.width = 66;
            Projectile.height = 66;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.netImportant = true;
            Projectile.Opacity = 0f;
            RotAmount = 0.25f;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.Opacity);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.Opacity = reader.ReadSingle();
        }
        public override void ExAI()
        {
            RotAmount = 0.25f;
            if (Owner.LAP().MouseLeft && !Owner.LAP().MouseRight)
            {
                if (UseDelay <= 0 && Owner.CheckMana(Owner.ActiveItem(), (int)(Owner.HeldItem.mana * Owner.manaCost), true, false))
                {
                    SoundEngine.PlaySound(SoundsMenu.CarnageLeftShoot, Projectile.Center);
                    Vector2 firePos = Projectile.Center + new Vector2(40, 0).RotatedBy(Projectile.rotation);
                    SoundEngine.PlaySound(SoundsMenu.NightRayHeavyAttack, Projectile.Center);
                    if (Projectile.IsLocalPlayer())
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), firePos, Projectile.rotation.ToRotationVector2() * 3, ProjectileType<CarnageEnergy>(), Projectile.damage, Projectile.knockBack, Owner.whoAmI, 1);
                    Projectile.velocity -= Projectile.velocity.RotatedBy(Projectile.spriteDirection * MathHelper.PiOver2) * 0.1f;

                    UseDelay = Owner.ApplyWeaponAttackSpeed(Owner.ActiveItem(), Owner.HeldItem.useTime, Owner.HeldItem.useTime / 2);
                }
            }
            float baseRotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            float directionVerticality = MathF.Abs(Projectile.velocity.X);
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, baseRotation + Owner.direction * directionVerticality * 1.5f);
            Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, baseRotation + Owner.direction * directionVerticality * 1.2f);
            Projectile.Opacity = MathHelper.Lerp(Projectile.Opacity, 1f, 0.16f);

            if (Owner.JustPressRightClick() && UseDelay == 0)
                Projectile.Kill();
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

            Texture2D Weapontexture = TextureAssets.Projectile[Type].Value;

            Main.graphics.GraphicsDevice.Textures[0] = Weapontexture;
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

            Main.graphics.GraphicsDevice.Textures[1] = LAPTextureRegister.Noise.Value;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;

            LAPUtilities.FastApplyEdgeMeltsShader(1 - Projectile.Opacity, Weapontexture.Size(), Color.Red, 0.01f, 0);

            Projectile.GetProjDrawInfo_Staff(out Texture2D texture, out Vector2 drawPosition, out float drawRotation, out Vector2 rotationPoint, out SpriteEffects flipSprite);

            Main.spriteBatch.Draw(texture, drawPosition, null, lightColor, drawRotation, rotationPoint, Projectile.scale * Main.player[Projectile.owner].gravDir, flipSprite, default);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }
}
