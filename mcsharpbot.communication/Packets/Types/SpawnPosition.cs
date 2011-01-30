﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace mcsharpbot.communication.Packets.Types
{
    public struct SpawnPosition : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.SpawnPosition; }
        }

        public int X;
        public int Y;
        public int Z;

        public void Read(NetworkStream stream)
        {
            X = StreamHelper.ReadInt(stream);
            Y = StreamHelper.ReadInt(stream);
            Z = StreamHelper.ReadInt(stream);
        }

        public void Write(NetworkStream stream)
        {
            stream.WriteByte((byte)this.Type);

            StreamHelper.WriteInt(stream, this.X);
            StreamHelper.WriteInt(stream, this.Y);
            StreamHelper.WriteInt(stream, this.Z);

            stream.Flush();
        }
    }
}
