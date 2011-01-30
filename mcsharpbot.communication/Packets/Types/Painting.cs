using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace mcsharpbot.communication.Packets.Types
{
    public struct Painting : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.Painting; }
        }

        public int EntityID;
        public string Title;
        public int X;
        public int Y;
        public int Z;
        public int PaintingType;

        public void Read(NetworkStream stream)
        {
            EntityID = StreamHelper.ReadInt(stream);
            Title = StreamHelper.ReadString(stream);
            X = StreamHelper.ReadInt(stream);
            Y = StreamHelper.ReadInt(stream);
            Z = StreamHelper.ReadInt(stream);
            PaintingType = StreamHelper.ReadInt(stream);
        }

        public void Write(NetworkStream stream)
        {
            stream.WriteByte((byte)this.Type);

            StreamHelper.WriteInt(stream, this.EntityID);
            StreamHelper.WriteString(stream, this.Title);
            StreamHelper.WriteInt(stream, this.X);
            StreamHelper.WriteInt(stream, this.Y);
            StreamHelper.WriteInt(stream, this.Z);
            StreamHelper.WriteInt(stream, this.PaintingType);

            stream.Flush();
        }
    }
}
