using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace mcsharpbot.communication.Packets.Types
{
    public struct MobSpawn : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.MobSpawn; }
        }

        public int EntityID;
        public byte MobType;
        public int X;
        public int Y;
        public int Z;
        public byte Yaw;
        public byte Pitch;

        public void Read(NetworkStream stream)
        {
            EntityID = StreamHelper.ReadInt(stream);
            MobType = (byte)stream.ReadByte();
            X = StreamHelper.ReadInt(stream);
            Y = StreamHelper.ReadInt(stream);
            Z = StreamHelper.ReadInt(stream);
            Yaw = (byte)stream.ReadByte();
            Pitch = (byte)stream.ReadByte();
        }

        public void Write(NetworkStream stream)
        {
            stream.WriteByte((byte)this.Type);

            StreamHelper.WriteInt(stream, this.EntityID);
            stream.WriteByte(this.MobType);
            StreamHelper.WriteInt(stream, this.X);
            StreamHelper.WriteInt(stream, this.Y);
            StreamHelper.WriteInt(stream, this.Z);
            stream.WriteByte(this.Yaw);
            stream.WriteByte(this.Pitch);

            stream.Flush();
        }
    }
}
