using Terraria.ModLoader;

namespace UCA.Core.GlobalInstance.Projectiles
{
    public partial class UCAGlobalProj : GlobalProjectile
    {
        public float[] ExtraAI = new float[10];
        public int StoredEU = -1;
        public int TargetIndex;
        public bool SetStealthStrike = false;
    }
}
