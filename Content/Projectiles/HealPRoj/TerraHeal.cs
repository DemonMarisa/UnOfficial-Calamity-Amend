using LAP.Core.BaseClass.Projectiles;
using LAP.Core.Presets.Content;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using UCA.Content.Particiles;
using UCA.Content.VFXs;

namespace UCA.Content.Projectiles.HealPRoj
{
    public class TerraHeal : BaseHealProj
    {
        // public List<TerraLanceVine> Vine = [];
        public List<Vector2> AvailableOldPos = [];
        public override int HealAmt => Main.rand.Next(6, 12);
        public override float FlySpeed => 12f;
        public override float Acceleration => 35f;
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
                ParticlePreset.NewTMGlowBall(Projectile.Center + vel * i, Vector2.Zero, RandomColor2, 15, 0.12f, Main.rand.NextFloat(0.1f, 0.3f));
            }
        }
        public override void ExKill()
        {
            Vector2 firPos = Projectile.Center;
            for (int i = 0; i < 3; i++)
            {
                float rot = MathHelper.TwoPi / 3;

                Vector2 firVec = Vector2.UnitX.RotatedBy(rot * i).RotatedByRandom(MathHelper.TwoPi);
                Color color = Main.rand.NextBool() ? Color.ForestGreen : Color.SaddleBrown;

                TerraVine.Spawn(firPos, firVec * Main.rand.NextFloat(0.8f, 1.4f), color, Main.rand.NextBool() ? -1 : 1, 1.8f, Main.rand.NextFloat(8f, 12), Main.rand.NextFloat(2f, 3f));
            }
            for (int i = 0; i < 5; i++)
            {
                Color RandomColor = Color.Lerp(Color.LightGreen, Color.Green, Main.rand.NextFloat(0, 1));
                new MediumGlowBall(Projectile.Center, Vector2.Zero, RandomColor, 180, 0, 1, 0.12f, Main.rand.NextFloat(0.5f, 0.7f)).Spawn();
            }
        }
    }
}
