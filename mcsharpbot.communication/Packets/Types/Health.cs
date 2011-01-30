using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace mcsharpbot.communication.Packets.Types
{
    public struct Health : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.Health; }
        }

        public short PlayerHealth;

        public void Read(NetworkStream stream)
        {
            PlayerHealth = StreamHelper.ReadShort(stream);
        }

        public void Write(NetworkStream stream)
        {
            stream.WriteByte((byte)this.Type);

            StreamHelper.WriteShort(stream, this.PlayerHealth);

            stream.Flush();
        }
    }
}
