using CalamityMod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Assets.Effects;
using UCA.Assets.Sounds;
using UCA.Content.Particiles;
using UCA.Content.Projectiles.Magic.Ray;
using UCA.Content.UCACooldowns;
using UCA.Core.Enums;
using LAP.Core.SpecificEffectManagers;
using UCA.Core.Utilities;
using static System.Net.Mime.MediaTypeNames;
using LAP.Core.Utilities;

namespace UCA.Content.Projectiles.HeldProj.Magic.ElementRayHeld
{
    public partial class ElementRaySpecialHeldProj
    {
        public int SoulStreamIndex = 0;
        public Vector2 Balloffset = new Vector2(60, 0);
        public Vector2 PostRotBalloffset => new Vector2(60, 0).RotatedBy(Projectile.rotation);
        public Projectile Child => Main.projectile[SoulStreamIndex];
        public float BallScale = 1f;
        public void InitializeStarDustStream()
        {
            MainFragmentOffset = new Vector2(0, 0);
            AuxFragmentOffset = new Vector2(0, -0);
            FilpAuxFragmentOffset = new Vector2(0, 0);
            SolarBladeXOffset = 128;

            animationHelper.MaxAniProgress[AnimationState.Begin] = 120;
            animationHelper.MaxAniProgress[AnimationState.Middle] = 5;
            animationHelper.MaxAniProgress[AnimationState.End] = 120;
            SoundEngine.PlaySound(SoundsMenu.MAGNOLIASPRelease);
            SoundEngine.PlaySound(SoundsMenu.SoulStreamCharge);
        }

        public void UpdateStarDustStream()
        {
            BeginRot = ToMouseVector;
            RelativeOwnerPos = new Vector2(0, 6 * Owner.direction);

            if (!animationHelper.HasFinish[AnimationState.Begin])
            {
                animationHelper.UpDateAni(AnimationState.Begin, 40);
                HandleStarDustBeginAni();
                if (animationHelper.Auxfloat[AnimationState.Begin] > 0)
                {
                    BallScale = MathHelper.Lerp(BallScale, 0f, 0.02f);
                }

                if (animationHelper.Auxfloat[AnimationState.Begin] == 15)
                {
                }
            }
            else if (!animationHelper.HasFinish[AnimationState.Middle])
            {
                animationHelper.UpDateAni(AnimationState.Middle, 0);
                HandleStarDustMiddleAni();
            }
            else if (!animationHelper.HasFinish[AnimationState.End])
            {
                UseSlowRot = true;
                animationHelper.UpDateAni(AnimationState.End, 0);
                HandleStarDustEndAni();
            }
            else
            {
                Projectile.Kill();
            }
        }
        #region 处理开始动画
        public void HandleStarDustBeginAni()
        {
            int MaxAni = animationHelper.MaxAniProgress[AnimationState.Begin];
            int CurAni = animationHelper.AniProgress[AnimationState.Begin];
            float easedProgress = EasingHelper.EaseOutCubic(CurAni / (float)MaxAni);
            float baseRotation = animationHelper.UpDateAngle(0, 45, Owner.direction, easedProgress, 0);
            RelativeOwnerPosRot = baseRotation + ToMouseVector;
            Projectile.rotation = RelativeOwnerPosRot;
            Projectile.Center += new Vector2(baseRotation, 0).RotatedBy(BeginRot) * -25 * Owner.direction;
            
            float beginrot = Main.rand.NextFloat(0, MathHelper.TwoPi);
            float rotSpeed = Main.rand.NextBool() ? 0.03f : -0.03f;
            int length = Main.rand.Next(250, 500);
            int LifeTime = 60;
            new ProjAbsorbGlowBall(Projectile.Center, Color.SkyBlue, LifeTime, 0.1f, beginrot, rotSpeed, Projectile.whoAmI, length, Balloffset).Spawn();
            float beginrot2 = Main.rand.NextFloat(0, MathHelper.TwoPi);
            float rotSpeed2 = Main.rand.NextBool() ? 0.07f : -0.07f;
            int length2 = Main.rand.Next(125, 175);
            new AbsorbFire(Projectile.Center, Color.SkyBlue, 60, 0.4f, beginrot2, rotSpeed2, Projectile.whoAmI, length2, Balloffset).Spawn();
        }
        #endregion
        #region 处理中间动画
        public void HandleStarDustMiddleAni()
        {
            int MaxAni = animationHelper.MaxAniProgress[AnimationState.Middle];
            int CurAni = animationHelper.AniProgress[AnimationState.Middle];
            float easedProgress = EasingHelper.EaseInCubic(CurAni / (float)MaxAni);
            float baseRotation = animationHelper.UpDateAngle(45, 0, Owner.direction, easedProgress, 0);
            RelativeOwnerPosRot = baseRotation + ToMouseVector;
            Projectile.rotation = RelativeOwnerPosRot;
            Projectile.Center += new Vector2(baseRotation, 0).RotatedBy(BeginRot) * -25 * Owner.direction;
            if (CurAni == MaxAni)
            {
                SoundEngine.PlaySound(SoundsMenu.SoulStreamFire);
                Vector2 offset = new Vector2(64, 0).RotatedBy(Projectile.rotation);
                for (int i = 0; i < 100; i++)
                {
                    Color RandomColor = LAPUtilities.LerpColor(Color.SkyBlue, Color.DeepSkyBlue);
                    new MediumGlowBall(Projectile.Center + offset + Owner.velocity * 6, RandomColor, 120, 0.4f, Main.rand.NextFloat(4f, 12f)).Spawn();
                }
                ScreenShakeSystem.AddScreenShakes(Projectile.Center, 250, 180, Projectile.rotation, 0.1f, true, 1000);
                SoulStreamIndex = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + offset, Vector2.Zero, ModContent.ProjectileType<SoulStream>(), Projectile.damage * 6, Projectile.knockBack, Projectile.owner, Projectile.whoAmI);
                Owner.AddCooldown(StarDustBoost.ID,  600);
            }
        }
        #endregion
        #region 处理结束动画
        public void HandleStarDustEndAni()
        {
            int MaxAni = animationHelper.MaxAniProgress[AnimationState.End];
            int CurAni = animationHelper.AniProgress[AnimationState.End];
            RelativeOwnerPosRot = BeginRot;
            Projectile.rotation = RelativeOwnerPosRot;

            if (CurAni == 1)
            {
            }
        }
        #endregion
        public void DrawChargeBall()
        {
            LAPUtilities.ReSetToBeginShader();
            DrawFlash();
            DrawGlow();
            DrawBallOutLine();
            LAPUtilities.ReSetToEndShader();
        }
        public void DrawGlow()
        {
            Texture2D texture = UCATextureRegister.CrossGlow.Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + PostRotBalloffset, null, Color.SkyBlue, 0, texture.Size() / 2, BallScale * 0.2f * new Vector2(1.5f, 1f), SpriteEffects.FlipHorizontally, 0f);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + PostRotBalloffset, null, Color.DeepSkyBlue, 0, texture.Size() / 2, BallScale * 0.15f * new Vector2(1.5f, 1f), SpriteEffects.None, 0f);
        }
        public void DrawFlash()
        {
            Texture2D texture = UCATextureRegister.Flash_01.Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + PostRotBalloffset, null, Color.SkyBlue, Main.GlobalTimeWrappedHourly, texture.Size() / 2, 0.2f * BallScale, SpriteEffects.FlipHorizontally, 0f);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + PostRotBalloffset, null, Color.DeepSkyBlue, -Main.GlobalTimeWrappedHourly, texture.Size() / 2, 0.2f * BallScale, SpriteEffects.None, 0f);
        }
        public void DrawBallOutLine()
        {
            LAPUtilities.ReSetToBeginShader(BlendState.Additive);
            UCAShaderRegister.PolarDistortShader.Parameters["uWidthMult"].SetValue(3f);
            UCAShaderRegister.PolarDistortShader.Parameters["uRingMult"].SetValue(1f);
            UCAShaderRegister.PolarDistortShader.Parameters["uYTime"].SetValue(-Main.GlobalTimeWrappedHourly);
            Main.instance.GraphicsDevice.Textures[1] = UCATextureRegister.FusableBall.Value;
            UCAShaderRegister.PolarDistortShader.CurrentTechnique.Passes[0].Apply();
            float Scale = 1.5f * BallScale;
            Texture2D texture = UCATextureRegister.Aura_01.Value;
            Vector2 orig = texture.Size() / 2;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + PostRotBalloffset, null, Color.SkyBlue, 0, orig, Scale, SpriteEffects.None, 0);
            Scale *= 0.5f;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + PostRotBalloffset, null, Color.DeepSkyBlue, MathHelper.PiOver2, orig, Scale, SpriteEffects.None, 0);
            LAPUtilities.ReSetToEndShader();
        }
    }
}
