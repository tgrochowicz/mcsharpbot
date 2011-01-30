using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace mcsharpbot.communication.Packets.Types
{
    public struct Chat : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.Chat; }
        }

        public string Message;

        public void Read(NetworkStream stream)
        {
            Message = StreamHelper.ReadString(stream);
        }

        public void Write(NetworkStream stream)
        {
            stream.WriteByte((byte)this.Type);

            StreamHelper.WriteString(stream, this.Message);

            stream.Flush();
        }
    }
}
