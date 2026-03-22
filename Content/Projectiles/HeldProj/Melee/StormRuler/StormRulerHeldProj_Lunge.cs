using LAP.Assets.Effects;
using LAP.Assets.Sounds;
using LAP.Assets.TextureRegister;
using LAP.Common.Blance;
using LAP.Content.Particles;
using LAP.Core.AnimationHandle;
using LAP.Core.Enums;
using LAP.Core.Graphics.PixelatedRender;
using LAP.Core.Keybind;
using LAP.Core.LAPSource;
using LAP.Core.SpecificEffectManagers;
using LAP.Core.SystemsLoader;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.Localization;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Content.Items.Weapons.Melee.GreatSword;
using UCA.Content.Projectiles.Melee.NormalProj;

namespace UCA.Content.Projectiles.HeldProj.Melee.StormRuler
{
    public class StormRulerHeldProj_Lunge : ModProjectile, ILocalizedModType, IPixelatedRenderer
    {
        public DrawLayer LayerToRenderTo => DrawLayer.BeforeDusts;
        public BlendState BlendState => BlendState.Additive;
        public override LocalizedText DisplayName => LAPUtilities.GetItemName<StormRulerAlt>();
        public override string Texture => UCATextureRegister.StormRulerAlt.Path;
        public Player Owner => Projectile.Owner();
        public Vector2 IdleOffset => new Vector2(12, 4 * Owner.direction);
        public ref float FatherHeldPosX => ref Projectile.ai[0];
        public ref float FatherHeldPosY => ref Projectile.ai[1];
        public ref float FatherTargetRot => ref Projectile.ai[2];
        public float FatherProjRotOffset => 0f;
        public int UseTime => Owner.ApplyWeaponAttackSpeed(Owner.HeldItem, Owner.HeldItem.useTime * 20, 250);
        public Vector2 BeginPos;
        public int BeginDir;
        public AniHelper AniHelper = new AniHelper(5);
        public Vector2 HeldPos;
        public float TargetRot;
        public float ProjRotOffset;
        public float ProjRotOffset2;
        public int HitCooldown;
        public int BeginDamage;
        public bool HasFocus;
        public override void SetStaticDefaults()
        {
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
            Projectile.localNPCHitCooldown = -1;
            Projectile.noEnchantmentVisuals = true;
            Projectile.netImportant = true;
            Projectile.extraUpdates = 10;
            Projectile.Opacity = 1f;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Projectile.LAP().FirstFrame)
                return false;
            if (BeginDamage > 0)
            {
                float _ = float.NaN;
                Vector2 beamBeginPos = BeginPos - Projectile.rotation.ToRotationVector2() * 400;
                Vector2 beamEndPos = BeginPos + Projectile.rotation.ToRotationVector2() * 1000;
                bool c = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), beamBeginPos, beamEndPos, 64f, ref _);
                return c;
            }
            return false;
        }
        public override void AI()
        {
            Owner.SetUseFocus(2);
            Init();
            UpdateGeneral();
            SetArmRot();
            UpdateAni();
        }
        public void Init()
        {
            if (!Projectile.LAP().FirstFrame)
                return;
            HasFocus = Owner.CheckFocus(Owner.ActiveItem().LAP().WeaponSkillRealFocusCost);
            SoundEngine.PlaySound(LAPSoundsMenu.StormRulerAttack with { Volume = 1f, Pitch = Main.rand.NextFloat(0.3f, 0.7f), MaxInstances = -1 }, Projectile.Center);
            BeginDamage = 120;
            BeginPos = Projectile.Center;
            TargetRot = FatherTargetRot;
            BeginDir = Owner.LocalMouseWorld().X > Owner.Center.X ? 1 : -1;
            Projectile.rotation = TargetRot;
            HeldPos = new Vector2(FatherHeldPosX, FatherHeldPosY);
            AniHelper.MaxAniProgress[AniState.Begin] = (int)(UseTime * 0.1f);
            AniHelper.MaxAniProgress[AniState.Middle] = (int)(UseTime * 0.9f);
            ScreenShakeSystem.AddScreenShakes(Projectile.Center, 100, 45, Projectile.rotation, 0, true, 1000);
            for (int i = 0; i < 120; i++)
            {
                Vector2 BeginPos = Projectile.Center + new Vector2(1, 0).RotatedBy(Projectile.rotation) * 24 + Main.rand.NextVector2Circular(18, 18);
                Vector2 FirVel = Projectile.rotation.ToRotationVector2() * 32 * Main.rand.NextFloat(0, 1f);
                new TrailGlowBall(BeginPos, FirVel, Color.White * 0.5f, Main.rand.Next(45, 65), 0.2f, true).Spawn();
            }
            for (int i = 0; i < 60; i++)
            {
                Vector2 BeginPos = Projectile.Center + new Vector2(1, 0).RotatedBy(Projectile.rotation) * 24 + Main.rand.NextVector2Circular(18, 18);
                Vector2 FirVel = Projectile.rotation.ToRotationVector2() * -18 * Main.rand.NextFloat(0, 1f);
                new TrailGlowBall(BeginPos, FirVel, Color.White * 0.5f, Main.rand.Next(45, 65), 0.2f, true).Spawn();
            }
            for (int i = 0; i < 120; i++)
            {
                Vector2 BeginPos = Projectile.Center + new Vector2(1, 0).RotatedBy(Projectile.rotation) * 24 + Main.rand.NextVector2Circular(18, 18);
                Vector2 FirVel = Projectile.rotation.ToRotationVector2() * 68 * Main.rand.NextFloat(0, 1f);
                new CampSmoke(BeginPos, FirVel, Color.White, 90, Main.rand.NextFloat(MathHelper.TwoPi), 0.3f, Main.rand.NextFloat(0.1f, 0.2f)).Spawn();
            }
            for (int i = 0; i < 40; i++)
            {
                Vector2 BeginPos = Projectile.Center + new Vector2(1, 0).RotatedBy(Projectile.rotation) * 24  + Main.rand.NextVector2Circular(18, 18);
                Vector2 FirVel = Projectile.rotation.ToRotationVector2() * 48 * Main.rand.NextFloat(0, 1f);
                new CampSmoke(BeginPos, FirVel, Color.White, 90, Main.rand.NextFloat(MathHelper.TwoPi), 0.2f, Main.rand.NextFloat(0.2f, 0.4f)).Spawn();
            }
            for (int i = 0; i < 120; i++)
            {
                Vector2 BeginPos = Projectile.Center + new Vector2(1, 0).RotatedBy(Projectile.rotation) * 24 + Main.rand.NextVector2Circular(18, 18);
                Vector2 FirVel = Projectile.rotation.ToRotationVector2() * 68 * Main.rand.NextFloat(0, 1f) * 1.4f;
                new Fire(BeginPos, FirVel, Color.White, 90, Main.rand.NextFloat(MathHelper.TwoPi), 1f, Main.rand.NextFloat(0.1f, 0.2f)).Spawn();
            }
            for (int i = 0; i < 40; i++)
            {
                Vector2 BeginPos = Projectile.Center + new Vector2(1, 0).RotatedBy(Projectile.rotation) * 24 + Main.rand.NextVector2Circular(18, 18);
                Vector2 FirVel = Projectile.rotation.ToRotationVector2() * 48 * Main.rand.NextFloat(0, 1f) * 1.4f;
                new Fire(BeginPos, FirVel, Color.White, 90, Main.rand.NextFloat(MathHelper.TwoPi), 1f, Main.rand.NextFloat(0.2f, 0.4f)).Spawn();
            }
            for (int i = 0; i < 15; i++)
            {
                Vector2 BeginPos = Projectile.Center + new Vector2(1, 0).RotatedBy(Projectile.rotation) * 24 + Main.rand.NextVector2Circular(18, 18);
                Vector2 firVel2 = Vector2.UnitX.RotateRandom(MathHelper.TwoPi) * 12f * Main.rand.NextFloat(0.2f, 1f);
                new TrailGlowBall(BeginPos, firVel2, Color.Gray, Main.rand.Next(15, 25), 0.3f, false).Spawn();
            }
        }
        public void UpdateGeneral()
        {
            Projectile.SetHeldProj(Owner);
            Owner.ChangeDir(BeginDir);
            Projectile.Center = Owner.GetArmRoot() + HeldPos;
            Projectile.rotation = TargetRot;
            Projectile.timeLeft = 2;
            if (BeginDamage > 0)
                BeginDamage--;
        }
        public void UpdateAni()
        {
            if (!AniHelper.HasFinish[AniState.Begin])
            {
                AniHelper.UpDateAni(AniState.Begin);
                HandleBeginAni();
            }
            else if (!AniHelper.HasFinish[AniState.Middle])
            {
                AniHelper.UpDateAni(AniState.Middle);
                HandleMiddleAni();
            }
            else
            {
                Projectile.Kill();
            }
        }
        public void HandleBeginAni()
        {
            float easedProgress = EasingHelper.EaseOutCubic(AniHelper.GetProgress(AniState.Begin));
            float baseRotation = AniHelper.UpDateAngle(-165, -115, Owner.direction, easedProgress);
            HeldPos = IdleOffset.RotatedBy(baseRotation).RotatedBy(TargetRot);
        }
        public void HandleMiddleAni()
        {
            float easedProgress = EasingHelper.EaseOutCubic(AniHelper.GetProgress(AniState.Middle));
            float baseRotation = AniHelper.UpDateAngle(-115, -15, Owner.direction, easedProgress);
            HeldPos = IdleOffset.RotatedBy(baseRotation).RotatedBy(TargetRot);
            float ProjRotation = AniHelper.UpDateAngle(0, -155, Owner.direction, easedProgress);
            ProjRotOffset2 = ProjRotation;
            easedProgress = AniHelper.GetProgress(AniState.Middle);
            Projectile.Opacity = 1 - easedProgress;
        }
        public void SetArmRot()
        {
            Vector2 target = Projectile.Center;
            float rot = LAPUtilities.GetVector2(Owner.GetArmRoot(), target).ToRotation();
            Owner.SetArmRot(rot);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (HasFocus)
                modifiers.SourceDamage *= UCABlanceRule.StormRulerSkillLungeDamageMult;
            else
                modifiers.SourceDamage *= UCABlanceRule.StormRulerSkillLungeNoFocusDamageMult;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (HitCooldown == 0)
            {
                Projectile p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ProjectileType<StormBlast>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                p.LAP().isWeaponSkillProj = true;
                HitCooldown = 5;
            }
        }
        public override void OnKill(int timeLeft)
        {
            if (LAPKeybind.WeaponSkillHotKey.Current)
            {
                Item item = ItemLoader.GetItem(ItemType<StormRulerAlt>()).Item;
                EntitySource_ItemUse_WeaponSkill source = new(Owner, item);
                if (!Owner.HasProj<StormRulerHeldSkillProj>())
                {
                    Projectile p = Projectile.NewProjectileDirect(source, Projectile.Center, Projectile.velocity, ProjectileType<StormRulerHeldSkillProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    p.LAP().isWeaponSkillProj = Projectile.LAP().isWeaponSkillProj;
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            PixelatedRenderManger.BeginDrawProj = true;
            if (!Projectile.active)
                return false;
            DrawBlade(lightColor);
            return false;
        }
        public void DrawBlade(Color lightColor)
        {
            Projectile.GetProjDrawInfo_Melee(out Texture2D texture, out Vector2 _, out float drawRotation, out Vector2 rotationPoint, out SpriteEffects flipSprite);
            Vector2 drawPosition = Projectile.Center - Main.screenPosition + Vector2.UnitX.RotatedBy(Projectile.rotation + ProjRotOffset + ProjRotOffset2) * -24;
            Main.spriteBatch.Draw(texture, drawPosition, null, lightColor, drawRotation + ProjRotOffset + ProjRotOffset2, rotationPoint, Projectile.scale * 1f, flipSprite, 0f);
        }
        public void RenderPixelated(SpriteBatch spriteBatch)
        {
            Texture2D texture = LAPTextureRegister.HoodTrail.Value;
            Vector2 drawPosition = BeginPos - Main.screenPosition + Vector2.UnitX.RotatedBy(Projectile.rotation + ProjRotOffset) * 1000;
            Vector2 orig = new Vector2(texture.Width, texture.Height / 2);
            Main.spriteBatch.Draw(texture, drawPosition, null, Color.White * 0.755f * Projectile.Opacity, Projectile.rotation + ProjRotOffset, orig, new Vector2(6f, 0.6f), SpriteEffects.FlipHorizontally, 0f);

            Main.spriteBatch.Draw(texture, drawPosition, null, Color.White * 0.4f * Projectile.Opacity, Projectile.rotation + ProjRotOffset, orig, new Vector2(8f, 0.2f), SpriteEffects.FlipHorizontally, 0f);

            LAPContent.ReSetToBeginShader_Pixel(BlendState.Additive);

            Vector2 drawPosition2 = BeginPos - Main.screenPosition + Vector2.UnitX.RotatedBy(Projectile.rotation + ProjRotOffset) * 1000 - Vector2.UnitY * 18 * Owner.direction;

            Texture2D texture2 = UCATextureRegister.Aura_01.Value;
            Effect effect2 = LAPShaderRegister.AlphaFade_ACut_OColor.Value;
            effect2.Parameters["uFadeoutLeftLength"].SetValue(0.2f);
            effect2.Parameters["uFadeinRigtLength"].SetValue(0.3f);
            effect2.Parameters["UVOffset"].SetValue(new Vector2(Main.GlobalTimeWrappedHourly * 0.5f, 0));
            effect2.Parameters["UVMult"].SetValue(new Vector2(3f, 1f));
            effect2.Parameters["OverlayColor"].SetValue(Color.White.ToVector4());
            effect2.CurrentTechnique.Passes[0].Apply();

            Main.spriteBatch.Draw(texture2, drawPosition2, null, Color.White * Projectile.Opacity, Projectile.rotation + ProjRotOffset, orig, new Vector2(7f, 0.3f), SpriteEffects.FlipHorizontally, 0f);

            Texture2D texture3 = UCATextureRegister.Aura_02.Value;

            Main.spriteBatch.Draw(texture3, drawPosition2, null, Color.White * Projectile.Opacity, Projectile.rotation + ProjRotOffset, orig, new Vector2(7f, 0.3f), SpriteEffects.FlipHorizontally, 0f);

            LAPContent.ReSetToEndShader_Pixel();
        }
    }
}
