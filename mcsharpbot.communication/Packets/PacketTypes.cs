using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mcsharpbot.communication.Packets
{
    public enum PacketType
    {
        Ping =          0x00,
        Login =         0x01,
        Handshake =     0x02,
        Chat =          0x03,
        Flying =        0x0A,
        PositionLook1 = 0x0B,
        PositionLook2 = 0x0C,
        PositionLook3 = 0x0D,
        Digging =       0x0D,
        Build =         0x0E,
        Equip =         0x0F,
        Animate =       0x12,
        Pickup =        0x15,
        WClose =        0x65,
        WAction =       0x66,
        Inventory =     0x68,
        Sign =          0x82,
        Quit =          0xFF
    }
}
