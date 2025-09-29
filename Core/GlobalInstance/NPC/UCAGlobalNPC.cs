using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace UCA.Core.GlobalInstance.NPC
{
    public partial class UCAGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
    }
}
