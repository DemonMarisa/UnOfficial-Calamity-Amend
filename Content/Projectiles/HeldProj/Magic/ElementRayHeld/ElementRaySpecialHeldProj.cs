using CalamityMod;
using CalamityMod.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Core.Utils;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Content.Paths;
using UCA.Content.Projectiles.Magic.Ray;
using UCA.Core.AnimationHandle;
using UCA.Core.SpecificEffectManagers;
using UCA.Core.Utilities;

namespace UCA.Content.Projectiles.HeldProj.Magic.ElementRayHeld
{
    public partial class ElementRaySpecialHeldProj : ModProjectile, ILocalizedModType
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<ElementalRay>();
        public override string Texture => $"{ProjPath.HeldProjPath}" + "Magic/ElementRayHeld/ElementRayHeldProj";
        public Player Owner => Main.player[Projectile.owner];
        public ref float WeaponStates => ref Projectile.ai[0];
        public AnimationHelper animationHelper = new AnimationHelper(10);
        public float ToMouseVector;
        public Vector2 RelativeOwnerPos;
        public float RelativeOwnerPosRot;
        public bool FollowOwner = true;
        public bool CanChangeDir = true;
        public int Time = 0;
        public int HitCooldown = 0;
        public override void SetDefaults()
        {
            Projectile.width = 74;
            Projectile.height = 74;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.netImportant = true;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (!candamage)
                return false;
            if (projHitbox.Intersects(targetHitbox))
            {
                return true;
            }

            float _ = float.NaN;
            Vector2 beamBeginPos = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * 100;
            Vector2 beamEndPos = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * 860;
            bool c = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), beamBeginPos, beamEndPos, 128f, ref _);
            return c;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override void AI()
        {
            Initialize();
            ToMouseVector = Utils.AngleLerp(ToMouseVector, Owner.GetPlayerToMouseVector2().ToRotation(), 0.2f);
            if (HitCooldown > 0)
                HitCooldown--;
            Time ++;
            // 基础信息
            Projectile.velocity = Projectile.rotation.ToRotationVector2();
            Projectile.timeLeft = 2;
            if (FollowOwner)
            {
                UpdateOwner();
                Projectile.Center = Owner.Center + RelativeOwnerPos.RotatedBy(RelativeOwnerPosRot);
                // 设置玩家手持效果
                float baseRotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
                float directionVerticality = MathF.Abs(Projectile.velocity.X);
                Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, baseRotation);
                Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, baseRotation);
            }
            // 更新模式
            if (WeaponStates == ElementalRayState.Solar)
            {
                UpdateSolarBlade();
                UpdateSolarFragmentOffset();
            }
            else if (WeaponStates == ElementalRayState.Vortex)
            {
            }
            else if (WeaponStates == ElementalRayState.Nebula)
            {

            }
            else if (WeaponStates == ElementalRayState.StarDust)
            {

            }
        }
        public void Initialize()
        {
            if (Projectile.UCA().FirstFrame)
            {
                ToMouseVector = Owner.GetPlayerToMouseVector2().ToRotation();

                if (Projectile.owner == Main.myPlayer)
                    WeaponStates = Owner.UCA().ElementalRayStates;

                if (WeaponStates == ElementalRayState.Solar)
                {
                    InitializeSolarBlade();
                }
            }
        }
        public void UpdateOwner()
        {
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;

            if (CanChangeDir)
                Owner.ChangeDir(Owner.LocalMouseWorld().X > Owner.Center.X ? 1 : -1);

            Owner.heldProj = Projectile.whoAmI;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (HitCooldown == 0)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<SolarBlast>(), Projectile.damage * 10, Projectile.knockBack, Projectile.owner, 15, 0.3f, 1); ;
                HitCooldown = 3;
            }

            if (Projectile.UCA().OnceHitEffect)
                ScreenShakeSystem.AddScreenShakes(Projectile.Center, -250 * -Owner.direction, 40, Projectile.rotation + MathHelper.PiOver2, 0.5f, true, 1000);

            SoundEngine.PlaySound(SoundsMenu.CarnageSkillMeleeHit, Projectile.Center);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            DrawSolar();
            if (!CanDraw)
                return false;
            DrawBaseElementalRay();
            FilpDrawAuxFragments();
            DrawMainFragments();
            DrawAuxFragments();
            return false;
        }
        public void DrawSolar()
        {
            if (!CanDraw)
                return;
            float DrawRot = Projectile.rotation + MathHelper.PiOver2;
            DrawSolarBlade(Projectile.Center, new Vector2(0, SolarBladeXOffset), DrawRot, new Vector2(1f, 1f));
        }

        public void DrawStar()
        {

        }
    }
}
