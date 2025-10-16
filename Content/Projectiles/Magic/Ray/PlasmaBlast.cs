using CalamityMod;
using CalamityMod.Graphics.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Core.BaseClass;
using UCA.Core.Utilities;

namespace UCA.Content.Projectiles.Magic.Ray
{
    public class PlasmaBlast : BaseMagicProj, IPixelatedPrimitiveRenderer
    {
        public PixelationPrimitiveLayer Layer = PixelationPrimitiveLayer.AfterPlayers;
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
            if (Projectile.UCA().FirstFrame)
            {
                Rot = Main.rand.NextFloat(0, MathHelper.TwoPi);
                Rot2 = Main.rand.NextFloat(0, MathHelper.TwoPi);
                Rot3 = Main.rand.NextFloat(0, MathHelper.TwoPi);
            }
            Scale = MathHelper.Lerp(0f, 1f, 1 - EasingHelper.EaseInCubic(Projectile.timeLeft / 30f));

            if (Projectile.timeLeft < 15)
                Opacity = MathHelper.Lerp(1f, 0f, 1 -  EasingHelper.EaseInCubic(Projectile.timeLeft / 15f));
        }

        public void RenderPixelatedPrimitives(SpriteBatch spriteBatch, PixelationPrimitiveLayer layer)
        {
            Vector2 DrawPos = Projectile.Center - Main.screenPosition;

            Color ThirdColor = new(148, 0, 211, 0);
            ThirdColor = ThirdColor * Opacity;

            Color SecondColor = new(218, 112, 214, 0);
            SecondColor = SecondColor * Opacity;

            Color color = new(255, 0, 255, 0);
            color = color * Opacity;

            spriteBatch.Draw(UCATextureRegister.Ring04.Value, DrawPos / 2, null, SecondColor, Rot, UCATextureRegister.Ring04.Size() / 2, Scale * 0.6f, SpriteEffects.None, 0);
            spriteBatch.Draw(UCATextureRegister.Ring04.Value, DrawPos / 2, null, color, Rot2, UCATextureRegister.Ring04.Size() / 2, Scale * 0.5f, SpriteEffects.None, 0);
            spriteBatch.Draw(UCATextureRegister.Ring04.Value, DrawPos / 2, null, ThirdColor, Rot3, UCATextureRegister.Ring04.Size() / 2, Scale * 0.7f, SpriteEffects.None, 0);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.ShadowFlame, 180); 
        }
    }
}
