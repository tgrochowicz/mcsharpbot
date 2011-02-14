using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace mcsharpbot.communication.Packets.Types
{
    public struct Explosion : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.Explosion; }
        }

        public double Unknown1;
        public double Unknown2;
        public double Unknown3;
        public float Unknown4;
        public int Count;
        public HashSet<ChunkPosition> Unknown5;

        public void Read(NetworkStream stream)
        {
            Unknown1 = StreamHelper.ReadDouble(stream);
            Unknown2 = StreamHelper.ReadDouble(stream);
            Unknown3 = StreamHelper.ReadDouble(stream);
            Unknown4 = StreamHelper.ReadFloat(stream);
            Count = StreamHelper.ReadInt(stream);
            Unknown5 = new HashSet<ChunkPosition>();

            for (int i = 0; i < Count; i++)
            {
                Unknown5.Add(new ChunkPosition(
                    stream.ReadByte() + (int)Unknown1,
                    stream.ReadByte() + (int)Unknown2,
                    stream.ReadByte() + (int)Unknown3));
            }
        }

        public void Write(NetworkStream stream)
        {
            stream.WriteByte((byte)this.Type);

            StreamHelper.WriteDouble(stream, this.Unknown1);
            StreamHelper.WriteDouble(stream, this.Unknown2);
            StreamHelper.WriteDouble(stream, this.Unknown3);
            StreamHelper.WriteFloat(stream, this.Unknown4);
            StreamHelper.WriteInt(stream, Unknown5.Count);

            int i = (int)this.Unknown1;
            int j = (int)this.Unknown2;
            int k = (int)this.Unknown3;
            sbyte j1;
            for (IEnumerator<ChunkPosition> iter = this.Unknown5.GetEnumerator(); iter.MoveNext(); stream.WriteByte(j1))
            {
                ChunkPosition chunkpos = iter.Current;
                int l = chunkpos.X - i;
                int i1 = chunkpos.Y - j;
                j1 = (sbyte)(chunkpos.Z - k);
                stream.WriteByte((byte)l);
                stream.WriteByte((byte)i1);
            }

            stream.Flush();
        }
    }
}
