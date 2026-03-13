using LAP.Core.AnimationHandle;
using LAP.Core.Enums;
using LAP.Core.SystemsLoader;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.Localization;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Assets.Sounds;
using UCA.Content.Items.Weapons.Magic.Ray;
using UCA.Content.Particiles;
using UCA.Content.Paths;
using UCA.Content.Projectiles.Magic.Ray;

namespace UCA.Content.Projectiles.HeldProj.Magic.ShadowBoltStaffHeld
{
    public class ShadowBoltStaffSpecialHeldProj : ModProjectile, ILocalizedModType
    {
        public override LocalizedText DisplayName => LAPUtilities.GetItemName<ShadowBoltStaffAlt>();
        public override string Texture => $"{ProjPath.HeldProjPath}" + "Magic/ShadowBoltStaffHeld/ShadowBoltStaffHeldProj";
        public AnimationHelper animationHelper = new AnimationHelper(3);
        public BasePartInfo ShadowOrb;
        public float Opacity = 1f;
        public float ToMouseRot;
        public Player Owner => Main.player[Projectile.owner];
        public override void SetDefaults()
        {
            Projectile.width = 66;
            Projectile.height = 64;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.netImportant = true;
        }
        public override void AI()
        {
            Owner.SetUseFocus(2);
            if (Projectile.LAP().FirstFrame)
            {
                SoundEngine.PlaySound(SoundsMenu.MagicStaffCharge, Projectile.Center);
                ToMouseRot = Owner.GetPlayerToMouseVector2().ToRotation();
                Texture2D texture2d = UCATextureRegister.ShadowBoltStaffOrb.Value;
                ShadowOrb = new BasePartInfo(texture2d, Vector2.Zero, Vector2.Zero, 0, texture2d.Size() / 2);
                animationHelper.MaxAniProgress[AnimationState.Begin] = 30;
                animationHelper.MaxAniProgress[AnimationState.Middle] = 5;
                animationHelper.MaxAniProgress[AnimationState.End] = 30;
            }
            if (!Owner.active || Owner.dead)
                Projectile.Kill();
            // 基础信息
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.ChangeDir(Owner.LocalMouseWorld().X > Owner.Center.X ? 1 : -1);
            Owner.heldProj = Projectile.whoAmI;
            Projectile.velocity = Projectile.rotation.ToRotationVector2();
            Projectile.Center = Owner.Center;
            // 设置玩家手持效果
            float baseRotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, baseRotation);
            Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, baseRotation);
            Projectile.spriteDirection = Owner.direction;
            ToMouseRot = Utils.AngleLerp(ToMouseRot, Owner.GetPlayerToMouseVector2().ToRotation(), 0.08f);
            UpdateAni();
            UpdateOrb();
        }
        #region 处理动画
        public void UpdateAni()
        {
            if (!animationHelper.HasFinish[AnimationState.Begin])
            {
                animationHelper.UpDateAni(AnimationState.Begin, 15);
                HandleBeginAni();
            }
            else if (!animationHelper.HasFinish[AnimationState.Middle])
            {
                animationHelper.UpDateAni(AnimationState.Middle, 0);
                HandleMiddleAni();
            }
            else if (!animationHelper.HasFinish[AnimationState.End])
            {
                animationHelper.UpDateAni(AnimationState.End, 0);
                HandleEndAni();
            }
            else
            {
                Projectile.Kill();
            }
        }
        #region 处理开始
        public void HandleBeginAni()
        {
            int MaxAni = animationHelper.MaxAniProgress[AnimationState.Begin];
            int CurAni = animationHelper.AniProgress[AnimationState.Begin];
            float easedProgress = EasingHelper.EaseOutCubic(CurAni / (float)MaxAni);
            float baseRotation = animationHelper.UpDateAngle(45, -145, Owner.direction, easedProgress);
            Projectile.rotation = baseRotation + ToMouseRot;
            Opacity = MathHelper.Lerp(1f, 0f, easedProgress);
            Vector2 offset = new(50, 0);
            if (CurAni < MaxAni / 2)
            {
                float beginrot = Main.rand.NextFloat(0, MathHelper.TwoPi);
                float rotSpeed = Main.rand.NextBool() ? 0.07f : -0.07f;
                int length = Main.rand.Next(250, 500);
                int LifeTime = Main.rand.Next(30, 60);
                new ProjAbsorbGlowBall(Owner.Center, Color.DarkViolet, LifeTime, 0.1f, beginrot, rotSpeed, Projectile.whoAmI, length, offset).Spawn();
            }
            if (CurAni == 1)
            {
                int LifeTime = 60;
                new FollowProjCrossGlow(Owner.Center, Color.DarkViolet, LifeTime, 0.8f, Projectile.whoAmI, offset).Spawn();
                new FollowProjCrossGlow(Owner.Center, Color.Violet, LifeTime, 0.4f, Projectile.whoAmI, offset).Spawn();
            }
        }
        #endregion
        #region 处理中间
        public void HandleMiddleAni()
        {
            int MaxAni = animationHelper.MaxAniProgress[AnimationState.Middle];
            int CurAni = animationHelper.AniProgress[AnimationState.Middle];
            float easedProgress = EasingHelper.EaseInCubic(CurAni / (float)MaxAni);
            float baseRotation = animationHelper.UpDateAngle(-145, -60, Owner.direction, easedProgress);
            Projectile.rotation = baseRotation + ToMouseRot;
            if (CurAni == MaxAni)
            {
                Vector2 offset = new Vector2(50, 0);
                Vector2 firpos = Projectile.Center + offset.RotatedBy(Projectile.rotation);
                for (int i = 0; i < 100; i++)
                {
                    Color Firecolor = LAPUtilities.LerpColor(Color.Black, Color.DarkViolet);
                    new Fire(firpos, Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.3f, 1f) * 18, Firecolor, 90, Main.rand.NextFloat(MathHelper.TwoPi), 1f, 0.3f).SpawnToPriorityNonPreMult();
                }
                for (int i = 0; i < 8; i++)
                {
                    Color color = LAPUtilities.LerpColor(Color.Violet, Color.DarkViolet);
                    new NoiseShockRing(firpos, Vector2.Zero, color, 60, 1f, 0.7f + i * 0.05f, -1, Vector2.Zero).Spawn();
                }
                new CrossGlow(firpos, Vector2.Zero, Color.Violet, 60, 1f, 0.7f, true).Spawn();
                new CrossGlow(firpos, Vector2.Zero, Color.DarkViolet, 60, 1f, 0.7f, true).Spawn();
                ResetShadowPos();
            }
        }
        #endregion
        #region 处理结束
        public void HandleEndAni()
        {
            int MaxAni = animationHelper.MaxAniProgress[AnimationState.End];
            int CurAni = animationHelper.AniProgress[AnimationState.End];
            float easedProgress = EasingHelper.EaseInCubic(CurAni / (float)MaxAni);
            float baseRotation = animationHelper.UpDateAngle(-60, -63, Owner.direction, easedProgress);
            Projectile.rotation = baseRotation + ToMouseRot;
            Opacity = MathHelper.Lerp(0f, 1f, easedProgress);
        }
        #endregion
        #endregion
        public void ResetShadowPos()
        {
            SoundEngine.PlaySound(SoundsMenu.ShadowBoltStaffSkillrelease, Projectile.Center);
            if (!LAPUtilities.IsLocalPlayer(Projectile.owner))
                return;
            bool HasProj = false;
            int ProjTime = 600;
            int TotalCount = 0;
            foreach (Projectile projectile in Main.ActiveProjectiles)
            {
                if (projectile.owner != Main.myPlayer)
                    continue;
                if (!projectile.active)
                    continue;
                if (projectile.type != ModContent.ProjectileType<ShadowPlayer>())
                    continue;
                HasProj = true;
                ProjTime = projectile.timeLeft;
                projectile.ai[1]++;
                projectile.netUpdate = true;
                TotalCount++;
            }

            if (!HasProj)
                return;

            for (int i = 0; i < TotalCount; i++)
            {
                float X = Main.rand.Next(300, 600);
                Vector2 SpawnPos = new Vector2(X, 0).RotatedByRandom(MathHelper.TwoPi);
                int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + SpawnPos, Vector2.Zero, ModContent.ProjectileType<ShadowPlayer>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 8 * i);
                Main.projectile[p].timeLeft = ProjTime;
                Main.projectile[p].netUpdate = true;
            }
        }
        public void UpdateOrb()
        {
            Vector2 TargetPos = new Vector2(52, 7 * Owner.direction).RotatedBy(Projectile.rotation);
            ShadowOrb.Position = Vector2.Lerp(ShadowOrb.Position, TargetPos, 0.4f);
        }
        public override void OnKill(int timeLeft)
        {
            // 重置状态，让玩家可以继续使用
            Main.mouseRight = false;
            Owner.itemTime = 0;
            Owner.itemAnimation = 0;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            DrawBaseStaff(lightColor);
            DrawOrb(lightColor);
            return false;
        }
        public void DrawBaseStaff(Color lightColor)
        {
            Texture2D DrawTexture = UCATextureRegister.ShadowBoltStaffLong.Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            float drawRotation = Projectile.rotation + (Projectile.spriteDirection == -1 ? MathHelper.Pi - MathHelper.PiOver4 : MathHelper.PiOver4);
            Vector2 rotationPoint = DrawTexture.Size() / 2f;
            SpriteEffects flipSprite = Projectile.spriteDirection * Main.player[Projectile.owner].gravDir == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.spriteBatch.Draw(DrawTexture, drawPosition, null, lightColor, drawRotation, rotationPoint, Projectile.scale * Main.player[Projectile.owner].gravDir, flipSprite, 0f);
        }
        public void DrawOrb(Color lightColor)
        {
            Texture2D DrawTexture = ShadowOrb.Texture;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition + ShadowOrb.Position;
            Vector2 rotationPoint = DrawTexture.Size() / 2f;
            SpriteEffects flipSprite = Projectile.spriteDirection * Main.player[Projectile.owner].gravDir == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            // spriteBatch会自动把textures0设置为当前使用的材质，所以需要你手动改一下
            Main.spriteBatch.Draw(DrawTexture, drawPosition, null, lightColor, 0, rotationPoint, Projectile.scale * Main.player[Projectile.owner].gravDir, flipSprite, 0f);
        }
    }
}
