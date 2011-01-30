using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace mcsharpbot.communication.Packets.Types
{
    public struct EntityPosition : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.EntityPosition; }
        }

        public int EntityID;
        public byte X;
        public byte Y;
        public byte Z;

        public void Read(NetworkStream stream)
        {
            EntityID = StreamHelper.ReadInt(stream);
            X = (byte)stream.ReadByte();
            Y = (byte)stream.ReadByte();
            Z = (byte)stream.ReadByte();
        }

        public void Write(NetworkStream stream)
        {
            stream.WriteByte((byte)this.Type);

            StreamHelper.WriteInt(stream, this.EntityID);
            stream.WriteByte(this.X);
            stream.WriteByte(this.Y);
            stream.WriteByte(this.Z);

            stream.Flush();
        }
    }
}
