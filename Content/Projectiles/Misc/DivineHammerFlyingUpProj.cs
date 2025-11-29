using CalamityMod;
using CalamityMod.Graphics.Primitives;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Assets.Sounds;
using UCA.Content.Items.Weapons.Rogue;
using UCA.Content.Projectiles.Rogue.DivineProj;
using UCA.Core.BaseClass;
using UCA.Core.Utilities;

namespace UCA.Content.Projectiles.Misc
{
    public class DivineHammerFlyingUpProj : RogueProjClass, ILocalizedModType
    {
        public override string Texture => ModContent.GetInstance<DivineHammer>().Texture;
        private ref float Timer => ref Projectile.ai[0];
        public static SpriteBatch SB { get=>Main.spriteBatch;}
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            //这个东西无所谓，因为弑神锤本身不会有任何伤害
            Projectile.width = Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.extraUpdates = 0;
            Projectile.knockBack = 0f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }
        private float Rosliate = 0f;
        public override void AI()
        {
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);
            ref float ThisType = ref Projectile.ai[1];
            //初始投掷上去时候先减速
            Timer++;
            Rosliate += 0.525f;
            if (ThisType == 0f)
            {
                Projectile.velocity *= 0.75f;
                //一定程度上修改射弹中心
                Vector2 anchorPos = new Vector2(Owner.Center.X, Projectile.Center.Y - (15f + 15f * (MathF.Sin(Rosliate) / 5f)));
                Projectile.Center = Vector2.Lerp(Projectile.Center, anchorPos, 0.1f);
                //速度为零切换状态
                if (Projectile.velocity.Length() < 0.2f)
                {
                    if (Timer > 25f)
                    {
                        Projectile.velocity = new Vector2(0f, -6f);
                        Projectile.extraUpdates = 2;
                        Timer = 1;
                        ThisType = 1;
                        SoundEngine.PlaySound(SoundsMenu.SwordHit, Projectile.Center);
                    }
                }
            }
            else if (ThisType == 1f)
            {
                Projectile.velocity.Y -= MathHelper.Clamp(Timer, 0f, 6f);
                //检查是否超出玩家屏幕，超出玩家屏幕则处死
                if (LAPUtilities.OutOffScreen(Projectile.Center))
                {
                    Projectile.Opacity = 0f;
                    Projectile.alpha = 0;
                    ThisType = 2;
                    Timer = 0;
                }
            }
            else if (Timer > 30f)
            {
                Projectile.Kill();
                Projectile.Center = new(Owner.Center.X, Owner.Center.Y - 1800f);
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
        public override void OnKill(int timeLeft)
        {
            //杀死这个射弹，并在玩家偏移的一定位置落下新的大锤
                
            Vector2 newCenter = new(Owner.Center.X + Owner.direction * Main.rand.NextFloat(270f, 320f), Owner.Center.Y - Main.rand.NextFloat(1400f, 1600f));
            if (Main.zenithWorld)
                newCenter.X = Owner.Center.X + Owner.direction * Main.rand.NextFloat(0);
            Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), newCenter, new Vector2(0f, 1f), ModContent.ProjectileType<ThunderHammerFallenDown>(), 0, 0f, Owner.whoAmI);
            //砸死你砸死你砸死你砸死你
            proj.damage = 400000;
            proj.penetrate = -1;
            proj.usesLocalNPCImmunity = true;
            proj.localNPCHitCooldown = 100;
        }
        public float SetProjWidth(float ratio)
        {
            float width = 66;
            width *= MathHelper.SmoothStep(0.8f, 0.6f, Utils.GetLerpValue(0f, 0.5f, ratio, true));
            return width;
        }
        public Color SetTrailColor(float ratio)
        {
            float velocityOpacityFadeout = Utils.GetLerpValue(2f, 5f, Projectile.velocity.Length(), true);
            Color c = DivineHammerProj.TrailColor * Projectile.Opacity * (1f - ratio);
            return c * Utils.GetLerpValue(0.04f, 0.1f, ratio, true) * velocityOpacityFadeout;
        }
        public Vector2 PrimitiveOffsetFunction(float ratio)
        {
            Vector2 off = Projectile.Size * 0.5f + Projectile.velocity.SafeNormalize(Vector2.Zero) * Projectile.scale * 0.2f * Vector2.UnitX;
            return off;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[1] == 2f)
                return false;

            Projectile.QuickDrawBloomEdge(rotOffset: -MathHelper.PiOver4);
            Projectile.QuickDrawWithTrailing(0.8f, Color.White, -MathHelper.PiOver4);
            if (!LAPUtilities.OutOffScreen(Projectile.Center) && Projectile.ai[1] == 1f && Timer > 3)
            {
                SB.EnterShaderRegion(BlendState.Additive);
                float spinRotation = Main.GlobalTimeWrappedHourly * 5.2f;
                GameShaders.Misc["CalamityMod:SideStreakTrail"].UseImage1("Images/Misc/Perlin");
                PrimitiveRenderer.RenderTrail(Projectile.oldPos, new(SetProjWidth, SetTrailColor, PrimitiveOffsetFunction, shader: GameShaders.Misc["CalamityMod:SideStreakTrail"]), 51);
                SB.ExitShaderRegion();
            }
            return false;
        }
    }
}
