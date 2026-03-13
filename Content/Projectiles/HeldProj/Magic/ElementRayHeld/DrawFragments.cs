using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using UCA.Assets;

namespace UCA.Content.Projectiles.HeldProj.Magic.ElementRayHeld
{
    public partial class ElementRaySpecialHeldProj
    {
        public Vector2 MainFragmentOffset = new Vector2(0, 0);
        public Vector2 AuxFragmentOffset = new Vector2(0, 0);
        public Vector2 FilpAuxFragmentOffset = new Vector2(0, 0);

        public float MainFragmentRot;
        public float AuxFragmentRot;
        public float FilpAuxFragmentRot;
        #region 绘制
        public void DrawBaseElementalRay(Color lightColor)
        {
            Texture2D texture = UCATextureRegister.ElementalRayBase.Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            float drawRotation = Projectile.rotation + (Projectile.spriteDirection == -1 ? MathHelper.PiOver2 + MathHelper.PiOver4 : MathHelper.PiOver4);
            Vector2 rotationPoint = texture.Size() / 2f;
            SpriteEffects flipSprite = Projectile.spriteDirection * Main.player[Projectile.owner].gravDir == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            // spriteBatch会自动把textures0设置为当前使用的材质，所以需要你手动改一下
            Main.spriteBatch.Draw(texture, drawPosition, null, lightColor, drawRotation, rotationPoint, Projectile.scale * Main.player[Projectile.owner].gravDir, flipSprite, 0f);
        }

        public void DrawMainFragments(Color lightColor)
        {
            Texture2D texture = UCATextureRegister.MainElementalFragments.Value;
            Vector2 drawPosition = Projectile.Center + MainFragmentOffset - Main.screenPosition;
            Rectangle frame = texture.Frame(5, 1, (int)WeaponStates, 0);
            Vector2 origin = frame.Size() * 0.5f;
            SpriteEffects flipSprite = Projectile.spriteDirection * Main.player[Projectile.owner].gravDir == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            // spriteBatch会自动把textures0设置为当前使用的材质，所以需要你手动改一下
            Main.spriteBatch.Draw(texture, drawPosition, frame, lightColor, MainFragmentRot, origin, Projectile.scale * Main.player[Projectile.owner].gravDir, flipSprite, 0f);
        }
        public void DrawAuxFragments(Color lightColor)
        {
            Texture2D texture = UCATextureRegister.AuxElementalFragments.Value;
            Vector2 drawPosition = Projectile.Center + AuxFragmentOffset - Main.screenPosition;
            Rectangle frame;
            if (WeaponStates == ElementalRayState.Misc)
            {
                int FilpFrag = Projectile.spriteDirection == 1 ? ElementalRayState.Nebula : ElementalRayState.Vortex;
                frame = texture.Frame(4, 1, FilpFrag, 0);
            }
            else
            {
                frame = texture.Frame(4, 1, (int)WeaponStates, 0);
            }
            Vector2 origin = frame.Size() * 0.5f;
            SpriteEffects flipSprite = SpriteEffects.FlipVertically;
            // spriteBatch会自动把textures0设置为当前使用的材质，所以需要你手动改一下
            Main.spriteBatch.Draw(texture, drawPosition, frame, lightColor, AuxFragmentRot, origin, Projectile.scale * Main.player[Projectile.owner].gravDir, flipSprite, 0f);
        }

        public void FilpDrawAuxFragments(Color lightColor)
        {
            Texture2D texture = UCATextureRegister.AuxElementalFragments.Value;
            Vector2 drawPosition = Projectile.Center + FilpAuxFragmentOffset - Main.screenPosition;
            Rectangle frame;
            if (WeaponStates == ElementalRayState.Misc)
            {
                int FilpFrag = Projectile.spriteDirection == 1 ? ElementalRayState.Vortex : ElementalRayState.Nebula;
                frame = texture.Frame(4, 1, FilpFrag, 0);
            }
            else
            {
                frame = texture.Frame(4, 1, (int)WeaponStates, 0);
            }
            Vector2 origin = frame.Size() * 0.5f;
            SpriteEffects flipSprite = SpriteEffects.None;
            // spriteBatch会自动把textures0设置为当前使用的材质，所以需要你手动改一下
            Main.spriteBatch.Draw(texture, drawPosition, frame, lightColor, FilpAuxFragmentRot, origin, Projectile.scale * Main.player[Projectile.owner].gravDir, flipSprite, 0f);
        }
        #endregion
    }
}
