using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace mcsharpbot.communication.Packets.Recieve
{
    public struct Handshake : IPacketReceive
    {
        public PacketType Type
        {
            get { return PacketType.Handshake; }
        }

        public string Hash;

        public Handshake(NetworkStream stream)
        {
            this.Hash = StreamHelper.ReadString(stream);
        }
    }
}
