using CalamityMod;
using CalamityMod.Graphics.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Content.MetaBalls;
using UCA.Content.Particiles;
using UCA.Core.BaseClass;
using UCA.Core.Utilities;
using static System.Net.Mime.MediaTypeNames;

namespace UCA.Content.Projectiles.Magic.Ray
{
    public class PlasmaPrimarySpark : BaseMagicProj, IPixelatedPrimitiveRenderer
    {
        public PixelationPrimitiveLayer LayerToRenderTo => PixelationPrimitiveLayer.AfterPlayers;
        public Player Owner => Main.player[Projectile.owner];
        public override string Texture => UCATextureRegister.InvisibleTexturePath;

        public NPC Target = null;

        public float DrawRot = 0;

        public bool FadeOut = false;

        public int FadeOutCount = 60;

        public float VelocityLength = 0;

        public override void SetStaticDefaults()
        {
            // 保存旧朝向与旧位置
            ProjectileID.Sets.TrailingMode[Type] = 2;
            // 一共爆粗20个数据
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 240;
            Projectile.extraUpdates = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            VelocityLength = Projectile.velocity.Length();
        }

        public override void AI()
        {
            DrawRot += 0.06f;
            if (Projectile.timeLeft > 160)
            {
                Projectile.velocity *= 1.02f;
                if (VelocityLength < 9)
                VelocityLength *= 1.02f;
            }

            for (int i = 0; i < 2; i++)
            {
                Color RandomColor = Color.Lerp(Color.Violet, Color.Purple, Main.rand.NextFloat(0, 1));
                new MediumGlowBall(Projectile.Center + Main.rand.NextVector2Circular(18, 18) + Projectile.velocity / 2 * i,
                    -Projectile.velocity, RandomColor, 120, 0, 1, 0.12f, Main.rand.NextFloat(0.5f, 1f)).Spawn();
            }

            TrackTarget();
        }
        public void TrackTarget()
        {
            if (Target is null)
            {
                Target = Projectile.FindClosestTarget(900, true, true);
            }
            else
            {
                Vector2 home = (Target.Center - Projectile.Center);
                VelocityLength = MathHelper.Lerp(VelocityLength, 9, 0.02f);
                Projectile.rotation = Projectile.rotation.AngleTowards(home.ToRotation(), MathHelper.ToRadians(1f));
                Projectile.velocity = Projectile.rotation.ToRotationVector2() * VelocityLength;
            }
        }

        public void RenderPixelatedPrimitives(SpriteBatch spriteBatch, PixelationPrimitiveLayer layer)
        {
            Vector2 DrawPos = Projectile.Center - Main.screenPosition;

            List<Vector2> validPositions = [];
            List<float> validRot = [];

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                if (Projectile.oldPos[i] != Vector2.Zero)
                {
                    validPositions.Add(Projectile.oldPos[i]);
                    validRot.Add(Projectile.oldRot[i]);
                }
            }

            List<VertexPosition2DColorTexture> Vertexlist = new List<VertexPosition2DColorTexture>();

            for (int i = 0; i < validPositions.Count; i++)
            {
                Vector2 validpos = validPositions[i] - Main.screenPosition + new Vector2(Projectile.width / 2, Projectile.height / 2) + new Vector2(18, 0).RotatedBy(Projectile.rotation);
                float progress = (float)i / validPositions.Count;
                Color DrawColor = new(208, 0, 255, 0);
                Vertexlist.Add(new VertexPosition2DColorTexture(validpos / 2 - new Vector2(0, 12).RotatedBy(validRot[i]), DrawColor, new Vector2(progress, 0), 0));
                Vertexlist.Add(new VertexPosition2DColorTexture(validpos / 2 + new Vector2(0, 12).RotatedBy(validRot[i]), DrawColor, new Vector2(progress, 1), 0));
            }

            List<VertexPosition2DColorTexture> SecondVertexlist = new List<VertexPosition2DColorTexture>();

            for (int i = 0; i < validPositions.Count; i++)
            {
                Vector2 validpos = validPositions[i] - Main.screenPosition + new Vector2(Projectile.width / 2, Projectile.height / 2) + new Vector2(18, 0).RotatedBy(Projectile.rotation);
                float progress = (float)i / validPositions.Count;
                Color DrawColor = new(255, 255, 255, 0);
                SecondVertexlist.Add(new VertexPosition2DColorTexture(validpos / 2 - new Vector2(0, 4).RotatedBy(validRot[i]), DrawColor, new Vector2(progress, 0), 0));
                SecondVertexlist.Add(new VertexPosition2DColorTexture(validpos / 2 + new Vector2(0, 4).RotatedBy(validRot[i]), DrawColor, new Vector2(progress, 1), 0));
            }

            List<VertexPosition2DColorTexture> ThirdVertexlist = new List<VertexPosition2DColorTexture>();

            for (int i = 0; i < validPositions.Count; i++)
            {
                Vector2 validpos = validPositions[i] - Main.screenPosition + new Vector2(Projectile.width / 2, Projectile.height / 2) + new Vector2(18, 0).RotatedBy(Projectile.rotation);
                float progress = (float)i / validPositions.Count;
                Color DrawColor = new(186, 50, 205, 0);
                ThirdVertexlist.Add(new VertexPosition2DColorTexture(validpos / 2 - new Vector2(0, 20).RotatedBy(validRot[i]), DrawColor, new Vector2(progress, 0), 0));
                ThirdVertexlist.Add(new VertexPosition2DColorTexture(validpos / 2 + new Vector2(0, 20).RotatedBy(validRot[i]), DrawColor, new Vector2(progress, 1), 0));
            }

            Main.graphics.GraphicsDevice.Textures[0] = ModContent.Request<Texture2D>($"UCA/Assets/LILES/Slash01").Value;
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, SecondVertexlist.ToArray(), 0, Vertexlist.Count - 2);
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, Vertexlist.ToArray(), 0, Vertexlist.Count - 2);
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, ThirdVertexlist.ToArray(), 0, Vertexlist.Count - 2);


            Texture2D texture = ModContent.Request<Texture2D>("UCA/Content/Particiles/GlowBall").Value;
            spriteBatch.Draw(texture, DrawPos / 2, null, new Color(0, 0, 0, 255), 0, texture.Size() / 2, Projectile.scale * 0.3f, SpriteEffects.None, 0);
            spriteBatch.Draw(UCATextureRegister.BallSoft.Value, DrawPos / 2, null, new Color(148, 0, 255, 0), 0, UCATextureRegister.BallSoft.Size() / 2, Projectile.scale * 0.4f, SpriteEffects.None, 0);
            spriteBatch.Draw(UCATextureRegister.Spirit.Value, DrawPos / 2, null, new Color(255, 255, 255, 0), DrawRot, UCATextureRegister.Spirit.Size() / 2, Projectile.scale * 0.2f, SpriteEffects.None, 0);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 45; i++)
            {
                float offset = MathHelper.TwoPi / 45;
                Color RandomColor = Color.Lerp(Color.DarkViolet, Color.LightPink, Main.rand.NextFloat(0, 1));
                new MediumGlowBall(Projectile.Center + Projectile.velocity.ToRotation().ToRotationVector2() * 12f, Projectile.velocity.RotatedBy(offset * i), RandomColor, 180, 0, 1, 0.2f, Main.rand.NextFloat(3f, 5f)).Spawn();
            }
            SoundEngine.PlaySound(SoundsMenu.PlasmaBlastBomb, Projectile.Center);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<PlasmaBlast>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.ShadowFlame, 300);
        }
    }
}
