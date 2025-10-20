using CalamityMod;
using CalamityMod.Graphics.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Assets.Effects;
using UCA.Content.Items;
using UCA.Core.Utilities;

namespace UCA.Content.Projectiles.Misc.Test
{
    public class TestProj : ModProjectile, ILocalizedModType
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<Sword>();

        public override string Texture => UCATextureRegister.InvisibleTexturePath;
        public Player Owner => Main.player[Projectile.owner];
        public List<Vector2> pos = [];
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.timeLeft = 120;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 0;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10 * (Projectile.extraUpdates + 1);
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.ai[0] = Main.rand.NextFloat(3, 5);
            Projectile.ai[2] = Main.rand.NextFloat(24f, 36f);
            Main.NewText("OnSpawn");
        }

        public override void AI()
        {
            Projectile.timeLeft = 2;
            Projectile.Center = Main.MouseWorld;
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            UCAUtilities.ReSetToBeginShader(BlendState.Additive);

            UCAShaderRegister.PolarDistortShader.Parameters["uWidthMult"].SetValue(2f);
            UCAShaderRegister.PolarDistortShader.Parameters["uRingMult"].SetValue(1f);
            UCAShaderRegister.PolarDistortShader.Parameters["uYTime"].SetValue(Main.GlobalTimeWrappedHourly * 0.1f);
            UCAShaderRegister.PolarDistortShader.CurrentTechnique.Passes[0].Apply();
            Main.instance.GraphicsDevice.Textures[1] = UCATextureRegister.BloomShockwave.Value;

            Texture2D texture = UCATextureRegister.MiscNoise01.Value;
            Vector2 orig = texture.Size() / 2;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.Orange, 0, orig, 1f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.Orange, 0, orig, 1f, SpriteEffects.None, 0);
            texture = UCATextureRegister.MiscNoise02.Value;
            orig = texture.Size() / 2;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.OrangeRed, 2, orig, 1f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.Red, 2, orig, 1f, SpriteEffects.None, 0);

            UCAUtilities.ReSetToEndShader();
            return false;
        }
    }
}
