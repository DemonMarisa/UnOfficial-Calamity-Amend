using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using UCA.Assets;
using UCA.Assets.Effects;
using UCA.Assets.Sounds;
using UCA.Content.Particiles;
using UCA.Content.Projectiles.Magic.Ray;
using UCA.Content.UCACooldowns;
using LAP.Core.Enums;
using LAP.Core.SpecificEffectManagers;
using LAP.Core.Utilities;
using LAP.Core.SystemsLoader;
using LAP.Content.Particles;
using LAP.Assets.TextureRegister;
using LAP.Core.Presets.Content;

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

            AniHelper.MaxAniProgress[AniState.Begin] = 120;
            AniHelper.MaxAniProgress[AniState.Middle] = 5;
            AniHelper.MaxAniProgress[AniState.End] = 120;
            SoundEngine.PlaySound(SoundsMenu.MAGNOLIASPRelease, Projectile.Center);
            SoundEngine.PlaySound(SoundsMenu.SoulStreamCharge, Projectile.Center);
        }

        public void UpdateStarDustStream()
        {
            BeginRot = ToMouseVector;
            RelativeOwnerPos = new Vector2(0, 6 * Owner.direction);

            if (!AniHelper.HasFinish[AniState.Begin])
            {
                AniHelper.UpDateAni(AniState.Begin, 40);
                HandleStarDustBeginAni();
                if (AniHelper.BreakTime[AniState.Begin] > 0)
                {
                    BallScale = MathHelper.Lerp(BallScale, 0f, 0.02f);
                }
            }
            else if (!AniHelper.HasFinish[AniState.Middle])
            {
                AniHelper.UpDateAni(AniState.Middle, 0);
                HandleStarDustMiddleAni();
            }
            else if (!AniHelper.HasFinish[AniState.End])
            {
                UseSlowRot = true;
                AniHelper.UpDateAni(AniState.End, 0);
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
            int MaxAni = AniHelper.MaxAniProgress[AniState.Begin];
            int CurAni = AniHelper.AniProgress[AniState.Begin];
            float easedProgress = EasingHelper.EaseOutCubic(CurAni / (float)MaxAni);
            float baseRotation = AniHelper.UpDateAngle(0, 45, Owner.direction, easedProgress, 0);
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
            int MaxAni = AniHelper.MaxAniProgress[AniState.Middle];
            int CurAni = AniHelper.AniProgress[AniState.Middle];
            float easedProgress = EasingHelper.EaseInCubic(CurAni / (float)MaxAni);
            float baseRotation = AniHelper.UpDateAngle(45, 0, Owner.direction, easedProgress, 0);
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
                    ParticlePreset.NewTGlowBall(Projectile.Center + offset + Owner.velocity * 6, Vector2.Zero,RandomColor, 120, 0.4f, Main.rand.NextFloat(4f, 12f));
                }
                ScreenShakeSystem.AddScreenShakes(Projectile.Center, 250, 180, Projectile.rotation, 0.01f, true, 1000);
                SoulStreamIndex = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + offset, Vector2.Zero, ProjectileType<SoulStream>(), Projectile.damage * 6, Projectile.knockBack, Projectile.owner, Projectile.whoAmI);
                Main.projectile[SoulStreamIndex].LAP().isWeaponSkillProj = true;
                Owner.AddCD(LAPContent.CDType<StarDustBoost>(),  1200);
            }
        }
        #endregion
        #region 处理结束动画
        public void HandleStarDustEndAni()
        {
            int MaxAni = AniHelper.MaxAniProgress[AniState.End];
            int CurAni = AniHelper.AniProgress[AniState.End];
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
            Texture2D texture = LAPTextureRegister.Flash_01.Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + PostRotBalloffset, null, Color.SkyBlue, Main.GlobalTimeWrappedHourly, texture.Size() / 2, 0.2f * BallScale, SpriteEffects.FlipHorizontally, 0f);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + PostRotBalloffset, null, Color.DeepSkyBlue, -Main.GlobalTimeWrappedHourly, texture.Size() / 2, 0.2f * BallScale, SpriteEffects.None, 0f);
        }
        public void DrawBallOutLine()
        {
            LAPUtilities.ReSetToBeginShader(BlendState.Additive);
            Effect shader = UCAShaderRegister.PolarDistortShader.Value;
            shader.Parameters["uWidthMult"].SetValue(3f);
            shader.Parameters["uRingMult"].SetValue(1f);
            shader.Parameters["uYTime"].SetValue(-Main.GlobalTimeWrappedHourly);
            shader.CurrentTechnique.Passes[0].Apply();
            Main.instance.GraphicsDevice.Textures[1] = UCATextureRegister.FusableBall.Value;
            float Scale = 1.5f * BallScale;
            Texture2D texture = LAPTextureRegister.Aura_01.Value;
            Vector2 orig = texture.Size() / 2;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + PostRotBalloffset, null, Color.SkyBlue, 0, orig, Scale, SpriteEffects.None, 0);
            Scale *= 0.5f;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + PostRotBalloffset, null, Color.DeepSkyBlue, MathHelper.PiOver2, orig, Scale, SpriteEffects.None, 0);
            LAPUtilities.ReSetToEndShader();
        }
    }
}
