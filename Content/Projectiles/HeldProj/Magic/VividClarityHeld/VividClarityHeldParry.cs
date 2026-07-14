using LAP.Assets.Sounds;
using LAP.Assets.TextureRegister;
using LAP.Core.AnimationHandle;
using LAP.Core.Graphics.DeepGlow;
using LAP.Core.NetCode.NetUtilities;
using LAP.Core.Presets.Content;
using LAP.Core.SpecificEffectManagers;
using LAP.Core.StateMachine.SynedHitEffect;
using LAP.Core.SystemsLoader;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using UCA.Content.HitEffect;
using UCA.Content.Items.Weapons.Magic.Ray;

namespace UCA.Content.Projectiles.HeldProj.Magic.VividClarityHeld
{
    public class VividClarityHeldParry : ModProjectile
    {
        public override string Texture => GetInstance<VividClarityAlt>().Texture;
        public override LocalizedText DisplayName => LAPUtilities.GetItemName<VividClarityAlt>();
        public Player Owner => Main.player[Projectile.owner];
        public Rectangle[] ParryHitBox = new Rectangle[10];
        public AniHelper AniHelper = new AniHelper(3);
        public float ToMouseRotation;
        public float EffectScale;
        public float EffectOpacity;

        public int EffectTimer;
        public int MaxParryTimer = 30;
        public bool HasParry;

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
            Projectile.Opacity = 1f;
        }
        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }
        public override void AI()
        {
            Init();
            UpdateAni();
            UpdateGeneral();
            UpdateRotation();
            UpdateEffect();
            CheckColliding();
            ParryProtect();
            UpdataInPut();
        }
        public void Init()
        {
            if (Projectile.LAP().FirstFrame)
            {
                SoundEngine.PlaySound(LAPSoundsMenu.MagicTrigger02 with { Volume = 1f }, Projectile.Center);
                ToMouseRotation = Projectile.Center.AngleTo(Owner.LocalMouseWorld());
                AniHelper.MaxAniProgress[0] = 25;
                Projectile.rotation = ToMouseRotation;
                Projectile.Center = Owner.MountedCenter;
            }
        }
        public void UpdateGeneral()
        {
            Projectile.SetHeldProj(Owner, false);
            Owner.SetArmRot(Projectile.rotation);
        }
        public void UpdateRotation()
        {
            ToMouseRotation = Utils.AngleLerp(ToMouseRotation, Owner.Center.AngleTo(Owner.LocalMouseWorld()), 0.2f);
        }
        public void UpdateEffect()
        {
            if (EffectTimer < MaxParryTimer)
                EffectTimer++;
            float progress = EffectTimer / (float)MaxParryTimer;
            EffectScale = EasingHelper.EaseOutCubic(progress);
            EffectOpacity = MathHelper.Lerp(1f, 0f, EasingHelper.EaseInCubic(progress));
        }
        public void UpdateAni()
        {
            // 蓄力
            if (!AniHelper.HasFinish[0])
            {
                AniHelper.UpDateAni(0, 20);
                HandleBeginAni();
            }
            else
                Projectile.Kill();
        }
        public void HandleBeginAni()
        {
            float progress = AniHelper.GetProgress(0);
            float easedProgress = EasingHelper.EaseOutBack(progress);
            float UpdateRot = AniHelper.UpDateAngle(-135, 135, Owner.direction, easedProgress);

            Projectile.Center = Owner.MountedCenter;
            Projectile.rotation = UpdateRot + ToMouseRotation;
            Projectile.spriteDirection = Owner.direction;

            float BreakTimer = AniHelper.BreakTime[0];
            Projectile.Opacity = MathHelper.Lerp(1f, 0f, BreakTimer / 20f);
        }
        public void CheckColliding()
        {
            if (!Projectile.IsLocalPlayer() || HasParry || EffectTimer >= MaxParryTimer)
                return;
            ParryHitBox = LAPUtilities.AABBCircularHitboxes(Projectile.Center, 60, 4);
            // 检查射弹
            foreach (Projectile proj in Main.ActiveProjectiles)
            {
                if (!proj.hostile || !proj.active || proj.damage < 5)
                    continue;
                // 根据extraUpdates分段回溯弹幕在这一帧内的运动轨迹
                int steps = proj.extraUpdates + 1;
                for (int i = 0; i < steps; i++)
                {
                    // 计算回溯位置
                    Vector2 checkPos = proj.position - proj.velocity * i;
                    Rectangle hitbox = proj.Hitbox;
                    hitbox.X = (int)checkPos.X;
                    hitbox.Y = (int)checkPos.Y;
                    // 使用回溯的 Hitbox 进行碰撞判定
                    if (proj.Colliding(hitbox, ParryHitBox[0]) || proj.Colliding(hitbox, ParryHitBox[1]) || proj.Colliding(hitbox, ParryHitBox[2]) || proj.Colliding(hitbox, ParryHitBox[3]))
                    {
                        Owner.SetImmuneTimeForAllTypes(60);
                        HitEffectManager.SpawnHitEffect(HitEffectManager.HEType<VividClarityParryHit>(), Projectile.owner, Projectile.GetSource_FromThis(), proj.Center, Vector2.Zero);

                        if (ProjectileID.Sets.DrawScreenCheckFluff[proj.type] < 500 && proj.velocity != Vector2.Zero)
                        {
                            Vector2 shieldNormal = LAPUtilities.GetVector2(Projectile.Center, proj.Center);
                            proj.velocity = proj.velocity - 2f * Vector2.Dot(proj.velocity, shieldNormal) * shieldNormal;
                        }
                        proj.damage = 0;
                        proj.netSpam = 0;
                        proj.netUpdate = true;
                        proj.SyncedReflectProj();
                        HasParry = true;
                        return;
                    }
                }
            }
        }
        public void ParryProtect()
        {
            if (EffectTimer < MaxParryTimer)
                Owner.SetImmuneTimeForAllTypes(2);
        }
        public void UpdataInPut()
        {
            if (!Projectile.IsLocalPlayer())
                return;
            if (HasParry)
            {
                if (Main.mouseRight && Main.mouseRightRelease)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, Type, Projectile.damage, Projectile.knockBack, Projectile.owner);
                    Owner.SetItemAnimation(0);
                    Owner.SetItemTime(0);
                    Projectile.Kill();
                }
            }
        }
        public override void OnKill(int timeLeft)
        {
            Owner.SetItemAnimation(0);
            Owner.SetItemTime(0);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            LAPUtilities.ReSetToBeginShader(BlendState.AlphaBlend);

            Projectile.GetProjDrawInfo_Staff(out Texture2D texture, out Vector2 drawPosition, out float drawRotation, out Vector2 rotationPoint, out SpriteEffects flipSprite);
            LAPUtilities.FastApplyEdgeMeltsShader(1 - Projectile.Opacity, texture.Size(), Color.White);
            LAPUtilities.SetTexture(LAPTextureRegister.Noise.Value, SamplerState.PointClamp, 1);
            Main.spriteBatch.Draw(texture, drawPosition, null, lightColor, drawRotation, rotationPoint, Projectile.scale, flipSprite, 0);

            LAPUtilities.ReSetToBeginShader();

            Texture2D Mowa = LAPTextureRegister.Mowa.Value;
            Texture2D Pray = LAPTextureRegister.Pray.Value;
            LAPUtilities.Draw(Mowa, drawPosition, null, Color.White * EffectOpacity, 0, Mowa.Size() / 2, 0.8f * EffectScale, 0);
            LAPUtilities.Draw(Pray, drawPosition, null, Color.White * EffectOpacity, MathHelper.PiOver4, Pray.Size() / 2, 1.1f * EffectScale, 0);

            LAPUtilities.ReSetToEndShader();

            DeepGlow.SubmitCustomGlow(() =>
            {
                LAPUtilities.ReSetToBeginShader(BlendState.Additive);

                LAPUtilities.Draw(Mowa, drawPosition, null, Color.White * EffectOpacity * 0.5f, 0, Mowa.Size() / 2, 0.8f * EffectScale, 0);
                LAPUtilities.Draw(Pray, drawPosition, null, Color.White * EffectOpacity * 0.5f, MathHelper.PiOver4, Pray.Size() / 2, 1.1f * EffectScale, 0);

                LAPUtilities.FastApplyEdgeMeltsShader(1 - Projectile.Opacity, texture.Size(), Color.White);
                LAPUtilities.SetTexture(LAPTextureRegister.Noise.Value, SamplerState.PointClamp, 1);
                Main.spriteBatch.Draw(texture, drawPosition, null, Color.Transparent, drawRotation, rotationPoint, Projectile.scale, flipSprite, 0);

                LAPUtilities.ReSetToBeginShader();
            });
            return false;
        }
    }
}
