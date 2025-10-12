using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using UCA.Content.DrawNodes;
using UCA.Content.MetaBalls;
using UCA.Content.Particiles;
using UCA.Core.BaseClass;
using UCA.Core.Graphics;
using UCA.Core.Utilities;

namespace UCA.Content.Projectiles.HealPRoj
{
    public class TerraHeal : BaseHealProj
    {
        public List<TerraLanceVine> Vine = [];
        public List<Vector2> AvailableOldPos = [];
        public override int HealAmt => Main.rand.Next(7, 15);
        public override float FlySpeed => 12f;
        public override float Acceleration => 35f;
        public override void SetStaticDefaults()
        {
            // 保存旧朝向与旧位置
            ProjectileID.Sets.TrailingMode[Type] = 2;
            // 一共保存25个数据
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
        }
        public override void OnSpawn(IEntitySource source)
        {
            float XScale = Main.rand.NextFloat(6, 12);
            float Height = Main.rand.NextFloat(4, 10);
            Vine.Add(new TerraLanceVine(Projectile.Center, Projectile.velocity, Color.ForestGreen, DrawLayer.BeforeDust, 3000, XScale, 1, Height));
            Vine.Add(new TerraLanceVine(Projectile.Center, Projectile.velocity, Color.LightGreen, DrawLayer.AfterDust, 3000, XScale, -1, Height));
        }
        public override void ExAI()
        {
            if (Projectile.timeLeft % 40 == 0)
            {
                Color RandomColor = Color.Lerp(Color.Pink, Color.Green, Main.rand.NextFloat(0, 1));
                new Petal(Projectile.Center, -Vector2.UnitY * 12f, RandomColor, 360, 0, 1, 0.1f, Main.rand.NextFloat(1f, 1.4f)).Spawn();
            }
            for (int i = 0; i < 5; i++)
            {
                Color RandomColor2 = Color.Lerp(Color.LightGreen, Color.Green, Main.rand.NextFloat(0, 1));
                Vector2 vel = -Projectile.velocity / 5;
                new MediumGlowBall(Projectile.Center + vel * i, Vector2.Zero, RandomColor2, 15, 0, 1, 0.12f, Main.rand.NextFloat(0.1f, 0.3f)).Spawn();
            }

            UpdateVine();
        }
        #region 更新藤蔓
        public void UpdateVine()
        {
            for (int i = 0; i < Vine.Count; i++)
            {
                Vine[i].Update();
                Vine[i].Time++;
                Vine[i].Position = Projectile.Center;
                Vine[i].Velocity = Projectile.velocity;

                if (Vine[i].Time >= Vine[i].Lifetime)
                {
                    Vine[i].OnKill();
                    Vine.Remove(Vine[i]);
                }
            }
        }
        #endregion
        public override bool PreDraw(ref Color lightColor)
        {
            if (UCAUtilities.OutOffScreen(Projectile.Center))
                return false;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            for (int i = 0; i < Vine.Count; i++)
            {
                Vine[i].Draw(Main.spriteBatch);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
        public override void ExKill()
        {
            Vector2 firPos = Projectile.Center;
            for (int i = 0; i < 3; i++)
            {
                float rot = MathHelper.TwoPi / 3;
                float XScale = Main.rand.NextFloat(9, 12);
                float Height = Main.rand.NextFloat(4f, 9f);

                Vector2 firVec = Vector2.UnitX.RotatedBy(rot * i).RotatedByRandom(MathHelper.TwoPi);
                Color color = Main.rand.NextBool() ? Color.ForestGreen : Color.SaddleBrown;
                new TerraTree(firPos, firVec * Main.rand.NextFloat(0.8f, 1.4f), color, 0, DrawLayer.BeforeDust, XScale, Main.rand.NextBool() ? 1 : -1, Height).Spawn();
            }
            for (int i = 0; i < 5; i++)
            {
                Color RandomColor = Color.Lerp(Color.LightGreen, Color.Green, Main.rand.NextFloat(0, 1));
                new MediumGlowBall(Projectile.Center, Vector2.Zero, RandomColor, 180, 0, 1, 0.12f, Main.rand.NextFloat(0.5f, 0.7f)).Spawn();
            }
        }
    }
}
