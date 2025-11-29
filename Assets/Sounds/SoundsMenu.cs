using Terraria;
using Terraria.Audio;

namespace UCA.Assets.Sounds
{
    public partial class SoundsMenu
    {
        public static float RandomPitch => Main.rand.NextBool() ? Main.rand.NextFloat(0.1f, 0.3f) : Main.rand.NextFloat(0.9f, 1.1f);
        public static string WeaponsSoundRoute => "UCA/Assets/Sounds/Weapons";
        public static string MiscSoundRoute => "UCA/Assets/Sounds/Misc";
        public static string MAGNOLIASoundRoute => "UCA/Assets/Sounds/MAGNOLIA";
        public static string DarkSoulSoundRoute => "UCA/Assets/Sounds/DS";
        #region 永夜射线
        public static SoundStyle NightRayCharge => new($"{WeaponsSoundRoute}/Magic/NightRay/MagicCharge") { Volume = 1f, Pitch = Main.rand.NextFloat(0.9f, 1.1f) };
        public static SoundStyle NightRayAttack => new($"{WeaponsSoundRoute}/Magic/NightRay/NightRayAttack") { Volume = 1f, Pitch = Main.rand.NextFloat(0.4f, 0.8f) };
        public static SoundStyle NightRayHeavyAttack => new($"{WeaponsSoundRoute}/Magic/NightRay/NightRayHeavyAttack") { Volume = 1f, Pitch = Main.rand.NextFloat(0.3f, 1.1f), MaxInstances = 0};
        public static SoundStyle NightRayHit => new($"{WeaponsSoundRoute}/Magic/NightRay/NightRayHit") { Volume = 1f, Pitch = Main.rand.NextFloat(0.4f, 0.9f), MaxInstances = 3 };
        public static SoundStyle NightRayShieldBreak => new($"{WeaponsSoundRoute}/Magic/NightRay/NightShieldBreak") { Volume = 1f, Pitch = Main.rand.NextFloat(0.4f, 0.9f) };
        public static SoundStyle NightShieldCharge => new($"{WeaponsSoundRoute}/Magic/NightRay/NightShieldCharge") { Volume = 1f, Pitch = Main.rand.NextFloat(0.4f, 0.9f) };
        public static SoundStyle NightShieldHit => new($"{WeaponsSoundRoute}/Magic/NightRay/NightShieldHit") { Volume = 1f, Pitch = Main.rand.NextFloat(0.4f, 0.9f) };
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
        #region 等离子射线
        public static SoundStyle PlasmaBlastBomb => new($"{WeaponsSoundRoute}/Magic/PlasmaRod/PlasmaBlastBomb") { Volume = 1f, Pitch = Main.rand.NextFloat(0.1f, 0.4f) };
        public static SoundStyle PlasmaRodAttack => new($"{WeaponsSoundRoute}/Magic/PlasmaRod/PlasmaRodAttack") { Volume = 1f, Pitch = Main.rand.NextFloat(0.4f, 0.9f) };
        public static SoundStyle PlasmaRodSwingHit => new($"{WeaponsSoundRoute}/Magic/PlasmaRod/PlasmaRodSwing") { Volume = 1f, Pitch = Main.rand.NextFloat(0.8f, 1f) };
        public static SoundStyle PlasmaSparkHit => new($"{WeaponsSoundRoute}/Magic/PlasmaRod/PlasmaSparkHit") { Volume = 1f, Pitch = Main.rand.NextFloat(0.8f, 1.2f) };
        #endregion
        #region 泰拉射线
        public static SoundStyle TerraRestore => new($"{WeaponsSoundRoute}/Magic/TerraRay/TerraRayRestore") { Volume = 1f, Pitch = Main.rand.NextFloat(0.15f, 0.6f) };
        public static SoundStyle TerraRestoreRelease => new($"{WeaponsSoundRoute}/Magic/TerraRay/TerraRayRestoreRelease") { Volume = 1f, Pitch = Main.rand.NextFloat(0.5f, 0.6f) };
        public static SoundStyle TerraRightCharge => new($"{WeaponsSoundRoute}/Magic/TerraRay/TerraRightCharge") { Volume = 1f, Pitch = 1f };
        public static SoundStyle TerraLanceShoot => new($"{WeaponsSoundRoute}/Magic/TerraRay/TerraLanceShoot") { Volume = 1f, Pitch = 1f };
        public static SoundStyle TerraRayHit => new($"{WeaponsSoundRoute}/Magic/TerraRay/TerraRayHit") { Volume = 1f, Pitch = Main.rand.NextFloat(0.8f, 1.2f) };
        public static SoundStyle TerraRayLeftFire => new($"{WeaponsSoundRoute}/Magic/TerraRay/TerraRayLeftFire") { Volume = 1f, Pitch = Main.rand.NextFloat(0.2f, 0.3f) };
        public static SoundStyle TerraTreeBreak => new($"{WeaponsSoundRoute}/Magic/TerraRay/TerraTreeBreak") { Volume = 0.3f, Pitch = Main.rand.NextFloat(0.3f, 1.1f) };
        #endregion
        #region 马格诺利亚
        public static SoundStyle MAGNOLIASPRelease => new($"{MAGNOLIASoundRoute}/MAGNOLIASPRelease") { Volume = 0.4f, Pitch = Main.rand.NextFloat(0.1f, 0.2f) };
        #endregion

        #region 锤子们
        private static SoundStyle Smash_AirHeavy1 => new($"{WeaponsSoundRoute}/Rogue/ThrownHammer/{nameof(Smash_AirHeavy1)}");
        private static SoundStyle Smash_AirHeavy2 => new($"{WeaponsSoundRoute}/Rogue/ThrownHammer/{nameof(Smash_AirHeavy2)}");
        public static SoundStyle Smash_GroundHeavy => new($"{WeaponsSoundRoute}/Rogue/ThrownHammer/{nameof(Smash_GroundHeavy)}");
        private static SoundStyle Hammer_Shoot1 => new($"{WeaponsSoundRoute}/Rogue/ThrownHammer/{nameof(Hammer_Shoot1)}");
        private static SoundStyle Hammer_Shoot2 => new($"{WeaponsSoundRoute}/Rogue/ThrownHammer/{nameof(Hammer_Shoot2)}");
        private static SoundStyle Hammer_Shoot3 => new($"{WeaponsSoundRoute}/Rogue/ThrownHammer/{nameof(Hammer_Shoot3)}");
        private static SoundStyle Atom_Strike1 => new($"{WeaponsSoundRoute}/Rogue/{nameof(Atom_Strike1)}");
        private static SoundStyle Atom_Strike2 => new($"{WeaponsSoundRoute}/Rogue/{nameof(Atom_Strike2)}");
        private static SoundStyle Atom_Strike3 => new($"{WeaponsSoundRoute}/Rogue/{nameof(Atom_Strike3)}");
        public static SoundStyle Pipes => new($"{WeaponsSoundRoute}/Rogue/ThrownHammer/{nameof(Pipes)}");
        public static SoundStyle Mana_Toss=> new($"{WeaponsSoundRoute}/Rogue/{nameof(Mana_Toss)}");
        public static SoundStyle[] Hammer_Shoot =>
            [
                Hammer_Shoot1,
                Hammer_Shoot2,
                Hammer_Shoot3,
            ];
        public static SoundStyle[] Smash_AirHeavy =>
            [
                Smash_AirHeavy1,
                Smash_AirHeavy2,
            ];
        public static SoundStyle[] Atom_Strike =>
            [
                Atom_Strike1,
                Atom_Strike2,
                Atom_Strike3,
            ];
        #endregion
    }
}
