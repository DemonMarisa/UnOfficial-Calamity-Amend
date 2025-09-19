using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Content.Configs;

namespace UCA.Core.ParticleSystem
{
    public abstract class BaseParticle
    {
        #region 基础属性

        /// <summary>
        /// 使用材质
        /// </summary>
        public virtual string Texture => (GetType().Namespace + "." + GetType().Name).Replace('.', '/');

        /// <summary>
        /// 该粒子存在了多少帧，一般不需要手动修改这个值
        /// </summary>
        public int Time;

        /// <summary>
        /// 粒子的存在时间上限
        /// </summary>
        public int Lifetime = 0;

        /// <summary>
        /// 位置与向量
        /// </summary>
        public Vector2 Position;
        public Vector2 Velocity;

        public Vector2 Origin;
        public Color DrawColor;
        public float Rotation;
        public float Scale = 1f;

        /// <summary>
        /// 粒子的透明度
        /// </summary>
        public float Opacity = 1f;

        /// <summary>
        /// 生命周期的进度，介于0到1之间。
        /// 0表示粒子刚生成，1表示粒子消失。
        /// </summary>
        public float LifetimeRatio => Time / (float)Lifetime;

        /// <summary>
        /// 渲染的混合模式，默认为<see cref="BlendState.AlphaBlend"/>.
        /// </summary>
        public virtual BlendState BlendState => BlendState.AlphaBlend;
        #endregion

        /// <summary>
        /// 在世界内生成粒子
        /// </summary>
        /// <returns></returns>
        public BaseParticle Spawn()
        {
            if (Main.netMode == NetmodeID.Server)
                return this;

            // 初始化时间
            Time = 0;
            // 如果粒子数量过多，则清除第一个粒子并添加
            if (BaseParticleManager.ActiveParticles.Count > UCAConfig.Instance.ParticleLimit)
                BaseParticleManager.ActiveParticles.RemoveAt(0);

            // 用于控制总数的列表
            BaseParticleManager.ActiveParticles.Add(this);

            OnSpawn();
            return this;
        }
        public virtual void OnSpawn() { }

        /// <summary>
        /// 粒子的更新，默认不做任何操作
        /// </summary>
        public virtual void Update()
        {

        }

        /// <summary>
        /// 立刻清除粒子
        /// </summary>
        public void Kill()
        {
            Time = Lifetime;
        }

        public virtual void OnKill() { }

        /// <summary>
        /// 覆写这个就可以自定义绘制
        /// </summary>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            spriteBatch.Draw(texture, Position - Main.screenPosition, null, DrawColor * Opacity, Rotation, texture.Size() / 2, Scale, SpriteEffects.None, 0);
        }
    }
}
