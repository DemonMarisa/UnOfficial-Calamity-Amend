using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using UCA.Assets;

namespace UCA.Content.Projectiles.HeldProj.Magic.ElementRayHeld
{
    public partial class ElementRaySpecialHeldProj
    {
        #region 绘制
        public void DrawBaseElementalRay()
        {
            Texture2D texture = UCATextureRegister.ElementalRayBase.Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            float drawRotation = Projectile.rotation + (Projectile.spriteDirection == -1 ? MathHelper.PiOver2 + MathHelper.PiOver4 : MathHelper.PiOver4);
            Vector2 rotationPoint = texture.Size() / 2f;
            SpriteEffects flipSprite = Projectile.spriteDirection * Main.player[Projectile.owner].gravDir == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            // spriteBatch会自动把textures0设置为当前使用的材质，所以需要你手动改一下
            Main.spriteBatch.Draw(texture, drawPosition, null, Color.White, drawRotation, rotationPoint, Projectile.scale * Main.player[Projectile.owner].gravDir, flipSprite, 0f);
        }

        public void DrawMainFragments()
        {
            Texture2D texture = UCATextureRegister.MainElementalFragments.Value;
            Vector2 drawPosition = Projectile.Center + MainFragmentOffset - Main.screenPosition;
            float drawRotation = Projectile.rotation + (Projectile.spriteDirection == -1 ? MathHelper.PiOver2 + MathHelper.PiOver4 : MathHelper.PiOver4);
            Rectangle frame = texture.Frame(5, 1, WeaponStates, 0);
            Vector2 origin = frame.Size() * 0.5f;
            SpriteEffects flipSprite = Projectile.spriteDirection * Main.player[Projectile.owner].gravDir == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            // spriteBatch会自动把textures0设置为当前使用的材质，所以需要你手动改一下
            Main.spriteBatch.Draw(texture, drawPosition, frame, Color.White, drawRotation, origin, Projectile.scale * Main.player[Projectile.owner].gravDir, flipSprite, 0f);
        }
        public void DrawAuxFragments(int Filp)
        {
            Texture2D texture = UCATextureRegister.AuxElementalFragments.Value;
            Vector2 drawPosition = Projectile.Center + AuxFragmentOffset - Main.screenPosition;
            float drawRotation = Projectile.rotation - MathHelper.PiOver4;
            Rectangle frame;
            if (WeaponStates == ElementalRayState.Misc)
            {
                int FilpFrag = Projectile.spriteDirection == 1 ? ElementalRayState.Nebula : ElementalRayState.Vortex;
                frame = texture.Frame(4, 1, FilpFrag, 0);
            }
            else
            {
                frame = texture.Frame(4, 1, WeaponStates, 0);
            }
            Vector2 origin = frame.Size() * 0.5f;
            SpriteEffects flipSprite = SpriteEffects.FlipVertically;
            // spriteBatch会自动把textures0设置为当前使用的材质，所以需要你手动改一下
            Main.spriteBatch.Draw(texture, drawPosition, frame, Color.White, drawRotation, origin, Projectile.scale * Main.player[Projectile.owner].gravDir, flipSprite, 0f);
        }

        public void FilpDrawAuxFragments(int Filp)
        {
            Texture2D texture = UCATextureRegister.AuxElementalFragments.Value;
            Vector2 drawPosition = Projectile.Center + FilpAuxFragmentOffset - Main.screenPosition;
            float drawRotation = Projectile.rotation + MathHelper.PiOver4;
            Rectangle frame;
            if (WeaponStates == ElementalRayState.Misc)
            {
                int FilpFrag = Projectile.spriteDirection == 1 ? ElementalRayState.Vortex : ElementalRayState.Nebula;
                frame = texture.Frame(4, 1, FilpFrag, 0);
            }
            else
            {
                frame = texture.Frame(4, 1, WeaponStates, 0);
            }
            Vector2 origin = frame.Size() * 0.5f;
            SpriteEffects flipSprite = SpriteEffects.None;
            // spriteBatch会自动把textures0设置为当前使用的材质，所以需要你手动改一下
            Main.spriteBatch.Draw(texture, drawPosition, frame, Color.White, drawRotation, origin, Projectile.scale * Main.player[Projectile.owner].gravDir, flipSprite, 0f);
        }
#endregion
    }
}
