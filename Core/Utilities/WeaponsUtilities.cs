using System;
using Terraria;

namespace UCA.Core.Utilities
{
    public static partial class UCAUtilities
    {
        //public static void DisabelStealthDamageBoost(this Player player)
        //{
        //    var calPlayer = player.Calamity();
        //    double averagedStealthGen = 0.8 * calPlayer.stealthGenMoving + 0.2 * calPlayer.stealthGenStandstill;
        //    double fakeStealthTime = 4f / averagedStealthGen;
        //    int realUseTime = Math.Max(player.HeldItem.useTime, player.HeldItem.useAnimation);
        //    double useTimeFactor = 0.75 + 0.75 * Math.Log(realUseTime + 2D, 4D);
        //    double stealthGenFactor = Math.Max(Math.Pow(fakeStealthTime, 2D / 3D), 1.5);
        //    calPlayer.stealthDamage -= (float)(calPlayer.rogueStealth * BalancingConstants.UniversalStealthStrikeDamageFactor * useTimeFactor * stealthGenFactor);
        //}
    }
}
