using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace mcsharpbot.communication.Packets
{
    public interface IPacketReceive
    {
        PacketType Type { get; }
    }
}
