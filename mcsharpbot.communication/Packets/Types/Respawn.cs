using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace mcsharpbot.communication.Packets.Types
{
    public struct Respawn : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.Respawn; }
        }

        public void Read(NetworkStream stream)
        {
        }

        public void Write(NetworkStream stream)
        {
            stream.WriteByte((byte)this.Type);


            stream.Flush();
        }
    }
}
