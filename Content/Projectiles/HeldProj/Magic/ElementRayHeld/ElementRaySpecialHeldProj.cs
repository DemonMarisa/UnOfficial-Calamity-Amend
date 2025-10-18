using CalamityMod;
using CalamityMod.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Core.Utils;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Content.Paths;
using UCA.Core.AnimationHandle;
using UCA.Core.Utilities;

namespace UCA.Content.Projectiles.HeldProj.Magic.ElementRayHeld
{
    public partial class ElementRaySpecialHeldProj : ModProjectile, ILocalizedModType
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<ElementalRay>();
        public override string Texture => $"{ProjPath.HeldProjPath}" + "Magic/ElementRayHeld/ElementRayHeldProj";
        public Player Owner => Main.player[Projectile.owner];
        public int WeaponStates => Owner.UCA().ElementalRayStates;
        public AnimationHelper animationHelper = new AnimationHelper(10);
        public Vector2 MainFragmentOffset = new Vector2(0, 0);
        public Vector2 AuxFragmentOffset = new Vector2(0, 0);
        public Vector2 FilpAuxFragmentOffset = new Vector2(0, 0);
        public float ToMouseVector => Owner.GetPlayerToMouseVector2().ToRotation();
        public Vector2 RelativeOwnerPos;
        public float RelativeOwnerPosRot;
        public override void SetDefaults()
        {
            Projectile.width = 74;
            Projectile.height = 74;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }
        public override void AI()
        {
            Initialize();
            UpdateOwner();
            // 基础信息
            Projectile.velocity = Projectile.rotation.ToRotationVector2();
            Projectile.timeLeft = 2;
            Projectile.Center = Owner.Center + RelativeOwnerPos.RotatedBy(RelativeOwnerPosRot);

            UpdateDrawOffset();

            // 设置玩家手持效果
            float baseRotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            float directionVerticality = MathF.Abs(Projectile.velocity.X);
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, baseRotation + Owner.direction * directionVerticality * 1.5f);
            Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, baseRotation + Owner.direction * directionVerticality * 1.2f);

            UpdateAni();
        }
        public override void PostAI()
        {
        }
        public void Initialize()
        {
            if (Projectile.UCA().FirstFrame)
            {
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
            Owner.ChangeDir(Owner.LocalMouseWorld().X > Owner.Center.X ? 1 : -1);
            Owner.heldProj = Projectile.whoAmI;
        }
        public void UpdateDrawOffset()
        {
            float MainHeightOffset = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 3 + MathHelper.Pi) * 2.5f;
            Vector2 targetPos = new Vector2(58 + MainHeightOffset, 0).RotatedBy(Projectile.rotation);
            MainFragmentOffset = Vector2.Lerp(MainFragmentOffset, targetPos, 0.3f);
            float HeightOffset = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 3) * 1.5f;
            Vector2 FragtargetPos = new Vector2(44 + HeightOffset, 10).RotatedBy(Projectile.rotation);
            AuxFragmentOffset = Vector2.Lerp(AuxFragmentOffset, FragtargetPos, 0.2f);
            Vector2 SecondFragtargetPos = new Vector2(44 + HeightOffset, -10).RotatedBy(Projectile.rotation);
            FilpAuxFragmentOffset = Vector2.Lerp(FilpAuxFragmentOffset, SecondFragtargetPos, 0.2f);
        }
        public void UpdateAni()
        {
            if (WeaponStates == ElementalRayState.Solar)
            {
                UpdateSolarBlade();
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            DrawBaseElementalRay();
            FilpDrawAuxFragments(1);
            DrawMainFragments();
            DrawAuxFragments(1);

            float DrawRot = Projectile.rotation + MathHelper.PiOver2;
            DrawSolarBlade(Projectile.Center, new Vector2(0, 128), DrawRot, new Vector2(1f, 1f));
            return false;
        }
    }
}
