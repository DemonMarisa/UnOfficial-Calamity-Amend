using LAP.Core.Graphics.VFX;
using LAP.Core.SystemsLoader;
using Microsoft.Xna.Framework;
using Terraria;
using UCA.Content.MetaBalls;

namespace UCA.Content.VFXs
{
    public class CosmicDustEmitting : VFXBehavior
    {
        public Vector2 TargetPos;
        public int Filp => VFXInstance.AiInt[0];
        public float LengthOffset;
        public Vector2 PositionOffset => new Vector2(LengthOffset * 1, 0);
        public Vector2 PositionOffset2 => new Vector2(LengthOffset * -1, 0);
        public Vector2 RealPos;
        public static void Spawn(Vector2 position, int filp)
        {
            VFXInstance vfx = LAPContent.SpawnVFX(LAPContent.VFXType<CosmicDustEmitting>(), position, Vector2.Zero, Color.White);
            vfx.AiInt[0] = filp;
        }
        public override void OnSpawn()
        {
            TargetPos = VFXInstance.Position;
            VFXInstance.Position = TargetPos + new Vector2(LengthOffset, 10 * Filp);
            VFXInstance.Lifetime = 60;
            VFXInstance.ExtraUpdate = 10;
        }
        public override void Update()
        {
            VFXInstance.Rotation = MathHelper.Lerp(0, -MathHelper.Pi * Filp, VFXInstance.LifetimeRatio);
            LengthOffset = MathHelper.Lerp(150, 0, VFXInstance.LifetimeRatio);
            CosmicMetaBall.SpawnCircleParticle(VFXInstance.Position + PositionOffset.RotatedBy(VFXInstance.Rotation), Vector2.Zero, 0.25f, 20);
            CosmicMetaBall.SpawnCircleParticle(VFXInstance.Position + PositionOffset2.RotatedBy(VFXInstance.Rotation), Vector2.Zero, 0.25f, 20);
        }
        public override void OnKill()
        {
            CosmicMetaBall.SpawnCrossParticle(TargetPos, 0.6f);
        }
    }
}
