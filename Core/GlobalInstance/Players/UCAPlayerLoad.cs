using Terraria.ModLoader;

namespace UCA.Core.GlobalInstance.Players
{
    public partial class UCAPlayer : ModPlayer
    {
        public bool NameIsMAGNOLIA = false;
        public override void Load()
        {
        }
        public override void Unload()
        {
            NameIsMAGNOLIA = false;
        }
        public override void OnEnterWorld()
        {
            if (Player.name == "MAGNOLIA" || Player.name == "Magnolia" || Player.name == "Lilac" || Player.name == "Nola" || Player.name == "Lilia")
            {
                NameIsMAGNOLIA = true;
            }
        }
    }
}
