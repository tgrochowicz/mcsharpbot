using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Collections;

namespace mcsharpbot.communication.Packets.Types
{
    public struct EntityMetadata : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.EntityMetadata; }
        }

        public int EntityID;
        public ArrayList Metadata;

        public void Read(NetworkStream stream)
        {
            EntityID = StreamHelper.ReadInt(stream);
            Metadata = DataWatcher.Read(stream);
        }

        public void Write(NetworkStream stream)
        {
            stream.WriteByte((byte)this.Type);

            StreamHelper.WriteInt(stream, this.EntityID);
            DataWatcher.Write(this.Metadata, stream);

            stream.Flush();
        }
    }
}
