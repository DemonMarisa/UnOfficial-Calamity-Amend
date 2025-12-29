using LAP.Core.Graphics.DrawNode;
using Microsoft.Xna.Framework;
using Terraria;
using UCA.Content.MetaBalls;

namespace UCA.Content.DrawNodes
{
    public class CosmicDustEmitting : DrawNode
    {
        public Vector2 TargetPos;
        public int Filp;
        public float LengthOffset = 0;
        public Vector2 PositionOffset => new Vector2(LengthOffset * 1, 0);
        public Vector2 PositionOffset2 => new Vector2(LengthOffset * -1, 0);
        public Vector2 RealPos;
        public CosmicDustEmitting(Vector2 position, int filp)
        {
            TargetPos = position;
            RealPos = position;
            Filp = filp;
            Lifetime = 60;
            ExtraUpdate = 10;
        }
        public override void OnSpawn()
        {
            Position = TargetPos + new Vector2(LengthOffset, 10 * Filp);
        }
        public override void Update()
        {
            Rotation = MathHelper.Lerp(0, -MathHelper.Pi * Filp, LifetimeRatio);
            LengthOffset = MathHelper.Lerp(150, 0, LifetimeRatio);
            CosmicMetaBall.SpawnCircleParticle(Position + PositionOffset.RotatedBy(Rotation), Vector2.Zero, 0.25f, 20);
            CosmicMetaBall.SpawnCircleParticle(Position + PositionOffset2.RotatedBy(Rotation), Vector2.Zero, 0.25f, 20);
        }
        public override void OnKill()
        {
            CosmicMetaBall.SpawnCrossParticle(RealPos, 0.6f);
        }
    }
}
