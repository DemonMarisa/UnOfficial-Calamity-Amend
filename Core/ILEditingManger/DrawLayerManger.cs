using Terraria;
using Terraria.ModLoader;
using UCA.Core.Graphics.DrawNode;
using UCA.Core.Graphics.PixelRender;
using UCA.Core.MetaBallsSystem;
using UCA.Core.ParticleSystem;

namespace UCA.Core.ILEditingManger
{
    public class DrawLayerManger : ModSystem
    {
        public override void Load()
        {
            On_Main.DrawDust += MetaBallManager.DrawRenderTarget;
            On_Main.DrawDust += BaseParticleManager.DrawParticles;
            On_Main.DrawDust += PixelRenderSystem.DrawTarget_Dust;
            On_Main.DrawDust += NodeManager.DrawNode;
        }

        public override void Unload()
        {
            On_Main.DrawDust -= MetaBallManager.DrawRenderTarget;
            On_Main.DrawDust -= BaseParticleManager.DrawParticles;
            On_Main.DrawDust -= PixelRenderSystem.DrawTarget_Dust;
            On_Main.DrawDust -= NodeManager.DrawNode;
        }
    }
}
