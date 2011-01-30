using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace mcsharpbot.communication.Packets.Types
{
    public struct Status : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.Status; }
        }

        public int EntityID;
        public byte Unknown;

        public void Read(NetworkStream stream)
        {
            EntityID = StreamHelper.ReadInt(stream);
            Unknown = (byte)stream.ReadByte();
        }

        public void Write(NetworkStream stream)
        {
            stream.WriteByte((byte)this.Type);

            StreamHelper.WriteInt(stream, this.EntityID);
            stream.WriteByte(this.Unknown);

            stream.Flush();
        }
    }
}
