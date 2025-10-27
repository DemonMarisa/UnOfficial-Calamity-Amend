using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using UCA.Assets;
using LAP.Core.ParticleSystem;
using UCA.Core.Utilities;
using LAP.Core.Utilities;

namespace UCA.Content.Particiles
{
    public class AbsorbFire : BaseParticle
    {
        public override int UseBlendStateID => BlendStateID.Additive;
        public float BeginRot = 0;
        public Vector2 Offset;
        public AbsorbFire(Vector2 position, Color color, int lifetime, float scale, float rot, float rotSpeed, int Owner, int Length, Vector2 offset)
        {
            Position = position;
            DrawColor = color;
            Lifetime = lifetime;
            Scale = scale;
            Rotation = rot;
            BeginRot = rot;
            RotSpeed = rotSpeed;
            OwnerID = Owner;
            DrawLength = Length;
            Offset = offset;
        }
        public int OwnerID;
        public float RotSpeed;
        public float DrawLengthOffset;
        public float DrawLength;
        public Projectile Owner => Main.projectile[OwnerID];
        public override void Update()
        {
            Velocity = Vector2.Zero;
            Position = Owner.Center + Offset.RotatedBy(Owner.rotation);
            Rotation += RotSpeed;
            DrawLengthOffset = MathHelper.Lerp(DrawLength, 0, EasingHelper.EaseOutCubic(LifetimeRatio));
            if (LifetimeRatio < 0.5f)
                Opacity = MathHelper.Lerp(0f, 1f, EasingHelper.EaseOutCubic(LifetimeRatio * 2));
            else
                Opacity = MathHelper.Lerp(1f, 0f, EasingHelper.EaseInCubic(LifetimeRatio));
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = UCATextureRegister.Fire.Value;
            Rectangle frame = UCATextureRegister.Fire.Frame(8, 8, (int)(LifetimeRatio * 64) % 8, (int)(LifetimeRatio * 8));
            Vector2 origin = frame.Size() * 0.5f;
            spriteBatch.Draw(texture, Position - Main.screenPosition + new Vector2(DrawLengthOffset, 0).RotatedBy(Rotation), frame, DrawColor * Opacity, BeginRot + Main.GlobalTimeWrappedHourly, origin, Scale, 0, 0f);


        }
    }
}
