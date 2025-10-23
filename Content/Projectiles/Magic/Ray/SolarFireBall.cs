using CalamityMod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Assets.Effects;
using UCA.Content.Particiles;
using UCA.Core.BaseClass;
using UCA.Core.Graphics.Primitives.Trail;
using UCA.Core.Utilities;

namespace UCA.Content.Projectiles.Magic.Ray
{
    public class SolarFireBall : BaseMagicProj
    {
        public override string Texture => UCATextureRegister.InvisibleTexturePath;
        public int MaxLife = 240;
        public float DustCount = 5f;
        public int Time;
        public float Opacity = 1f;
        public bool UseSpawnEffect => Projectile.ai[0] == 0;
        public override void SetStaticDefaults()
        {
            // 保存旧朝向与旧位置
            ProjectileID.Sets.TrailingMode[Type] = 2;
            // 一共爆粗20个数据
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
        }
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
            if (Projectile.UCA().FirstFrame)
            {
                if (!UseSpawnEffect)
                    return;

                for (int j = 0; j < 25; j++)
                {
                    Color RandomColor = Color.Lerp(Color.OrangeRed, Color.Orange, Main.rand.NextFloat(0, 1));
                    new MediumGlowBall(Projectile.Center, RandomColor, 60, 0.2f, Main.rand.NextFloat(2f, 4f)).Spawn();
                }
                new CrossGlow(Projectile.Center, Vector2.Zero, Color.Orange, 25, 1f, 0.35f).Spawn();
                new CrossGlow(Projectile.Center, Vector2.Zero, Color.OrangeRed, 25, 1f, 0.45f).Spawn();

                new BloomShockwave(Projectile.Center, Vector2.Zero, Color.OrangeRed, 25, 1f, 0.2f).Spawn();
                new BloomShockwave(Projectile.Center, Vector2.Zero, Color.Orange, 35, 1f, 0.25f).Spawn();
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            Time++;

            Vector2 firVel = Vector2.UnitX.RotatedBy(Projectile.rotation) * 2f;
            Color DrawColor = Color.Lerp(Color.Orange, Color.OrangeRed, Main.rand.NextFloat());
            new Fire(Projectile.Center, firVel, DrawColor, Main.rand.Next(30, 45), Main.rand.NextFloat(MathHelper.TwoPi), 1f, 0.2f).Spawn();

            CalamityUtils.HomeInOnNPC(Projectile, true, 2500f, 12f, 100f);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!Projectile.UCA().OnceHitEffect)
                return;
        }
        public override void OnKill(int timeLeft)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<SolarBlast>(), Projectile.damage, Projectile.knockBack, Projectile.owner, -35, -0.7f, -4f);
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overWiresUI.Add(index);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            UCAUtilities.ReSetToBeginShader();
            Texture2D texture = UCATextureRegister.CrossGlow.Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.Orange, 0, texture.Size() / 2, Projectile.scale * 0.2f * new Vector2(1.25f, 1f), SpriteEffects.FlipHorizontally, 0f);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.OrangeRed, 0, texture.Size() / 2, Projectile.scale * 0.15f * new Vector2(1.25f, 1f), SpriteEffects.None, 0f);
            DrawTrail(36, Color.OrangeRed);
            DrawTrail(24, Color.Orange);
            DrawTrail(12, Color.White);
            
            DrawBall(Color.Red);
            DrawBallOutLine();
            UCAUtilities.ReSetToEndShader();
            return false;
        }
        public void DrawTrail(int height, Color drawColor)
        {
            Vector2 HalfProj = new Vector2(Projectile.width / 2, Projectile.height / 2);
            List<TrailDrawDate> trailDrawDate = [];
            DrawSetting drawSetting = new(UCATextureRegister.Slash_Wrap.Value, false, false);
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                if (Projectile.oldPos[i] != Vector2.Zero)
                {
                    Vector2 DrawPos = Projectile.oldPos[i] - Main.screenPosition + HalfProj + new Vector2(24, 0).RotatedBy(Projectile.rotation);

                    TrailDrawDate TrailDrawDate = new(DrawPos, drawColor, new Vector2(0, height), Projectile.oldRot[i]);
                    trailDrawDate.Add(TrailDrawDate);
                }
            }
            TrailRender.RenderTrail(trailDrawDate.ToArray(), drawSetting);
        }
        public void DrawBall(Color color, int Speed = -50)
        {
            float TextureHeight = UCATextureRegister.Tornade_Fire.Height();
            float TextureWidth = UCATextureRegister.Tornade_Fire.Width();
            UCAShaderRegister.FlowWithAShader.Parameters["FlowTextureSize"].SetValue(UCATextureRegister.Tornade_Fire.Size());
            UCAShaderRegister.FlowWithAShader.Parameters["targetSize"].SetValue(new Vector2(TextureWidth, TextureHeight));
            UCAShaderRegister.FlowWithAShader.Parameters["uTime"].SetValue(-Main.GlobalTimeWrappedHourly * Speed);
            UCAShaderRegister.FlowWithAShader.Parameters["uColor"].SetValue(color.ToVector4() * Opacity);
            UCAShaderRegister.FlowWithAShader.CurrentTechnique.Passes[0].Apply();

            Main.instance.GraphicsDevice.Textures[0] = UCATextureRegister.Tornade_Fire.Value;
            Main.instance.GraphicsDevice.Textures[1] = UCATextureRegister.WhiteCircle.Value;
            Vector2 orig = UCATextureRegister.Tornade_Fire.Size() / 2;
            Main.spriteBatch.Draw(UCATextureRegister.Tornade_Fire.Value, Projectile.Center - Main.screenPosition, null, Color.White, 0, orig, 0.04f, SpriteEffects.None, 0);
        }

        public void DrawBallOutLine()
        {
            UCAUtilities.ReSetToBeginShader(BlendState.Additive);

            UCAShaderRegister.PolarDistortShader.Parameters["uWidthMult"].SetValue(2f);
            UCAShaderRegister.PolarDistortShader.Parameters["uRingMult"].SetValue(1f);
            UCAShaderRegister.PolarDistortShader.Parameters["uYTime"].SetValue(Main.GlobalTimeWrappedHourly * 0.1f);
            UCAShaderRegister.PolarDistortShader.CurrentTechnique.Passes[0].Apply();
            Main.instance.GraphicsDevice.Textures[1] = UCATextureRegister.BloomShockwave.Value;
            float Scale = 0.2f;
            Texture2D texture = UCATextureRegister.MiscNoise01.Value;
            Vector2 orig = texture.Size() / 2;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.Orange, 0, orig, Scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.Orange, 0, orig, Scale, SpriteEffects.None, 0);
            texture = UCATextureRegister.MiscNoise02.Value;
            orig = texture.Size() / 2;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.OrangeRed, 2, orig, Scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.Red, 2, orig, Scale, SpriteEffects.None, 0);

            UCAUtilities.ReSetToEndShader();
        }
    }
}
