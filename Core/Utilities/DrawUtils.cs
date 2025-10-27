using Microsoft.Xna.Framework;
using UCA.Content.Particiles;

namespace UCA.Core.Utilities
{
    public static partial class UCAUtilities
    {
        public static void GenStarLine(Vector2 BeginPos, Vector2 EndPos, float GenStep, Color color)
        {
            for (int i = 0; i < GenStep; i++)
            {
                Vector2 SpawnVector = Vector2.Lerp(BeginPos, EndPos, i / GenStep);
                new MediumGlowBall(SpawnVector, Vector2.Zero, color, 60, 0, 1f, 0.1f, 0).Spawn();
            }
        }
    }
}
