using LAP.Core.AnimationHandle;
using LAP.Core.SpecificEffectManagers;
using LAP.Core.SystemsLoader;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.Localization;
using Terraria.ModLoader;
using UCA.Assets.Sounds;
using UCA.Content.Items.Weapons.Magic.Ray;
using UCA.Content.Paths;
using UCA.Content.Projectiles.Magic.Ray;
using UCA.Content.UCACooldowns;
using UCA.Core.Utilities;

namespace UCA.Content.Projectiles.HeldProj.Magic.ElementRayHeld
{
    public partial class ElementRaySpecialHeldProj : ModProjectile, ILocalizedModType
    {
        public override LocalizedText DisplayName => LAPUtilities.GetItemName<ElementRayAlt>();
        public override string Texture => $"{ProjPath.HeldProjPath}" + "Magic/ElementRayHeld/ElementRayHeldProj";
        public Player Owner => Main.player[Projectile.owner];
        public ref float WeaponStates => ref Projectile.ai[0];
        public bool IsMAGBOLIABlue => Projectile.ai[1] != 0;
        public AnimationHelper animationHelper = new AnimationHelper(10);
        public float ToMouseVector;
        public Vector2 RelativeOwnerPos;
        public float RelativeOwnerPosRot;
        public bool FollowOwner = true;
        public bool CanChangeDir = true;
        public int Time = 0;
        public int HitCooldown = 0;
        public bool candamage = false;
        public bool CanDraw = true;
        public float BeginRot = 0;
        public bool UseSlowRot = false;
        public int HitCount = 0;
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
            if (UseSlowRot)
                ToMouseVector = Utils.AngleLerp(ToMouseVector, Owner.GetPlayerToMouseVector2().ToRotation(), 0.08f);
            else
                ToMouseVector = Utils.AngleLerp(ToMouseVector, Owner.GetPlayerToMouseVector2().ToRotation(), 0.2f);
            if (HitCooldown > 0)
                HitCooldown--;
            if (!Owner.active || Owner.dead)
                Projectile.Kill();
            Time ++;
            // 基础信息
            Projectile.velocity = Projectile.rotation.ToRotationVector2();
            Projectile.timeLeft = 2;
            if (FollowOwner)
            {
                UpdateOwner();
                Projectile.Center = Owner.RotatedRelativePoint(Owner.Center) + RelativeOwnerPos.RotatedBy(RelativeOwnerPosRot);
                // 设置玩家手持效果
                float baseRotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
                float directionVerticality = MathF.Abs(Projectile.velocity.X);
                if (WeaponStates == ElementalRayState.StarDust)
                    baseRotation += MathHelper.PiOver2 * Owner.direction;
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
                UpdateVortexMissle();
                UpdateSolarFragmentOffset();
            }
            else if (WeaponStates == ElementalRayState.Nebula)
            {
                UpdateNebulaDust();
                UpdateSolarFragmentOffset();
            }
            else if (WeaponStates == ElementalRayState.StarDust)
            {
                UpdateStarDustStream();
                UpdateSolarFragmentOffset();
            }
            else
            {
                UpdateMisc();
                UpdateSolarFragmentOffset();
            }
        }
        public void Initialize()
        {
            if (Projectile.LAP().FirstFrame)
            {
                ToMouseVector = Owner.GetPlayerToMouseVector2().ToRotation();

                if (Projectile.owner == Main.myPlayer)
                    WeaponStates = Owner.UCA().ElementalRayStates;

                if (WeaponStates == ElementalRayState.Solar)
                {
                    InitializeSolarBlade();
                }
                else if (WeaponStates == ElementalRayState.Nebula)
                {
                    InitializeNebulaDust();
                }
                else if (WeaponStates == ElementalRayState.Vortex)
                {
                    InitializeVortexMissle();
                }
                else if (WeaponStates == ElementalRayState.StarDust)
                {
                    InitializeStarDustStream();
                }
                else
                {
                    InitializeMisc();
                }
            }
        }
        public void UpdateOwner()
        {
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;

            if (CanChangeDir)
                Owner.ChangeDir(Owner.LocalMouseWorld().X > Owner.Center.X ? 1 : -1);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (HitCount > 10)
                HitCount = 10;
            modifiers.SourceDamage *= MathHelper.Lerp(100f, 10f, HitCount / 10f);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            float DamageMult = MathHelper.Lerp(50f, 5f, HitCount / 10f);
            if (HitCooldown == 0)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<SolarBlast>(), (int)(Projectile.damage * DamageMult), Projectile.knockBack, Projectile.owner, 15, 0.3f, 1); ;
                HitCooldown = 3;
            }

            if (Projectile.LAP().OnceHitEffect)
                ScreenShakeSystem.AddScreenShakes(Projectile.Center, -250 * -Owner.direction, 40, Projectile.rotation + MathHelper.PiOver2, 0.5f, true, 1000);
            Owner.AddCD(LAPContent.CDType<SolorShield>(), 1200);
            SoundEngine.PlaySound(SoundsMenu.RiseBlast, Projectile.Center);
            SoundEngine.PlaySound(SoundsMenu.CarnageSkillMeleeHit, Projectile.Center);
            HitCount++;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (WeaponStates == ElementalRayState.StarDust && CanDraw)
            {
                DrawChargeBall();
                DrawBaseElementalRay();
                FilpDrawAuxFragments();
                DrawMainFragments();
                DrawAuxFragments();
                return false;
            }
            if (WeaponStates == ElementalRayState.Solar && CanDraw)
            {
                DrawSolar();
                DrawBaseElementalRay();
                FilpDrawAuxFragments();
                DrawMainFragments();
                DrawAuxFragments();
                return false;
            }
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
    }
}
