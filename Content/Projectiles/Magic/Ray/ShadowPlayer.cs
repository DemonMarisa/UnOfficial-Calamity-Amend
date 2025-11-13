using CalamityMod.DataStructures;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using UCA.Assets;
using UCA.Content.Configs;
using UCA.Content.Particiles;
using UCA.Core.BaseClass;

namespace UCA.Content.Projectiles.Magic.Ray
{
    public class ShadowPlayer : BaseMagicProj
    {
        public override string Texture => UCATextureRegister.InvisibleTexturePath;
        public Vector2 offset = new Vector2(0, -6);
        public ref float AttackTimer => ref Projectile.ai[0];
        public ref float ToNewPos => ref Projectile.ai[1];
        public float StaffRot;
        public Player Owner => Main.player[Projectile.owner];
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 48;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 10;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 1800;
            Projectile.extraUpdates = 0;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 30;
        }

        public override void AI()
        {
            if (Owner.dead || !Owner.active)
                Projectile.Kill();
            if (Projectile.LAP().FirstFrame)
            {
                float DistanceToPlayer = Vector2.Distance(Owner.Center , Projectile.Center);
                DistanceToPlayer /= 16;
                if (UCAConfig.Instance.PerformanceMode)
                    DistanceToPlayer /= 2;
                GenLine(Owner.Center, Projectile.Center, DistanceToPlayer);
                StaffRot = Owner.GetToMouseVector2(Projectile.Center).ToRotation();
                if (!LAPUtilities.OutOffScreen(Projectile.Center, 0.4f))
                {
                    for (int i = 0; i < 35; i++)
                    {
                        float offset = MathHelper.TwoPi / 35;
                        Color RandomColor = Color.Lerp(Color.DarkViolet, Color.LightPink, Main.rand.NextFloat(0, 1));
                        new MediumGlowBall(Projectile.Center, Projectile.velocity.RotatedBy(offset * i), RandomColor, 60, 0, 1, 0.2f, Main.rand.NextFloat(2f, 2.2f)).Spawn();
                    }
                }
            }
            if (!LAPUtilities.OutOffScreen(Projectile.Center, 0.4f))
            {
                if (UCAConfig.Instance.PerformanceMode)
                {
                    Vector2 GenPos = Projectile.Center + new Vector2(Main.rand.Next(-16, 16), Main.rand.Next(-24, 24));
                    Color Firecolor = LAPUtilities.LerpColor(Color.Black, Color.DarkViolet);
                    Vector2 fireVel = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.2f, 1.2f);
                    new Fire(GenPos, fireVel, Firecolor, 90, Main.rand.NextFloat(MathHelper.TwoPi), 1f, 0.2f).SpawnToPriorityNonPreMult();
                }
                else
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Vector2 GenPos = Projectile.Center + new Vector2(Main.rand.Next(-16, 16), Main.rand.Next(-24, 24));
                        Color Firecolor = LAPUtilities.LerpColor(Color.Black, Color.DarkViolet);
                        Vector2 fireVel = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.2f, 1.2f);
                        new Fire(GenPos, fireVel, Firecolor, 90, Main.rand.NextFloat(MathHelper.TwoPi), 1f, 0.2f).SpawnToPriorityNonPreMult();
                    }
                }
            }
            AttackTimer++;
            if (AttackTimer > 60)
            {
                NPC npc = Projectile.FindClosestTarget(3000);
                if (npc is not null)
                {
                    SoundEngine.PlaySound(SoundID.Item91, Projectile.Center);
                    Vector2 direction = (npc.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 12;
                    int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, direction, ModContent.ProjectileType<ShadowBeam>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner, 0.6f);
                    Main.projectile[p].penetrate = 1;
                    Main.projectile[p].tileCollide = false;
                    StaffRot = direction.ToRotation();
                    for (int i = 0; i < 35; i++)
                    {
                        float offset = MathHelper.TwoPi / 35;
                        Color RandomColor = Color.Lerp(Color.DarkViolet, Color.LightPink, Main.rand.NextFloat(0, 1));
                        new MediumGlowBall(Projectile.Center, Projectile.velocity.RotatedBy(offset * i), RandomColor, 60, 0, 1, 0.2f, Main.rand.NextFloat(2f, 2.2f)).Spawn();
                    }
                }
                AttackTimer = 0;
            }
            if (ToNewPos != 0)
            {
                Projectile.Kill();
                ToNewPos = 0;
            }
        }

        public void GenLine(Vector2 BeginPos, Vector2 EndPos, float GenStep)
        {
            float AddProgress = MathHelper.Pi / GenStep;
            for (int i = 0; i < GenStep; i++)
            {
                float YAdd = (float)(Math.Sin(AddProgress * i) * 100);
                Vector2 SpawnVector = Vector2.Lerp(BeginPos, EndPos, i / GenStep);
                SpawnVector.Y += YAdd;
                Color color = LAPUtilities.LerpColor(Color.Black, Color.DarkViolet);
                new Fire(SpawnVector, Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.6f, 1.2f), color, 90, Main.rand.NextFloat(MathHelper.TwoPi), 1f, 0.3f).SpawnToPriorityNonPreMult();
            }
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 50; i++)
            {
                Color Firecolor = LAPUtilities.LerpColor(Color.Black, Color.DarkViolet);
                new Fire(Projectile.Center, Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.6f, 1.2f) * 12, Firecolor, 90, Main.rand.NextFloat(MathHelper.TwoPi), 1f, 0.3f).SpawnToPriorityNonPreMult();
            }
            for (int i = 0; i < 35; i++)
            {
                float offset = MathHelper.TwoPi / 35;
                Color RandomColor = Color.Lerp(Color.DarkViolet, Color.LightPink, Main.rand.NextFloat(0, 1));
                new MediumGlowBall(Projectile.Center, Projectile.velocity.RotatedBy(offset * i), RandomColor, 90, 0, 1, 0.2f, Main.rand.NextFloat(2f, 6f)).Spawn();
            }

            Vector2 GetVec = LAPUtilities.GetVector2(Owner.Center, Projectile.Center);
            int MaxDistance = Main.rand.Next(150, 300);

            float AddProgress = MathHelper.Pi / 25f;
            for (int i = 0; i < 50; i++)
            {
                float YAdd = MathF.Sin(AddProgress * i) * 100;
                Vector2 GenPos = Vector2.Lerp(Owner.Center, Owner.Center + GetVec * MaxDistance, i / 25f);
                GenPos.Y += YAdd;
                Color Firecolor = LAPUtilities.LerpColor(Color.Black, Color.DarkViolet);
                Vector2 fireVel = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.2f, 1.2f);
                new Fire(GenPos, fireVel, Firecolor, 90, Main.rand.NextFloat(MathHelper.TwoPi), 1f, 0.2f).SpawnToPriorityNonPreMult();
            }
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overWiresUI.Add(index);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            LAPUtilities.ReSetToBeginShader();
            Texture2D texture = UCATextureRegister.CrossGlow.Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + offset, null, Color.Violet, 0, texture.Size() / 2, Projectile.scale * 0.2f * new Vector2(1.25f, 1f), SpriteEffects.FlipHorizontally, 0f);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + offset, null, Color.DarkViolet, 0, texture.Size() / 2, Projectile.scale * 0.15f * new Vector2(1.25f, 1f), SpriteEffects.None, 0f);
            LAPUtilities.ReSetToEndShader();
            return false;
        }
        public void DrawStaff()
        {

        }
    }
}
