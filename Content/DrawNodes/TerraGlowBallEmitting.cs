using Microsoft.Xna.Framework;
using System;
using Terraria;
using UCA.Content.Particiles;
using UCA.Core.Graphics.DrawNode;
using static Terraria.GameContent.Animations.IL_Actions.Sprites;

namespace UCA.Content.DrawNodes
{
    public class TerraGlowBallEmitting : DrawNode
    {
        public TerraGlowBallEmitting(Vector2 position, Vector2 velocity, int life, float Xscale, int filp, float height, int owner)
        {
            Position = position;
            Velocity = velocity;
            Lifetime = life;

            XScale = Xscale;
            Filp = filp;
            Height = height;

            Owner = owner;
        }
        public int Owner;
        public Player Player => Main.player[Owner];
        public Vector2 oldDustPos;
        public Vector2 DustPos;
        public float XScale;
        public int Filp;
        public float Height;
        public int BeginOffset;
        public float Length;
        public override void OnSpawn()
        {
            DustPos = Vector2.Zero;
            oldDustPos = Vector2.Zero;
            BeginOffset = Main.rand.Next(0, 20);
        }
        public override void Update()
        {
            Velocity = Vector2.Zero;
            oldDustPos = DustPos;
            Length = MathHelper.Lerp(100f, 0f, LifetimeRatio);
            // 半径的缩放
            float radiusScale = MathHelper.Lerp(1f, 0f, Utils.GetLerpValue(Lifetime * 0.5f, Lifetime, Time, true));
            // Y向量偏移
            float standVector2Y = (float)(Math.Sin((Time + BeginOffset) / XScale) * Height * radiusScale * Filp);
            // 最终应用偏移
            Vector2 PreAddVector = new(0, standVector2Y);
            // 根据弹幕旋转，将固定向右转换为向量的旋转
            PreAddVector = PreAddVector.RotatedBy(MathHelper.PiOver2);
            // 最终粒子的点
            DustPos = Player.Center + new Vector2(0, -Length) + PreAddVector;
            for (int i = 0; i < 5; i++)
            {
                Color RandomColor2 = Color.Lerp(Color.LightGreen, Color.Green, Main.rand.NextFloat(0, 1));
                Vector2 SpawnPos = Vector2.Lerp(DustPos, oldDustPos, i / 5f);
                new MediumGlowBall(SpawnPos, Vector2.Zero, RandomColor2, 15, 0, 1, 0.12f, Main.rand.NextFloat(0.1f, 0.3f)).Spawn();
            }
        }
    }
}
