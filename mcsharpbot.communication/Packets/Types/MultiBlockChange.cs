using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace mcsharpbot.communication.Packets.Types
{
    public struct MultiBlockChange : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.MultiBlockChange; }
        }

        public int X;
        public int Z;
        public short[] CoordinateArray;
        public byte[] TypeArray;
        public byte[] MetadataArray;
        public short Size;

        public void Read(NetworkStream stream)
        {
            X = StreamHelper.ReadInt(stream);
            Z = StreamHelper.ReadInt(stream);
            Size = (short)(StreamHelper.ReadShort(stream) & 0xffff);
            CoordinateArray = new short[Size];
            TypeArray = new byte[Size];
            MetadataArray = new byte[Size];

            for (int i = 0; i < Size; i++)
            {
                CoordinateArray[i] = StreamHelper.ReadShort(stream);
            }

            stream.Read(TypeArray, 0, Size);
            stream.Read(MetadataArray, 0, Size);
        }

        public void Write(NetworkStream stream)
        {
            stream.WriteByte((byte)this.Type);

            StreamHelper.WriteInt(stream, this.X);
            StreamHelper.WriteInt(stream, this.Z);
            StreamHelper.WriteShort(stream, Size);
            for (int i = 0; i < this.Size; i++)
            {
                StreamHelper.WriteShort(stream, CoordinateArray[i]);
            }

            stream.Write(this.TypeArray, 0, this.TypeArray.Length);
            stream.Write(this.MetadataArray, 0, this.MetadataArray.Length);

            stream.Flush();
        }
    }
}
