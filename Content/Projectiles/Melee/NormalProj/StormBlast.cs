using LAP.Assets.Sounds;
using LAP.Assets.TextureRegister;
using LAP.Content.Particles;
using LAP.Core.SystemsLoader;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using UCA.Core.BaseClass;

namespace UCA.Content.Projectiles.Melee.NormalProj
{
    public class StormBlast : BaseMeleeProj
    {
        public override string Texture => LAPTextureRegister.InvisibleTexturePath;
        public override void SetStaticDefaults()
        {
            Projectile.AddProtectedProj();
        }
        public override void SetDefaults()
        {
            Projectile.width = 200;
            Projectile.height = 200;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 45;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 45;
            Projectile.scale = 0f;
            Projectile.Opacity = 1f;
            Projectile.noEnchantmentVisuals = true;
            Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return LAPUtilities.CircularHitboxCollision(Projectile.Center, 300, targetHitbox);
        }
        public override void AI()
        {
            if (Projectile.LAP().FirstFrame)
            {
                SoundEngine.PlaySound(LAPSoundsMenu.WeaponSkillSound with { Pitch = Main.rand.NextFloat(0.2f, 0.8f) }, Projectile.Center);
                Vector2 Center = Projectile.Center;
                for (int i = 0; i < 15; i++)
                {
                    Vector2 vel = new Vector2(12, 0).RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.5f, 1.5f);
                    new CampSmoke(Center, vel, Color.White, 45, Main.rand.NextFloat(MathHelper.TwoPi), 0.5f, Main.rand.NextFloat(0.3f, 0.5f)).Spawn();
                }
                for (int i = 0; i < 25; i++)
                {
                    Vector2 vel = new Vector2(24, 0).RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.5f, 1.5f);
                    new Fire(Center, vel, Color.White, 25, Main.rand.NextFloat(MathHelper.TwoPi), 0.5f, Main.rand.NextFloat(0.3f, 0.5f)).Spawn();
                }
                for (int i = 0; i < 30; i++)
                {
                    new SmallGlowBall(Center, Vector2.Zero, Color.White, Main.rand.Next(15, 80), 0.3f, Main.rand.NextFloat(0f, 16f)).Spawn();
                }
                for (int i = 0; i < 18; i++)
                {
                    Vector2 vel = new Vector2(12, 0).RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.5f, 1.5f);
                    new TrailGlowBall(Center, vel, Color.White, Main.rand.Next(15, 45), 0.2f, true).Spawn();
                }
            }
            Projectile.scale += 0.15f;
            Projectile.scale *= 1.05f;
            if (Projectile.Opacity > 0)
                Projectile.Opacity -= 0.1f;
            else
                Projectile.Opacity = 0;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = LAPTextureRegister.Shockwave_01.Value;
            Vector2 Pos = Projectile.Center - Main.screenPosition;
            LAPUtilities.Draw(texture, Pos, null, Color.White with { A = 0 } * Projectile.Opacity, Projectile.rotation, texture.Size() / 2, Projectile.scale, 0, 0);
            LAPUtilities.Draw(texture, Pos, null, Color.White with { A = 0 } * Projectile.Opacity, Projectile.rotation, texture.Size() / 2, Projectile.scale, 0, 0);
            LAPUtilities.Draw(texture, Pos, null, Color.White with { A = 0 } * Projectile.Opacity, Projectile.rotation, texture.Size() / 2, Projectile.scale, 0, 0);
            LAPUtilities.Draw(texture, Pos, null, Color.White with { A = 0 } * Projectile.Opacity, Projectile.rotation, texture.Size() / 2, Projectile.scale, 0, 0);
            LAPUtilities.Draw(texture, Pos, null, Color.White with { A = 0 } * Projectile.Opacity, Projectile.rotation, texture.Size() / 2, Projectile.scale, 0, 0);
            return false;
        }
    }
}
