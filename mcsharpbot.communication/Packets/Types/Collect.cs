using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace mcsharpbot.communication.Packets.Types
{
    public struct Collect : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.Collect; }
        }

        public int CollectedEntityID;
        public int CollectorEntityID;

        public void Read(NetworkStream stream)
        {
            CollectedEntityID = StreamHelper.ReadInt(stream);
            CollectorEntityID = StreamHelper.ReadInt(stream);
        }

        public void Write(NetworkStream stream)
        {
            stream.WriteByte((byte)this.Type);

            StreamHelper.WriteInt(stream, this.CollectedEntityID);
            StreamHelper.WriteInt(stream, this.CollectorEntityID);

            stream.Flush();
        }
    }
}
