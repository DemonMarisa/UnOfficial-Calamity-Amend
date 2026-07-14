using LAP.Core.Graphics.VFX;
using LAP.Core.Presets.Content;
using LAP.Core.SystemsLoader;
using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace UCA.Content.VFXs
{
    public class TerraGlowBallEmitting : VFXBehavior
    {
        public static void Spawn(Vector2 position, int life, float Xscale, int filp, float height, int owner)
        {
            VFXInstance vfx = LAPContent.SpawnVFX(LAPContent.VFXType<TerraGlowBallEmitting>(), position, Vector2.Zero, Color.White);
            vfx.Lifetime = life;
            vfx.AiInt[0] = filp;
            vfx.AiInt[1] = owner;

            vfx.AiFloat[0] = Xscale;
            vfx.AiFloat[1] = height;
        }
        public int Owner => VFXInstance.AiInt[1];
        public Player Player => Main.player[Owner];
        public Vector2 oldDustPos;
        public Vector2 DustPos;
        public float XScale => VFXInstance.AiFloat[0];
        public int Filp => VFXInstance.AiInt[0];
        public float Height => VFXInstance.AiFloat[1];
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
            VFXInstance.Velocity = Vector2.Zero;
            oldDustPos = DustPos;
            Length = MathHelper.Lerp(100f, 0f, VFXInstance.LifetimeRatio);
            // 半径的缩放
            float radiusScale = MathHelper.Lerp(1f, 0f, Utils.GetLerpValue(VFXInstance.Lifetime * 0.5f, VFXInstance.Lifetime, VFXInstance.Time, true));
            // Y向量偏移
            float standVector2Y = (float)(Math.Sin((VFXInstance.Time + BeginOffset) / XScale) * Height * radiusScale * Filp);
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
                ParticlePreset.NewTMGlowBall(SpawnPos, Vector2.Zero, RandomColor2, 15, 0.12f, Main.rand.NextFloat(0.1f, 0.3f));
            }
        }
    }
}
