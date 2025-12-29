using CalamityMod;
using LAP.Core.Enums;
using LAP.Core.Graphics.PixelatedRender;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Core.BaseClass;

namespace UCA.Content.Projectiles.Magic.Ray
{
    public class PlasmaBlast : BaseMagicProj, IPixelatedRenderer
    {
        public DrawLayer Layer = DrawLayer.BeforePlayer;
        public override string Texture => UCATextureRegister.InvisibleTexturePath;

        public float Scale = 0f;

        public float Opacity = 1f;

        public float Rot = 0;
        public float Rot2 = 0;
        public float Rot3 = 0;
        public override void SetDefaults()
        {
            Projectile.width = 200;
            Projectile.height = 200;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 30;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 45;
        }

        public override void OnSpawn(IEntitySource source)
        {
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return CalamityUtils.CircularHitboxCollision(Projectile.Center, 110, targetHitbox);
        }

        public override void AI()
        {
            PixelatedRenderManger.BeginDrawProj = true;
            if (Projectile.LAP().FirstFrame)
            {
                Rot = Main.rand.NextFloat(0, MathHelper.TwoPi);
                Rot2 = Main.rand.NextFloat(0, MathHelper.TwoPi);
                Rot3 = Main.rand.NextFloat(0, MathHelper.TwoPi);
            }
            Scale = MathHelper.Lerp(0f, 1f, 1 - EasingHelper.EaseInCubic(Projectile.timeLeft / 30f));

            if (Projectile.timeLeft < 15)
                Opacity = MathHelper.Lerp(1f, 0f, 1 -  EasingHelper.EaseInCubic(Projectile.timeLeft / 15f));
        }

        public void RenderPixelated(SpriteBatch spriteBatch)
        {
            Vector2 DrawPos = Projectile.Center - Main.screenPosition;

            Color ThirdColor = new(148, 0, 211, 0);
            ThirdColor = ThirdColor * Opacity;

            Color SecondColor = new(218, 112, 214, 0);
            SecondColor = SecondColor * Opacity;

            Color color = new(255, 0, 255, 0);
            color = color * Opacity;

            spriteBatch.Draw(UCATextureRegister.Ring04.Value, DrawPos , null, SecondColor, Rot, UCATextureRegister.Ring04.Size() / 2, Scale * 1.2f, SpriteEffects.None, 0);
            spriteBatch.Draw(UCATextureRegister.Ring04.Value, DrawPos , null, color, Rot2, UCATextureRegister.Ring04.Size() / 2, Scale, SpriteEffects.None, 0);
            spriteBatch.Draw(UCATextureRegister.Ring04.Value, DrawPos , null, ThirdColor, Rot3, UCATextureRegister.Ring04.Size() / 2, Scale * 1.4f, SpriteEffects.None, 0);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            PixelatedRenderManger.BeginDrawProj = true;
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.ShadowFlame, 180); 
        }
    }
}
