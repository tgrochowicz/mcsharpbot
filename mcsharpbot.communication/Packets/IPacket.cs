using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace mcsharpbot.communication.Packets
{
    public interface IPacket
    {
        PacketType Type { get; }
        void Read(NetworkStream s);
        void Write(NetworkStream s);
    }
}
