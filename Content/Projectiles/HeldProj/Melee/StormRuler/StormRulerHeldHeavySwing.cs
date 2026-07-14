using LAP.Assets.Effects;
using LAP.Assets.Sounds;
using LAP.Assets.TextureRegister;
using LAP.Content.Configs;
using LAP.Content.Particles;
using LAP.Core.AnimationHandle;
using LAP.Core.Enums;
using LAP.Core.Graphics.PixelatedRender;
using LAP.Core.Graphics.Primitives.Trail;
using LAP.Core.Keybind;
using LAP.Core.LAPSource;
using LAP.Core.SpecificEffectManagers;
using LAP.Core.SystemsLoader;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.Localization;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Common.Blance;
using UCA.Content.Items.Weapons.Melee.GreatSword;
using UCA.Content.Projectiles.Melee.NormalProj;

namespace UCA.Content.Projectiles.HeldProj.Melee.StormRuler
{
    public class StormRulerHeldHeavySwing : ModProjectile, ILocalizedModType, IPixelatedRenderer
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
        public float FatherProjRotOffset => MathHelper.ToRadians(225f);
        public int UseTime => Owner.ApplyWeaponAttackSpeed(Owner.HeldItem, Owner.HeldItem.useTime * 15, 250);
        public int BeginDir;
        public AniHelper AniHelper = new AniHelper(3);
        public List<Vector2> OldAimPos = [];
        public Vector2 BeginMouseWorld;
        public Vector2 HeldPos;
        public float TargetRot;
        public float ProjRotOffset;
        public float SlashOpacity = 1f;
        public int SwordLength = 220;
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
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
                return true;
            if (Projectile.LAP().FirstFrame)
                return false;
            float _ = float.NaN;
            Vector2 beamBeginPos = Owner.Center;
            Vector2 beamEndPos = Projectile.Center + OldAimPos[^1];
            bool c = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), beamBeginPos, beamEndPos, 64f, ref _);
            return c;
        }
        public override void AI()
        {
            Owner.SetUseFocus(2);
            Init();
            UpdateGeneral();
            SetArmRot();
            UpdateAnimation();
        }
        public void Init()
        {
            if (!Projectile.LAP().FirstFrame)
                return;
            HasFocus = Owner.CheckFocus(Owner.ActiveItem().LAP().WeaponSkillRealFocusCost);
            SoundEngine.PlaySound(LAPSoundsMenu.SwingAttack with { Volume = 1f, Pitch = Main.rand.NextFloat(-0.4f, -0.2f), MaxInstances = -1 }, Projectile.Center);
            SoundEngine.PlaySound(LAPSoundsMenu.SPSwing with { Volume = 1f, Pitch = Main.rand.NextFloat(-0.4f, -0.2f), MaxInstances = -1 }, Projectile.Center);
            BeginMouseWorld = Owner.LocalMouseWorld();
            BeginDir = BeginMouseWorld.X > Owner.Center.X ? 1 : -1;
            TargetRot = FatherTargetRot;
            Projectile.rotation = TargetRot;
            HeldPos = new Vector2(FatherHeldPosX, FatherHeldPosY);
            ProjRotOffset = FatherProjRotOffset;
            AniHelper.MaxAniProgress[AniState.Begin] = (int)(UseTime * 0.25f);
            AniHelper.MaxAniProgress[AniState.Middle] = (int)(UseTime * 0.75f);
        }
        #region 常规更新
        public void UpdateGeneral()
        {
            Projectile.SetHeldProj(Owner);
            Owner.ChangeDir(BeginDir);
            Projectile.Center = Owner.GetArmRoot() + HeldPos;
            Projectile.timeLeft = 2;
        }
        public void SetArmRot()
        {
            Vector2 target = Projectile.Center;
            float rot = LAPUtilities.GetVector2(Owner.GetArmRoot(), target).ToRotation();
            Owner.SetArmRot(rot);
        }
        #endregion
        #region 更新动画
        public void UpdateAnimation()
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
            float easedProgress = EasingHelper.EaseInCubic(AniHelper.GetProgress(AniState.Begin));
            float baseRotation = AniHelper.UpDateAngle(155, -150, Owner.direction, easedProgress);
            HeldPos = IdleOffset.RotatedBy(baseRotation).RotatedBy(TargetRot);
            float ProjRotation = AniHelper.UpDateAngle(225f, -145, Owner.direction, easedProgress);
            ProjRotOffset = ProjRotation;

            float easedProgress2 = AniHelper.GetProgress(AniState.Begin);
            float baseSlashRotation = AniHelper.UpDateAngle(155, -125, Owner.direction, easedProgress2);
            Matrix Slashtransform = Matrix.CreateRotationZ(baseSlashRotation) * Matrix.CreateScale(1.2f, 0.8f, 1f);
            Vector2 SlashTargetPos = Vector2.Transform(Vector2.UnitX, Slashtransform) * 1.25f;
            Vector2 TargetPos = SlashTargetPos.RotatedBy(TargetRot) * SwordLength;
            OldAimPos.Add(TargetPos);
            if (Main.rand.NextBool(2))
            {
                Vector2 beginSpawnPos = Owner.Center;
                Vector2 EndSpawnPos = Owner.Center + TargetPos;
                Vector2 SpawnPos = Vector2.Lerp(beginSpawnPos, EndSpawnPos, Main.rand.NextFloat());
                Vector2 firVel = Vector2.UnitX.RotatedBy(baseSlashRotation + TargetRot + MathHelper.PiOver2) * 12 * -Owner.direction;
                Color DrawColor = Color.White;
                new TrailGlowBall(SpawnPos, firVel, DrawColor * 0.5f, Main.rand.Next(45, 65), 0.2f, true).Spawn();
            }
            if (AniHelper.AniProgress[AniState.Begin] == 50)
            {
                if (Projectile.IsLocalPlayer())
                {
                    Vector2 fireVel = TargetRot.ToRotationVector2() * 56;
                    int damage = Projectile.damage;
                    if (HasFocus)
                        damage = (int)(Projectile.damage * UCABlanceRule.StormRulerSkillHeavySwingDamageMult);
                    else
                        damage = (int)(Projectile.damage * UCABlanceRule.StormRulerSkillHeavyNoFocusSwingDamageMult);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center - fireVel * 3.5f, fireVel, ProjectileType<StormShockWaveBig>(), damage, Projectile.knockBack, Projectile.owner);
                }
            }
            for (int i = 0; i < 2; i++)
            {
                Vector2 Pos = Vector2.Lerp(Projectile.Center, Projectile.Center + TargetPos, Main.rand.NextFloat(0.5f, 1f));
                new CampSmoke(Pos, Owner.velocity * Main.rand.NextFloat(0f, 1.5f), Color.White, 45, Main.rand.NextFloat(MathHelper.TwoPi), 0.4f, Main.rand.NextFloat(0.4f, 0.6f)).Spawn();
            }
            Vector2 Pos2 = Vector2.Lerp(Projectile.Center, Projectile.Center + TargetPos, Main.rand.NextFloat(0.35f, 1f));
            new SmallGlowBall(Pos2, Vector2.Zero, Color.White, Main.rand.Next(30, 120), 0.1f, 3f).Spawn();
        }
        public void HandleMiddleAni()
        {
            float easedProgress = EasingHelper.EaseOutCubic(AniHelper.GetProgress(AniState.Middle));
            float baseRotation = AniHelper.UpDateAngle(-150, -160, Owner.direction, easedProgress);
            HeldPos = IdleOffset.RotatedBy(baseRotation).RotatedBy(TargetRot);
            float ProjRotation = AniHelper.UpDateAngle(-145, -155, Owner.direction, easedProgress);
            ProjRotOffset = ProjRotation;

            SlashOpacity = 1f - EasingHelper.EaseInCubic(AniHelper.GetProgress(AniState.Middle));

            float easedProgress2 = EasingHelper.EaseOutCubic(AniHelper.GetProgress(AniState.Middle));
            float baseSlashRotation = AniHelper.UpDateAngle(-125, -155, Owner.direction, easedProgress2);
            Matrix Slashtransform = Matrix.CreateRotationZ(baseSlashRotation) * Matrix.CreateScale(1.2f, 0.8f, 1f);
            Vector2 SlashTargetPos = Vector2.Transform(Vector2.UnitX, Slashtransform) * 1.25f;
            if (Projectile.FinalExtraUpdate())
                OldAimPos.Add(SlashTargetPos.RotatedBy(TargetRot) * SwordLength);
        }
        #endregion
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (HasFocus)
                modifiers.SourceDamage *= UCABlanceRule.StormRulerSkillHeavySwingDamageMult;
            else
                modifiers.SourceDamage *= UCABlanceRule.StormRulerSkillHeavyNoFocusSwingDamageMult;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.LAP().OnceHitEffect)
                ScreenShakeSystem.AddScreenShakes(Projectile.Center, -5 * -Owner.direction, 20, MathHelper.TwoPi, 0.5f, true, 1000);
        }
        public override void OnKill(int timeLeft)
        {
            if (LAPKeybind.WeaponSkillHotKey.Current)
            {
                Item item = ItemLoader.GetItem(ItemType<StormRulerAlt>()).Item;
                EntitySource_ItemUse_WeaponSkill source = new(Owner, item);
                if (!Owner.HasProj<StormRulerHeldSkillProj>())
                    Projectile.NewProjectile(source, Projectile.Center, Projectile.velocity, ProjectileType<StormRulerHeldSkillProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            PixelatedRenderManger.BeginDrawProj = true;
            DrawBlade(lightColor);
            return false;
        }
        public void DrawBlade(Color lightColor)
        {
            if (Projectile.LAP().FirstFrame)
                return;
            Projectile.GetProjDrawInfo_Melee(out Texture2D texture, out Vector2 _, out float drawRotation, out Vector2 rotationPoint, out SpriteEffects flipSprite);
            Vector2 drawPosition = Projectile.Center - Main.screenPosition + Vector2.UnitX.RotatedBy(Projectile.rotation + ProjRotOffset) * -24;
            Main.spriteBatch.Draw(texture, drawPosition, null, lightColor, drawRotation + ProjRotOffset, rotationPoint, Projectile.scale * 1.5f, flipSprite, 0f);
        }
        public void RenderPixelated(SpriteBatch spriteBatch)
        {
            LAPContent.ReSetToBeginShader_Pixel(BlendState.Additive);

            Texture2D texture = LAPTextureRegister.StandardGradient.Value;
            Effect effect = LAPShaderRegister.AlphaFade.Value;
            effect.Parameters["uFadeoutLeftLength"].SetValue(0.1f);
            effect.Parameters["uFadeinRigtLength"].SetValue(0.1f);
            effect.Parameters["UVMult"].SetValue(new Vector2(1f, 1f));
            effect.CurrentTechnique.Passes[0].Apply();
            DrawSlash(texture, Color.White * 0.5f, 0.95f);
            DrawSlash(texture, Color.White * 0.6f, 0.7f);
            DrawSlash(texture, Color.White * 0.4f, 0.3f);
            DrawSlash(texture, Color.White * 0.3f, 0f);
            if (!LAPConfig.Instance.PerformanceMode)
            {
                Texture2D texture2 = LAPTextureRegister.Aura_01.Value;
                Effect effect2 = LAPShaderRegister.AlphaFade_Noise_OColor.Value;
                effect2.Parameters["uFadeoutLeftLength"].SetValue(0.1f);
                effect2.Parameters["uFadeinRigtLength"].SetValue(0.1f);
                effect2.Parameters["UVOffset"].SetValue(new Vector2(Main.GlobalTimeWrappedHourly * 0.3f, 0));
                effect2.Parameters["UVMult"].SetValue(new Vector2(3f, 3f));
                effect2.Parameters["OverlayColor"].SetValue(Color.White.ToVector4());
                effect2.CurrentTechnique.Passes[0].Apply();
                DrawSlash(texture2, Color.White, 0.4f);
                texture2 = UCATextureRegister.Slash.Value;
                DrawSlash(texture2, Color.White, 0.4f);
            }
            DrawSlash2(texture, Color.White * 0.5f * 0.5f, 0.95f);
            LAPContent.ReSetToEndShader_Pixel();
        }
        public void DrawSlash(Texture2D texture, Color drawcolor, float mult = 0.8f)
        {
            if (OldAimPos.Count < 3)
                return;
            List<VertexPositionColorTexture2D> Vertexlist = new List<VertexPositionColorTexture2D>();
            for (int i = 0; i < OldAimPos.Count; i++)
            {
                float progress = (float)i / OldAimPos.Count;
                Vector2 DrawPos_Head = OldAimPos[i] + Owner.Center - Main.screenPosition;
                Vector2 DrawPos_Source = OldAimPos[i] * mult + Owner.Center - Main.screenPosition;
                Vertexlist.Add(new VertexPositionColorTexture2D(DrawPos_Head, drawcolor * SlashOpacity, new Vector3(progress, 0, 0)));
                Vertexlist.Add(new VertexPositionColorTexture2D(DrawPos_Source, drawcolor * SlashOpacity, new Vector3(progress, 1, 0)));
            }
            Main.graphics.GraphicsDevice.Textures[0] = texture;
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, Vertexlist.ToArray(), 0, Vertexlist.Count - 2);
        }
        public void DrawSlash2(Texture2D texture, Color drawcolor, float mult = 0.8f)
        {
            if (OldAimPos.Count < 3)
                return;
            List<VertexPositionColorTexture2D> Vertexlist = new List<VertexPositionColorTexture2D>();
            for (int i = 0; i < OldAimPos.Count; i++)
            {
                float progress = (float)i / OldAimPos.Count;
                Vector2 DrawPos_Head = OldAimPos[i] * 0.5f + Owner.Center - Main.screenPosition;
                Vector2 DrawPos_Source = OldAimPos[i] * mult * 0.5f + Owner.Center - Main.screenPosition;
                Vertexlist.Add(new VertexPositionColorTexture2D(DrawPos_Head, drawcolor * SlashOpacity, new Vector3(progress, 0, 0)));
                Vertexlist.Add(new VertexPositionColorTexture2D(DrawPos_Source, drawcolor * SlashOpacity, new Vector3(progress, 1, 0)));
            }
            Main.graphics.GraphicsDevice.Textures[0] = texture;
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, Vertexlist.ToArray(), 0, Vertexlist.Count - 2);
        }
    }
}
