using LAP.Core.SystemsLoader;
using LAP.Core.Utilities;
using Terraria.Localization;
using Terraria.ModLoader;
using UCA.Content.Items.Weapons.Magic.Ray;

namespace UCA.Content.Projectiles.HeldProj.Magic.VividClarityHeld
{
    public class VividClarityHeldParry : ModProjectile
    {
        public override string Texture => GetInstance<VividClarityAlt>().Texture;
        public override LocalizedText DisplayName => LAPUtilities.GetItemName<VividClarityAlt>();
        public override void SetStaticDefaults()
        {
            Projectile.AddHeldProj();
        }
        public override void SetDefaults()
        {
            
        }
    }
}
