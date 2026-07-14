using LAP.Assets.TextureRegister;
using LAP.Core.StateMachine.SynedHitEffect;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Assets.Effects;
using UCA.Content.HitEffect;
using UCA.Content.Particiles;
using UCA.Core.BaseClass;

namespace UCA.Content.Projectiles.Magic.Ray
{
    public class TerraEnergy : BaseMagicProj
    {
        public override string Texture => LAPTextureRegister.InvisibleTexturePath;
        public int MaxTime = 360;
        public int FadeOut = 0;
        public int MaxFade = 30;
        public float LaserLength = 0;
        public Vector2 BeginPos = Vector2.Zero;
        public float Opacity = 0;
        public bool inToFadeOut = false;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 4400;
        }
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.extraUpdates = 5;
            Projectile.friendly = true;
            Projectile.timeLeft = MaxTime;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10 * (Projectile.extraUpdates + 1);
            Projectile.netImportant = true;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(inToFadeOut);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            inToFadeOut = reader.ReadBoolean();
        }
        public override void AI()
        {
            if (Projectile.LAP().FirstFrame)
            {
                BeginPos = Projectile.Center;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            LaserLength = (Projectile.Center - BeginPos).Length();
            if (inToFadeOut)
            {
                Opacity = MathHelper.Lerp(Opacity, 0f, 0.01f);
                if (Opacity < 0.05f)
                    Projectile.Kill();
                Projectile.velocity *= 0.8f;
                Projectile.damage = 0;
                Projectile.netUpdate = true;
            }
            else
            {
                #region 发射粒子
                if (Projectile.timeLeft % 25 == 0)
                {
                    Color RandomColor = Color.Lerp(Color.LightGreen, Color.Green, Main.rand.NextFloat(0, 1));
                    new MediumGlowBall(Projectile.Center, -Projectile.velocity, RandomColor, 180, 0, 1, 0.12f, Main.rand.NextFloat(0.5f, 0.7f)).Spawn();

                    Color RandomColor2 = Color.Lerp(Color.Pink, Color.Green, Main.rand.NextFloat(0, 1));
                    new Petal(Projectile.Center, -Vector2.UnitY * 9f, RandomColor2, 360, 0, 1, 0.1f, Main.rand.NextFloat(0.5f, 0.7f)).Spawn();
                }
                #endregion
                Opacity = MathHelper.Lerp(Opacity, 1f, 0.1f);
            }
            if (Projectile.timeLeft < MaxTime / 2)
                inToFadeOut = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            
            DrawLaser(Color.ForestGreen, 0.15f);
            DrawLaser(Color.White, 0.02f);
            DrawLaser(Color.LimeGreen, 0.1f);
            
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
        public void DrawLaser(Color colro, float height = 0.2f)
        {
            float TextureHeight = UCATextureRegister.TerrarRayFlow.Height();
            float TextureWidth = UCATextureRegister.TerrarRayFlow.Width();

            Effect shader = UCAShaderRegister.TerrarRayLaser.Value;
            shader.Parameters["LaserTextureSize"].SetValue(UCATextureRegister.TerrarRayFlow.Size());
            shader.Parameters["targetSize"].SetValue(new Vector2(LaserLength, TextureHeight));
            shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -50);
            shader.Parameters["uColor"].SetValue(colro.ToVector4() * Opacity);
            shader.Parameters["uFadeoutLength"].SetValue(0.1f);
            shader.Parameters["uFadeinLength"].SetValue(0.1f);
            shader.CurrentTechnique.Passes[0].Apply();

            Main.graphics.GraphicsDevice.Textures[0] = UCATextureRegister.TerrarRayFlow.Value;
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            Vector2 orig = new Vector2(0, TextureHeight / 2);
            float xScale = LaserLength / TextureWidth;
            Main.spriteBatch.Draw(UCATextureRegister.TerrarRayFlow.Value, BeginPos - Main.screenPosition,null, Color.White, Projectile.rotation, orig, new Vector2(xScale, height), SpriteEffects.None, 0);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            HitEffectManager.SpawnHitEffect(HitEffectManager.HEType<TerraEnergyHit>(), Projectile.owner, Projectile.GetSource_FromThis(), target.Center, Vector2.Zero);
            inToFadeOut = true;
            Projectile.netUpdate = true;
            Projectile.netSpam = 0;
        }
    }
}
