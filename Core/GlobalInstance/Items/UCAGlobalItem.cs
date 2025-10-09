using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
using UCA.Assets;
using UCA.Content.Configs;

namespace UCA.Core.GlobalInstance.Items
{
    public partial class UCAGlobalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;

        public override void PostDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (!UCAConfig.Instance.UCATurnoffCorner)
            {
                if (DrawSmallIcon)
                {
                    Vector2 iconPosition = position + new Vector2(4f, 4f);
                    float iconScale = 0.45f;

                    spriteBatch.Draw(UCATextureRegister.SmallIcon.Value, iconPosition, null, Color.White, 0f, Vector2.Zero, iconScale, SpriteEffects.None, 0f);
                }
            }
        }
    }
}
