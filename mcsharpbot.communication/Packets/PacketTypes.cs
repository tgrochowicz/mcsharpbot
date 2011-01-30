using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mcsharpbot.communication.Packets
{
    public enum PacketType
    {
        Ping =              0x00,
        Login =             0x01,
        Handshake =         0x02,
        Chat =              0x03,
        UpdateTime =        0x04,
        PlayerInventory =   0x05,
        SpawnPosition =     0x06,
        Use =               0x07,
        Health =            0x08,
        Respawn =           0x09,
        Flying =            0x0A,
        PlayerPosition =    0x0B,
        PlayerLook =        0x0C,
        PlayerLookMove =    0x0D,
        BlockDig =          0x0E,
        Place =             0x0F,
        BlockItemSwitch =   0x10,
        ArmAnimation =      0x12,
        Action =            0x13,
        NamedEntitySpawn =  0x14,
        PickupSpawn =       0x15,
        Collect =           0x16,
        VehicleSpawn =      0x17,
        MobSpawn =          0x18,
        Painting =          0x19,
        Velocity =          0x1C,
        DestroyEntity =     0x1D,
        Entity =            0x1E,
        EntityPosition =    0x1F,
        EntityLook =        0x20,
        EntityLookMove =    0x21,
        EntityTeleport =    0x22,
        Status =            0x26,
        VehicleAttach =     0x27,
        PreChunk =          0x32,
        MapChunk =          0x33,
        MultiBlockChange =  0x34,
        BlockChange =       0x35,
        Note =              0x36,
        Explosion =         0x3C,
        WindowOpen =        0x64,
        WindowClose =       0x65,
        WindowAction =      0x66,
        WindowSlot =        0x67,
        Inventory =         0x68,
        WindowProgress =    0x69,
        WindowToken =       0x6A,
        Sign =              0x82,
        Quit =              0xFF
    }
}
