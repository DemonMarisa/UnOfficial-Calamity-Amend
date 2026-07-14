using LAP.Core.AnimationHandle;
using LAP.Core.Enums;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.ModLoader;
using UCA.Assets.Sounds;
using UCA.Content.Items.Weapons.Magic.Ray;
using UCA.Content.MetaBalls;
using UCA.Content.Particiles;
using UCA.Content.Paths;
using UCA.Content.Projectiles.Magic.Ray;

namespace UCA.Content.Projectiles.HeldProj.Magic.SoulPiercerHeld
{
    public class SoulPiercerSpecialHeldProj : ModProjectile, ILocalizedModType
    {
        public override LocalizedText DisplayName => LAPUtilities.GetItemName<SoulPiercerAlt>();
        public override string Texture => $"{ProjPath.HeldProjPath}" + "Magic/SoulPiercerHeld/SoulPiercerHeldProj";
        public Player Owner => Main.player[Projectile.owner];
        public AniHelper AniHelper = new AniHelper(3);
        public float TargetRot;
        public ref float Filp => ref Projectile.ai[1];
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }
        public override void AI()
        {
            if (Projectile.LAP().FirstFrame)
            {
                if (Filp != -1 && Filp != 1)
                    Filp = 1;
                SoundEngine.PlaySound(SoundsMenu.MAGNOLIASPRelease, Projectile.Center);
                SoundEngine.PlaySound(SoundsMenu.MagicStaffCharge, Projectile.Center);
                AniHelper.MaxAniProgress[AniState.Begin] = 30;
                AniHelper.MaxAniProgress[AniState.End] = 7;
                TargetRot = Owner.GetPlayerToMouseVector2().ToRotation();
            }
            Projectile.SetHeldProj(Owner);
            Projectile.velocity = Vector2.Zero;
            Projectile.timeLeft = 2;
            TargetRot = TargetRot.AngleLerp(Owner.GetPlayerToMouseVector2().ToRotation(), 0.25f);
            HandleAni();
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);
            Projectile.Center = Owner.Center + new Vector2(10, 0).RotatedBy(Projectile.rotation);
        }
        #region 处理动画
        public void HandleAni()
        {
            if (!AniHelper.HasFinish[AniState.Begin]) {
                AniHelper.UpDateAni(AniState.Begin, 10);
                HandleBeginAni();
            }
            else if (!AniHelper.HasFinish[AniState.End]) {
                AniHelper.UpDateAni(AniState.End, 25);
                HandleEndAni();
            }
            else Projectile.Kill();
        }
        public void HandleBeginAni()
        {
            int MaxAni = AniHelper.MaxAniProgress[AniState.Begin];
            int CurAni = AniHelper.AniProgress[AniState.Begin];
            float easedProgress = EasingHelper.EaseOutCubic(CurAni / (float)MaxAni);
            float baseRotation = AniHelper.UpDateAngle(-145 * Filp, -145 * Filp, Owner.direction, easedProgress);
            Projectile.rotation = baseRotation + TargetRot;
            Vector2 offset = new(50, 0);
            if (CurAni < MaxAni / 2)
            {
                float beginrot = Main.rand.NextFloat(0, MathHelper.TwoPi);
                float rotSpeed = Main.rand.NextBool() ? 0.07f : -0.07f;
                int length = Main.rand.Next(250, 500);
                int LifeTime = Main.rand.Next(30, 60);
                new LAP.Content.Particles.ProjAbsorbGlowBall(Owner.Center, Color.DarkViolet, LifeTime, 0.1f, beginrot, rotSpeed, Projectile.whoAmI, length, offset).Spawn();
            }
            if (CurAni == 1)
            {
                int LifeTime = 60;
                new FollowProjCrossGlow(Owner.Center, Color.DarkViolet, LifeTime, 0.8f, Projectile.whoAmI, offset).Spawn();
                new FollowProjCrossGlow(Owner.Center, Color.Violet, LifeTime, 0.4f, Projectile.whoAmI, offset).Spawn();
            }
            if (AniHelper.BreakTime[AniState.Begin] == 10)
            {
                Vector2 firpos = Projectile.Center + offset.RotatedBy(Projectile.rotation);
                for (int i = 0; i < 100; i++)
                    CosmicMetaBall.SpawnCircleParticle(firpos, Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.3f, 1f) * 18, 0.4f, 60);
                if (Projectile.owner == Main.myPlayer)
                {
                    SoundEngine.PlaySound(SoundsMenu.MagicStaffFire with {Pitch = Main.rand.NextFloat(0.2f, 0.5f) });
                    Vector2 FirePoint = Projectile.Center + new Vector2(-400 * Owner.direction, 0);
                    Vector2 FireVel = LAPUtilities.GetVector2(FirePoint, Owner.LocalMouseWorld());
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), FirePoint, FireVel, ModContent.ProjectileType<CosmicSlash>(),  Projectile.damage, Projectile.knockBack, Projectile.owner);

                    Vector2 FirePoint2 = Projectile.Center + new Vector2(-250 * Owner.direction, -300);
                    Vector2 FireVel2 = LAPUtilities.GetVector2(FirePoint2, Owner.LocalMouseWorld());
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), FirePoint2, FireVel2, ModContent.ProjectileType<CosmicSlash>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    // AddDeathMark();
                    for (int i = 0; i < 50; i++)
                    {
                        Vector2 spawnVec = Vector2.UnitX.RotateRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.7f, 1f) * 24;
                        CosmicMetaBall.SpawnCircleParticle(Owner.LocalMouseWorld(), spawnVec, 0.5f, 60);
                    }
                }
            }
        }
        public void HandleEndAni()
        {
            int MaxAni = AniHelper.MaxAniProgress[AniState.End];
            int CurAni = AniHelper.AniProgress[AniState.End];
            float easedProgress = EasingHelper.EaseOutCubic(CurAni / (float)MaxAni);
            float baseRotation = AniHelper.UpDateAngle(-145 * Filp, 145 * Filp, Owner.direction, easedProgress);
            Projectile.rotation = baseRotation + TargetRot;
            Projectile.ai[0]++;
            if (Projectile.ai[0] % 5 == 0)
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    NPC npc = LAPUtilities.FindClosestTarget(Owner.LocalMouseWorld(), 250, false);
                    Vector2 FirePoint;
                    Vector2 FireVel;
                    if (npc is not null)
                    {
                        FirePoint = npc.Center + new Vector2(Main.rand.Next(200, 600), 0).RotatedByRandom(MathHelper.TwoPi);
                        FireVel = LAPUtilities.GetVector2(FirePoint, npc.Center);
                    }
                    else
                    {
                        FirePoint = Owner.LocalMouseWorld() + new Vector2(Main.rand.Next(200, 600), 0).RotatedByRandom(MathHelper.TwoPi);
                        FireVel = LAPUtilities.GetVector2(FirePoint, Owner.LocalMouseWorld());
                    }
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), FirePoint, FireVel, ModContent.ProjectileType<CosmicSlash>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
                Vector2 offset = new(50, 0);
                Vector2 firpos = Projectile.Center + offset.RotatedBy(Projectile.rotation);
                for (int i = 0; i < 10; i++)
                    CosmicMetaBall.SpawnLozengeParticle(firpos, Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.3f, 1f) * 18, 0.4f, 60);
            }
        }
        //public void AddDeathMark()
        //{
        //    foreach(NPC npc in Main.ActiveNPCs)
        //    {
        //        if (!npc.active)
        //            continue;
        //        if (npc.Distance(Owner.LocalMouseWorld()) > 200)
        //            continue;
        //        npc.AddBuff(ModContent.BuffType<MarkedforDeath>(), 900);
        //    }
        //}
        #endregion
        public override void OnKill(int timeLeft)
        {
            Owner.itemTime = 0;
            Owner.itemAnimation = 0;
            if (Main.mouseRight && Owner.CheckMana(Owner.ActiveItem(), (int)(Owner.ActiveItem().mana * Owner.manaCost), true))
            {
                AniHelper = new AniHelper();
                if (Filp == 1)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, Type, Projectile.damage, Projectile.knockBack, Projectile.owner, 0, -1);
                else
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, Type, Projectile.damage, Projectile.knockBack, Projectile.owner, 0, 1);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            float drawRotation = Projectile.rotation + (Projectile.spriteDirection == -1 ? MathHelper.PiOver2 + MathHelper.PiOver4 : MathHelper.PiOver4);
            Vector2 rotationPoint = texture.Size() / 2f;
            SpriteEffects flipSprite = Projectile.spriteDirection * Main.player[Projectile.owner].gravDir == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            // spriteBatch会自动把textures0设置为当前使用的材质，所以需要你手动改一下
            Main.spriteBatch.Draw(texture, drawPosition, null, lightColor, drawRotation, rotationPoint, Projectile.scale * Main.player[Projectile.owner].gravDir, flipSprite, 0f);
            return false;
        }
    }
}
