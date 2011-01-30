using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace mcsharpbot.communication.Packets.Types
{
    public struct Flying : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.Flying; }
        }

        public bool OnGround;

        public void Read(NetworkStream stream)
        {
            OnGround = stream.ReadByte() != 0;
        }

        public void Write(NetworkStream stream)
        {
            stream.WriteByte((byte)this.Type);

            stream.WriteByte((OnGround ? (byte)1 : (byte)0));

            stream.Flush();
        }
    }
}
