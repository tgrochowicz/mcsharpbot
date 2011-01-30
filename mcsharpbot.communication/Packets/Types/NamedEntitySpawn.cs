using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace mcsharpbot.communication.Packets.Types
{
    public struct NamedEntitySpawn : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.NamedEntitySpawn; }
        }

        public int EntityID;
        public string Name;
        public int X;
        public int Y;
        public int Z;
        public byte Rotation;
        public byte Pitch;
        public short CurrentItem;

        public void Read(NetworkStream stream)
        {
            EntityID = StreamHelper.ReadInt(stream);
            Name = StreamHelper.ReadString(stream);
            X = StreamHelper.ReadInt(stream);
            Y = StreamHelper.ReadInt(stream);
            Z = StreamHelper.ReadInt(stream);
            Rotation = (byte)stream.ReadByte();
            Pitch = (byte)stream.ReadByte();
            CurrentItem = StreamHelper.ReadShort(stream);
        }

        public void Write(NetworkStream stream)
        {
            stream.WriteByte((byte)this.Type);

            StreamHelper.WriteInt(stream, this.EntityID);
            StreamHelper.WriteString(stream, this.Name);
            StreamHelper.WriteInt(stream, this.X);
            StreamHelper.WriteInt(stream, this.Y);
            StreamHelper.WriteInt(stream, this.Z);
            stream.WriteByte(this.Rotation);
            stream.WriteByte(this.Pitch);
            StreamHelper.WriteShort(stream, this.CurrentItem);

            stream.Flush();
        }
    }
}
