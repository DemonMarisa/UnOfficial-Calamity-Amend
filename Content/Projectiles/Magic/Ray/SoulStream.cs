using LAP.Content.Configs;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Content.Items.Weapons.Magic.Ray;
using UCA.Content.Particiles;
using UCA.Content.Projectiles.HeldProj.Magic.ElementRayHeld;
using UCA.Core.BaseClass;
using LAP.Assets.TextureRegister;
using LAP.Core.SystemsLoader;

namespace UCA.Content.Projectiles.Magic.Ray
{
    public class SoulStream : BaseMagicProj
    {
        public override string Texture => LAPTextureRegister.InvisibleTexturePath;
        public int MaxLife = 240;
        public ref float FollowProj => ref Projectile.ai[0];
        public bool FadeEnd => Projectile.ai[1] != 0;
        public bool BeginFire => Projectile.ai[2] != 0;
        public Projectile Father => Main.projectile[(int)FollowProj];
        public float LaserLength = 2000;
        public Vector2 OldPos;
        public int Time = 0;
        public Player Owner => Main.player[Projectile.owner];
        public float CutMult = 0.5f;
        public override void SetStaticDefaults()
        {
            // 保存旧朝向与旧位置
            ProjectileID.Sets.TrailingMode[Type] = 2;
            // 一共爆粗20个数据
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            Projectile.AddHeldProj();
        }
        public override void SetDefaults()
        {
            Projectile.width = 128;
            Projectile.height = 128;
            Projectile.extraUpdates = 0;
            Projectile.friendly = true;
            Projectile.timeLeft = MaxLife;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 3;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
            {
                return true;
            }
            float _ = float.NaN;
            Vector2 beamBeginPos = Projectile.Center;
            Vector2 beamEndPos = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * LaserLength;
            bool c = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), beamBeginPos, beamEndPos, 72f, ref _);
            return c;
        }
        public override bool? CanHitNPC(NPC target)
        {
            return base.CanHitNPC(target);
        }
        public override void AI()
        {
            Time++;
            if (Father.type != ModContent.ProjectileType<ElementRaySpecialHeldProj>() || !Father.active || Owner.HeldItem.type != ModContent.ItemType<ElementRayAlt>() || Owner.channel)
                Projectile.Kill();

            Vector2 Posoffset = new Vector2(64, 0).RotatedBy(Father.rotation);
            if (Projectile.LAP().FirstFrame)
            {
                Projectile.scale = 0f;
                Projectile.velocity = Father.velocity;
                Projectile.Center = Father.Center + Posoffset;
                Projectile.velocity = Father.velocity;
                Projectile.rotation = Father.rotation;
                Projectile.timeLeft = 2;
                Projectile.scale = MathHelper.Lerp(Projectile.scale, 1f, 0.08f);
                for (int i = 0; i < 8; i++)
                {
                    Color color = LAPUtilities.LerpColor(Color.SkyBlue, Color.DeepSkyBlue);
                    new NoiseShockRing(Projectile.Center, Vector2.Zero, color, 120, 1f, 1.8f + i * 0.2f, Projectile.whoAmI, Vector2.Zero).Spawn();
                }
                return;
            }
            Projectile.Center = Father.Center + Posoffset;
            Projectile.velocity = Father.velocity;
            Projectile.rotation = Father.rotation;
            Projectile.timeLeft = 2;
            Projectile.scale = MathHelper.Lerp(Projectile.scale, 1.2f, 0.08f);
            #region 生成组成主光束的粒子
            int Timeleft5 = 5;
            if (LAPConfig.Instance.PerformanceMode)
                Timeleft5 = 3;
            for (int i = 0; i < LaserLength; i += 40)
            {
                Vector2 Spawn = Projectile.Center + Projectile.velocity * i + Main.rand.NextVector2Circular(36, 36);
                if (LAPUtilities.OutOffScreen(Spawn, CutMult))
                    continue;

                Color color = LAPUtilities.LerpColor(Color.DeepSkyBlue, Color.SkyBlue);
                new TrailGlowBall(Spawn, Projectile.velocity * 6, color, Main.rand.Next(45, 65), 0.08f, true).Spawn();
            }
            for (int i = 0; i < LaserLength; i += 20)
            {
                Vector2 Spawn = Projectile.Center + Projectile.velocity * i + new Vector2(0, -36).RotatedBy(Projectile.rotation) + Main.rand.NextVector2Circular(9, 9);
                if (LAPUtilities.OutOffScreen(Spawn, CutMult))
                    continue;

                Color color = LAPUtilities.LerpColor(Color.White, Color.White);
                new FusableBall(Spawn, Projectile.velocity * 3, color, Timeleft5, 1f, new Vector2(1f, 0.08f)).Spawn();
            }
            for (int i = 0; i < LaserLength; i += 20)
            {
                Vector2 Spawn = Projectile.Center + Projectile.velocity * i + new Vector2(0, 36).RotatedBy(Projectile.rotation) + Main.rand.NextVector2Circular(9, 9);
                if (LAPUtilities.OutOffScreen(Spawn, CutMult))
                    continue;
                Color color = LAPUtilities.LerpColor(Color.White, Color.White);
                new FusableBall(Spawn, Projectile.velocity * 3, color, Timeleft5, 1f, new Vector2(1f, 0.08f)).Spawn();
            }
            for (int i = 0; i < LaserLength; i += 20)
            {
                Vector2 Spawn = Projectile.Center + Projectile.velocity * i + Main.rand.NextVector2Circular(36, 36);
                if (LAPUtilities.OutOffScreen(Spawn, CutMult))
                    continue;
                Color color = LAPUtilities.LerpColor(Color.Black, Color.DarkBlue);
                new FusableBall(Spawn, Projectile.velocity * 3, color, Timeleft5, 1f, new Vector2(1f, 0.3f)).SpawnToPriority();
            }
            for (int i = 0; i < LaserLength; i += 40)
            {
                Vector2 Spawn = Projectile.Center + Projectile.velocity * i + Main.rand.NextVector2Circular(64, 64);
                if (LAPUtilities.OutOffScreen(Spawn, CutMult))
                    continue;
                Color color = LAPUtilities.LerpColor(Color.SkyBlue, Color.DarkBlue);
                new Fire(Spawn, Projectile.velocity * 3, color, 45, Main.rand.NextFloat(MathHelper.TwoPi), 1f, 0.5f).SpawnToPriority();
            }
            #endregion
            #region 生成边缘的粒子
            for (int i = 0; i < LaserLength; i += 400)
            {
                Vector2 Spawn = Projectile.Center + Projectile.velocity * i - Projectile.velocity * 200 + Main.rand.NextVector2Circular(256, 32).RotatedBy(Projectile.rotation) + new Vector2(0, 36);
                if (LAPUtilities.OutOffScreen(Spawn, CutMult))
                    continue;
                Color color = LAPUtilities.LerpColor(Color.White, Color.SkyBlue);
                new FusableBall(Spawn, Projectile.velocity * 12, color, Main.rand.Next(60, 90), 1f, new Vector2(1f, 0.1f)).Spawn();
            }
            for (int i = 0; i < LaserLength; i += 400)
            {
                Vector2 Spawn = Projectile.Center + Projectile.velocity * i - Projectile.velocity * 200 + Main.rand.NextVector2Circular(256, 32).RotatedBy(Projectile.rotation) + new Vector2(0, -36);
                if (LAPUtilities.OutOffScreen(Spawn, CutMult))
                    continue;
                Color color = LAPUtilities.LerpColor(Color.White, Color.SkyBlue);
                new FusableBall(Spawn, Projectile.velocity * 12, color, Main.rand.Next(60, 90), 1f, new Vector2(1f, 0.1f)).Spawn();
            }
            if (LAPConfig.Instance.PerformanceMode)
                return;
            if (Time > 5)
            {
                for (int i = 0; i < LaserLength + 200; i += 200)
                {
                    Vector2 SpawnPos = Projectile.Center + Projectile.velocity * i + Main.rand.NextVector2Circular(128, 128).RotatedBy(Projectile.rotation) - Projectile.velocity * 200;
                    if (LAPUtilities.OutOffScreen(SpawnPos, CutMult))
                        continue;
                    new CrossGlow(SpawnPos, Vector2.Zero, Color.SkyBlue, 60, 1f, 0.2f).Spawn();
                    if (i != 0)
                        GenStarLine(OldPos, SpawnPos, 60);
                    OldPos = SpawnPos;
                }
                Time = 0;
            }
            #endregion
        }
        public void GenStarLine(Vector2 BeginPos, Vector2 EndPos, float GenStep)
        {
            for (int i = 0; i < GenStep; i++)
            {
                Vector2 SpawnVector = Vector2.Lerp(BeginPos, EndPos, i / GenStep);
                new MediumGlowBall(SpawnVector, Vector2.Zero, Color.SkyBlue, 60, 0, 1f, 0.1f, 0).Spawn();
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
        }
        public override void OnKill(int timeLeft)
        { 
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overWiresUI.Add(index);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            LAPUtilities.ReSetToBeginShader();
            DrawGlow();
            DrawFlash();
            DrawOutLine();
            LAPUtilities.ReSetToEndShader();
            return false;
        }
        public void DrawGlow()
        {
            Texture2D texture = UCATextureRegister.CrossGlow.Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.SkyBlue, 0, texture.Size() / 2, Projectile.scale * 0.4f * new Vector2(1.5f, 1f), SpriteEffects.FlipHorizontally, 0f);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.DeepSkyBlue, 0, texture.Size() / 2, Projectile.scale * 0.3f * new Vector2(1.5f, 1f), SpriteEffects.None, 0f);
        }
        public void DrawFlash()
        {
            Texture2D texture = UCATextureRegister.Flash_01.Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.SkyBlue, Main.GlobalTimeWrappedHourly, texture.Size() / 2, 0.2f * Projectile.scale, SpriteEffects.FlipHorizontally, 0f);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.DeepSkyBlue, -Main.GlobalTimeWrappedHourly, texture.Size() / 2, 0.2f * Projectile.scale, SpriteEffects.None, 0f);
        }
        public void DrawOutLine()
        {
            Texture2D texture = UCATextureRegister.BloomRing.Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White * 0.5f, Main.GlobalTimeWrappedHourly, texture.Size() / 2, Projectile.scale * 0.6f, SpriteEffects.FlipHorizontally, 0f);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White * 0.5f, Main.GlobalTimeWrappedHourly, texture.Size() / 2, Projectile.scale * 0.6f, SpriteEffects.FlipHorizontally, 0f);
        }
    }
}
