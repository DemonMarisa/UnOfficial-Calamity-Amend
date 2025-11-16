using LAP.Core.GlobalInstance.Players;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using UCA.Core.GlobalInstance.Players;

namespace UCA.Core.NetCode
{
    public partial class UCANetCode
    {
        public static void HandlePacket(BinaryReader reader, int whoAmI)
        {
            MessageType msgType = (MessageType)reader.ReadByte();

            switch (msgType)
            {
                case MessageType.SyncWeaponSkill:
                    // 从数据包中按写入顺序读取数据
                    byte playerIndex = reader.ReadByte();
                    bool weaponSkill = reader.ReadBoolean();
                    // 如果是在服务器端收到了这个包
                    if (Main.netMode == NetmodeID.Server)
                    {
                        // 将这个信息转发给所有其他客户端，让他们也知道
                        // 创建一个新的包用于广播
                        ModPacket broadcastPacket = UCA.Instance.GetPacket();
                        broadcastPacket.Write((byte)MessageType.SyncWeaponSkill);
                        broadcastPacket.Write(playerIndex);
                        broadcastPacket.Write(weaponSkill);
                        // 发送给所有人 (-1)，除了原始发送者 (whoAmI)
                        broadcastPacket.Send(-1, whoAmI);
                    }
                    // 如果是在客户端收到了服务器转发的包
                    else if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        // 在本地更新对应玩家的鼠标位置
                        UCAPlayer modPlayer = Main.player[playerIndex].GetModPlayer<UCAPlayer>();
                        modPlayer.JustPressedWeaponSKill = weaponSkill;
                    }
                    break;
            }
        }
    }
}
