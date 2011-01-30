using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mcsharpbot.communication.Packets
{
    public enum AnimationType
    {
        Noop = 0,
        Arm = 1,
        Hit = 2,
        Unknown = 102,
        Crouch = 104,
        Uncrouch = 105,
    }
}
