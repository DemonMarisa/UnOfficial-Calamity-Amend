using LAP.Assets.Sounds;
using LAP.Assets.TextureRegister;
using LAP.Core.AnimationHandle;
using LAP.Core.Graphics.DeepGlow;
using LAP.Core.NetCode.NetUtilities;
using LAP.Core.StateMachine.SynedHitEffect;
using LAP.Core.SystemsLoader;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using UCA.Content.HitEffect;
using UCA.Content.Items.Weapons.Magic.Ray;
using UCA.Content.Projectiles.Magic.Ray;

namespace UCA.Content.Projectiles.HeldProj.Magic.VividClarityHeld
{
    public class VividWeakParryProj : ModProjectile
    {
        public override string Texture => LAPTextureRegister.InvisibleTexturePath;
        public override LocalizedText DisplayName => LAPUtilities.GetItemName<VividClarityAlt>();
        public Player Owner => Main.player[Projectile.owner];

        public Rectangle[] ParryHitBox = new Rectangle[10];
        public float EffectScale;
        public float EffectOpacity;

        public int EffectTimer;
        public int MaxParryTimer = 8;
        public bool HasParry;
        public bool PlayEffect;
        public Vector2 parryKnockBack;
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
            Projectile.Opacity = 1f;
            Projectile.timeLeft = 30;
        }
        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }
        public override void AI()
        {
            Owner.SetUseFocus(2);
            Projectile.Center = Owner.MountedCenter;
            UpdataInPut();
            Init();
            UpdateEffect();
            CheckColliding();
            ParryProtect();
            if (PlayEffect && Projectile.IsLocalPlayer())
            {
                Owner.NCHeal(Owner.statLifeMax2 / 10);
                Owner.SetImmuneTimeForAllTypes(60);
                HitEffectManager.SpawnHitEffect(HitEffectManager.HEType<VividClarityWeakParryHit>(), Projectile.owner, Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero);
                Owner.velocity += parryKnockBack * 6f;
                if (Owner.velocity.Y == 0 && Math.Abs(Owner.velocity.X) > 1f)
                    Owner.velocity.Y -= 2.5f;
                float rotAngle = MathHelper.TwoPi / 6f;
                float BeginOffset = Main.rand.NextFloat() * MathHelper.TwoPi;
                for (int i = 0; i < 6; i++)
                {
                    Vector2 vel = Vector2.UnitX.RotatedBy(rotAngle * i + BeginOffset) * 9f;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, vel, ProjectileType<ExoEnergy>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
                PlayEffect = false;
            }
        }
        public void Init()
        {
            if (Projectile.LAP().FirstFrame)
            {
                Projectile.velocity = Vector2.Zero;
                SoundEngine.PlaySound(LAPSoundsMenu.MagicTrigger02 with { Volume = 1f }, Projectile.Center);
                Projectile.Center = Owner.MountedCenter;
            }
        }
        public void UpdateEffect()
        {
            if (EffectTimer < MaxParryTimer)
                EffectTimer++;
            float progress = EffectTimer / (float)MaxParryTimer;
            EffectScale = EasingHelper.EaseOutCubic(progress);
            EffectOpacity = MathHelper.Lerp(1f, 0f, EasingHelper.EaseInCubic(progress));
        }
        public void CheckColliding()
        {
            if (!Projectile.IsLocalPlayer() || HasParry || EffectTimer >= MaxParryTimer)
                return;
            ParryHitBox = LAPUtilities.AABBCircularHitboxes(Projectile.Center, 60, 4);
            // 检查射弹
            foreach (Projectile proj in Main.ActiveProjectiles)
            {
                if (!proj.hostile || !proj.active || proj.damage < 5 || proj.LAP().BeParry)
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
                        PlayEffect = true;
                        if (ProjectileID.Sets.DrawScreenCheckFluff[proj.type] < 500 && proj.velocity != Vector2.Zero)
                        {
                            Vector2 shieldNormal = LAPUtilities.GetVector2(Projectile.Center, proj.Center);
                            proj.velocity = proj.velocity - 2f * Vector2.Dot(proj.velocity, shieldNormal) * shieldNormal;
                        }
                        parryKnockBack = LAPUtilities.GetVector2(proj.Center, Owner.Center);
                        proj.LAP().BeParry = true;
                        proj.damage = 0;
                        proj.netSpam = 0;
                        proj.netUpdate = true;
                        proj.SyncedParryProj();
                        HasParry = true;
                        return;
                    }
                }
            }
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npc.friendly || !npc.active)
                    continue;
                if (HasParry)
                    return;
                bool CanHitPlayer = true;
                if (npc.ModNPC is not null)
                {
                    int type = 0;
                    CanHitPlayer = npc.ModNPC.CanHitPlayer(Main.LocalPlayer, ref type);
                }
                if (CanHitPlayer)
                {
                    if (npc.Hitbox.Intersects(ParryHitBox[0]) || npc.Hitbox.Intersects(ParryHitBox[1]) || npc.Hitbox.Intersects(ParryHitBox[2]) || npc.Hitbox.Intersects(ParryHitBox[3]))
                    {
                        parryKnockBack = LAPUtilities.GetVector2(npc.Center, Owner.Center);
                        PlayEffect = true;
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
                if (Main.mouseRight && Main.mouseRightRelease && EffectTimer > 2)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, Type, Projectile.damage, Projectile.knockBack, Projectile.owner);
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
