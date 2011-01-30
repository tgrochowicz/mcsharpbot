using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace mcsharpbot.communication.Packets.Types
{
    public struct PlayerInventory : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.PlayerInventory; }
        }

        public int EID;
        public short Slot;
        public short Primary;
        public short Secondary;

        public void Read(NetworkStream stream)
        {
            EID = StreamHelper.ReadInt(stream);
            Slot = StreamHelper.ReadShort(stream);
            Primary = StreamHelper.ReadShort(stream);
            Secondary = StreamHelper.ReadShort(stream);
        }

        public void Write(NetworkStream stream)
        {
            stream.WriteByte((byte)this.Type);

            StreamHelper.WriteInt(stream, this.EID);
            StreamHelper.WriteShort(stream, this.Slot);
            StreamHelper.WriteShort(stream, this.Primary);
            StreamHelper.WriteShort(stream, this.Secondary);

            stream.Flush();
        }
    }
}
