using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace mcsharpbot.communication.Packets.Recieve
{
    public struct Login : IPacketReceive
    {
        public PacketType Type
        {
            get { return PacketType.Login; }
        }

        public string Hash;

        public Login(NetworkStream stream)
        {
            this.Hash = StreamHelper.ReadString(stream);
        }
    }
}
