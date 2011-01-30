using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace mcsharpbot.communication.Packets.Types
{
    public struct VehicleAttach : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.VehicleAttach; }
        }

        public int EntityID;
        public int VehicleID;

        public void Read(NetworkStream stream)
        {
            EntityID = StreamHelper.ReadInt(stream);
            VehicleID = StreamHelper.ReadInt(stream);
        }

        public void Write(NetworkStream stream)
        {
            stream.WriteByte((byte)this.Type);

            StreamHelper.WriteInt(stream, this.EntityID);
            StreamHelper.WriteInt(stream, this.VehicleID);

            stream.Flush();
        }
    }
}
