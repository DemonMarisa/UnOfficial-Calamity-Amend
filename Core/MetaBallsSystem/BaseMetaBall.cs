using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using UCA.Assets;
using UCA.Assets.Effects;

namespace UCA.Core.MetaBallsSystem
{
    public abstract class BaseMetaBall
    {
        // 这个元球对应的渲染目标
        public RenderTarget2D AlphaTexture;
        // 这个元球对应的背景
        public virtual Texture2D BgTexture => UCATextureRegister.ShabowBackGround.Value;

        /// <summary>
        /// 描边颜色
        /// </summary>
        public virtual Color EdgeColor => Color.White;

        /// <summary>
        /// 是否更新
        /// </summary>
        public virtual bool Active()
        {
            return false;
        }

        /// <summary>
        /// 提供的更新方法
        /// </summary>
        public virtual void Update() { }

        /// <summary>
        /// 提供的绘制方法
        /// </summary>
        public virtual void PrepareRenderTarget()
        {
            Main.spriteBatch.Draw(UCATextureRegister.WhiteCube.Value, new Vector2(960, 540), null, Color.White, 0, UCATextureRegister.WhiteCube.Size() / 2, 10, SpriteEffects.None, 0f);
        }

        public virtual void PrepareShader()
        {

            Main.graphics.GraphicsDevice.Textures[0] = AlphaTexture;
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

            Main.graphics.GraphicsDevice.Textures[1] = BgTexture;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;

            UCAShaderRegister.MetaballShader.Parameters["renderTargetSize"].SetValue(AlphaTexture.Size());
            UCAShaderRegister.MetaballShader.Parameters["bakcGroundSize"].SetValue(BgTexture.Size());
            UCAShaderRegister.MetaballShader.Parameters["edgeColor"].SetValue(EdgeColor.ToVector4());
            UCAShaderRegister.MetaballShader.Parameters["uTime"].SetValue((float)Main.time);

            UCAShaderRegister.MetaballShader.CurrentTechnique.Passes[0].Apply();

        }
    }
}
