using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;

namespace UCA.Assets
{
    public class SoundsMenu
    {
        public static float RandomPitch => Main.rand.NextBool() ? Main.rand.NextFloat(0.1f, 0.3f) : Main.rand.NextFloat(0.9f, 1.1f);
        public static string WeaponsSoundRoute => "UCA/Assets/Sounds/Weapons";
        public static SoundStyle NightRayCharge => new($"{WeaponsSoundRoute}/Magic/MagicCharge") { Volume = 1f, Pitch = Main.rand.NextFloat(0.9f, 1.1f) };
        public static SoundStyle NightRayAttack => new($"{WeaponsSoundRoute}/Magic/NightRayAttack") { Volume = 1f, Pitch = Main.rand.NextFloat(0.4f, 0.8f) };
        public static SoundStyle NightRayHeavyAttack => new($"{WeaponsSoundRoute}/Magic/NightRayHeavyAttack") { Volume = 1f, Pitch = Main.rand.NextFloat(0.3f, 1.1f) };
        public static SoundStyle NightRayHit => new($"{WeaponsSoundRoute}/Magic/NightRayHit") { Volume = 1f, Pitch = Main.rand.NextFloat(0.4f, 0.9f) };
        public static SoundStyle NightRayShieldBreak => new($"{WeaponsSoundRoute}/Magic/NightShieldBreak") { Volume = 1f, Pitch = Main.rand.NextFloat(0.4f, 0.9f) };
        public static SoundStyle NightShieldCharge => new($"{WeaponsSoundRoute}/Magic/NightShieldCharge") { Volume = 1f, Pitch = Main.rand.NextFloat(0.4f, 0.9f) };
        public static SoundStyle NightShieldHit => new($"{WeaponsSoundRoute}/Magic/NightShieldHit") { Volume = 1f, Pitch = Main.rand.NextFloat(0.4f, 0.9f) };
    }
}
