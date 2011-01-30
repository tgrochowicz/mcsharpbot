using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace mcsharpbot.communication.Packets.Types
{
    public struct Action : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.Action; }
        }

        public int EntityID;
        public ActionType EntityAction;

        public void Read(NetworkStream stream)
        {
            EntityID = StreamHelper.ReadInt(stream);
            EntityAction = (ActionType)stream.ReadByte();
        }

        public void Write(NetworkStream stream)
        {
            stream.WriteByte((byte)this.Type);

            StreamHelper.WriteInt(stream, this.EntityID);
            stream.WriteByte((byte)this.EntityAction);

            stream.Flush();
        }
    }
}
