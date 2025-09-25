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
        #region 永夜射线
        public static SoundStyle NightRayCharge => new($"{WeaponsSoundRoute}/Magic/MagicCharge") { Volume = 1f, Pitch = Main.rand.NextFloat(0.9f, 1.1f) };
        public static SoundStyle NightRayAttack => new($"{WeaponsSoundRoute}/Magic/NightRayAttack") { Volume = 1f, Pitch = Main.rand.NextFloat(0.4f, 0.8f) };
        public static SoundStyle NightRayHeavyAttack => new($"{WeaponsSoundRoute}/Magic/NightRayHeavyAttack") { Volume = 1f, Pitch = Main.rand.NextFloat(0.3f, 1.1f) };
        public static SoundStyle NightRayHit => new($"{WeaponsSoundRoute}/Magic/NightRayHit") { Volume = 1f, Pitch = Main.rand.NextFloat(0.4f, 0.9f) };
        public static SoundStyle NightRayShieldBreak => new($"{WeaponsSoundRoute}/Magic/NightShieldBreak") { Volume = 1f, Pitch = Main.rand.NextFloat(0.4f, 0.9f) };
        public static SoundStyle NightShieldCharge => new($"{WeaponsSoundRoute}/Magic/NightShieldCharge") { Volume = 1f, Pitch = Main.rand.NextFloat(0.4f, 0.9f) };
        public static SoundStyle NightShieldHit => new($"{WeaponsSoundRoute}/Magic/NightShieldHit") { Volume = 1f, Pitch = Main.rand.NextFloat(0.4f, 0.9f) };
        #endregion

        #region 屠杀射线
        public static SoundStyle CarnageBallHit => new($"{WeaponsSoundRoute}/Magic/CarnageRay/CarnageBallHit") { Volume = 1f, Pitch = Main.rand.NextFloat(0.9f, 1.1f) };
        public static SoundStyle CarnageBallSpawn => new($"{WeaponsSoundRoute}/Magic/CarnageRay/CarnageBallSpawn") { Volume = 1f, Pitch = Main.rand.NextFloat(0.4f, 0.8f) };
        public static SoundStyle CarnageCharge => new($"{WeaponsSoundRoute}/Magic/CarnageRay/CarnageCharge") { Volume = 1f, Pitch = Main.rand.NextFloat(0.3f, 1.1f) };
        public static SoundStyle CarnageLeftShoot => new($"{WeaponsSoundRoute}/Magic/CarnageRay/CarnageLeftShoot") { Volume = 1f, Pitch = Main.rand.NextFloat(0.1f, 0.4f) };
        public static SoundStyle CarnageRightUse => new($"{WeaponsSoundRoute}/Magic/CarnageRay/CarnageRightUse") { Volume = 1f, Pitch = Main.rand.NextFloat(0.4f, 0.9f) };
        public static SoundStyle CarnageSkillMeleeHit => new($"{WeaponsSoundRoute}/Magic/CarnageRay/CarnageSkillMeleeHit") { Volume = 1f, Pitch = Main.rand.NextFloat(0.3f, 1.3f) };
        public static SoundStyle CarnageSwingBeign => new($"{WeaponsSoundRoute}/Magic/CarnageRay/CarnageSwingBeign") { Volume = 1f, Pitch = Main.rand.NextFloat(0.8f, 1.2f) };
        #endregion
    }
}
