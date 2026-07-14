using LAP.Assets.TextureRegister;
using LAP.Core.Enums;
using LAP.Core.ParticleSystem_ECS;
using LAP.Core.SystemsLoader;
using LAP.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace UCA.Content.VFXs.ExoBlasts
{
    public class CampFire : ParticleBehaviors
    {
        public static void Spawn(Vector2 position, Vector2 velocity, Color color, int lifetime, float Rot, float scale,int blendstate = BlendStateID.Additive)
        {
            int type = LAPContent.ParticleType<CampFire>();
            LAPContent.NewParticle(type, lifetime, position, velocity, color, Rot, scale, blendstate);
        }
        public override void OnSpawn(ref ParticleData particleDate)
        {
            particleDate.Opacity = 1f;
            particleDate.aifloat0 = 1f;
            particleDate.aiint0 = Main.rand.Next(0, 4);
            particleDate.aiint1 = Main.rand.Next(0, 4);
        }
        public override void Update(ref ParticleData particleDate)
        {
            particleDate.Velocity *= 0.9f;
            particleDate.Opacity = MathHelper.Lerp(particleDate.aifloat0, 0f, EasingHelper.EaseOutCubic(particleDate.LifetimeRatio));
        }
        public override void Draw(ref ParticleData particleDate)
        {
            Texture2D texture = LAPTextureRegister.CampFire.Value;
            Rectangle frame = texture.Frame(4, 4, particleDate.aiint0, particleDate.aiint1);
            Vector2 origin = frame.Size() * 0.5f;
            Main.spriteBatch.Draw(texture, particleDate.Position - Main.screenPosition, frame, particleDate.DrawColor * particleDate.Opacity, particleDate.Rotation, origin, particleDate.Scale, 0, 0f);
        }
    }
}
