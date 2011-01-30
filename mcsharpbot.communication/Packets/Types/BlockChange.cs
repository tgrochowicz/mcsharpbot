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
        public byte Y;
        public int Z;
        public byte BlockType;
        public byte Metadata;

        public void Read(NetworkStream stream)
        {
            X = StreamHelper.ReadInt(stream);
            Y = (byte)stream.ReadByte();
            Z = StreamHelper.ReadInt(stream);
            BlockType = (byte)stream.ReadByte();
            Metadata = (byte)stream.ReadByte();
        }

        public void Write(NetworkStream stream)
        {
            stream.WriteByte((byte)this.Type);

            StreamHelper.WriteInt(stream, this.X);
            stream.WriteByte(this.Y);
            StreamHelper.WriteInt(stream, this.Z);
            stream.WriteByte(this.BlockType);
            stream.WriteByte(this.Metadata);

            stream.Flush();
        }
    }
}
