using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace mcsharpbot.communication.Packets.Types
{
    public struct ArmAnimation : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.ArmAnimation; }
        }

        public int EntityID;
        public AnimationType Animation;

        public void Read(NetworkStream stream)
        {
            EntityID = StreamHelper.ReadInt(stream);
            Animation = (AnimationType)stream.ReadByte();
        }

        public void Write(NetworkStream stream)
        {
            stream.WriteByte((byte)this.Type);

            StreamHelper.WriteInt(stream, this.EntityID);
            stream.WriteByte((byte)this.Animation);

            stream.Flush();
        }
    }
}
