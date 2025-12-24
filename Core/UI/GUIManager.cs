using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria.UI;
using UCA.Content.GUI;

namespace UCA.Core.UI
{
    public class GUIManager : ModSystem
    {
        public override void UpdateUI(GameTime gameTime)
        {
            if (ElementalRayUI.Active)
            {
                ElementalRayUI.Update();
            }
        }
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseIndex = layers.FindIndex(layer => layer.Name == "Vanilla: Mouse Text");
            if (ElementalRayUI.Active)
            {
                if (mouseIndex != -1)
                {
                    layers.Insert(mouseIndex, new LegacyGameInterfaceLayer("UCA ElementalRay UI", delegate ()
                    {
                        ElementalRayUI.Draw();
                        return true;
                    }, InterfaceScaleType.UI));
                }
            }
        }
    }
}
