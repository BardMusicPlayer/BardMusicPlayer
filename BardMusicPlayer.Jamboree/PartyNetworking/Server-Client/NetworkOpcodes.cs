using System;

namespace BardMusicPlayer.Jamboree.PartyNetworking.Server_Client
{
    public static class NetworkOpcodes
    {
        public enum OpcodeEnum : byte
        {
            NULL_OPCODE             = 0x00,
            PING                    = 0x01,
            PONG                    = 0x02,
            MSG_JOIN_PARTY          = 0x03,
            MSG_LEAVE_PARTY         = 0x04,
            MSG_PLAY                = 0x05,
            MSG_STOP                = 0x06,
            MSG_SONG_DATA           = 0x07
        }
    }

    public static class ZeroTierPacketBuilder
    {
        /// <summary>
        /// Send we joined the party
        /// | type 0 = bard
        /// | type 1 = dancer
        /// </summary>
        /// <param name="type"></param>
        /// <param name="performer_name"></param>
        /// <returns>data as byte[]</returns>
        public static byte[] MSG_JOIN_PARTY(byte type, string performer_name)
        {
            NetworkPacket buffer = new NetworkPacket(NetworkOpcodes.OpcodeEnum.MSG_JOIN_PARTY);
            buffer.WriteUInt8(type);
            buffer.WriteCString(performer_name);
            return buffer.GetData();
        }

        /// <summary>
        /// Send the performance start
        /// </summary>
        /// <returns>data as byte[]</returns>
        public static byte[] PerformanceStart()
        {
            NetworkPacket buffer = new NetworkPacket(NetworkOpcodes.OpcodeEnum.MSG_PLAY);
            buffer.WriteInt64(DateTimeOffset.Now.ToUnixTimeMilliseconds());
            return buffer.GetData();
        }
    }
}
