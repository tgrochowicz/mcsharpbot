using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace mcsharpbot.communication.Packets.Types
{
    public struct Use : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.Use; }
        }

        public int EID;
        public int Target;
        public byte Button;

        public void Read(NetworkStream stream)
        {
            EID = StreamHelper.ReadInt(stream);
            Target = StreamHelper.ReadInt(stream);
            Button = (byte)stream.ReadByte();
        }

        public void Write(NetworkStream stream)
        {
            stream.WriteByte((byte)this.Type);

            StreamHelper.WriteInt(stream, this.EID);
            StreamHelper.WriteInt(stream, this.Target);
            stream.WriteByte(this.Button);

            stream.Flush();
        }
    }
}
