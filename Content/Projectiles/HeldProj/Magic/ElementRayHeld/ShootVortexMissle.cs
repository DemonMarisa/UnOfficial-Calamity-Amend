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
using LAP.Core.Enums;

namespace UCA.Content.Projectiles.HeldProj.Magic.ElementRayHeld
{
    public partial class ElementRaySpecialHeldProj
    {
        public void InitializeVortexMissle()
        {
            MainFragmentOffset = new Vector2(0, 0);
            AuxFragmentOffset = new Vector2(0, -0);
            FilpAuxFragmentOffset = new Vector2(0, 0);

            RelativeOwnerPos = new Vector2(10, 0);
            AniHelper.MaxAniProgress[AniState.Begin] = 30;
            AniHelper.MaxAniProgress[AniState.Middle] = 5;
            AniHelper.MaxAniProgress[AniState.End] = 30;
            SoundEngine.PlaySound(SoundsMenu.MAGNOLIASPRelease, Projectile.Center);
        }
        public void UpdateVortexMissle()
        {
            BeginRot = ToMouseVector;
            if (!AniHelper.HasFinish[AniState.Begin])
            {
                AniHelper.UpDateAni(AniState.Begin, 35);
                HandleVortexBeginAni();
            }
            else if (!AniHelper.HasFinish[AniState.Middle])
            {
                AniHelper.UpDateAni(AniState.Middle, 0);
                HandleVortexMiddleAni();
            }
            else if (!AniHelper.HasFinish[AniState.End])
            {
                AniHelper.UpDateAni(AniState.End, 0);
                HandleVortexEndAni();
            }
            else
            {
                Projectile.Kill();
            }
            if (Time % 12 == 0 && !AniHelper.HasFinish[AniState.Middle])
            {
                SoundStyle sound = SoundsMenu.LightingHit;
                sound.Volume = 0.2f;
                sound.Pitch = Main.rand.NextFloat(-0.6f, 1.1f);
                SoundEngine.PlaySound(sound, Projectile.Center);
            }
        }
        #region 处理开始动画
        public void HandleVortexBeginAni()
        {
            int MaxAni = AniHelper.MaxAniProgress[AniState.Begin];
            int CurAni = AniHelper.AniProgress[AniState.Begin];
            float easedProgress = EasingHelper.EaseOutCubic(CurAni / (float)MaxAni);
            float baseRotation = AniHelper.UpDateAngle(45, -145, Owner.direction, easedProgress);
            RelativeOwnerPosRot = baseRotation + ToMouseVector;
            Projectile.rotation = RelativeOwnerPosRot;
            if (CurAni < MaxAni / 2)
            {
                float beginrot = Main.rand.NextFloat(0, MathHelper.TwoPi);
                float rotSpeed = Main.rand.NextBool() ? 0.07f : -0.07f;
                int length = Main.rand.Next(250, 500);
                int LifeTime = Main.rand.Next(30, 60);
                Vector2 offset = new Vector2(50, 0);
                new ProjAbsorbGlowBall(Owner.Center, Color.Turquoise, LifeTime, 0.1f, beginrot, rotSpeed, Projectile.whoAmI, length, offset).Spawn();
            }
            if (CurAni == 1)
            {
                int LifeTime = 60;
                Vector2 offset = new Vector2(50, 0);
                new FollowProjCrossGlow(Owner.Center, Color.DarkTurquoise, LifeTime, 0.8f, Projectile.whoAmI, offset).Spawn();
                new FollowProjCrossGlow(Owner.Center, Color.Turquoise, LifeTime, 0.4f, Projectile.whoAmI, offset).Spawn();
            }
        }
        #endregion
        #region 处理中间的动画
        public void HandleVortexMiddleAni()
        {
            int MaxAni = AniHelper.MaxAniProgress[AniState.Middle];
            int CurAni = AniHelper.AniProgress[AniState.Middle];
            float easedProgress = EasingHelper.EaseInCubic(CurAni / (float)MaxAni);
            float baseRotation = AniHelper.UpDateAngle(-145, -60, Owner.direction, easedProgress);

            RelativeOwnerPosRot = baseRotation + BeginRot;
            Projectile.rotation = RelativeOwnerPosRot;

            if (CurAni == MaxAni)
            {
                SoundEngine.PlaySound(SoundsMenu.Lighting, Projectile.Center);
                Owner.AddCD(LAPContent.CDType<VortexBoost>(), 1200);
            }
        }
        #endregion
        #region 处理结束的动画
        public void HandleVortexEndAni()
        {
            int MaxAni = AniHelper.MaxAniProgress[AniState.End];
            ref float CurAni = ref AniHelper.BreakTime[AniState.End];
            float easedProgress = EasingHelper.EaseInCubic(CurAni / MaxAni);
            float baseRotation = AniHelper.UpDateAngle(-60, -60, Owner.direction, easedProgress);

            if (CurAni == 0)
            {
                int LifeTime = 30;
                Vector2 offset = new Vector2(64, 0);
                new FollowProjCrossGlow(Owner.Center, Color.DarkTurquoise, LifeTime, 0.8f, Projectile.whoAmI, offset).Spawn();
                new FollowProjCrossGlow(Owner.Center, Color.Turquoise, LifeTime, 0.4f, Projectile.whoAmI, offset).Spawn();
                Vector2 firoffset = new Vector2(64, 0).RotatedBy(Projectile.rotation);
                Vector2 velocity = new Vector2(12, 0).RotatedBy(Projectile.rotation);
                if (Projectile.IsLocalPlayer())
                {
                    int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + firoffset, velocity.RotatedByRandom(MathHelper.TwoPi), ProjectileType<VortexMissle>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner);
                    Main.projectile[p].LAP().isWeaponSkillProj = true;
                }
            }

            CurAni++;
            if (CurAni >= 3)
            {
                Vector2 firoffset = new Vector2(64, 0).RotatedBy(Projectile.rotation);
                Vector2 velocity = new Vector2(12, 0).RotatedBy(Projectile.rotation);
                if (Projectile.IsLocalPlayer())
                {
                    int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + firoffset, velocity.RotatedByRandom(MathHelper.TwoPi), ProjectileType<VortexMissle>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner, 1);
                    Main.projectile[p].LAP().isWeaponSkillProj = true;
                }
                ShootVortexLighting();
                CurAni = 1;
            }
            Projectile.rotation = BeginRot + baseRotation;
        }
        public void ShootVortexLighting()
        {
            Vector2 SpawnPos = Owner.Center + new Vector2(Main.rand.Next(100, 200), 0).RotatedByRandom(MathHelper.TwoPi);
            NPC npc = LAPUtilities.FindClosestTarget(Projectile.Center, 1500, false);
            if (npc != null)
            {
                Vector2 ToNPCVel = (npc.Center - SpawnPos).SafeNormalize(Projectile.rotation.ToRotationVector2());
                if (Projectile.owner == Main.myPlayer)
                {
                    int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), SpawnPos, ToNPCVel * 18, ModContent.ProjectileType<VortexLightning>(), Projectile.damage, Projectile.knockBack, Owner.whoAmI, 0.5f);
                    Main.projectile[p].LAP().isWeaponSkillProj = true;
                }
            }
            else
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    Vector2 firePos = -Projectile.velocity.RotateRandom(MathHelper.TwoPi) * Main.rand.Next(250, 350);
                    Vector2 firvel = Projectile.velocity.RotateRandom(MathHelper.TwoPi) * 18;
                    int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + firePos, firvel, ModContent.ProjectileType<VortexLightning>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    Main.projectile[p].LAP().isWeaponSkillProj = true;
                }
            }
        }
        #endregion
    }
}
