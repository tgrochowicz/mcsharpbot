using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace mcsharpbot.communication.Packets.Send
{
    public struct Handshake : IPacketSend
    {
        public PacketType Type
        {
            get { return PacketType.Handshake; }
        }
        public string Username;

        public void Write(NetworkStream stream)
        {
            stream.WriteByte((byte)this.Type);
            StreamHelper.WriteString(stream, this.Username);

            stream.Flush();
        }
    }
}
