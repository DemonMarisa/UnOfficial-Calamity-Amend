using LAP.Core.BaseClass.Projectiles;
using LAP.Core.SystemsLoader;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Localization;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Assets.Sounds;
using UCA.Content.Items.Weapons.Magic.Ray;
using UCA.Content.Projectiles.Magic.Ray;
using UCA.Core.Utilities;

namespace UCA.Content.Projectiles.HeldProj.Magic.ElementRayHeld
{
    public static class ElementalRayState
    {
        public static int Vortex = 0;
        public static int Nebula = 1;
        public static int Solar = 2;
        public static int StarDust = 3;
        public static int Misc = 4;
    }

    public partial class ElementRayHeldProj : BaseHeldProj
    {
        // 日耀模式下会发射散射火球，击中敌人，玩家背后发射火球
        // 星云模式下会在周围发射星云，击中敌人发射额外星云
        // 星璇模式下会在玩家背后发射更多导弹，分裂改为发射闪电
        public override LocalizedText DisplayName => LAPUtilities.GetItemName<ElementRayAlt>();
        public Vector2 RotVector => new Vector2(8 * Owner.direction, 7).BetterRotatedBy(Owner.GetPlayerToMouseVector2().ToRotation(), default, 0.5f, 1f);
        public override Vector2 PositionOffset => RotVector * Owner.direction;

        public Vector2 MainFragmentOffset = new Vector2(0, 0);
        public Vector2 AuxFragmentOffset = new Vector2(0, 0);
        public Vector2 FilpAuxFragmentOffset = new Vector2(0, 0);
        public ref float WeaponStates => ref Projectile.ai[2];
        public ref float LightShootTime => ref Projectile.ai[0];
        public ref float LightShootCount => ref Projectile.ai[1];
        public Vector2 OldSpawnPos;
        public override void ExSD()
        {
            Projectile.width = 74;
            Projectile.height = 74;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            RotAmount = 0.25f;
        }
        public override void Initialize()
        {
            RotAmount = 0.25f;
            // 只在本地玩家设置状态，随后依靠收发包来同步状态，防止同步问题
            if (Projectile.owner == Main.myPlayer)
                WeaponStates = Owner.UCA().ElementalRayStates;
            OldSpawnPos = Vector2.Zero;
        }
        public override void ExAI()
        {
            if (Owner.LAP().MouseLeft && UseDelay <= 0 && Owner.CheckMana(Owner.ActiveItem(), (int)(Owner.HeldItem.mana * Owner.manaCost), true, false))
            {
                // 生成弹幕
                SoundEngine.PlaySound(SoundsMenu.MagicStaffFire, Projectile.Center);
                Vector2 FireOffset = new Vector2(48, 0).RotatedBy(Projectile.rotation);
                Vector2 FireVel = new Vector2(1, 0).RotatedBy(Projectile.rotation);
                if (Projectile.owner == Main.myPlayer)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + FireOffset, FireVel, ModContent.ProjectileType<ElementalLaser>(), Projectile.damage, Projectile.knockBack, Projectile.owner, WeaponStates);
                if (WeaponStates == ElementalRayState.Solar)
                {
                    SoundEngine.PlaySound(SoundsMenu.Fire, Projectile.Center);
                    ShootFireBall();
                }
                OldSpawnPos = Vector2.Zero;
                LightShootCount = 4;
                if (WeaponStates == ElementalRayState.Nebula || WeaponStates == ElementalRayState.Vortex)
                {
                    LightShootCount = 6;
                }
                // 后坐力
                Projectile.velocity -= Projectile.velocity.RotatedBy(Projectile.spriteDirection * MathHelper.PiOver2) * 0.15f;
                MainFragmentOffset *= 1.2f;
                AuxFragmentOffset *= 1.2f;
                FilpAuxFragmentOffset *= 1.2f;
                UseDelay = Owner.ApplyWeaponAttackSpeed(Owner.ActiveItem(), Owner.HeldItem.useTime, Owner.HeldItem.useTime / 2);
            }
        }
        #region 日耀
        public void ShootFireBall()
        {
            if (Projectile.owner == Main.myPlayer)
            {
                Vector2 FireOffset = new Vector2(48, 0).RotatedBy(Projectile.rotation);
                Vector2 FireVel = new Vector2(12, 0).RotatedBy(Projectile.rotation);
                for (int i = 0; i < 11; i++)
                {
                    float rotAdd = MathHelper.ToRadians(3);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + FireOffset, FireVel.RotatedBy(MathHelper.ToRadians(-15) + rotAdd * i) * Main.rand.NextFloat(0.6f, 1f), ModContent.ProjectileType<SolarFireBall>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner, i);
                }
            }
        }
        #endregion
        public override void OnKill(int timeLeft)
        {
            if (Projectile.owner == Main.myPlayer)
                Main.mouseRight = false;
            Owner.itemTime = 0;
            Owner.itemAnimation = 0;
        }
        public override void PostAI()
        {
            UpdateDrawOffset();
            // 设置玩家手持效果
            float baseRotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            float directionVerticality = MathF.Abs(Projectile.velocity.X);
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, baseRotation + Owner.direction * directionVerticality * 1.5f);
            Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, baseRotation + Owner.direction * directionVerticality * 1.2f);

            ShootVortex();

            base.PostAI();
            if (!Owner.LAP().MouseLeft && Owner.LAP().MouseRight && UseDelay == 0)
            {
                Main.mouseRight = false;
                Owner.itemTime = 0;
                Owner.itemAnimation = 0;
                Projectile.Kill();
            }
        }
        public void ShootVortex()
        {
            if (LightShootCount > 0 && LightShootTime <= 0)
            {
                Vector2 firePos = -Projectile.velocity.RotateRandom(MathHelper.PiOver4) * Main.rand.Next(350, 500);
                Vector2 Spawn = Projectile.Center + firePos;
                Vector2 firvel = LAPUtilities.GetVector2(Spawn, Owner.LAP().SyncedMouseWorld) * 12;
                int damage = Projectile.damage;

                int Type = ModContent.ProjectileType<VortexMissle>();

                if (WeaponStates == ElementalRayState.Vortex)
                    SoundEngine.PlaySound(SoundsMenu.FastLighting, Projectile.Center);

                if (WeaponStates == ElementalRayState.Solar)
                    Type = ModContent.ProjectileType<SolarFireBall>();

                if (WeaponStates == ElementalRayState.Nebula)
                {
                    Type = ModContent.ProjectileType<NebulaEnergy>();
                    Spawn = Projectile.Center + -Projectile.velocity.RotateRandom(MathHelper.TwoPi) * Main.rand.Next(100, 300);
                    firvel = LAPUtilities.GetVector2(Owner.Center, Spawn) * 6;
                    damage *= 2;
                    SoundEngine.PlaySound(SoundsMenu.NightRayHit, Spawn);
                }

                if (WeaponStates == ElementalRayState.StarDust)
                {
                    Type = ModContent.ProjectileType<StarDustLaser>();
                    damage *= 2;
                    SoundEngine.PlaySound(SoundsMenu.MetalHit, Spawn);
                    if (OldSpawnPos != Vector2.Zero)
                         LAPUtilities.GenStarLine(OldSpawnPos, Spawn, 100);
                }
                OldSpawnPos = Spawn;
                if (Projectile.owner == Main.myPlayer)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Spawn, firvel, Type, damage, Projectile.knockBack, Projectile.owner);
                LightShootCount--;
                LightShootTime = 4;
            }
            if (LightShootTime > 0)
                LightShootTime--;
        }
        public void UpdateDrawOffset()
        {
            float MainHeightOffset = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 3 + MathHelper.Pi) * 2.5f;
            Vector2 targetPos = new Vector2(58 + MainHeightOffset, 0).RotatedBy(Projectile.rotation);
            MainFragmentOffset = Vector2.Lerp(MainFragmentOffset, targetPos, 0.3f);
            float HeightOffset = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 3) * 1.5f;
            Vector2 FragtargetPos = new Vector2(44 + HeightOffset, 10).RotatedBy(Projectile.rotation);
            AuxFragmentOffset = Vector2.Lerp(AuxFragmentOffset, FragtargetPos, 0.2f);
            Vector2 SecondFragtargetPos = new Vector2(44 + HeightOffset, -10).RotatedBy(Projectile.rotation);
            FilpAuxFragmentOffset = Vector2.Lerp(FilpAuxFragmentOffset, SecondFragtargetPos, 0.2f);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            DrawBaseElementalRay(lightColor);
            FilpDrawAuxFragments(1, lightColor);
            DrawMainFragments(lightColor);
            DrawAuxFragments(1, lightColor);
            return false;
        }
        #region 绘制
        public void DrawBaseElementalRay(Color lightColor)
        {
            Texture2D texture = UCATextureRegister.ElementalRayBase.Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            float drawRotation = Projectile.rotation + (Projectile.spriteDirection == -1 ? MathHelper.PiOver2 + MathHelper.PiOver4 : MathHelper.PiOver4);
            Vector2 rotationPoint = texture.Size() / 2f;
            SpriteEffects flipSprite = Projectile.spriteDirection * Main.player[Projectile.owner].gravDir == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            // spriteBatch会自动把textures0设置为当前使用的材质，所以需要你手动改一下
            Main.spriteBatch.Draw(texture, drawPosition, null, lightColor, drawRotation, rotationPoint, Projectile.scale * Main.player[Projectile.owner].gravDir, flipSprite, 0f);
        }

        public void DrawMainFragments(Color lightColor)
        {
            Texture2D texture = UCATextureRegister.MainElementalFragments.Value;
            Vector2 drawPosition = Projectile.Center + MainFragmentOffset - Main.screenPosition ;
            float drawRotation = Projectile.rotation + (Projectile.spriteDirection == -1 ? MathHelper.PiOver2 + MathHelper.PiOver4 : MathHelper.PiOver4);
            Rectangle frame = texture.Frame(5, 1, (int)WeaponStates, 0);
            Vector2 origin = frame.Size() * 0.5f;
            SpriteEffects flipSprite = Projectile.spriteDirection * Main.player[Projectile.owner].gravDir == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            // spriteBatch会自动把textures0设置为当前使用的材质，所以需要你手动改一下
            Main.spriteBatch.Draw(texture, drawPosition, frame, lightColor, drawRotation, origin, Projectile.scale * Main.player[Projectile.owner].gravDir, flipSprite, 0f);
        }
        public void DrawAuxFragments(int Filp, Color lightColor)
        {
            Texture2D texture = UCATextureRegister.AuxElementalFragments.Value;
            Vector2 drawPosition = Projectile.Center + AuxFragmentOffset - Main.screenPosition;
            float drawRotation = Projectile.rotation - MathHelper.PiOver4;
            Rectangle frame;
            if (WeaponStates == ElementalRayState.Misc)
            {
                int FilpFrag = Projectile.spriteDirection == 1 ? ElementalRayState.Nebula : ElementalRayState.Vortex;
                frame = texture.Frame(4, 1, FilpFrag, 0);
            }
            else
            {
                frame = texture.Frame(4, 1, (int)WeaponStates, 0);
            }
            Vector2 origin = frame.Size() * 0.5f;
            SpriteEffects flipSprite = SpriteEffects.FlipVertically;
            // spriteBatch会自动把textures0设置为当前使用的材质，所以需要你手动改一下
            Main.spriteBatch.Draw(texture, drawPosition, frame, lightColor, drawRotation, origin, Projectile.scale * Main.player[Projectile.owner].gravDir, flipSprite, 0f);
        }

        public void FilpDrawAuxFragments(int Filp, Color lightColor)
        {
            Texture2D texture = UCATextureRegister.AuxElementalFragments.Value;
            Vector2 drawPosition = Projectile.Center + FilpAuxFragmentOffset - Main.screenPosition;
            float drawRotation = Projectile.rotation + MathHelper.PiOver4;
            Rectangle frame;
            if (WeaponStates == ElementalRayState.Misc)
            {
                int FilpFrag = Projectile.spriteDirection == 1 ? ElementalRayState.Vortex : ElementalRayState.Nebula;
                frame = texture.Frame(4, 1, FilpFrag, 0);
            }
            else
            {
                frame = texture.Frame(4, 1, (int)WeaponStates, 0);
            }
            Vector2 origin = frame.Size() * 0.5f;
            SpriteEffects flipSprite = SpriteEffects.None;
            // spriteBatch会自动把textures0设置为当前使用的材质，所以需要你手动改一下
            Main.spriteBatch.Draw(texture, drawPosition, frame, lightColor, drawRotation, origin, Projectile.scale * Main.player[Projectile.owner].gravDir, flipSprite, 0f);
        }
        #endregion
    }
}
