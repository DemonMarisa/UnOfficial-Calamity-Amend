using CalamityMod;
using LAP.Core.SystemsLoader;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using UCA.Assets.Sounds;
using UCA.Content.Particiles;
using UCA.Content.Projectiles.Magic.Ray;
using UCA.Content.UCACooldowns;
using UCA.Core.Enums;
using UCA.Core.Utilities;

namespace UCA.Content.Projectiles.HeldProj.Magic.ElementRayHeld
{
    public partial class ElementRaySpecialHeldProj
    {
        public void InitializeMisc()
        {
            MainFragmentOffset = new Vector2(0, 0);
            AuxFragmentOffset = new Vector2(0, -0);
            FilpAuxFragmentOffset = new Vector2(0, 0);

            RelativeOwnerPos = new Vector2(10, 0);
            animationHelper.MaxAniProgress[AnimationState.Begin] = 40;
            animationHelper.MaxAniProgress[AnimationState.Middle] = 10;
            animationHelper.MaxAniProgress[AnimationState.End] = 60;
            SoundEngine.PlaySound(SoundsMenu.MAGNOLIASPRelease, Projectile.Center);
            SoundEngine.PlaySound(SoundsMenu.ReStoreCharge, Projectile.Center);
        }
        public void UpdateMisc()
        {
            BeginRot = ToMouseVector;
            if (!animationHelper.HasFinish[AnimationState.Begin])
            {
                animationHelper.UpDateAni(AnimationState.Begin, 35);
                HandleMiscBeginAni();
            }
            else if (!animationHelper.HasFinish[AnimationState.Middle])
            {
                animationHelper.UpDateAni(AnimationState.Middle, 0);
                HandleMiscMiddleAni();
            }
            else if (!animationHelper.HasFinish[AnimationState.End])
            {
                animationHelper.UpDateAni(AnimationState.End, 0);
                HandleMiscEndAni();
            }
            else
            {
                Projectile.Kill();
            }
        }
        #region 处理开始动画
        public void HandleMiscBeginAni()
        {
            int MaxAni = animationHelper.MaxAniProgress[AnimationState.Begin];
            int CurAni = animationHelper.AniProgress[AnimationState.Begin];
            float easedProgress = EasingHelper.EaseOutCubic(CurAni / (float)MaxAni);
            float baseRotation = animationHelper.UpDateAngle(45, -145, Owner.direction, easedProgress);
            RelativeOwnerPosRot = baseRotation + ToMouseVector;
            Projectile.rotation = RelativeOwnerPosRot;
            if (CurAni < MaxAni / 2)
            {
                float beginrot = Main.rand.NextFloat(0, MathHelper.TwoPi);
                float rotSpeed = Main.rand.NextBool() ? 0.07f : -0.07f;
                int length = Main.rand.Next(250, 500);
                int LifeTime = Main.rand.Next(30, 60);
                Vector2 offset = new Vector2(50, 0);
                new ProjAbsorbGlowBall(Owner.Center, Color.White, LifeTime, 0.1f, beginrot, rotSpeed, Projectile.whoAmI, length, offset).Spawn();
            }
            if (CurAni == 1)
            {
                int LifeTime = 60;
                Vector2 offset = new Vector2(50, 0);
                new FollowProjCrossGlow(Owner.Center, Color.White, LifeTime, 0.8f, Projectile.whoAmI, offset).Spawn();
            }
        }
        #endregion
        #region 处理中间的动画
        public void HandleMiscMiddleAni()
        {
            int MaxAni = animationHelper.MaxAniProgress[AnimationState.Middle];
            int CurAni = animationHelper.AniProgress[AnimationState.Middle];
            float easedProgress = EasingHelper.EaseInCubic(CurAni / (float)MaxAni);
            float baseRotation = animationHelper.UpDateAngle(-145, -40, Owner.direction, easedProgress);
            if (CurAni == 1)
            {
                int LifeTime = 75;
                Vector2 offset = new Vector2(64, 0);
                new FollowProjCrossGlow(Owner.Center, Color.White, LifeTime, 0.8f, Projectile.whoAmI, offset).Spawn();
                for (int i = 0; i < 8; i++)
                {
                    Color color = LAPUtilities.LerpColor(Color.White, Color.WhiteSmoke);
                    new NoiseShockRing(Projectile.Center, Vector2.Zero, color, LifeTime, 1f, 1 + i * 0.1f, Projectile.whoAmI, offset).Spawn();
                }
            }
            RelativeOwnerPosRot = baseRotation + BeginRot;
            Projectile.rotation = RelativeOwnerPosRot;
            if (CurAni == MaxAni)
            {
                SoundEngine.PlaySound(SoundsMenu.ReStoreRelease, Projectile.Center);
                foreach (Player player in Main.ActivePlayers)
                {
                    if (player.active && player.Center.Distance(Owner.Center) < 650)
                    {
                        Owner.NCHeal(Owner.statLifeMax2 / 20);
                        Owner.AddCD(LAPContent.CDType<MiscBoost>(), 1200);
                    }
                }
                Vector2 offset = new Vector2(64, 0).RotatedBy(Projectile.rotation);
                for (int i = 0; i < 100; i++)
                {
                    Color RandomColor = LAPUtilities.LerpColor(Color.White, Color.WhiteSmoke);
                    new MediumGlowBall(Projectile.Center + offset, RandomColor, 120, 0.4f, Main.rand.NextFloat(4f, 12f)).Spawn();
                }
            }
        }
        #endregion
        #region 处理结束的动画
        public void HandleMiscEndAni()
        {
            int MaxAni = animationHelper.MaxAniProgress[AnimationState.End];
            ref float CurAni = ref animationHelper.Auxfloat[AnimationState.End];
            float easedProgress = EasingHelper.EaseInCubic(CurAni / (float)MaxAni);
            float baseRotation = animationHelper.UpDateAngle(-40, -45, Owner.direction, easedProgress);

            RelativeOwnerPosRot = baseRotation + BeginRot;
            Projectile.rotation = RelativeOwnerPosRot;
        }
        #endregion
    }
}
