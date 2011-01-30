using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace mcsharpbot.communication.Packets.Types
{
    public struct Quit : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.Quit; }
        }

        public string Reason;

        public void Read(NetworkStream stream)
        {
            Reason = StreamHelper.ReadString(stream);
        }

        public void Write(NetworkStream stream)
        {
            stream.WriteByte((byte)this.Type);

            StreamHelper.WriteString(stream, this.Reason);

            stream.Flush();
        }
    }
}
