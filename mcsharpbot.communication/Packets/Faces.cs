using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mcsharpbot.communication.Packets
{
    public enum Face
    {
        Noop =      -1,
        NegativeY = 0,
        PositiveY = 1,
        NegativeZ = 2,
        PositiveZ = 3,
        NegativeX = 4,
        PositiveX = 5
    }
}
