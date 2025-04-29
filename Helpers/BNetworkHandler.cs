using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DBTBalance.Helpers
{
    internal static class BNetworkHandler
    {
        public const byte SYNC_UNLOCK_STATUS = 1;
        public static void SendUnlockStatus(int who, bool value)
        {
            ModPacket packet = DBTBalance.Instance.GetPacket();

            packet.Write(SYNC_UNLOCK_STATUS);
            packet.Write((byte)who);
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
                case SYNC_UNLOCK_STATUS:
                    int who = reader.ReadByte();
                    bool state = reader.ReadBoolean();
                    ReceiveUnlockStatus(who, state);
                break;
            }
        }
    }
}
