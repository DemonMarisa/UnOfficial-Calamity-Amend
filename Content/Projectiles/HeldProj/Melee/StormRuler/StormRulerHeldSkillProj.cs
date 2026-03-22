using LAP.Assets.Sounds;
using LAP.Content.Particles;
using LAP.Core.AnimationHandle;
using LAP.Core.Enums;
using LAP.Core.Keybind;
using LAP.Core.SystemsLoader;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Localization;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Content.Items.Weapons.Melee.GreatSword;
using UCA.Content.Particiles;
using UCA.Core.BaseClass;
using UCA.Core.Utilities;

namespace UCA.Content.Projectiles.HeldProj.Melee.StormRuler
{
    public class StormRulerHeldSkillProj : BaseMeleeProj
    {
        public override string Texture => UCATextureRegister.StormRulerAlt.Path;
        public override LocalizedText DisplayName => LAPUtilities.GetItemName<StormRulerAlt>();
        public Player Owner => Projectile.Owner();
        public Vector2 IdleOffset => new Vector2(12, 4 * Owner.direction);
        public int UseTime => Owner.ApplyWeaponAttackSpeed(Owner.HeldItem, 50, 35);
        public AniHelper AniHelper = new AniHelper(3);
        public Vector2 HeldPos;
        public float TargetRot;
        public float ProjRotOffset;
        public int ChargeTimer;
        public bool BeginAttack;
        public bool BeginRightAttack;
        public override void SetStaticDefaults()
        {
            Projectile.AddToSkillProj();
            Projectile.AddHeldProj();
        }
        public override void SetDefaults()
        {
            Projectile.width = 5;
            Projectile.height = 5;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 450;
            Projectile.noEnchantmentVisuals = true;
            Projectile.netImportant = true;
        }
        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override void AI()
        {
            if (Projectile.LAP().FirstFrame)
                Init();
            Owner.SetUseFocus(2);
            UpdateGeneral();
            UpdateAnimation();
            SpawnParticle();
            UpdateCharge();
            SetArmRot();
        }
        public void Init()
        {
            TargetRot = LAPUtilities.GetVector2(Projectile.Center, Owner.LocalMouseWorld()).ToRotation();
            HeldPos = IdleOffset;
            AniHelper.MaxAniProgress[AniState.Begin] = (int)(UseTime * 0.5f);
            AniHelper.MaxAniProgress[AniState.Middle] = (int)(UseTime * 0.5f);
            if (Owner.UCA().KingOfStorm)
                ChargeTimer = 182;
        }
        public void UpdateGeneral()
        {
            Projectile.SetHeldProj(Owner);
            Projectile.Center = Owner.GetArmRoot() + HeldPos;
            Projectile.rotation = TargetRot;
            Projectile.timeLeft = 2;
            if (Projectile.IsLocalPlayer())
            {
                if (!LAPKeybind.WeaponSkillHotKey.Current && !BeginAttack && !BeginRightAttack)
                    Projectile.Kill();
            }
            float target;
            if (BeginAttack || BeginRightAttack)
            {
                Vector2 vec = LAPUtilities.GetVector2(Projectile.Center, Owner.LocalMouseWorld());
                target = TargetRot.AngleLerp(vec.ToRotation(), 0.2f);
            }
            else
            {
                Vector2 vec = LAPUtilities.GetVector2(Projectile.Center, Owner.LocalMouseWorld());
                target = TargetRot.AngleLerp(Utils.ToRotation(new Vector2(MathF.Cos(vec.X), MathF.Sin(vec.Y * 0.4f * Owner.direction)) * Owner.direction), 0.2f);
            }
            if (Projectile.soundDelay == 0)
            {
                SoundEngine.PlaySound(LAPSoundsMenu.StormRulerCharge with { Volume = Main.rand.NextFloat(0.6f, 1f), Pitch = Main.rand.NextFloat(-0.2f, 0.2f), MaxInstances = -1 }, Projectile.Center);
                Projectile.soundDelay = 20;
            }
            TargetRot = target;
        }
        #region 处理动画
        public void UpdateAnimation()
        {
            if (!AniHelper.HasFinish[AniState.Begin])
            {
                AniHelper.UpDateAni(AniState.Begin);
                HandleBeginAni();
            }
            else if (BeginAttack || BeginRightAttack)
            {
                if (!AniHelper.HasFinish[AniState.Middle])
                    AniHelper.UpDateAni(AniState.Middle);
                else
                {
                    if (Owner.UCA().KingOfStorm)
                    {
                        if (BeginAttack)
                        {
                            Projectile p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), HeldPos + Projectile.Center, Projectile.velocity, ProjectileType<StormRulerHeld_KingofStorm>(), Projectile.damage, Projectile.knockBack, Projectile.owner, HeldPos.X, HeldPos.Y, TargetRot);
                            p.LAP().isWeaponSkillProj = true;
                        }
                        else if (BeginRightAttack)
                        {
                            Projectile p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), HeldPos + Projectile.Center, Projectile.velocity, ProjectileType<StormRulerHeldProj_Lunge_KingofStorm>(), Projectile.damage, Projectile.knockBack, Projectile.owner, HeldPos.X, HeldPos.Y, TargetRot);
                            p.LAP().isWeaponSkillProj = true;
                        }
                        Owner.UCA().KingOfStorm = false;
                    }
                    else
                    {
                        if (BeginAttack)
                        {
                            Projectile p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), HeldPos + Projectile.Center, Projectile.velocity, ProjectileType<StormRulerHeldHeavySwing>(), (int)(Projectile.damage), Projectile.knockBack, Projectile.owner, HeldPos.X, HeldPos.Y, TargetRot);
                            p.LAP().isWeaponSkillProj = true;
                        }
                        else if (BeginRightAttack)
                        {
                            Projectile p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), HeldPos + Projectile.Center, Projectile.velocity, ProjectileType<StormRulerHeldProj_Lunge>(), (int)(Projectile.damage), Projectile.knockBack, Projectile.owner, HeldPos.X, HeldPos.Y, TargetRot);
                            p.LAP().isWeaponSkillProj = true;
                        }
                    }
                    Projectile.Kill();
                }    
                HandleAttackAni();
            }
            else
            {
                HandleEndAni();
            }
            if (Owner.LAP().MouseLeft)
                BeginAttack = true;
            if (Owner.LAP().MouseRight)
                BeginRightAttack = true;
        }
        public void HandleBeginAni()
        {
            float easedProgress = EasingHelper.EaseOutCubic(AniHelper.GetProgress(AniState.Begin));
            float baseRotation = AniHelper.UpDateAngle(0, -155, Owner.direction, easedProgress);
            HeldPos = IdleOffset.RotatedBy(baseRotation).RotatedBy(TargetRot);
            float ProjRotation = AniHelper.UpDateAngle(-155, 0, Owner.direction, easedProgress);
            ProjRotOffset = ProjRotation;
        }
        public void HandleAttackAni()
        {
            if (Owner.UCA().KingOfStorm)
            {
                if (BeginAttack)
                {
                    float easedProgress = EasingHelper.EaseOutCubic(AniHelper.GetProgress(AniState.Middle));
                    float baseRotation = AniHelper.UpDateAngle(-155, -115, Owner.direction, easedProgress);
                    HeldPos = IdleOffset.RotatedBy(baseRotation).RotatedBy(TargetRot);
                    float ProjRotation = AniHelper.UpDateAngle(0, -155, Owner.direction, easedProgress);
                    ProjRotOffset = ProjRotation;
                }
                else if (BeginRightAttack)
                {
                    float easedProgress = EasingHelper.EaseOutCubic(AniHelper.GetProgress(AniState.Middle));
                    float baseRotation = AniHelper.UpDateAngle(-155, -165, Owner.direction, easedProgress);
                    HeldPos = IdleOffset.RotatedBy(baseRotation).RotatedBy(TargetRot);
                    float ProjRotation = AniHelper.UpDateAngle(0, 0, Owner.direction, easedProgress);
                    ProjRotOffset = ProjRotation;
                }
            }
            else
            {
                if (BeginAttack)
                {
                    float easedProgress = EasingHelper.EaseOutCubic(AniHelper.GetProgress(AniState.Middle));
                    float baseRotation = AniHelper.UpDateAngle(-155, 155, Owner.direction, easedProgress);
                    HeldPos = IdleOffset.RotatedBy(baseRotation).RotatedBy(TargetRot);
                    float ProjRotation = AniHelper.UpDateAngle(0, 225, Owner.direction, easedProgress);
                    ProjRotOffset = ProjRotation;
                }
                else if (BeginRightAttack)
                {
                    float easedProgress = EasingHelper.EaseOutCubic(AniHelper.GetProgress(AniState.Middle));
                    float baseRotation = AniHelper.UpDateAngle(-155, -165, Owner.direction, easedProgress);
                    HeldPos = IdleOffset.RotatedBy(baseRotation).RotatedBy(TargetRot);
                    float ProjRotation = AniHelper.UpDateAngle(0, 0, Owner.direction, easedProgress);
                    ProjRotOffset = ProjRotation;
                }
            }
        }
        public void HandleEndAni()
        {
            float baseRotation = AniHelper.UpDateAngle(0, -155, Owner.direction, 1);
            HeldPos = IdleOffset.RotatedBy(baseRotation).RotatedBy(TargetRot);
            float ProjRotation = AniHelper.UpDateAngle(-155, 0, Owner.direction, 1);
            ProjRotOffset = ProjRotation;
        }
        #endregion
        #region 粒子
        public void SpawnParticle()
        {
            Vector2 Pos = Vector2.Lerp(Projectile.Center - Vector2.UnitX.RotatedBy(Projectile.rotation + ProjRotOffset) * 40, Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.rotation + ProjRotOffset) * 40, Main.rand.NextFloat(0f, 1f));
            Pos = Pos + Main.rand.NextVector2Circular(16, 16);
            Vector2 firVel = Vector2.UnitX.RotatedBy(Projectile.rotation + ProjRotOffset) * 12f * Main.rand.NextFloat(1f, 1.5f);
            new CampSmoke(Pos, firVel.RotatedByRandom(0.1f) + Owner.velocity, Color.White, 45, Main.rand.NextFloat(MathHelper.TwoPi), 0.5f, Main.rand.NextFloat(0.15f, 0.2f)).Spawn();
            new CampSmoke(Pos, firVel.RotatedByRandom(0.1f) * -1 * 0.2f + Owner.velocity, Color.White, 45, Main.rand.NextFloat(MathHelper.TwoPi), 0.35f, Main.rand.NextFloat(0.15f, 0.2f)).Spawn();
            if (Main.rand.NextBool(2))
            {
                new LAP.Content.Particles.Fire(Pos, firVel + Owner.velocity, Color.White, 45, Main.rand.NextFloat(MathHelper.TwoPi), 0.5f, 0.25f).Spawn();
                new PoisonSmoke(Pos, firVel.RotatedByRandom(0.1f) + Owner.velocity, Color.White * 0.5f, 25, Main.rand.NextFloat(MathHelper.TwoPi), 0.25f, 0.35f).Spawn();

                new LAP.Content.Particles.Fire(Pos, firVel + Owner.velocity, Color.White, 45, Main.rand.NextFloat(MathHelper.TwoPi), 0.5f, 0.25f).Spawn();
                new PoisonSmoke(Pos, firVel.RotatedByRandom(0.1f) * -0.2f + Owner.velocity, Color.White * 0.35f, 25, Main.rand.NextFloat(MathHelper.TwoPi), 0.25f, 0.35f).Spawn();
            }
            if (ChargeTimer < 180 && !BeginAttack)
            {
                if (Main.rand.NextBool(5))
                {
                    Vector2 Balloffset = new Vector2(0, -1 * Owner.direction) + Vector2.UnitX.RotatedBy(Projectile.rotation + ProjRotOffset) * 26 * Owner.direction;
                    float beginrot = Main.rand.NextFloat(0, MathHelper.TwoPi);
                    float rotSpeed = Main.rand.NextBool() ? 0.06f : -0.06f;
                    int length = Main.rand.Next(100, 300);
                    int LifeTime = 30;
                    new ProjAbsorbGlowBall(Projectile.Center, Color.White, LifeTime, 0.08f, beginrot, rotSpeed, Projectile.whoAmI, length, Balloffset).Spawn();
                }
            }
            else if (!BeginAttack)
            {
                if (Main.rand.NextBool(3))
                {
                    Vector2 Pos2 = Vector2.Lerp(Projectile.Center, Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.rotation + ProjRotOffset) * 100, Main.rand.NextFloat(0f, 1f));
                    new SmallGlowBall(Pos2, Vector2.Zero, Color.White, Main.rand.Next(30, 120), 0.1f, 3f).Spawn();

                    Vector2 firVel2 = Vector2.UnitX.RotateRandom(MathHelper.TwoPi) * 1f * Main.rand.NextFloat(1f, 1.5f);
                    new LAP.Content.Particles.TrailGlowBall(Pos2, firVel2, Color.Gray, Main.rand.Next(15, 25), 0.1f, true).Spawn();
                }
                if (Main.rand.NextBool(4))
                {
                    Vector2 Pos3 = Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.rotation + ProjRotOffset) * 26;
                    Vector2 firVel2 = Vector2.UnitX.RotateRandom(MathHelper.TwoPi) * 1f * Main.rand.NextFloat(1f, 1.5f);
                    new LAP.Content.Particles.TrailGlowBall(Pos3, firVel2, Color.Gray, Main.rand.Next(15, 25), 0.1f, true).Spawn();
                }
            }
        }
        #endregion
        #region 更新充能动画
        public void UpdateCharge()
        {
            if (BeginAttack)
                return;
            if (ChargeTimer < 180)
                ChargeTimer++;
            if (ChargeTimer == 180)
            {
                SoundEngine.PlaySound(LAPSoundsMenu.WeaponSkillSound with { Volume = 1f, Pitch = Main.rand.NextFloat(-0.2f, 0.2f), MaxInstances = -1 }, Projectile.Center);
                Owner.UCA().KingOfStorm = true;
                Vector2 ToCenterOffset = new Vector2(0, -1 * Owner.direction) + Vector2.UnitX.RotatedBy(Projectile.rotation + ProjRotOffset) * 26;
                Vector2 FirePos = ToCenterOffset + Projectile.Center;
                for (int i = 0; i < 30; i++)
                {
                    Vector2 FireVel = new Vector2(12, 0).RotateRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.2f, 1f);
                    new CampSmoke(FirePos, FireVel + Owner.velocity, Color.White, 90, Main.rand.NextFloat(MathHelper.TwoPi), 1f, Main.rand.NextFloat(0.2f, 0.4f)).Spawn();
                    if (Main.rand.NextBool())
                    {
                        Vector2 FireVel2 = new Vector2(12, 0).RotateRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.2f, 1f);
                        new LAP.Content.Particles.Fire(FirePos, FireVel2 + Owner.velocity, Color.White, 90, Main.rand.NextFloat(MathHelper.TwoPi), 1f, Main.rand.NextFloat(0.1f, 0.3f)).Spawn();
                    }
                }
                for (int i = 0; i < 30; i++)
                {
                    new SmallGlowBall(FirePos, Vector2.Zero, Color.White, Main.rand.Next(90, 120), 0.1f, 6f).Spawn();
                    Vector2 firVel2 = Vector2.UnitX.RotateRandom(MathHelper.TwoPi) * 6f * Main.rand.NextFloat(0.2f, 1f);
                    new LAP.Content.Particles.TrailGlowBall(FirePos, firVel2, Color.Gray, Main.rand.Next(15, 25), 0.3f, false).Spawn();
                }
                ChargeTimer++;
            }
        }
        #endregion
        public void SetArmRot()
        {
            Vector2 target = Projectile.Center;
            float rot = LAPUtilities.GetVector2(Owner.GetArmRoot(), target).ToRotation();
            Owner.SetArmRot(rot);
        }
        public override void OnKill(int timeLeft)
        {
            if (BeginAttack || BeginRightAttack)
            {
                Vector2 ToCenterOffset = new Vector2(0, -1 * Owner.direction) + Vector2.UnitX.RotatedBy(Projectile.rotation + ProjRotOffset) * 26;
                Vector2 FirePos = ToCenterOffset + Projectile.Center;
                for (int i = 0; i < 10; i++)
                {
                    Vector2 FireVel = new Vector2(12, 0).RotateRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.2f, 1f);
                    new CampSmoke(FirePos, FireVel + Owner.velocity, Color.White, 90, Main.rand.NextFloat(MathHelper.TwoPi), 1f, Main.rand.NextFloat(0.2f, 0.4f)).Spawn();
                    if (Main.rand.NextBool())
                    {
                        Vector2 FireVel2 = new Vector2(12, 0).RotateRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.2f, 1f);
                        new LAP.Content.Particles.Fire(FirePos, FireVel2 + Owner.velocity, Color.White, 90, Main.rand.NextFloat(MathHelper.TwoPi), 1f, Main.rand.NextFloat(0.1f, 0.3f)).Spawn();
                    }
                }
                for (int i = 0; i < 30; i++)
                {
                    new SmallGlowBall(FirePos, Vector2.Zero, Color.White, Main.rand.Next(90, 120), 0.1f, 6f).Spawn();

                    Vector2 firVel2 = Vector2.UnitX.RotateRandom(MathHelper.TwoPi) * 6f * Main.rand.NextFloat(0.2f, 1f);
                    new LAP.Content.Particles.TrailGlowBall(FirePos, firVel2, Color.Gray, Main.rand.Next(15, 25), 0.3f, false).Spawn();
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.LAP().FirstFrame)
                return false;
            DrawBlade(lightColor);
            return false;
        }
        public void DrawBlade(Color lightColor)
        {
            Projectile.GetProjDrawInfo_Melee(out Texture2D _, out Vector2 _, out float drawRotation, out Vector2 rotationPoint, out SpriteEffects flipSprite);
            Texture2D texture = UCATextureRegister.StormRulerAlt.Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition + Vector2.UnitX.RotatedBy(Projectile.rotation + ProjRotOffset) * -16;
            Main.spriteBatch.Draw(texture, drawPosition, null, lightColor, drawRotation + ProjRotOffset, rotationPoint, Projectile.scale, flipSprite, 0f);
        }
        public void RenderPixelated(SpriteBatch spriteBatch)
        {

        }
    }
}
