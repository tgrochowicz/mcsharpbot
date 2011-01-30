using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace mcsharpbot.communication.Packets.Types
{
    public struct PreChunk : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.PreChunk; }
        }

        public int X;
        public int Y;
        public bool Mode;

        public void Read(NetworkStream stream)
        {
            X = StreamHelper.ReadInt(stream);
            Y = StreamHelper.ReadInt(stream);
            Mode = stream.ReadByte() != 0;
        }

        public void Write(NetworkStream stream)
        {
            stream.WriteByte((byte)this.Type);

            StreamHelper.WriteInt(stream, this.X);
            StreamHelper.WriteInt(stream, this.Y);
            stream.WriteByte((Mode ? (byte)1 : (byte)0));

            stream.Flush();
        }
    }
}
