using CalamityMod;
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
    public class NebulaEnergy : BaseMagicProj
    {
        public override string Texture => UCATextureRegister.InvisibleTexturePath;
        public bool CanHit = true;
        public int MaxLife = 360;
        public float DustCount = 5f;
        public int Time;
        public int Filp;
        public float Vel;
        public Vector2 OldPos;
        public Vector2 OldPos2;
        public bool CanHomeIn => Projectile.ai[0] == 0;
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
            if (!CanHomeIn)
                return base.CanHitNPC(target);
            if (Time < 30)
                return false;
            else
                return base.CanHitNPC(target);
        }
        public override void AI()
        {
            if (Projectile.UCA().FirstFrame)
            {
                if (!CanHomeIn)
                    Projectile.penetrate = -1;
                Projectile.netUpdate = true;
                for (int j = 0; j < 10; j++)
                {
                    Color RandomColor = Color.Lerp(Color.Violet, Color.Purple, Main.rand.NextFloat(0, 1));
                    new MediumGlowBall(Projectile.Center, RandomColor, 60, 0.2f, Main.rand.NextFloat(1.6f, 2f)).Spawn();
                }
                for (int i = 0; i < 10; i++)
                {
                    Vector2 spawnVec = Vector2.UnitX.RotateRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.2f, 0.3f) * 12;
                    NebulaMetaBall.SpawnParticle(Projectile.Center, spawnVec, 0.2f, 45);
                }
                Filp = Main.rand.NextBool() ? 1 : -1;
                OldPos = Projectile.Center;
                Vel = Projectile.velocity.Length();
            }
            Time++;
            if (!UCAUtilities.OutOffScreen(Projectile.Center))
            {
                for (int i = 0; i < DustCount; i++)
                {
                    NebulaMetaBall.SpawnParticle(Projectile.Center + Projectile.velocity / DustCount * i, Vector2.Zero, 0.08f, 45);
                }

                Vector2 SpawnPos = Projectile.Center + Vector2.UnitX.RotatedBy(Time * 0.1f * Filp) * 5;
                for (int i = 0; i < 3; i++)
                {
                    Vector2 finalPos = Vector2.Lerp(OldPos, SpawnPos, i / 3f);
                    NebulaMetaBall.SpawnParticle(finalPos, Vector2.Zero, 0.05f, 45);
                }
                SpawnPos = Projectile.Center + Vector2.UnitX.RotatedBy(Time * 0.1f * Filp) * 10;

                for (int i = 0; i < 3; i++)
                {
                    Vector2 finalPos = Vector2.Lerp(OldPos2, SpawnPos, i / 3f);
                    NebulaMetaBall.SpawnParticle(finalPos, Vector2.Zero, 0.05f, 45);
                }
                OldPos = SpawnPos;
                OldPos2 = SpawnPos;
            }

            if (Projectile.timeLeft % 20 == 0)
            {
                if (UCAUtilities.OutOffScreen(Projectile.Center))
                    return;
                if (Projectile.velocity.Length() > 1)
                {             
                    Vector2 GenPosOffset = Main.rand.NextVector2Circular(20, 20);
                    Color color = Color.Lerp(Color.DarkViolet, Color.Violet, Main.rand.NextFloat());
                    new CrossGlow(Projectile.Center + GenPosOffset, Vector2.Zero, color, 60, 1f, 0.1f, false).Spawn();
                }
            }

            if (PlayerHitEffect)
            {
                for (int i = 0; i < 15; i++)
                {
                    Vector2 spawnVec = Projectile.velocity.RotateRandom(MathHelper.PiOver4 * 0.5f) * Main.rand.NextFloat(0.2f, 1f);
                    NebulaMetaBall.SpawnParticle(Projectile.Center, spawnVec, 0.2f, 45);
                }
                Projectile.ai[1] = 0;
            }

            if (!CanHomeIn)
                return;
            NPC target = Projectile.FindClosestTarget(2500);
            if (target is not null)
            {
                Projectile.HomingTarget(target.Center, 2500, 12f, 70f);
            }
            else
                Projectile.velocity *= 0.96f;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.UCA().OnceHitEffect && !CanHomeIn)
            {
                Projectile.ai[1]++;
                Projectile.netUpdate = true;
                return;
            }
        }
        public override void OnKill(int timeLeft)
        {

            for (int i = 0; i < 10; i++)
            {
                Vector2 spawnVec = Vector2.UnitX.RotateRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.2f, 0.3f) * 12;
                NebulaMetaBall.SpawnParticle(Projectile.Center, spawnVec, 0.2f, 45);
            }
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overWiresUI.Add(index);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            UCAUtilities.ReSetToBeginShader();
            Texture2D texture = UCATextureRegister.CrossGlow.Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.Violet, 0, texture.Size() / 2, Projectile.scale * 0.2f * new Vector2(1.25f, 1f), SpriteEffects.FlipHorizontally, 0f);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.DarkViolet, 0, texture.Size() / 2, Projectile.scale * 0.15f * new Vector2(1.25f, 1f), SpriteEffects.None, 0f);
            UCAUtilities.ReSetToEndShader();
            return false;
        }
    }
}
