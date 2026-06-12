using LAP.Core.AnimationHandle;
using LAP.Core.Enums;
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
using UCA.Content.GUI.ERayUI;
using UCA.Content.Items.Weapons.Magic.Ray;
using UCA.Content.Particiles;
using UCA.Core.Utilities;

namespace UCA.Content.Projectiles.HeldProj.Magic.ElementRayHeld
{
    public class ElementRaySkillHeldProj : ModProjectile, ILocalizedModType
    {
        public override LocalizedText DisplayName => LAPUtilities.GetItemName<ElementRayAlt>();
        public override string Texture => GetInstance<ElementRayHeldProj>().Texture;
        public float ToMouseVector;
        public Player Owner => Main.player[Projectile.owner];
        public Vector2 OffsetToOwner;
        public AniHelper AniHelper = new AniHelper(10);
        public float OffsetToOwnerLength;
        public ref float WeaponStates => ref Projectile.ai[0];
        public ref float CanPlayerEnd => ref Projectile.ai[1]; 
        public override void SetStaticDefaults()
        {
            Projectile.AddHeldProj();
            Projectile.AddToSkillProj();
        }
        public override void SetDefaults()
        {
            Projectile.width = 74;
            Projectile.height = 74;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.netImportant = true;
        }
        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override void AI()
        {
            Owner.SetUseFocus(2);
            Initialize();
            ToMouseVector = Utils.AngleLerp(ToMouseVector, Owner.GetPlayerToMouseVector2().ToRotation(), 0.2f);
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.ChangeDir(Owner.LocalMouseWorld().X > Owner.Center.X ? 1 : -1);
            Owner.heldProj = Projectile.whoAmI;
            Projectile.velocity = Projectile.rotation.ToRotationVector2();
            Projectile.timeLeft = 2;
            if (!Owner.active || Owner.dead)
                Projectile.Kill();
            HandleAni();
            Projectile.Center = Owner.Center + OffsetToOwner.RotatedBy(Projectile.rotation);
            UpdateDrawOffset();
            Vector2 HeldAimPoint = new Vector2(12, 0).RotatedBy(Projectile.rotation);
            float ArmRot = (Projectile.Center + HeldAimPoint - Owner.Center).ToRotation();
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, ArmRot - MathHelper.PiOver2);
        }
        public void Initialize()
        {
            if (Projectile.LAP().FirstFrame)
            {
                AniHelper.MaxAniProgress[AniState.Begin] = 15;
                AniHelper.MaxAniProgress[AniState.End] = 15;
                ToMouseVector = Owner.GetPlayerToMouseVector2().ToRotation();
                OffsetToOwnerLength = 12;
                OffsetToOwner = new Vector2(OffsetToOwnerLength, 0);
                if (Projectile.IsLocalPlayer())
                    LAPContent.ActiveUI(LAPContent.UIType<ERayUI>());
            }
        }
        public void HandleAni()
        {
            if (!AniHelper.HasFinish[AniState.Begin])
            {
                if (AniHelper.AniProgress[AniState.Begin] < AniHelper.MaxAniProgress[AniState.Begin])
                    AniHelper.AniProgress[AniState.Begin]++;
                HandleBeginAni();
                if (AniHelper.AniProgress[AniState.Begin] >= AniHelper.MaxAniProgress[AniState.Begin])
                {
                    if (CanPlayerEnd != 0)
                        AniHelper.HasFinish[AniState.Begin] = true;
                    if (Projectile.owner == Main.myPlayer)
                    {
                        // 在外部UIDeActive后，弹幕也立刻进入淡出动作
                        if (!LAPContent.GetUI<ERayUI>().Active || CanPlayerEnd != 0)
                        {
                            AniHelper.HasFinish[AniState.Begin] = true;
                            CanPlayerEnd++;
                            Projectile.netUpdate = true;
                        }
                    }
                }
            }
            else if (!AniHelper.HasFinish[AniState.End])
            {
                AniHelper.UpDateAni(AniState.End, 25);
                HandleEndAni();
            }
            else if (AniHelper.HasFinish[AniState.End])
            {
                Projectile.Kill();
            }
        }
        public void HandleBeginAni()
        {
            int MaxAni = AniHelper.MaxAniProgress[AniState.Begin];
            int CurAni = AniHelper.AniProgress[AniState.Begin];
            float easedProgress = EasingHelper.EaseOutCubic(CurAni / (float)MaxAni);
            float baseRotation = AniHelper.UpDateAngle(45, -145, Owner.direction, easedProgress);
            Projectile.rotation = baseRotation + ToMouseVector;
            if (CurAni == 1)
            {
                SoundEngine.PlaySound(SoundsMenu.MagicStaffCharge, Projectile.Center);
                int LifeTime = 45;
                Vector2 offset = new Vector2(64, 0);
                new FollowProjCrossGlow(Owner.Center, Color.White, LifeTime, 0.4f, Projectile.whoAmI, offset).Spawn();
            }
        }
        public void HandleEndAni()
        {
            int MaxAni = AniHelper.MaxAniProgress[AniState.End];
            int CurAni = AniHelper.AniProgress[AniState.End];
            float easedProgress = EasingHelper.EaseOutCubic(CurAni / (float)MaxAni);
            float baseRotation = AniHelper.UpDateAngle(-145, 145, Owner.direction, easedProgress);
            Projectile.rotation = baseRotation + ToMouseVector;
            OffsetToOwnerLength = MathHelper.Lerp(12, -64, easedProgress);
            if (CurAni == 1)
                SoundEngine.PlaySound(SoundsMenu.SoulOfCinderChange, Projectile.Center);
            if (CurAni == 5)
            {
                new CrossGlow(Owner.Center, Vector2.Zero ,Color.White, 90, 1f, 1f).Spawn();
                for (int i = 0; i < 4; i++)
                {
                    Color color = LAPUtilities.LerpColor(Color.White, Color.WhiteSmoke);
                    new NoiseShockRing(Projectile.Center, Vector2.Zero, color, 45, 1f, 2f + i * 0.2f, Projectile.whoAmI, Vector2.Zero, false).Spawn();
                }
                for (int i = 0; i < 50; i++)
                {
                    Color RandomColor = LAPUtilities.LerpColor(Color.White, Color.WhiteSmoke);
                    new MediumGlowBall(Projectile.Center, RandomColor, 120, 0.4f, Main.rand.NextFloat(6f, 12)).Spawn();
                }

                if (Projectile.owner == Main.myPlayer)
                    WeaponStates = Owner.UCA().ElementalRayStates;

                Projectile.netUpdate = true;
            }
        }

        public override void OnKill(int timeLeft)
        {
            Main.mouseRight = false;
            Main.mouseRightRelease = true;
            Owner.itemAnimation = 0;
            Owner.itemTime = 0;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            DrawBaseElementalRay(lightColor);
            DrawMainFragments(lightColor);
            DrawAuxFragments(lightColor);
            FilpDrawAuxFragments(lightColor);
            return false;
        }



        public Vector2 MainFragmentOffset = new Vector2(0, 0);
        public Vector2 AuxFragmentOffset = new Vector2(0, 0);
        public Vector2 FilpAuxFragmentOffset = new Vector2(0, 0);

        public float MainFragmentRot;
        public float AuxFragmentRot;
        public float FilpAuxFragmentRot;
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
            Vector2 drawPosition = Projectile.Center + MainFragmentOffset - Main.screenPosition;
            Rectangle frame = texture.Frame(5, 1, (int)WeaponStates, 0);
            Vector2 origin = frame.Size() * 0.5f;
            SpriteEffects flipSprite = Projectile.spriteDirection * Main.player[Projectile.owner].gravDir == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            float drawRotation = Projectile.rotation + (Projectile.spriteDirection == -1 ? MathHelper.PiOver2 + MathHelper.PiOver4 : MathHelper.PiOver4);
            Main.spriteBatch.Draw(texture, drawPosition, frame, lightColor, drawRotation, origin, Projectile.scale * Main.player[Projectile.owner].gravDir, flipSprite, 0f);
        }
        public void DrawAuxFragments(Color lightColor)
        {
            Texture2D texture = UCATextureRegister.AuxElementalFragments.Value;
            Vector2 drawPosition = Projectile.Center + AuxFragmentOffset - Main.screenPosition;
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
            float drawRotation = Projectile.rotation - MathHelper.PiOver4;
            Vector2 origin = frame.Size() * 0.5f;
            SpriteEffects flipSprite = SpriteEffects.FlipVertically;
            // spriteBatch会自动把textures0设置为当前使用的材质，所以需要你手动改一下
            Main.spriteBatch.Draw(texture, drawPosition, frame, lightColor, drawRotation, origin, Projectile.scale * Main.player[Projectile.owner].gravDir, flipSprite, 0f);
        }

        public void FilpDrawAuxFragments(Color lightColor)
        {
            Texture2D texture = UCATextureRegister.AuxElementalFragments.Value;
            Vector2 drawPosition = Projectile.Center + FilpAuxFragmentOffset - Main.screenPosition;
            Rectangle frame;
            float drawRotation = Projectile.rotation + MathHelper.PiOver4;
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
