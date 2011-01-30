using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace mcsharpbot.communication.Packets.Types
{
    public struct Sign : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.Sign; }
        }

        public int X;
        public short Y;
        public int Z;
        public string[] Lines;

        public void Read(NetworkStream stream)
        {
            X = StreamHelper.ReadInt(stream);
            Y = StreamHelper.ReadShort(stream);
            Z = StreamHelper.ReadInt(stream);

            Lines = new string[4];
            for (int i = 0; i < 4; i++)
            {
                Lines[i] = StreamHelper.ReadString(stream);
            }
        }

        public void Write(NetworkStream stream)
        {
            stream.WriteByte((byte)this.Type);

            StreamHelper.WriteInt(stream, this.X);
            StreamHelper.WriteShort(stream, this.Y);
            StreamHelper.WriteInt(stream, this.Z);

            for (int i = 0; i < 4; i++)
            {
                StreamHelper.WriteString(stream, this.Lines[i]);
            }

            stream.Flush();
        }
    }
}
