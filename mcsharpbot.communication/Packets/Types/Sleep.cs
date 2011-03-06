using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace mcsharpbot.communication.Packets.Types
{
    public struct Sleep : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.Sleep; }
        }

        public int a;
        public int b;
        public int c;
        public int d;
        public int e;

        public void Read(NetworkStream stream)
        {
            a = StreamHelper.ReadInt(stream);
            b = stream.ReadByte();
            c = StreamHelper.ReadInt(stream);
            d = stream.ReadByte();
            e = StreamHelper.ReadInt(stream);
        }

        public void Write(NetworkStream stream)
        {
            stream.WriteByte((byte)this.Type);

            StreamHelper.WriteInt(stream, this.a);
            stream.WriteByte((byte)this.b);
            StreamHelper.WriteInt(stream, this.c);
            stream.WriteByte((byte)this.d);
            StreamHelper.WriteInt(stream, this.e);

            stream.Flush();
        }
    }
}
