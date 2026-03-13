using LAP.Core.Graphics.DrawNode;
using LAP.Core.ParticleSystem;

namespace UCA.Content.DrawNodes
{
    public class TerraVine : DrawNode
    {
        public override int BlendState => BlendStateID.Additive;
        public override bool UseShader => true;
    }
}
