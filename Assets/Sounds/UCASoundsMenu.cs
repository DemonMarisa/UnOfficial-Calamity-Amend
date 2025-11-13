using Terraria;
using Terraria.Audio;

namespace UCA.Assets.Sounds
{
    public partial class SoundsMenu
    {
        #region 黑魂的
        public static SoundStyle MagicStaffCharge => new($"{DarkSoulSoundRoute}/MagicStaffCharge") { Volume = 1f, Pitch = 0 };
        public static SoundStyle MagicStaffFire => new($"{DarkSoulSoundRoute}/MagicStaffFire") { Volume = 0.4f, Pitch = Main.rand.NextFloat(0, 0.4f), MaxInstances = 0 };
        public static SoundStyle ReStoreCharge => new($"{DarkSoulSoundRoute}/ReStoreCharge") { Volume = 1f, Pitch = 0 };
        public static SoundStyle ReStoreRelease => new($"{DarkSoulSoundRoute}/ReStoreRelease") { Volume = 1f, Pitch = 0 };
        public static SoundStyle SoulGreatSwordSwimg => new($"{DarkSoulSoundRoute}/SoulGreatSwordSwimg") { Volume = 1f, Pitch = Main.rand.NextFloat(0.6f, 1.1f) };
        public static SoundStyle SoulOfCinderChange => new($"{DarkSoulSoundRoute}/SoulOfCinderChange") { Volume = 1f, Pitch = 0 };
        public static SoundStyle SoulStreamCharge => new($"{DarkSoulSoundRoute}/SoulStreamCharge") { Volume = 1f, Pitch = 0 };
        public static SoundStyle SoulStreamFire => new($"{DarkSoulSoundRoute}/SoulStreamFire") { Volume = 1f, Pitch = 0 };
        #endregion
        #region 杂项
        public static SoundStyle Fire => new($"{MiscSoundRoute}/Fire") { Volume = 1f, Pitch = Main.rand.NextFloat(0.6f, 1.1f) };
        public static SoundStyle FireBlast => new($"{MiscSoundRoute}/FireBlast") { Volume = 1f, Pitch = Main.rand.NextFloat(0.6f, 1.1f) };
        public static SoundStyle FireBallBlast => new($"{MiscSoundRoute}/FireBallBlast") { Volume = 0.4f, Pitch = 0f, MaxInstances = 3 };
        public static SoundStyle RiseBlast => new($"{MiscSoundRoute}/RiseBlast") { Volume = 1f, Pitch = Main.rand.NextFloat(0.6f, 1.1f) };
        public static SoundStyle Blast01 => new($"{MiscSoundRoute}/Blast01") { Volume = 1f, Pitch = 1f };
        public static SoundStyle Lighting => new($"{MiscSoundRoute}/Lighting") { Volume = 1f, Pitch = Main.rand.NextFloat(0.6f, 1.1f) };
        public static SoundStyle LightingHit => new($"{MiscSoundRoute}/LightingHit") { Volume = 1f, Pitch = Main.rand.NextFloat(0.6f, 1.1f), MaxInstances = 0 };
        public static SoundStyle SmallLighting => new($"{MiscSoundRoute}/SmallLighting") { Volume = 1f, Pitch = Main.rand.NextFloat(0.6f, 1.1f) };
        public static SoundStyle FastLighting => new($"{MiscSoundRoute}/FastLighting") { Volume = 1f, Pitch = Main.rand.NextFloat(0.6f, 1.1f), MaxInstances = 0 };
        public static SoundStyle MetalHit => new($"{MiscSoundRoute}/MetalHit") { Volume = 1f, Pitch = Main.rand.NextFloat(0.6f, 1.1f) };
        public static SoundStyle ShadowBoltStaffSkillrelease => new($"{MiscSoundRoute}/ShadowBoltStaffSkillrelease") { Volume = 1f, Pitch = Main.rand.NextFloat(-0.4f, -0.2f), MaxInstances = 0 };
        public static SoundStyle SwordAttack => new($"{MiscSoundRoute}/SwordAttack") { Volume = 1f, Pitch = Main.rand.NextFloat(0.6f, 1.1f), MaxInstances = 0 };
        public static SoundStyle SwordSwing => new($"{MiscSoundRoute}/SwordSwing") { Volume = 1f, Pitch = Main.rand.NextFloat(0.6f, 1.1f), MaxInstances = 0 };
        public static SoundStyle SwordSwing2 => new($"{MiscSoundRoute}/SwordSwing2") { Volume = 1f, Pitch = Main.rand.NextFloat(0.1f, 0.3f), MaxInstances = 0 };
        #endregion
    }
}
