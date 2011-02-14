using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace mcsharpbot.communication.Packets.Types
{
    public struct EntityTeleport : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.EntityTeleport; }
        }

        public int EntityID;
        public int X;
        public int Y;
        public int Z;
        public sbyte Yaw;
        public sbyte Pitch;

        public void Read(NetworkStream stream)
        {
            EntityID = StreamHelper.ReadInt(stream);
            X = StreamHelper.ReadInt(stream);
            Y = StreamHelper.ReadInt(stream);
            Z = StreamHelper.ReadInt(stream);
            Yaw = (sbyte)stream.ReadByte();
            Pitch = (sbyte)stream.ReadByte();
        }

        public void Write(NetworkStream stream)
        {
            stream.WriteByte((byte)this.Type);

            StreamHelper.WriteInt(stream, this.EntityID);
            StreamHelper.WriteInt(stream, this.X);
            StreamHelper.WriteInt(stream, this.Y);
            StreamHelper.WriteInt(stream, this.Z);
            stream.WriteByte(this.Yaw);
            stream.WriteByte(this.Pitch);

            stream.Flush();
        }
    }
}
