using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace mcsharpbot.communication.Packets.Types
{
    public struct WindowClose : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.WindowClose; }
        }

        public byte WindowId;

        public void Read(NetworkStream stream)
        {
            WindowId = (byte)stream.ReadByte();
        }

        public void Write(NetworkStream stream)
        {
            stream.WriteByte((byte)this.Type);

            stream.WriteByte(this.WindowId);

            stream.Flush();
        }
    }
}
