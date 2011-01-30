using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.IO.Compression;

namespace mcsharpbot.communication.Packets.Types
{
    public struct MapChunk : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.MapChunk; }
        }

        public int X;
        public short Y;
        public int Z;
        public int XSize;
        public int YSize;
        public int ZSize;
        public byte[] Chunk;
        private int ChunkSize;

        public void Read(NetworkStream stream)
        {
            X = StreamHelper.ReadInt(stream);
            Y = StreamHelper.ReadShort(stream);
            Z = StreamHelper.ReadInt(stream);
            XSize = stream.ReadByte() + 1;
            YSize = stream.ReadByte() + 1;
            ZSize = stream.ReadByte() + 1;
            int i = StreamHelper.ReadInt(stream);
            byte[] buffer = new byte[i];
            buffer = StreamHelper.ReadBytes(stream, i);
            Chunk = new byte[(XSize * YSize * ZSize * 5) / 2];

            MemoryStream ms = new MemoryStream();
            using(GZipStream zip = new GZipStream(ms, CompressionMode.Decompress, true))
            {
                zip.Write(buffer, 0, buffer.Length);
                zip.Flush();
            }

            ms.Position = 0;
            Chunk = ms.ToArray();

        }

        public void Write(NetworkStream stream)
        {
            stream.WriteByte((byte)this.Type);

            StreamHelper.WriteInt(stream, this.X);
            StreamHelper.WriteShort(stream, this.Y);
            StreamHelper.WriteInt(stream, this.Z);
            stream.WriteByte((byte)(XSize - 1));
            stream.WriteByte((byte)(YSize - 1));
            stream.WriteByte((byte)(ZSize - 1));
            StreamHelper.WriteInt(stream, Chunk.Length);
            stream.Write(Chunk, 0, Chunk.Length);

            stream.Flush();
        }
    }
}
