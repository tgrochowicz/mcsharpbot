using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace mcsharpbot.communication.Packets.Types
{
    public struct UpdateTime : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.UpdateTime; }
        }

        public long Time;

        public void Read(NetworkStream stream)
        {
            Time = StreamHelper.ReadLong(stream);
        }

        public void Write(NetworkStream stream)
        {
            stream.WriteByte((byte)this.Type);

            StreamHelper.WriteLong(stream, this.Time);

            stream.Flush();
        }
    }
}
