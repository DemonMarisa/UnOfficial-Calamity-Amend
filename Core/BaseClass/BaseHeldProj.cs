using CalamityMod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Core.Utilities;

namespace UCA.Core.BaseClass
{
    public abstract class BaseHeldProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "HeldProj";
        public virtual Vector2 RotPoint => TextureAssets.Projectile[Type].Size() / 2;
        public virtual Vector2 Posffset => Vector2.Zero;
        public virtual float RotOffset => 0;
        public virtual float RotAmount => 1f;

        public Player Owner => Main.player[Projectile.owner];

        public Vector2 LocalMouseWorld => Owner.Calamity().mouseWorld;

        public bool Active => (Owner.channel || Owner.controlUseTile) && !Owner.noItems && !Owner.CCed;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.NeedsUUID[Projectile.type] = true;
            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
        }
        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }

        public int UseDelay = 0;

        public override void AI()
        {
            if (UseDelay > 0)
                UseDelay--;

            if (!Owner.active || Owner.dead)
                Projectile.Kill();

            Projectile.extraUpdates = 0;

            Player player = Main.player[Projectile.owner];
            Vector2 rrp = player.RotatedRelativePoint(player.MountedCenter, true);

            // Update the Prism's position in the world and relevant variables of the player holding it.
            UpdatePlayerVisuals(player, rrp);

            if (Projectile.owner == Main.myPlayer)
            {
                UpdateAim(Projectile.Center);

                if (StillInUse())
                {
                    HoldoutAI();
                }
                else if (CanDel())
                {
                    InDel();
                }
            }
            // 确保不会使用的时候消失
            Projectile.timeLeft = 2;
        }
        public virtual bool StillInUse()
        {
            return Active;
        }

        public virtual bool CanDel()
        {
            return UseDelay <= 0;
        }
        #region 更新玩家视觉效果
        public virtual void UpdatePlayerVisuals(Player player, Vector2 playerHandPos)
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            Projectile.Center = playerHandPos + Posffset;

            Projectile.spriteDirection = Projectile.direction;

            player.ChangeDir(Owner.LocalMouseWorld().X > player.Center.X ? 1 : -1);
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;

            player.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();
        }
        #endregion

        #region 更新弹幕朝向
        public virtual void UpdateAim(Vector2 source)
        {
            Vector2 aim = Vector2.Normalize(Owner.LocalMouseWorld() - source);
            if (aim.HasNaNs())
            {
                aim = -Vector2.UnitY;
            }

            aim = Vector2.Normalize(Vector2.Lerp(Vector2.Normalize(Projectile.velocity), aim, RotAmount));

            if (aim != Projectile.velocity)
            {
                Projectile.netUpdate = true;
            }
            Projectile.velocity = aim;
        }
        #endregion

        /// <summary>
        /// 手持弹幕的AI逻辑<br/>
        /// </summary>
        public virtual void HoldoutAI()
        {

        }
        /// <summary>
        /// 删除条件<br/>
        /// </summary>
        public virtual void InDel()
        {
            Projectile.Kill();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ExtraPreDraw(ref lightColor);
            return false;
        }
        public virtual bool ExtraPreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            float drawRotation = Projectile.rotation + (Owner.direction == -1 ? MathHelper.Pi : 0f) +  RotOffset * Owner.direction;

            Vector2 rotationPoint = RotPoint;

            SpriteEffects flipSprite = (Owner.direction * Main.player[Projectile.owner].gravDir == -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Main.spriteBatch.Draw(texture, drawPosition, null, Projectile.GetAlpha(lightColor), drawRotation, rotationPoint, Projectile.scale * Main.player[Projectile.owner].gravDir, flipSprite, default);
            MorePreDraw(ref lightColor);
            return false;
        }
        public virtual void MorePreDraw(ref Color lightColor)
        {
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }
    }
}
