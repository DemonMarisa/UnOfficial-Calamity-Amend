using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Assets.Sounds;
using UCA.Content.Configs;
using UCA.Content.MetaBalls;
using UCA.Content.Particiles;
using UCA.Content.Projectiles.HealPRoj;
using UCA.Core.BaseClass;
using UCA.Core.Utilities;

namespace UCA.Content.Projectiles.Magic.Ray
{
    public class NebulaCrystal : BaseMagicProj
    {
        public override string Texture => UCATextureRegister.InvisibleTexturePath;
        public bool CanHit = true;
        public float DustCount = 5f;
        public int MaxLife = 90;
        public int Filp;
        public float Vel;
        public Vector2 OldPos;
        public Vector2 OldPos2;
        public Vector2 OldStarPos;
        public bool PlayerHitEffect => Projectile.ai[1] != 0;
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.extraUpdates = 3;
            Projectile.friendly = true;
            Projectile.timeLeft = MaxLife;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }
        public override bool? CanHitNPC(NPC target)
        {
            return base.CanHitNPC(target);
        }
        public override void AI()
        {
            if (Projectile.LAP().FirstFrame)
            {
                Projectile.netUpdate = true;
                for (int j = 0; j < 5; j++)
                {
                    Color RandomColor = Color.Lerp(Color.Violet, Color.Purple, Main.rand.NextFloat(0, 1));
                    new MediumGlowBall(Projectile.Center, RandomColor, 60, 0.2f, Main.rand.NextFloat(1.6f, 2f)).Spawn();
                }
                for (int i = 0; i < 5; i++)
                {
                    Vector2 spawnVec = Vector2.UnitX.RotateRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.2f, 0.3f) * 12;
                    NebulaMetaBall.SpawnParticle(Projectile.Center, spawnVec, 0.2f, 45);
                }
                OldPos = Projectile.Center;
                Vel = Projectile.velocity.Length();
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (!LAPUtilities.OutOffScreen(Projectile.Center, 0.5f))
            {
                if (UCAConfig.Instance.PerformanceMode)
                    DustCount = 3f;
                for (int i = 0; i < DustCount; i++)
                {
                    NebulaMetaBall.SpawnParticle(Projectile.Center + Projectile.velocity / DustCount * i, Vector2.Zero, 0.1f, 45);
                }
                Vector2 offset = Main.rand.NextVector2Circular(6, 6);
                if (!UCAConfig.Instance.PerformanceMode)
                    new Fire(Projectile.Center + offset, Projectile.velocity * 0.3f, Color.Violet, 30, Main.rand.NextFloat(MathHelper.TwoPi), 1, 0.2f).Spawn();
                else
                    new Fire(Projectile.Center + offset, Projectile.velocity * 0.3f, Color.Violet, 15, Main.rand.NextFloat(MathHelper.TwoPi), 1, 0.2f).Spawn();
            }

            if (Projectile.timeLeft % 15 == 0)
            {
                if (LAPUtilities.OutOffScreen(Projectile.Center, 0.5f))
                    return;

                if (Projectile.velocity.Length() > 1)
                {
                    Vector2 GenPosOffset = Main.rand.NextVector2Circular(45, 45);

                    if (!UCAConfig.Instance.PerformanceMode)
                    {
                        Color color = Color.Lerp(Color.DarkViolet, Color.Violet, Main.rand.NextFloat());
                        new CrossGlow(Projectile.Center + GenPosOffset, Vector2.Zero, color, 60, 1f, 0.1f, false).Spawn();
                    }

                    if (OldStarPos != Vector2.Zero)
                    {
                        if (!UCAConfig.Instance.PerformanceMode)
                            UCAUtilities.GenStarLine(OldStarPos, Projectile.Center + GenPosOffset, 50, Color.Violet);
                        else
                            UCAUtilities.GenStarLine(OldStarPos, Projectile.Center + GenPosOffset, 25, Color.Violet);
                    }

                    OldStarPos = Projectile.Center + GenPosOffset;
                }
            }

            if (PlayerHitEffect)
            {
                for (int i = 0; i < 10; i++)
                {
                    Vector2 spawnVec = Projectile.velocity.RotateRandom(MathHelper.PiOver4 * 0.5f) * Main.rand.NextFloat(0.2f, 1f);
                    NebulaMetaBall.SpawnParticle(Projectile.Center, spawnVec, 0.2f, 65);
                }
                SoundEngine.PlaySound(SoundsMenu.MetalHit, Projectile.Center);
                Projectile.ai[1] = 0;
            }

        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.LAP().OnceHitEffect)
            {
                Projectile.ai[1]++;
                Projectile.netUpdate = true;

                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * 3f, ModContent.ProjectileType<NebulaHeal>(), 0, 0, Projectile.owner, 1);
                return;
            }
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                Vector2 spawnVec = Vector2.UnitX.RotateRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.2f, 0.3f) * 12;
                NebulaMetaBall.SpawnParticle(Projectile.Center, spawnVec, 0.2f, 45);
            }

            for (int i = 0; i < 8; i++)
            {
                Vector2 spawnVec = Vector2.UnitX.RotateRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.6f, 1f) * 12;
                Color color = LAPUtilities.LerpColor(Color.Violet, Color.LightPink);
                new BrokenGlass(Projectile.Center, spawnVec, color, Main.rand.Next(45, 60), Main.rand.NextFloat(MathHelper.TwoPi), 1f, 0.2f, false).Spawn();
            }
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overWiresUI.Add(index);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            LAPUtilities.ReSetToBeginShader();
            if (!UCAConfig.Instance.PerformanceMode)
            {
                Texture2D texture = UCATextureRegister.CrossGlow.Value;
                Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.Violet, 0, texture.Size() / 2, Projectile.scale * 0.2f * new Vector2(1f, 1f), SpriteEffects.FlipHorizontally, 0f);
                Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.DarkViolet, 0, texture.Size() / 2, Projectile.scale * 0.15f * new Vector2(1f, 1f), SpriteEffects.None, 0f);
            }
            Texture2D Crystal = UCATextureRegister.Crystal.Value;
            Main.spriteBatch.Draw(Crystal, Projectile.Center - Main.screenPosition, null, Color.Violet, Projectile.rotation, Crystal.Size() / 2, Projectile.scale * 0.2f, SpriteEffects.FlipHorizontally, 0f);
            LAPUtilities.ReSetToEndShader();
            return false;
        }
    }
}
