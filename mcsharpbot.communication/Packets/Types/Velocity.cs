using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace mcsharpbot.communication.Packets.Types
{
    public struct Velocity : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.Velocity; }
        }

        public int EntityID;
        public short dx;
        public short dy;
        public short dz;

        public void Read(NetworkStream stream)
        {
            EntityID = StreamHelper.ReadInt(stream);
            dx = StreamHelper.ReadShort(stream);
            dy = StreamHelper.ReadShort(stream);
            dz = StreamHelper.ReadShort(stream);
        }

        public void Write(NetworkStream stream)
        {
            stream.WriteByte((byte)this.Type);

            StreamHelper.WriteInt(stream, this.EntityID);
            StreamHelper.WriteShort(stream, this.dx);
            StreamHelper.WriteShort(stream, this.dy);
            StreamHelper.WriteShort(stream, this.dz);

            stream.Flush();
        }
    }
}
