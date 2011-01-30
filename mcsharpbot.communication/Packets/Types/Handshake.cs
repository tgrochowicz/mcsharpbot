using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace mcsharpbot.communication.Packets.Types
{
    public struct Handshake : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.Handshake; }
        }
        public string Username;

        public void Read(NetworkStream stream)
        {
            this.Username = StreamHelper.ReadString(stream);
        }

        public void Write(NetworkStream stream)
        {
            stream.WriteByte((byte)this.Type);
            StreamHelper.WriteString(stream, this.Username);

            stream.Flush();
        }
    }
}
