using CalamityMod;
using CalamityMod.Graphics.Primitives;
using CalamityMod.Particles;
using LAP.Core.ParticleSystem;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Assets.Sounds;
using UCA.Content.Items.Weapons.Rogue;
using UCA.Content.Particiles;
using UCA.Core.BaseClass;
using UCA.Core.Utilities;

namespace UCA.Content.Projectiles.Rogue.DivineProj
{
    public class DivineHammerProjClone: BaseRogueProj
    {
        public override string Texture => ModContent.GetInstance<DivineHammer>().Texture;
        private enum DoType
        {
            IsShooted,
            IsReturn,
            IsReverse
        }
        private DoType AttackType
        {
            get => (DoType)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }
        private ref float AttackTimer => ref Projectile.ai[1];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 30;
            ProjectileID.Sets.TrailingMode[Type] = 2;

        }
        public override void ExSD()
        {
            Projectile.width = 86;
            Projectile.height = 72;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            Projectile.extraUpdates = 6;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            switch (AttackType)
            {
                case DoType.IsShooted:
                    DoShooted();
                    break;
                case DoType.IsReturn:
                    DoReturn();
                    break;
                case DoType.IsReverse:
                    DoReverse();
                    break;
            }
        }
        private void DoShooted()
        {
            AttackTimer += 1;
            if (AttackTimer > 65f)
            {
                Projectile.netUpdate = true;
                AttackTimer = 0;
                AttackType = DoType.IsReturn;
            }
        }
        private void DoReturn()
        {
            Projectile.AccelerateToTarget(Owner.Center, 28f, 0.4f);
            if (Projectile.Hitbox.Intersects(Owner.Hitbox))
            {
                AttackType = DoType.IsReverse;
                Projectile.localNPCHitCooldown = 45;
                Projectile.netUpdate = true;
            }
        }
        private void DoReverse()
        {
            AttackTimer += 1;
            if (AttackTimer > 10f)
            {
                AttackTimer = 10f;
                if (Projectile.GetTargetSafe(out NPC target))
                {
                    Projectile.HomingNPCBetter(target, 24f, 18f, 2);
                    if (Projectile.Hitbox.Intersects(target.Hitbox))
                        Projectile.Kill();
                }
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundStyle pickSound2 = Utils.SelectRandom(Main.rand, SoundsMenu.Smash_GroundHeavy);
            SoundEngine.PlaySound(pickSound2 with { Pitch = Main.rand.NextFloat(0.8f, 0.7f), Volume = 0.7f, MaxInstances = 1 }, target.Center);
            PrettySpark(hit.Damage);
        }
        private void PrettySpark(int hitDamage)
        {
            //圆环
            Vector2 dir = Projectile.velocity.SafeNormalize(Vector2.UnitX) * Projectile.scale;
            for (int i = 0; i < 36; i++)
            {
                Vector2 dir2 = MathHelper.ToRadians(i * 10f).ToRotationVector2() * Projectile.scale;
                dir2.X /= 3.6f;
                dir2 = dir2.RotatedBy(Projectile.velocity.ToRotation());
                Vector2 pos = Projectile.Center + dir * 12f + dir2 * 18f;
                ShinyOrbParticle shinyOrbParticle = new ShinyOrbParticle(pos, dir2 * 5f, Main.rand.NextBool() ? Color.White : Color.HotPink, 40, 3.5f - Math.Abs(18f - i) / 6f, BlendStateID.Additive);
                shinyOrbParticle.Spawn();
            }
            //从灾厄抄写的锤子特效
            float damageInterpolant = Utils.GetLerpValue(950f, 2000f, hitDamage, true);
            Vector2 splatterDirection = Projectile.velocity * 0.8f;
            for (int i = 0; i < 10; i++)
            {
                int sparkLifetime = Main.rand.Next(55, 70);
                float sparkScale = Main.rand.NextFloat(0.7f, Main.rand.NextFloat(3.3f, 5.5f)) + damageInterpolant * 0.85f;
                Color sparkColor = Color.Lerp(Color.Purple, Color.GhostWhite, Main.rand.NextFloat(0.7f));
                sparkColor = Color.Lerp(sparkColor, Color.HotPink, Main.rand.NextFloat());

                Vector2 sparkVelocity = splatterDirection.RotatedByRandom(0.7f) * Main.rand.NextFloat(1.4f, 1.8f);
                sparkVelocity.Y -= 7f;
                SparkParticle spark = new(Projectile.Center, sparkVelocity, false, sparkLifetime, sparkScale, sparkColor);
                GeneralParticleHandler.SpawnParticle(spark);
            }
        }

        private SpriteBatch SB { get => Main.spriteBatch; }
        #region DrawMethod
        public float SetProjWidth(float ratio)
        {
            float width = Projectile.width;
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
        #endregion
        //TODO：下面那个轨迹把归元漩涡的轨迹改成另外一种，现在这个纯纯占位符
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.QuickDrawBloomEdge(rotOffset: -MathHelper.PiOver4);
            Projectile.QuickDrawWithTrailing(0.7f, Color.White, 4, -MathHelper.PiOver4);
            if (!LAPUtilities.OutOffScreen(Projectile.Center))
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
