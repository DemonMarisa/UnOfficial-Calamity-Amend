using CalamityMod;
using LAP.Core.BaseClass;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using UCA.Content.MetaBalls;
using UCA.Content.Particiles;
using UCA.Content.UCACooldowns;
using UCA.Core.Utilities;

namespace UCA.Content.Projectiles.HealPRoj
{
    public class NebulaHeal : BaseHealProj
    {
        public override int HealAmt => Main.rand.Next(6, 11);
        public int Time;
        public Vector2 OldPos;
        public Vector2 OldPos2;
        public override void ExAI()
        {
            if (!LAPUtilities.OutOffScreen(Projectile.Center))
            {
                NebulaMetaBall.SpawnParticle(Projectile.Center, Vector2.Zero, 0.15f, 45);
            }
        }

        public override void ExKill()
        {
            for (int i = 0; i < 5; i++)
            {
                Vector2 spawnVec = Vector2.UnitX.RotateRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.2f, 0.3f) * 6;
                NebulaMetaBall.SpawnParticle(Projectile.Center, spawnVec, 0.2f, 45);
            }
            for (int i = 0; i < 5; i++)
            {
                Vector2 spawnVec = Vector2.UnitX.RotateRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.6f, 1f) * 6;
                Color color = LAPUtilities.LerpColor(Color.Violet, Color.LightPink);
                new BrokenGlass(Projectile.Center, spawnVec, color, Main.rand.Next(45, 60), Main.rand.NextFloat(MathHelper.TwoPi), 1f, 0.1f, false).Spawn();
            }
            Healer.AddCooldown(NebulaBoost.ID, 1800);
        }
    }
}
