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
    }
}
