using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Assets.Effects;
using UCA.Content.DrawNodes;
using UCA.Content.Particiles;
using UCA.Core.BaseClass;
using UCA.Core.Graphics;
using UCA.Core.Utilities;

namespace UCA.Content.Projectiles.Magic.Ray
{
    public class ElementalLaser : BaseMagicProj
    {
        public override string Texture => UCATextureRegister.InvisibleTexturePath;
        public float Opacity = 0f;
        public int MaxLife = 75;
        public float LaserLength = 0;
        public Vector2 BeginPos;
        public Vector2 EndPos;
        public int LaserTimeOffset;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 4400;
        }
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.extraUpdates = 0;
            Projectile.friendly = true;
            Projectile.timeLeft = MaxLife;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Opacity < 0.1f)
                return false;
            if (projHitbox.Intersects(targetHitbox))
            {
                return true;
            }
            float _ = float.NaN;
            Vector2 beamEndPos = BeginPos + Projectile.velocity.SafeNormalize(Vector2.Zero) * LaserLength;
            bool c = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), BeginPos, beamEndPos, 24f, ref _);
            return c;
        }

        public override void AI()
        {
            UpdateMisc();
            FirstFrame();
            UpdateLaserLength();
        }
        #region 初始化
        public void FirstFrame()
        {
            if (!Projectile.UCA().FirstFrame)
                return;
            LaserTimeOffset = Main.rand.Next(0, 100);
            BeginPos = Projectile.Center;
            EndPos = Projectile.Center;
            Vector2 Posoffset = new Vector2(10, 0).RotatedBy(Projectile.rotation);
            new CrossGlow(Projectile.Center + Posoffset, Vector2.Zero, Color.White, 60, 1f, 0.4f).Spawn();
            for (int i = 0; i < 35; i++)
            {
                Color RandomColor = Color.Lerp(Color.White, Color.AntiqueWhite, Main.rand.NextFloat(0, 1));
                new MediumGlowBall(Projectile.Center + Posoffset, RandomColor, 120, 0.2f, Main.rand.NextFloat(2f, 3f), false).Spawn();
            }
            #region 生成伴随主弹幕的树
            Vector2 firVec = Projectile.velocity.SafeNormalize(Vector2.Zero) * 3f;
            Vector2 ProjFireOffset = new Vector2(-24, 0).RotatedBy(Projectile.velocity.ToRotation());
            Vector2 firPos = Projectile.Center + ProjFireOffset;
            int Filp = Main.rand.NextBool() ? 1 : -1;
            for (int i = 0; i < 2; i++)
            {
                new TerraTree(firPos, firVec * Main.rand.NextFloat(6, 7f), Color.White, 0, DrawLayer.AfterDust, Main.rand.NextFloat(2, 5), -1 * Filp, Main.rand.NextFloat(9, 18f)).Spawn();
                new TerraTree(firPos, firVec * Main.rand.NextFloat(6, 7f), Color.GhostWhite, 0, DrawLayer.AfterDust, Main.rand.NextFloat(3, 6), 1 * Filp, Main.rand.NextFloat(11, 22)).Spawn();
            }
            #endregion
        }
        #endregion
        #region 更新激光长度
        public void UpdateLaserLength()
        {
            if (Projectile.timeLeft > MaxLife - 15)
            {
                LaserLength = (EndPos - BeginPos).Length();
                EndPos += new Vector2(128, 0).RotatedBy(Projectile.rotation);
            }
        }
        #endregion
        #region 更新杂项
        public void UpdateMisc()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.timeLeft > MaxLife / 2)
                Opacity = MathHelper.Lerp(Opacity, 1f, 0.2f);
            else
                Opacity = MathHelper.Lerp(Opacity, 0f, 0.15f);
        }
        #endregion
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            UCAUtilities.ReSetToBeginShader(BlendState.Additive);
            DrawLaser(Color.Gray, 0.25f, 0.1f, -100);
            UCAUtilities.ReSetToBeginShader();
            DrawLaser(Color.White, 0.15f, 0.1f, -50);
            DrawLaser(Color.White, 0.07f, 0.02f , -100);
            UCAUtilities.ReSetToEndShader();
            return false;
        }
        public void DrawLaser(Color colro, float height = 0.2f, float op = 0.1f, int Speed = -50)
        {
            float TextureHeight = UCATextureRegister.ElementalRayFlow.Height();
            float TextureWidth = UCATextureRegister.ElementalRayFlow.Width();

            UCAShaderRegister.StandardLaserShader.Parameters["LaserTextureSize"].SetValue(UCATextureRegister.ElementalRayFlow.Size());
            UCAShaderRegister.StandardLaserShader.Parameters["targetSize"].SetValue(new Vector2(LaserLength, TextureHeight));
            UCAShaderRegister.StandardLaserShader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * Speed + LaserTimeOffset);
            UCAShaderRegister.StandardLaserShader.Parameters["uColor"].SetValue(colro.ToVector4() * Opacity);
            UCAShaderRegister.StandardLaserShader.Parameters["uFadeoutLength"].SetValue(op);
            UCAShaderRegister.StandardLaserShader.Parameters["uFadeinLength"].SetValue(op);
            UCAShaderRegister.StandardLaserShader.CurrentTechnique.Passes[0].Apply();

            Vector2 orig = new(0, TextureHeight / 2);
            float xScale = LaserLength / TextureWidth;
            Main.spriteBatch.Draw(UCATextureRegister.TerrarRayFlow.Value, BeginPos - Main.screenPosition, null, Color.White, Projectile.rotation, orig, new Vector2(xScale, height), SpriteEffects.None, 0);
        }
    }
}
