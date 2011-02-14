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
        public sbyte X;
        public sbyte Y;
        public sbyte Z;

        public void Read(NetworkStream stream)
        {
            EntityID = StreamHelper.ReadInt(stream);
            X = (sbyte)stream.ReadByte();
            Y = (sbyte)stream.ReadByte();
            Z = (sbyte)stream.ReadByte();
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
