namespace UCA.Core.ParticleSystem
{
    public static class ParticleUtilities
    {
        /// <summary>
        /// 移除所有的粒子
        /// </summary>
        public static void RemoveAll()
        {
            foreach (BaseParticle particle in BaseParticleManager.ActiveParticles)
            {
                particle.Kill();
            }
        }
    }
}
