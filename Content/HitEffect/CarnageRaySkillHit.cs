using LAP.Core.StateMachine.SynedHitEffect;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using UCA.Content.MetaBalls;
using UCA.Content.Particiles;

namespace UCA.Content.HitEffect
{
    public class CarnageRaySkillHit : BaseHitEffect
    {
        public override void HitEffect(Entity entity, IEntitySource source, Player owner)
        {
            Vector2 ToMouseVector = entity.Center - owner.Center;
            ToMouseVector = ToMouseVector.SafeNormalize(Vector2.UnitX);
            for (int i = 0; i < 10; i++)
            {
                Vector2 shootVel = ToMouseVector.RotatedByRandom(MathHelper.PiOver4 * 0.7f) * Main.rand.NextFloat(0.2f, 1.2f) * 24f;

                if (shootVel.ToRotation() > 0)
                    shootVel.Y *= 0.15f;

                Color color = Main.rand.NextBool(3) ? Color.Black : Color.DarkRed;
                new BloodDrop(entity.Center, shootVel, color, Main.rand.Next(60, 90), 0, 1, 0.1f).Spawn();
            }
            for (int i = 0; i < 10; i++)
            {
                Vector2 SpawnVector = ToMouseVector.RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(0f, 1.2f) * 36f;
                CarnageMetaBall.SpawnParticle(entity.Center,
                    SpawnVector,
                    1.5f, SpawnVector.ToRotation());
            }
        }
    }
}
