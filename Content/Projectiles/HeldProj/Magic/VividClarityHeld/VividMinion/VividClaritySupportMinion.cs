using LAP.Assets.Sounds;
using LAP.Assets.TextureRegister;
using LAP.Core.AnimationHandle;
using LAP.Core.Graphics.DeepGlow;
using LAP.Core.Presets.Content;
using LAP.Core.SystemsLoader;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Content.GUI.VividClarityUI;
using UCA.Content.Items.Weapons.Magic.Ray;
using UCA.Core.Enum;
using UCA.Core.Utilities;

namespace UCA.Content.Projectiles.HeldProj.Magic.VividClarityHeld
{
    public partial class VividClaritySupportMinion : ModProjectile
    {
        public const int Idle = 0;
        public const int MeleeAttack = 1;
        public const int RangedAttack = 2;
        public const int HealPlayer = 3;
        public int State;
        public int AttackTimer;
        public int RealTimer;
        public int MeleeCD;
        public override string Texture => GetInstance<VividClarityAlt>().Texture;
        public Player Owner => Main.player[Projectile.owner];
        public bool FadeOut;
        public bool FadeOutBlade;
        public int BlockHealStateChangeTimer;
        public bool CanIntoHeal;
        public Vector2 IdlePos => new Vector2(-30 * Owner.direction, -10);
        public float IdleRot => -MathHelper.PiOver2 + 0.2f * Owner.direction;
        public AniHelper aniHelper = new AniHelper(2);
        public override void SetStaticDefaults()
        {
            Projectile.AddHeldProj();
        }
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 0;
            Projectile.Opacity = 0f;
            Projectile.timeLeft = 2;
            Projectile.minion = true;
            Projectile.ContinuouslyUpdateDamageStats = true;
            State = Idle;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return base.Colliding(projHitbox, targetHitbox);
        }
        public override bool? CanDamage()
        {
            return false;
        }
        public override void AI()
        {
            UpdateCanFadeOut();
            Init();
            AttackTimer++;
            if (aniHelper.HasFinish[0] && !FadeOut)
            {
                if (State == Idle)
                {
                    UpdateIdle();
                }
                else if (State == MeleeAttack)
                {
                    UpdateMeleeAttack();
                }
                else if (State == RangedAttack)
                {
                    UpdateRangedAttack();
                }
                else if (State == HealPlayer)
                {
                    UpdateHealPlayer();
                }
                UpdateIntoHeal();
            }
            UpdateGeneral();
            UpdateAni();
        }
        public void Init()
        {
            if (Projectile.LAP().FirstFrame)
            {
                aniHelper.MaxAniProgress[0] = 30;
                aniHelper.MaxAniProgress[1] = 30;
                MeleeCD = 500;
                SoundEngine.PlaySound(LAPSoundsMenu.MagicCharge_ER, Projectile.Center);
            }
        }
        public void UpdateCanFadeOut()
        {
            RealTimer++;
            if (RealTimer > 7200)
                FadeOut = true;
            if (Owner.UCA().VividClarityStates == VividClarityState.Support && !Owner.HasProj<VividClaritySkillHeldProj>() && !LAPContent.GetUI<VividClarityUI>().Active)
            {
                if (aniHelper.HasFinish[0] && !FadeOut)
                {
                    if (Main.mouseRight && Main.mouseRightRelease)
                    {
                        SoundEngine.PlaySound(LAPSoundsMenu.MagicHit02, Projectile.Center);
                        FadeOut = true;
                    }
                }
            }
        }
        public void UpdateIntoHeal()
        {
            if (BlockHealStateChangeTimer > 0)
                BlockHealStateChangeTimer--;
            if (Owner.UCA().VividClarityStates == VividClarityState.Support && !Owner.HasProj<VividClaritySkillHeldProj>() && !LAPContent.GetUI<VividClarityUI>().Active)
            {
                if (State == HealPlayer || BlockHealStateChangeTimer > 0)
                    return;
                if (Projectile.IsLocalPlayer())
                {
                    if (Main.mouseLeft && Main.mouseLeftRelease)
                    {
                        CanIntoHeal = true;
                    }
                    if (State != MeleeAttack)
                    {
                        if (CanIntoHeal)
                        {
                            ChangeState(HealPlayer);
                            CanIntoHeal = false;
                        }
                    }
                }
            }
        }
        public void UpdateGeneral()
        {
            Projectile.timeLeft = 2;
            Projectile.spriteDirection = Projectile.direction;
            if (FadeOutBlade)
            {
                if (GreatSword is not null)
                {
                    GreatSword.Scale = MathHelper.Lerp(GreatSword.Scale, 0f, 0.12f);
                    GreatSword.AiFloat[2] = MathHelper.Lerp(GreatSword.AiFloat[2], 0f, 0.12f);
                    if (GreatSword.AiFloat[2] < 0.05f)
                    {
                        GreatSword.Kill();
                        GreatSword = null;
                        FadeOutBlade = false;
                    }
                }
            }
            if (State != HealPlayer)
            {
                HealEffectScale = MathHelper.Lerp(HealEffectScale, 0f, 0.12f);
            }
        }
        public void UpdateAni()
        {
            if (!aniHelper.HasFinish[0])
            {
                aniHelper.UpDateAni(0);
                UpdateInto();
            }
            else if (!aniHelper.HasFinish[1] && FadeOut)
            {
                aniHelper.UpDateAni(1);
                UpdateOut();
            }
            else if (aniHelper.HasFinish[1] && FadeOut)
                Projectile.Kill();
        }
        public void ChangeState(int Next)
        {
            if (State == Next)
                return;
            if (Next == MeleeAttack)
                MeleeCD = 300;
            AttackTimer = 0;
            State = Next;
        }
        public void UpdateInto()
        {
            float progress = aniHelper.GetProgress(0);
            float easingProg = EasingHelper.EaseOutCubic(progress);
            Vector2 veladd = Vector2.Lerp(new Vector2(-30 * Owner.direction, -40), IdlePos, easingProg);
            Projectile.velocity = Vector2.Zero;
            Projectile.Center = Vector2.SmoothStep(Projectile.Center, Owner.Center + veladd, 0.2f);
            Projectile.Opacity = MathHelper.Lerp(0f, 1f, easingProg);
            Projectile.rotation = IdleRot;
        }
        public void UpdateOut()
        {
            if (aniHelper.AniProgress[1] == 1)
            {
                for (int i = 0; i < 15;i++)
                {
                    Vector2 vel = Vector2.UnitX.RotateRandom(MathHelper.TwoPi) * Main.rand.NextFloat(6f, 12f);
                    Color color = LAPUtilities.LerpColor(Color.WhiteSmoke, Color.LightGreen);
                    ParticlePreset.NewDustGlow(Projectile.Center, vel, 0, color, 45, 0.2f, 0);
                }
                for (int i = 0; i < 25; i++)
                {
                    Vector2 vel = Vector2.UnitX.RotateRandom(MathHelper.TwoPi) * Main.rand.NextFloat(6f, 12f);
                    Color color = LAPUtilities.LerpColor(Color.WhiteSmoke, Color.LightGreen);
                    ParticlePreset.NewTGlowBall(Projectile.Center, Vector2.Zero, color, 70, 0.25f, 4f);
                }
            }
            float progress = aniHelper.GetProgress(1);
            Projectile.velocity *= 0.95f;
            Projectile.Opacity = MathHelper.Lerp(1f, 0f, progress);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            LAPUtilities.ReSetToBeginShader(BlendState.Additive, SamplerState.PointClamp);

            Vector4 cut = new Vector4(0.5f, 1f, 0f, 1f);
            LAPUtilities.ApplyUVRot(cut, 1f);
            DrawControlCircle(0.2f, 10);
            LAPUtilities.ApplyUVRot(cut, -1f);
            DrawControlCircle(0.15f, -25);

            LAPUtilities.ReSetToBeginShader(BlendState.Additive);

            Projectile.GetProjDrawInfo_Staff(out Texture2D texture, out Vector2 drawPosition, out float drawRotation, out Vector2 rotationPoint, out SpriteEffects flipSprite);
            LAPUtilities.SetTexture(LAPTextureRegister.Noise.Value, SamplerState.PointWrap, 1);
            LAPUtilities.FastApplyEdgeMeltsShader(1 - Projectile.Opacity, texture.Size(), Color.GhostWhite, 0.01f, 0);
            float angle = MathHelper.TwoPi / 10f;
            for (int i = 0; i < 14; i++)
            {
                Vector2 DrawOffset = new Vector2(6 * HealEffectScale, 0).RotatedBy(angle * i + Main.GlobalTimeWrappedHourly);
                Main.spriteBatch.Draw(texture, drawPosition + DrawOffset, null, Color.ForestGreen * 0.5f, drawRotation, rotationPoint, Projectile.scale, flipSprite, 0);
            }

            LAPUtilities.ReSetToBeginShader(BlendState.AlphaBlend);
            
            LAPUtilities.SetTexture(LAPTextureRegister.Noise.Value, SamplerState.PointWrap, 1);
            LAPUtilities.FastApplyEdgeMeltsShader(1 - Projectile.Opacity, texture.Size(), Color.GhostWhite, 0.01f, 0);

            Main.spriteBatch.Draw(texture, drawPosition, null, lightColor, drawRotation, rotationPoint, Projectile.scale, flipSprite, 0);

            LAPUtilities.ReSetToBeginShader(BlendState.Additive, SamplerState.PointClamp);

            Vector4 Frontcut = new Vector4(0f, 0.5f, 0f, 1f);
            LAPUtilities.ApplyUVRot(Frontcut, 1f);
            DrawControlCircle(0.2f, 10);
            LAPUtilities.ApplyUVRot(Frontcut, -1f);
            DrawControlCircle(0.15f, -25);

            LAPUtilities.ReSetToEndShader();


            DeepGlow.SubmitCustomGlow(() =>
            {
                LAPUtilities.ReSetToBeginShader(BlendState.AlphaBlend);

                LAPUtilities.SetTexture(LAPTextureRegister.Noise.Value, SamplerState.PointWrap, 1);
                LAPUtilities.FastApplyEdgeMeltsShader(1 - Projectile.Opacity, texture.Size(), Color.GhostWhite, 0.01f, 0);
                Main.spriteBatch.Draw(texture, drawPosition, null, Color.Transparent, drawRotation, rotationPoint, Projectile.scale, flipSprite, 0);

                LAPUtilities.ReSetToBeginShader(BlendState.Additive, SamplerState.PointClamp);

                Vector4 Frontcut = new Vector4(0f, 0.5f, 0f, 1f);
                LAPUtilities.ApplyUVRot(Frontcut, 1f);
                DrawControlCircle(0.2f, 10);
                LAPUtilities.ApplyUVRot(Frontcut, -1f);
                DrawControlCircle(0.15f, -25);

                LAPUtilities.ReSetToEndShader();
            });
            return false;
        }
        public void DrawControlCircle(float scale = 1f, float YOffset = 24f)
        {
            Vector2 offset = new Vector2(YOffset, 0).RotatedBy(Projectile.rotation) + new Vector2(0, 12).RotatedBy(Projectile.rotation - MathHelper.PiOver2);
            Texture2D circle = UCATextureRegister.TechCircle.Value;
            Color color = Color.Lerp(Color.White, Color.LightGreen, HealEffectScale);
            LAPUtilities.Draw(circle, Projectile.Center - Main.screenPosition + offset, null, color * Projectile.Opacity, Projectile.rotation, circle.Size() / 2, new Vector2(0.25f, 1f) * scale, 0);
        }
    }
}
