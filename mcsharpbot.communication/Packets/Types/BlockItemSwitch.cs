using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace mcsharpbot.communication.Packets.Types
{
    public struct BlockItemSwitch : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.BlockItemSwitch; }
        }

        public short ID;

        public void Read(NetworkStream stream)
        {
            ID = StreamHelper.ReadShort(stream);
        }

        public void Write(NetworkStream stream)
        {
            stream.WriteByte((byte)this.Type);

            StreamHelper.WriteShort(stream, this.ID);

            stream.Flush();
        }
    }
}
