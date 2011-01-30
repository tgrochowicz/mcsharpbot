using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace mcsharpbot.communication.Packets.Types
{
    public struct PickupSpawn : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.PickupSpawn; }
        }

        public int EntityID;
        public short ItemID;
        public byte Count;
        public short Secondary;
        public int X;
        public int Y;
        public int Z;
        public byte Rotation;
        public byte Pitch;
        public byte Roll;

        public void Read(NetworkStream stream)
        {
            EntityID = StreamHelper.ReadInt(stream);
            ItemID = StreamHelper.ReadShort(stream);
            Count = (byte)stream.ReadByte();
            Secondary = StreamHelper.ReadShort(stream);
            X = StreamHelper.ReadInt(stream);
            Y = StreamHelper.ReadInt(stream);
            Z = StreamHelper.ReadInt(stream);
            Rotation = (byte)stream.ReadByte();
            Pitch = (byte)stream.ReadByte();
            Roll = (byte)stream.ReadByte();
        }

        public void Write(NetworkStream stream)
        {
            stream.WriteByte((byte)this.Type);

            StreamHelper.WriteInt(stream, this.EntityID);
            StreamHelper.WriteShort(stream, this.ItemID);
            stream.WriteByte(this.Count);
            StreamHelper.WriteShort(stream, this.Secondary);
            StreamHelper.WriteInt(stream, this.X);
            StreamHelper.WriteInt(stream, this.Y);
            StreamHelper.WriteInt(stream, this.Z);
            stream.WriteByte(this.Rotation);
            stream.WriteByte(this.Pitch);
            stream.WriteByte(this.Roll);

            stream.Flush();
        }
    }
}
