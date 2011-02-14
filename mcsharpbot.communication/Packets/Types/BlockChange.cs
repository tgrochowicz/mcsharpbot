using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace mcsharpbot.communication.Packets.Types
{
    public struct BlockChange : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.BlockChange; }
        }

        public int X;
        public int Y;
        public int Z;
        public int BlockType;
        public int Metadata;

        public void Read(NetworkStream stream)
        {
            X = StreamHelper.ReadInt(stream);
            Y = stream.ReadByte();
            Z = StreamHelper.ReadInt(stream);
            BlockType = stream.ReadByte();
            Metadata = stream.ReadByte();
        }

        public void Write(NetworkStream stream)
        {
            stream.WriteByte((byte)this.Type);

            StreamHelper.WriteInt(stream, this.X);
            stream.WriteByte((byte)this.Y);
            StreamHelper.WriteInt(stream, this.Z);
            stream.WriteByte((byte)this.BlockType);
            stream.WriteByte((byte)this.Metadata);

            stream.Flush();
        }
    }
}
