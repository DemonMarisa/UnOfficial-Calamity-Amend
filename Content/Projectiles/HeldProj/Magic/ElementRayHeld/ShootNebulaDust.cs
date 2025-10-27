using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using UCA.Assets.Sounds;
using UCA.Content.Particiles;
using UCA.Content.Projectiles.Magic.Ray;
using UCA.Core.Enums;
using UCA.Core.Utilities;

namespace UCA.Content.Projectiles.HeldProj.Magic.ElementRayHeld
{
    public partial class ElementRaySpecialHeldProj
    {
        public void InitializeNebulaDust()
        {
            MainFragmentOffset = new Vector2(0, 0);
            AuxFragmentOffset = new Vector2(0, -0);
            FilpAuxFragmentOffset = new Vector2(0, 0);
            SolarBladeXOffset = 128;

            RelativeOwnerPos = new Vector2(0, 0);
            animationHelper.MaxAniProgress[AnimationState.Begin] = 30;
            animationHelper.MaxAniProgress[AnimationState.Middle] = 5;
            animationHelper.MaxAniProgress[AnimationState.End] = 30;
            SoundEngine.PlaySound(SoundsMenu.MAGNOLIASPRelease, Projectile.Center);
            SoundEngine.PlaySound(SoundsMenu.MagicStaffCharge, Projectile.Center);
        }
        public void UpdateNebulaDust()
        {
            BeginRot = ToMouseVector;
            RelativeOwnerPos = new Vector2(0, 6 * Owner.direction);

            if (!animationHelper.HasFinish[AnimationState.Begin])
            {
                animationHelper.UpDateAni(AnimationState.Begin, 35);
                HandleNebulaBeginAni();
            }
            else if (!animationHelper.HasFinish[AnimationState.Middle])
            {
                animationHelper.UpDateAni(AnimationState.Middle, 0);

                HandleNebulaMiddleAni();
            }
            else if (!animationHelper.HasFinish[AnimationState.End])
            {
                animationHelper.UpDateAni(AnimationState.End, 0);

                HandleNebulEndAni();
            }
            else
            {
                Projectile.Kill();
            }
        }
        #region 处理开始动画
        public void HandleNebulaBeginAni()
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
                new ProjAbsorbGlowBall(Owner.Center, Color.Violet, LifeTime, 0.1f, beginrot, rotSpeed, Projectile.whoAmI, length, offset).Spawn();
            }
            if (CurAni == 1)
            {
                int LifeTime = 60;
                Vector2 offset = new Vector2(50, 0);
                new FollowProjCrossGlow(Owner.Center, Color.DarkViolet, LifeTime, 0.8f, Projectile.whoAmI, offset).Spawn();
                new FollowProjCrossGlow(Owner.Center, Color.Violet, LifeTime, 0.4f, Projectile.whoAmI, offset).Spawn();
            }
        }
        #endregion
        #region 处理中间的动画
        public void HandleNebulaMiddleAni()
        {
            int MaxAni = animationHelper.MaxAniProgress[AnimationState.Middle];
            int CurAni = animationHelper.AniProgress[AnimationState.Middle];
            float easedProgress = EasingHelper.EaseInCubic(CurAni / (float)MaxAni);
            float baseRotation = animationHelper.UpDateAngle(-145, 0, Owner.direction, easedProgress);

            RelativeOwnerPosRot = baseRotation + BeginRot;
            Projectile.rotation = RelativeOwnerPosRot;
        }
        #endregion
        #region 处理结束的动画
        public void HandleNebulEndAni()
        {
            ref float CurAni = ref animationHelper.Auxfloat[AnimationState.End];
            if (CurAni == 1)
            {
                int LifeTime = 60;
                Vector2 offset = new Vector2(64, 0);
                new FollowProjCrossGlow(Owner.Center, Color.DarkViolet, LifeTime, 0.4f, Projectile.whoAmI, offset).Spawn();
                new FollowProjCrossGlow(Owner.Center, Color.Violet, LifeTime, 0.2f, Projectile.whoAmI, offset).Spawn();
            }
            CurAni++;
            if (CurAni >= 10)
            {
                SoundStyle sound = SoundsMenu.MagicStaffFire;
                sound.Pitch = 1f;
                SoundEngine.PlaySound(sound, Projectile.Center);
                Vector2 firoffset = new Vector2(64, 0).RotatedBy(Projectile.rotation);
                Vector2 velocity = new Vector2(12, 0).RotatedBy(Projectile.rotation);
                for (int i = 0; i < 11; i++)
                {
                    float rotAdd = MathHelper.ToRadians(3);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + firoffset, velocity.RotatedBy(MathHelper.ToRadians(-15) + rotAdd * i) * Main.rand.NextFloat(1f, 2f), ModContent.ProjectileType<NebulaCrystal>(), Projectile.damage * 4, Projectile.knockBack, Projectile.owner, 1);
                }
                for (int i = 0; i < 45; i++)
                {
                    Color RandomColor = LAPUtilities.LerpColor(Color.Violet, Color.BlueViolet);
                    new MediumGlowBall(Projectile.Center + firoffset + Owner.velocity * 6, RandomColor, 120, 0.2f, Main.rand.NextFloat(4f, 6f)).Spawn();
                }
                FirePorj();
                CurAni = 0;
            }
            RelativeOwnerPosRot = BeginRot;
            Projectile.rotation = RelativeOwnerPosRot;
        }
        public void FirePorj()
        {
            for (int i = 0; i < 4; i++)
            {
                Vector2 firePos = -Projectile.velocity.RotateRandom(MathHelper.PiOver4) * Main.rand.Next(250, 350);
                Vector2 firvel = Main.player[Projectile.owner].GetPlayerToMouseVector2() * 9;
                firvel = firvel.RotatedBy(MathHelper.PiOver2 * i + MathHelper.PiOver4);
                if (Projectile.owner == Main.myPlayer)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + firePos, firvel, ModContent.ProjectileType<NebulaEnergy>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
        }
        #endregion
    }
}
