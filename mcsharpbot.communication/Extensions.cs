using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace mcsharpbot.communication
{
    public static class Extensions
    {
        public static void WriteByte(this NetworkStream ns, sbyte b) {
            ns.WriteByte((byte)b);
        }
    }
}
