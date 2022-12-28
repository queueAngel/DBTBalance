using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DBTBalance.Helpers
{
    internal static class BNetworkHandler
    {
        public static byte SYNC_UNLOCK_STATUS = 1;
        public static void SendUnlockStatus(int who, bool value)
        {
            ModPacket packet = DBTBalance.Instance.GetPacket();

            packet.Write(SYNC_UNLOCK_STATUS);
            packet.Write(who);
            packet.Write(value);

            packet.Send(-1);
        }
        public static void ReceiveUnlockStatus(int who, bool value)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
                return;
            
            Player player = Main.player[who];
            BPlayer modPlayer = player.GetModPlayer<BPlayer>();

            modPlayer.MP_Unlock = value;
        }
        public static void HandlePacket(BinaryReader reader, int fromWho)
        {
            byte command = reader.ReadByte();

            switch (command)
            {
                case 1:
                    int who = reader.ReadInt32();
                    bool state = reader.ReadBoolean();
                    ReceiveUnlockStatus(who, state);
                break;
            }
        }
    }
}
