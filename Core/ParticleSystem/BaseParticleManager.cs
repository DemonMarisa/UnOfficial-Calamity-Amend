using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace UCA.Core.ParticleSystem
{
    public class BaseParticleManager : ModSystem
    {
        // 别在外部可以修改了，至少别人都加了readonly（
        public static readonly List<BaseParticle> ActiveParticles = [];
        // 先绘制先更新的粒子
        public static readonly List<BaseParticle> PriorityActiveParticles = [];
        // 储存所有粒子类型的ID
        // public static readonly Dictionary<Type, int> particleTypes;
        #region 加载卸载
        public override void Load()
        {
            On_Main.DrawDust += DrawParticles;
        }
        public override void Unload()
        {
            On_Main.DrawDust -= DrawParticles;
        }
        #endregion

        /// <summary>
        /// 清除世界状态时调用（例如退出世界时）。
        /// </summary>
        public override void ClearWorld()
        {
            ActiveParticles.Clear();
        }

        // 粒子更新
        public override void PostUpdateDusts()
        {
            if (Main.dedServ)
                return;

            UpdatePriorityParticles();

            if (ActiveParticles.Count == 0)
                return;

            for (int i = 0; i < ActiveParticles.Count; i++)
            {
                ActiveParticles[i].Update();
                ActiveParticles[i].Position += ActiveParticles[i].Velocity;
                ActiveParticles[i].Time++;
            }

            // 移除生命周期已结束的粒子
            ActiveParticles.RemoveAll(particle =>
            {
                if (particle.Time >= particle.Lifetime)
                {
                    particle.OnKill();
                    return true;
                }
                return false;
            });
        }
        public static void UpdatePriorityParticles()
        {
            if (PriorityActiveParticles.Count == 0)
                return;

            for (int i = 0; i < PriorityActiveParticles.Count; i++)
            {
                PriorityActiveParticles[i].Update();
                PriorityActiveParticles[i].Position += PriorityActiveParticles[i].Velocity;
                PriorityActiveParticles[i].Time++;
            }
            // 移除生命周期已结束的粒子
            PriorityActiveParticles.RemoveAll(particle =>
            {
                if (particle.Time >= particle.Lifetime)
                {
                    particle.OnKill();
                    return true;
                }
                return false;
            });
        }
        // 绘制粒子
        public static void DrawParticles(On_Main.orig_DrawDust orig, Main self)
        {
            // 调用源
            orig(self);

            #region 渲染粒子
            #region 渲染优先粒子
            if (PriorityActiveParticles.Count != 0)
            {
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

                for (int i = 0; i < PriorityActiveParticles.Count; i++)
                {
                    if (PriorityActiveParticles[i].BlendState != BlendState.AlphaBlend)
                        continue;

                    PriorityActiveParticles[i].Draw(Main.spriteBatch);
                }

                Main.spriteBatch.End();

                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

                for (int i = 0; i < PriorityActiveParticles.Count; i++)
                {
                    if (PriorityActiveParticles[i].BlendState != BlendState.NonPremultiplied)
                        continue;

                    PriorityActiveParticles[i].Draw(Main.spriteBatch);
                }

                Main.spriteBatch.End();

                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

                for (int i = 0; i < PriorityActiveParticles.Count; i++)
                {
                    if (PriorityActiveParticles[i].BlendState != BlendState.Additive)
                        continue;

                    PriorityActiveParticles[i].Draw(Main.spriteBatch);
                }

                Main.spriteBatch.End();
            }
            #endregion
            #region 渲染常规粒子

            if (ActiveParticles.Count != 0)
            {
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

                for (int i = 0; i < ActiveParticles.Count; i++)
                {
                    if (ActiveParticles[i].BlendState != BlendState.AlphaBlend)
                        continue;

                    ActiveParticles[i].Draw(Main.spriteBatch);
                }

                Main.spriteBatch.End();

                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

                for (int i = 0; i < ActiveParticles.Count; i++)
                {
                    if (ActiveParticles[i].BlendState != BlendState.NonPremultiplied)
                        continue;

                    ActiveParticles[i].Draw(Main.spriteBatch);
                }

                Main.spriteBatch.End();

                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

                for (int i = 0; i < ActiveParticles.Count; i++)
                {
                    if (ActiveParticles[i].BlendState != BlendState.Additive)
                        continue;

                    ActiveParticles[i].Draw(Main.spriteBatch);
                }

                Main.spriteBatch.End();
            }
            #endregion
            #endregion
        }
    }
}
