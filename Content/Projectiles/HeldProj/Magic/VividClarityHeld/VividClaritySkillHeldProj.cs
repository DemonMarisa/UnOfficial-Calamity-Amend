using LAP.Assets.Sounds;
using LAP.Assets.TextureRegister;
using LAP.Core.AnimationHandle;
using LAP.Core.Enums;
using LAP.Core.Presets.Content;
using LAP.Core.SystemsLoader;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Localization;
using Terraria.ModLoader;
using UCA.Assets.Sounds;
using UCA.Content.GUI.ERayUI;
using UCA.Content.GUI.VividClarityUI;
using UCA.Content.Items.Weapons.Magic.Ray;
using UCA.Content.Particiles;
using UCA.Content.Projectiles.HeldProj.Magic.ElementRayHeld;
using UCA.Content.VFXs;
using UCA.Core.Utilities;

namespace UCA.Content.Projectiles.HeldProj.Magic.VividClarityHeld
{
    public class VividClaritySkillHeldProj : ModProjectile, ILocalizedModType
    {
        public override LocalizedText DisplayName => LAPUtilities.GetItemName<VividClarityAlt>();
        public override string Texture => GetInstance<VividClarityAlt>().Texture;
        public Player Owner => Main.player[Projectile.owner];
        public bool CanOut;
        public AniHelper AniHelper = new AniHelper(3);
        public float ToMouseRot;
        public override void SetStaticDefaults()
        {
            Projectile.AddHeldProj();
            Projectile.AddToSkillProj();
        }
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
            Init();
            HandleAni();
            UpdateGeneral();
        }
        public void Init()
        {
            if (Projectile.LAP().FirstFrame)
            {
                if (Projectile.IsLocalPlayer())
                    LAPContent.ActiveUI(LAPContent.UIType<VividClarityUI>());
                ToMouseRot = LAPUtilities.GetVector2(Projectile.Center, Owner.LocalMouseWorld()).ToRotation();
                AniHelper.MaxAniProgress[AniState.Begin] = 15;
                AniHelper.MaxAniProgress[1] = 30;
            }
        }
        public void UpdateGeneral()
        {
            Projectile.timeLeft = 2;
            Projectile.SetHeldProj(Owner);
            Projectile.spriteDirection = Owner.direction;
            Owner.SetArmRot(Projectile.rotation);
            ToMouseRot = Utils.AngleLerp(ToMouseRot, LAPUtilities.GetVector2(Projectile.Center, Owner.LocalMouseWorld()).ToRotation(), 0.12f);
        }
        public void HandleAni()
        {
            if (!AniHelper.HasFinish[0] || !CanOut)
            {
                AniHelper.UpDateAni(0);
                UpdateBegin();
                if (Owner.LAP().MouseLeft && !LAPContent.GetUI<VividClarityUI>().Active)
                    CanOut = true;
            }
            else if (!AniHelper.HasFinish[1] && CanOut)
            {
                AniHelper.UpDateAni(1);
                UpdateEnd();
            }
            else if (AniHelper.HasFinish[1] && CanOut)
            {
                Projectile.Kill();
            }
        }
        public void UpdateBegin()
        {
            float progress = AniHelper.GetProgress(0);
            float easedProgress = EasingHelper.EaseOutCubic(progress);
            float baseRotation = AniHelper.UpDateAngle(-45, -145, Owner.direction, easedProgress);
            Projectile.rotation = baseRotation + ToMouseRot;
            Projectile.Center = Owner.Center;
            Projectile.Opacity = progress;
            int CurAni = AniHelper.AniProgress[0];
            if (CurAni == 1)
            {
                SoundEngine.PlaySound(LAPSoundsMenu.TerraMagicaRelease with { Pitch = Main.rand.NextFloat(0.4f, 0.8f)}, Projectile.Center);
                int LifeTime = 45;
                Vector2 offset = new Vector2(96, 0);
                new FollowProjCrossGlow(Owner.Center, Color.White, LifeTime, 0.8f, Projectile.whoAmI, offset).Spawn();
            }
        }
        public void UpdateEnd()
        {
            float progress = AniHelper.GetProgress(1);
            float easedProgress = EasingHelper.EaseOutBack(progress);
            float baseRotation = AniHelper.UpDateAngle(-145, 135, Owner.direction, easedProgress);
            Projectile.rotation = baseRotation + ToMouseRot;
            Projectile.Center = Owner.Center;
            Projectile.Opacity = 1 - EasingHelper.EaseInCubic(progress);
            int CurAni = AniHelper.AniProgress[1];
            if (CurAni == 1)
                SoundEngine.PlaySound(SoundsMenu.SoulOfCinderChange, Projectile.Center);
            if (CurAni == 5)
            {
                new CrossGlow(Owner.Center, Vector2.Zero, Color.White, 90, 1f, 1f).Spawn();
                for (int i = 0; i < 4; i++)
                {
                    Color color = LAPUtilities.LerpColor(Color.LightGreen, Color.WhiteSmoke);
                    new NoiseShockRing(Projectile.Center, Vector2.Zero, color, 45, 1f, 2f + i * 0.2f, Projectile.whoAmI, Vector2.Zero, false).Spawn();
                }
                for (int i = 0; i < 50; i++)
                {
                    Color RandomColor = LAPUtilities.LerpColor(Color.LightGreen, Color.WhiteSmoke);
                    ParticlePreset.NewTGlowBall(Projectile.Center, Vector2.Zero,RandomColor, 120, 0.4f, Main.rand.NextFloat(6f, 12));
                }
                for (int i = 0; i < 25; i++)
                {
                    Color RandomColor = LAPUtilities.LerpColor(Color.LightGreen, Color.WhiteSmoke);
                    ParticlePreset.NewDustGlow(Projectile.Center, Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(12f, 42f), 0, RandomColor, Main.rand.Next(45, 90), 0.2f, 0);
                }
                Projectile.netUpdate = true;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            LAPUtilities.ReSetToBeginShader(BlendState.AlphaBlend);

            Projectile.GetProjDrawInfo_Staff(out Texture2D texture, out Vector2 drawPosition, out float drawRotation, out Vector2 rotationPoint, out SpriteEffects flipSprite);
            LAPUtilities.SetTexture(LAPTextureRegister.Noise.Value, SamplerState.PointWrap, 1);
            LAPUtilities.FastApplyEdgeMeltsShader(1 - Projectile.Opacity, texture.Size(), Color.GhostWhite, 0.01f, 0);
            Main.spriteBatch.Draw(texture, drawPosition, null, lightColor, drawRotation, rotationPoint, Projectile.scale, flipSprite, 0);

            LAPUtilities.ReSetToBeginShader(BlendState.AlphaBlend, SamplerState.PointClamp);
            return false;
        }
    }
}
