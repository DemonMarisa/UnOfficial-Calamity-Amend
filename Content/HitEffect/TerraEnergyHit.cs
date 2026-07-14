using LAP.Core.Presets.Content;
using LAP.Core.StateMachine.SynedHitEffect;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using UCA.Assets.Sounds;
using UCA.Content.Particiles;
using UCA.Content.VFXs;

namespace UCA.Content.HitEffect
{
    public class TerraEnergyHit : BaseHitEffect
    {
        public override void HitEffect(Entity entity, IEntitySource source, Player owner)
        {
            // 生成枝条
            Vector2 firPos = entity.Center;
            for (int i = 0; i < 2; i++)
            {
                float rot = MathHelper.TwoPi / 3;

                Vector2 firVec = Vector2.UnitX.RotatedBy(rot * i).RotatedByRandom(MathHelper.TwoPi);
                Color color = Main.rand.NextBool() ? Color.DarkGreen : Color.SaddleBrown;

                TerraVine.Spawn(firPos, firVec * Main.rand.NextFloat(0.8f, 1.4f), color, Main.rand.NextBool() ? 1 : -1, 2f, Main.rand.NextFloat(3, 6), Main.rand.NextFloat(4f, 6f));
            }
            for (int i = 0; i < 5; i++)
            {
                float offset = MathHelper.TwoPi / 5;
                Color RandomColor = Color.Lerp(Color.LightGreen, Color.ForestGreen, Main.rand.NextFloat(0, 1));
                Vector2 firVel = Vector2.UnitX.BetterRotatedBy(offset * i, default, 0.75f, 1f);

                ParticlePreset.NewTMGlowBall(firPos, firVel * 1.5f, RandomColor, 60, 0.2f, Main.rand.NextFloat(2, 3));
            }
            SoundEngine.PlaySound(SoundsMenu.TerraRayHit, entity.Center);
        }
    }
}
