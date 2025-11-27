using LAP.Core.Graphics.Primitives.Trail;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using UCA.Assets;
using UCA.Assets.Effects;
using UCA.Core.BaseClass;
using UCA.Core.Utilities;

namespace UCA.Content.Projectiles.Rogue.ThunderProj.RightHandHammer
{
    public class PhantomRay : BaseRogueProj
    {
        private float DrawScale = 1f;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 24;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 6;
            Projectile.extraUpdates = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 12;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 400;
            Projectile.penetrate = -1;
        }
        public override void AI()
        {
            //AI里只会接管绘制相关的东西
            Projectile.rotation = Projectile.velocity.ToRotation();
            DrawScale = 1;
            

        }
        SpriteBatch SB { get => Main.spriteBatch; }
        GraphicsDevice GD { get => Main.graphics.GraphicsDevice; }
        public override bool PreDraw(ref Color lightColor)
        {
            
            SB.End();
            SB.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            DrawNebulaTrail(Color.MidnightBlue, 12f); 
            DrawNebulaTrail(Color.RoyalBlue, 8.2f); 
            DrawNebulaTrail(Color.White, 4.8f); 
            SB.End();
            SB.BeginDefault();
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float fac = (float)(Projectile.oldPos.Length - i) / Projectile.oldPos.Length;
                Dust newDust = Dust.NewDustPerfect(Projectile.oldPos[i] + Projectile.Size / 2 + Main.rand.NextVector2Circular(6f, 6f), DustID.UnusedWhiteBluePurple, Projectile.velocity.RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4 / 6, MathHelper.PiOver4 / 6)), 0, default, 1);
                newDust.noGravity = true;
                newDust.scale *= 1.22f;
            }
            return false;
        }
        public void DrawNebulaTrail(Color trailColor, float height)
        {
            float laserLength = 25;
            UCAShaderRegister.TerrarRayLaser.Parameters["LaserTextureSize"].SetValue(UCATextureRegister.Trail_ManaStreak.Size());
            UCAShaderRegister.TerrarRayLaser.Parameters["targetSize"].SetValue(new Vector2(laserLength, UCATextureRegister.Trail_ManaStreak.Height()));
            UCAShaderRegister.TerrarRayLaser.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -50);
            UCAShaderRegister.TerrarRayLaser.Parameters["uColor"].SetValue(trailColor.ToVector4() * DrawScale);
            UCAShaderRegister.TerrarRayLaser.Parameters["uFadeoutLength"].SetValue(0.1f);
            UCAShaderRegister.TerrarRayLaser.Parameters["uFadeinLength"].SetValue(0.05f);
            UCAShaderRegister.TerrarRayLaser.CurrentTechnique.Passes[0].Apply();

            //做掉可能存在的零向量
            Projectile.ClearInvaidData(out List<Vector2> validPosition, out List<float> validRot, Projectile.oldPos, Projectile.oldRot);
            GD.Textures[0] = UCATextureRegister.Trail_ManaStreak.Value;
            GD.SamplerStates[0] = SamplerState.PointClamp;
            //直接获取需要的贝塞尔曲线。
            List<VertexPositionColorTexture2D> list = [];
            int totalpoints = validPosition.Count;
            //创建顶点列表
            for (int i = 0; i < validPosition.Count; i++)
            {
                Vector2 oldCenter = validPosition[i] + Projectile.Size / 2  - Main.screenPosition;
                float progress = (float)i / (validPosition.Count - 1);
                Vector2 posOffset = new Vector2(0, 3f * height * DrawScale * ((float)(totalpoints - i) / totalpoints)).RotatedBy(validRot[i]);
                VertexPositionColorTexture2D upClass = new(oldCenter + posOffset, trailColor, new Vector3(progress, 0, 0f));
                VertexPositionColorTexture2D downClass = new(oldCenter - posOffset, trailColor, new Vector3(progress, 1, 0f));
                list.Add(upClass);
                list.Add(downClass);    
            }
            if (list.Count >= 3)
            {
                GD.DrawUserPrimitives(PrimitiveType.TriangleStrip, list.ToArray(), 0, list.Count - 2);
            }
        }

    }
}
