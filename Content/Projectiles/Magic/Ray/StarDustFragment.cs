using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Content.MetaBalls;
using UCA.Content.Particiles;
using UCA.Core.BaseClass;
using UCA.Core.Utilities;

namespace UCA.Content.Projectiles.Magic.Ray
{
    public class StarDustFragment : BaseMagicProj
    {
        public override string Texture => UCATextureRegister.CollectableLightPath;
        public bool CanHit = true;
        public int MaxLife = 90;
        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = MaxLife;
            Projectile.extraUpdates = 0;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }
        public override bool? CanHitNPC(NPC target)
        {
            if (target.friendly)
                return false;
            else
                return CanHit;
        }
        public override void AI()
        {
            if (Projectile.UCA().FirstFrame)
            {
                Projectile.scale = 0;
            }

            if (!CanHit)
            {
                Projectile.velocity *= 0.9f;
                if (Projectile.timeLeft % 4 == 0)
                {
                    Vector2 GenPosOffset = Main.rand.NextVector2Circular(10, 10);
                    Color color = Color.Lerp(Color.DarkBlue, Color.DeepSkyBlue, Main.rand.NextFloat());
                    new CrossGlow(Projectile.Center + GenPosOffset, Vector2.Zero, color, 30, 1f, 0.1f, false).Spawn();
                    new CrossGlow(Projectile.Center + GenPosOffset, Vector2.Zero, Color.White, 20, 1f, 0.02f, false).Spawn();
                }
            }
            else
            {
                if (Projectile.timeLeft > MaxLife / 2)
                {
                    Projectile.scale = MathHelper.Lerp(Projectile.scale, 1f, 0.1f);
                    Projectile.velocity *= 1.03f;
                }
                if (Projectile.timeLeft % 2 == 0)
                {
                    Vector2 GenPosOffset = Main.rand.NextVector2Circular(10, 10);
                    Color color = Color.Lerp(Color.DarkBlue, Color.DeepSkyBlue, Main.rand.NextFloat());
                    new CrossGlow(Projectile.Center + GenPosOffset, Vector2.Zero, color, 30, 1f, 0.1f, false).Spawn();
                    new CrossGlow(Projectile.Center + GenPosOffset, Vector2.Zero, Color.White, 20, 1f, 0.02f, false).Spawn();
                }
            }

            for (int i = 0; i < 6; i++)
            {
                Vector2 vec = Projectile.velocity / 6;
                StarDustMetaBall.SpawnParticle(Projectile.Center + vec * i, Vector2.Zero, 0.1f, 30);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            UCAUtilities.ReSetToBeginShader();
            Texture2D texture = UCATextureRegister.CrossGlow.Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.DarkBlue, 0, texture.Size() / 2, Projectile.scale * 0.2f * new Vector2(1.25f, 1f), SpriteEffects.FlipHorizontally, 0f);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.SkyBlue, 0, texture.Size() / 2, Projectile.scale * 0.15f * new Vector2(1.25f, 1f), SpriteEffects.None, 0f);
            UCAUtilities.ReSetToEndShader();
            return false;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overWiresUI.Add(index);
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 15; i++)
            {
                Color RandomColor = Color.Lerp(Color.SkyBlue, Color.DarkBlue, Main.rand.NextFloat(0, 1));
                new MediumGlowBall(Projectile.Center, RandomColor, 120, 0.2f, Main.rand.NextFloat(1f, 1.6f)).Spawn();
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            CanHit = false;
            Projectile.netUpdate = true;
            Projectile.timeLeft = 60;
        }
    }
}
