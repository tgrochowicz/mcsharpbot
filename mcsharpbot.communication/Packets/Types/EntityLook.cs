﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace mcsharpbot.communication.Packets.Types
{
    public struct EntityLook : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.EntityLook; }
        }

        public int EntityID;
        public byte Yaw;
        public byte Pitch;

        public void Read(NetworkStream stream)
        {
            EntityID = StreamHelper.ReadInt(stream);
            Yaw = (byte)stream.ReadByte();
            Pitch = (byte)stream.ReadByte();
        }

        public void Write(NetworkStream stream)
        {
            stream.WriteByte((byte)this.Type);

            StreamHelper.WriteInt(stream, this.EntityID);
            stream.WriteByte(this.Yaw);
            stream.WriteByte(this.Pitch);

            stream.Flush();
        }
    }
}