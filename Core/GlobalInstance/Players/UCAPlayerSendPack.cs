using LAP.Core.Keybind;
using LAP.Core.NetCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Core.Keybinds;
using UCA.Core.NetCode;

namespace UCA.Core.GlobalInstance.Players
{
    public partial class UCAPlayer : ModPlayer
    {
        public bool JustPressedWeaponSKill;
        public bool OldJustPressedWeaponSKill;
        public void UpdateMouseWorld()
        {
            if (Main.myPlayer == Player.whoAmI)
            {
                JustPressedWeaponSKill = LAPKeybind.WeaponSkillHotKey.JustPressed;
            }
            if (JustPressedWeaponSKill != OldJustPressedWeaponSKill)
            {
                // 只在多人模式的客户端执行
                if (Main.netMode == NetmodeID.MultiplayerClient && Main.myPlayer == Player.whoAmI)
                {
                    // 创建一个新的网络数据包
                    ModPacket packet = Mod.GetPacket();
                    // 写入一个自定义的消息类型，以便HandlePacket能识别
                    packet.Write((byte)UCANetCode.MessageType.SyncWeaponSkill);
                    // 写入是哪个玩家发送的
                    packet.Write((byte)Player.whoAmI);
                    // 写入鼠标坐标
                    packet.Write(LAPKeybind.WeaponSkillHotKey.JustPressed);
                    // 发送给服务器
                    packet.Send();
                }
            }
            if (Main.myPlayer == Player.whoAmI)
            {
                OldJustPressedWeaponSKill = LAPKeybind.WeaponSkillHotKey.JustPressed;
            }
        }
    }
}
