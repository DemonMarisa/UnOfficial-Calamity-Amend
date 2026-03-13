using LAP.Content.Configs;
using LAP.Core.AnimationHandle;
using LAP.Core.Enums;
using LAP.Core.Graphics.Primitives.Trail;
using LAP.Core.SpecificEffectManagers;
using LAP.Core.SystemsLoader;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Assets.Effects;
using UCA.Assets.Sounds;
using UCA.Content.Items.Weapons.Magic.Ray;
using UCA.Content.Particiles;
using UCA.Content.Paths;
using UCA.Content.Projectiles.Misc;
using UCA.Core.Utilities;

namespace UCA.Content.Projectiles.HeldProj.Magic.SoulPiercerHeld
{
    public class SoulPiercerSkillHeldProj : ModProjectile, ILocalizedModType
    {
        public override LocalizedText DisplayName => LAPUtilities.GetItemName<SoulPiercerAlt>();
        public override string Texture => $"{ProjPath.HeldProjPath}" + "Magic/SoulPiercerHeld/SoulPiercerHeldProj";
        public Player Owner => Main.player[Projectile.owner];
        public bool CanHit;
        public bool CanUpdateAngle = true;
        public int SwordLength = 900;
        public float TargetRot;
        public AnimationHelper animationHelper = new AnimationHelper(3);
        public List<Vector2> OldAimPos = [];
        public List<float> OldRot = [];
        public List<float> OldScale = [];
        public bool BeginRemovePos;
        public float Opacity;
        public float XScale = 0.5f;
        public int Time = 0;
        public int HitCount;
        public Vector2 SourceOffset => new Vector2(0, 75 * Owner.direction);
        public override void SetStaticDefaults()
        {
            // 保存旧朝向与旧位置
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 120;
            Projectile.AddToSkillProj();
        }
        public override void SetDefaults()
        {
            Projectile.width = 5;
            Projectile.height = 5;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 45;
            Projectile.netImportant = true;
        }
        public override bool? CanHitNPC(NPC target)
        {
            if (CanHit)
                return null;
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float _ = float.NaN;
            bool c = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * SwordLength * Projectile.scale * 0.9f, 128f, ref _);
            return c;
        }
        public override void AI()
        {
            Owner.SetUseFocus(2);
            Time++;
            if (Projectile.LAP().FirstFrame)
            {
                SoundEngine.PlaySound(SoundsMenu.MAGNOLIASPRelease, Projectile.Center);
                SoundEngine.PlaySound(SoundsMenu.MagicStaffCharge, Projectile.Center);
                animationHelper.MaxAniProgress[AnimationState.Begin] = 30; 
                animationHelper.MaxAniProgress[AnimationState.Middle] = 50;
                animationHelper.MaxAniProgress[AnimationState.End] = 300;
                TargetRot = Owner.GetPlayerToMouseVector2().ToRotation();
            }
            Projectile.SetHeldProj(Owner, false, CanUpdateAngle);
            if (CanUpdateAngle)
                TargetRot = Utils.AngleLerp(TargetRot, Owner.GetPlayerToMouseVector2().ToRotation(), 0.2f);
            HandleAni();
            Projectile.velocity = Projectile.rotation.ToRotationVector2();
            Projectile.Center = Owner.Center;
            Projectile.spriteDirection = Owner.direction;
            float baseRotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, baseRotation);
            Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, baseRotation);
        }
        #region 对动画的处理
        public void HandleAni()
        {
            if (!animationHelper.HasFinish[AnimationState.Begin]) {
                HandleBeginAni();
                animationHelper.UpDateAni(AnimationState.Begin, 30);
            }
            else if (!animationHelper.HasFinish[AnimationState.Middle]) {
                HandleMiddleAni();
                animationHelper.UpDateAni(AnimationState.Middle);
            }
            else if (!animationHelper.HasFinish[AnimationState.End]) {
                HandleEndAni();
                animationHelper.UpDateAni(AnimationState.End);
            }
            else Projectile.Kill();
        }
        public void HandleBeginAni()
        {
            float easedProgress = EasingHelper.EaseOutCubic(animationHelper.GetProgress(AnimationState.Begin));
            float baseRotation = animationHelper.UpDateAngle(-45, -145, Owner.direction, easedProgress);
            Opacity = easedProgress;
            // 确定椭圆的点
            Vector2 TargetPos = new Vector2(SwordLength, 0).BetterRotatedBy(baseRotation, SourceOffset, 1, XScale);
            Projectile.scale = TargetPos.Distance(Vector2.Zero) / (float)SwordLength;
            Projectile.rotation = TargetPos.ToRotation() + TargetRot;
            if (Time % 4 == 0)
            {
                Vector2 RealAimPoint = TargetPos.RotatedBy(TargetRot);
                Vector2 beginSpawnPos = Projectile.Center + new Vector2(64, 0).RotatedBy(Projectile.rotation);
                Vector2 EndSpawnPos = Projectile.Center + RealAimPoint;
                Color DrawColor = Color.Lerp(Color.DarkViolet, Color.SkyBlue, Main.rand.NextFloat());
                new TLineOF(Vector2.Lerp(beginSpawnPos, EndSpawnPos, Main.rand.NextFloat()), Main.rand.NextFloat(2f, 4f), DrawColor, Main.rand.Next(60, 120), 0.15f, Main.rand.NextFloat(MathHelper.TwoPi)).Spawn();
                Color TGBColor = Color.Lerp(Color.Violet, Color.SkyBlue, Main.rand.NextFloat());
                new TrailGlowBall_T(Vector2.Lerp(beginSpawnPos, EndSpawnPos, Main.rand.NextFloat() - 0.1f), TGBColor, Main.rand.Next(45, 90), 0.15f, Main.rand.NextFloat(1f, 2f), Main.rand.NextFloat(MathHelper.TwoPi), 1f).Spawn();
            }
            if (easedProgress < 0.8f)
            {
                Vector2 Balloffset = new Vector2(76, 0);
                float beginrot = Main.rand.NextFloat(0, MathHelper.TwoPi);
                float rotSpeed = Main.rand.NextBool() ? 0.03f : -0.03f;
                int length = Main.rand.Next(250, 800);
                int LifeTime = 60;
                new ProjAbsorbGlowBall(Projectile.Center, Color.Violet, LifeTime, 0.1f, beginrot, rotSpeed, Projectile.whoAmI, length, Balloffset).Spawn();
                float beginrot2 = Main.rand.NextFloat(0, MathHelper.TwoPi);
                float rotSpeed2 = Main.rand.NextBool() ? 0.07f : -0.07f;
                int length2 = Main.rand.Next(125, 350);
                new AbsorbFire(Projectile.Center, Color.Violet, 60, 0.4f, beginrot2, rotSpeed2, Projectile.whoAmI, length2, Balloffset).Spawn();
            }
            Owner.UCA().SoulPiercerSGSUse = 2;
        }
        public void HandleMiddleAni()
        {
            CanHit = true;
            CanUpdateAngle = false;
            Projectile.extraUpdates = 10;
            float easedProgress = animationHelper.GetProgress(AnimationState.Middle);
            if (easedProgress == 0)
                SoundEngine.PlaySound(SoundsMenu.SoulGreatSwordSwimg with { Volume = 0.6f, Pitch = 0f });
            float baseRotation = animationHelper.UpDateAngle(-145, 125, Owner.direction, easedProgress);
            // 确定椭圆的点
            Vector2 TargetPos = new Vector2(SwordLength, 0).BetterRotatedBy(baseRotation, SourceOffset, 1, XScale);
            Projectile.scale = TargetPos.Distance(Vector2.Zero) / (float)SwordLength;
            Projectile.rotation = TargetPos.ToRotation() + TargetRot;
            Vector2 RealAimPoint = TargetPos.RotatedBy(TargetRot);
            OldAimPos.Add(RealAimPoint);
            OldRot.Add(Projectile.rotation);
            OldScale.Add(Projectile.scale);
            if (Time % 2 == 0)
            {
                float SpawRate = 2f;
                if (LAPConfig.Instance.PerformanceMode)
                    SpawRate = 1f;
                for (int i = 0; i < SpawRate; i++)
                {
                    Vector2 beginSpawnPos = Projectile.Center + new Vector2(64, 0).RotatedBy(Projectile.rotation);
                    Vector2 EndSpawnPos = Projectile.Center + RealAimPoint;
                    Color DrawColor;
                    DrawColor = Color.Lerp(Color.Violet, Color.DarkViolet, Main.rand.NextFloat());
                    new TrailGlowBall_T(Vector2.Lerp(beginSpawnPos, EndSpawnPos, Main.rand.NextFloat()), DrawColor, Main.rand.Next(45, 90), 0.15f, Main.rand.NextFloat(2f, 4f), Main.rand.NextFloat(MathHelper.TwoPi), 1f).Spawn();
                }
                for (int i = 0; i < 1; i++)
                {
                    Vector2 beginSpawnPos = Projectile.Center + new Vector2(64, 0).RotatedBy(Projectile.rotation);
                    Vector2 EndSpawnPos = Projectile.Center + RealAimPoint;
                    Color DrawColor;
                    DrawColor = Color.Lerp(Color.SkyBlue, Color.Violet, Main.rand.NextFloat());
                    new TLineOF(Vector2.Lerp(beginSpawnPos, EndSpawnPos, Main.rand.NextFloat()), Main.rand.NextFloat(2f, 4f), DrawColor, Main.rand.Next(60, 120), 0.15f, Main.rand.NextFloat(MathHelper.TwoPi)).Spawn();
                }
            }
            Owner.UCA().SoulPiercerSGSUse = 2;
        }
        public void HandleEndAni()
        {
            CanHit = false;
            float easedProgress = EasingHelper.EaseOutCubic(animationHelper.GetProgress(AnimationState.End));
            Opacity = 1 - easedProgress;
            float baseRotation = animationHelper.UpDateAngle(125, 145, Owner.direction, easedProgress);
            // 确定椭圆的点
            Vector2 TargetPos = new Vector2(SwordLength, 0).BetterRotatedBy(baseRotation, SourceOffset, 1, XScale);
            Projectile.scale = TargetPos.Distance(Vector2.Zero) / (float)SwordLength;
            Projectile.rotation = TargetPos.ToRotation() + TargetRot;
            if (LAPUtilities.FinalExtraUpdate(Projectile))
            {
                Vector2 RealAimPoint = TargetPos.RotatedBy(TargetRot);
                OldAimPos.Add(RealAimPoint);
                OldRot.Add(Projectile.rotation);
                OldScale.Add(Projectile.scale);
                OldAimPos.RemoveAt(0);
                OldRot.RemoveAt(0);
                OldScale.RemoveAt(0);
            }
        }
        #endregion
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.LAP().OnceHitEffect)
            {
                ScreenShakeSystem.AddScreenShakes(Projectile.Center, 25, 15, 0);
                Owner.SetImmuneTimeForAllTypes(40);
            }
            if (HitCount < 5)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<UseForOnHitNPCProj>(), 0, 0, Projectile.owner, Type);
            HitCount++;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            float drawRotation = Projectile.rotation + (Projectile.spriteDirection == -1 ? MathHelper.PiOver2 + MathHelper.PiOver4 : MathHelper.PiOver4);
            Vector2 rotationPoint = texture.Size() / 2f;
            SpriteEffects flipSprite = Projectile.spriteDirection * Main.player[Projectile.owner].gravDir == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            LAPUtilities.ReSetToBeginShader();

            // 绘制基础的灵魂大剑
            Texture2D Sword = UCATextureRegister.SoulGreatSword.Value;
            Vector2 SwordPoint = new Vector2(Sword.Width / 2, Sword.Height - 100);
            float SwordRot = Projectile.rotation + MathHelper.PiOver2;
            Main.spriteBatch.Draw(Sword, drawPosition, null, Color.White * 1f * Opacity, SwordRot, SwordPoint, Projectile.scale * Main.player[Projectile.owner].gravDir, flipSprite, 0f);
            
            // 绘制辉光和流光
            Texture2D Ball = UCATextureRegister.FusableBall.Value;
            float Ballrot = Projectile.rotation;
            Vector2 BallrotPoint = new Vector2(0, Ball.Height / 2);
            Vector2 DrawOffset = new Vector2(-60, 0).RotatedBy(Projectile.rotation);
            Main.spriteBatch.Draw(Ball, drawPosition + DrawOffset, null, Color.White * 1f * Opacity, Ballrot, BallrotPoint, Projectile.scale * Main.player[Projectile.owner].gravDir * new Vector2(1.3f, 0.25f), flipSprite, 0f);
            Vector2 BallDrawOffset2 = new Vector2(75, 0).RotatedBy(Projectile.rotation);
            Vector2 BallrotPoint2 = Ball.Size() / 2;
            Main.spriteBatch.Draw(Ball, drawPosition + BallDrawOffset2, null, Color.White * 1f * Opacity, Ballrot + MathHelper.PiOver2, BallrotPoint2, Projectile.scale * Main.player[Projectile.owner].gravDir * new Vector2(1.25f, 1f) * 0.25f, flipSprite, 0f);
            // 画两次流光
            Effect shader = UCAShaderRegister.SoulGreatSwordFlowShader.Value;
            shader.Parameters["UVOffset"].SetValue(new Vector2(-Main.GlobalTimeWrappedHourly * Owner.direction, 0));
            shader.Parameters["NoiseTextureScale"].SetValue(new Vector2(2f, 1f));
            shader.CurrentTechnique.Passes[0].Apply();
            LAPUtilities.SetTexture(UCATextureRegister.Aura_01.Value, SamplerState.LinearWrap, 1);
            Vector2 DrawOffset2 = new Vector2(-90, 0).RotatedBy(Projectile.rotation);
            Main.spriteBatch.Draw(Ball, drawPosition + DrawOffset2, null, Color.White * 1f * Opacity, Ballrot, BallrotPoint, Projectile.scale * Main.player[Projectile.owner].gravDir * new Vector2(1.8f, 0.4f), flipSprite, 0f);
            shader.Parameters["NoiseTextureScale"].SetValue(new Vector2(1.5f, 0.5f));
            Main.spriteBatch.Draw(Ball, drawPosition + DrawOffset2, null, Color.White * 1f * Opacity, Ballrot, BallrotPoint, Projectile.scale * Main.player[Projectile.owner].gravDir * new Vector2(1.4f, 0.2f), flipSprite, 0f);
           
            shader.Parameters["UVOffset"].SetValue(new Vector2(Main.GlobalTimeWrappedHourly * Owner.direction, 0));
            shader.Parameters["NoiseTextureScale"].SetValue(new Vector2(2f, 1f));
            shader.CurrentTechnique.Passes[0].Apply();

            Vector2 FlowDrawOffset2 = new Vector2(-60, 45 * Projectile.spriteDirection).RotatedBy(Projectile.rotation);
            SpriteEffects flipSprite2 = Projectile.spriteDirection * Main.player[Projectile.owner].gravDir == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.spriteBatch.Draw(Ball, drawPosition + FlowDrawOffset2, null, Color.Violet * 1f * Opacity, Ballrot, BallrotPoint, Projectile.scale * Main.player[Projectile.owner].gravDir * new Vector2(1.8f, 0.4f) * 0.6f, flipSprite2, 0f);
            Main.spriteBatch.Draw(Ball, drawPosition + FlowDrawOffset2, null, Color.Purple * 1f * Opacity, Ballrot, BallrotPoint, Projectile.scale * Main.player[Projectile.owner].gravDir * new Vector2(1.4f, 0.2f) * 0.6f, flipSprite2, 0f);
            
            if (Projectile.spriteDirection == 1)
            {
                shader.Parameters["UVOffset"].SetValue(new Vector2(-Main.GlobalTimeWrappedHourly * Owner.direction, 0));
                shader.Parameters["NoiseTextureScale"].SetValue(new Vector2(2f, 1f));
                shader.CurrentTechnique.Passes[0].Apply();
            }

            FlowDrawOffset2 = new Vector2(-60, -45 * Projectile.spriteDirection).RotatedBy(Projectile.rotation);
            SpriteEffects flipSprite3 = Projectile.spriteDirection * Main.player[Projectile.owner].gravDir == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None;
            Main.spriteBatch.Draw(Ball, drawPosition + FlowDrawOffset2, null, Color.Violet * 1f * Opacity, Ballrot, BallrotPoint, Projectile.scale * Main.player[Projectile.owner].gravDir * new Vector2(1.8f, 0.4f) * 0.6f, flipSprite3, 0f);
            Main.spriteBatch.Draw(Ball, drawPosition + FlowDrawOffset2, null, Color.Purple * 1f * Opacity, Ballrot, BallrotPoint, Projectile.scale * Main.player[Projectile.owner].gravDir * new Vector2(1.4f, 0.2f) * 0.6f, flipSprite3, 0f);

            if (OldAimPos.Count > 2)
            {
                // 绘制拖尾
                Vector2 UVOffset = new Vector2(Main.GlobalTimeWrappedHourly * 0.1f, 0);
                Vector2 TextureScale = new Vector2(0.75f, 1f);
                LAPUtilities.ApplyTrailShader(UVOffset, TextureScale, 1 - Opacity, TextureScale, Vector2.Zero, true);
                LAPUtilities.SetTexture(UCATextureRegister.Slash2.Value, SamplerState.LinearWrap, 0);
                LAPUtilities.SetTexture(UCATextureRegister.HarshNoise.Value, SamplerState.LinearWrap, 1);
                LAPUtilities.SetTexture(UCATextureRegister.HarshNoise.Value, SamplerState.LinearWrap, 2);
                DrawTrail(0.9f, 0.2f);
                DrawTrail(0.9f, 0.2f);
                LAPUtilities.SetTexture(UCATextureRegister.Slash.Value, SamplerState.LinearWrap, 0);
                LAPUtilities.SetTexture(UCATextureRegister.HarshNoise.Value, SamplerState.LinearWrap, 1);
                LAPUtilities.SetTexture(UCATextureRegister.HarshNoise.Value, SamplerState.LinearWrap, 2);
                DrawTrail(0.9f, 0.2f);
                DrawTrail(0.9f, 0.2f);
            }
            LAPUtilities.ReSetToEndShader();

            Main.spriteBatch.Draw(texture, drawPosition, null, Color.White, drawRotation, rotationPoint, Projectile.scale * Main.player[Projectile.owner].gravDir, flipSprite, 0f);

            return false;
        }
        public void DrawTrail(float BeginScale , float EndScale)
        {
            List<VertexPositionColorTexture2D> Vertexlist = new List<VertexPositionColorTexture2D>();
            for (int i = 0; i < OldAimPos.Count; i++)
            {
                float progress = (float)i / OldAimPos.Count;
                Vector2 DrawPos_Head = OldAimPos[i] * BeginScale + Projectile.Center - Main.screenPosition;
                Vector2 DrawPos_Source = OldAimPos[i] * EndScale + Projectile.Center - Main.screenPosition;
                Vertexlist.Add(new VertexPositionColorTexture2D(DrawPos_Head, Color.DarkViolet, new Vector3(progress, 0, 0)));
                Vertexlist.Add(new VertexPositionColorTexture2D(DrawPos_Source, Color.LightPink * Opacity, new Vector3(progress, 1, 0)));
            }
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, Vertexlist.ToArray(), 0, Vertexlist.Count - 2);
        }
    }
}
