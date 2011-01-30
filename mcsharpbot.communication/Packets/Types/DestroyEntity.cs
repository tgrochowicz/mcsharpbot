using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace mcsharpbot.communication.Packets.Types
{
    public struct DestroyEntity : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.DestroyEntity; }
        }

        public int EntityID;

        public void Read(NetworkStream stream)
        {
            EntityID = StreamHelper.ReadInt(stream);
        }

        public void Write(NetworkStream stream)
        {
            stream.WriteByte((byte)this.Type);

            StreamHelper.WriteInt(stream, this.EntityID);

            stream.Flush();
        }
    }
}
