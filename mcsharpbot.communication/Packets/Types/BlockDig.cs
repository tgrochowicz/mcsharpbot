using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace mcsharpbot.communication.Packets.Types
{
    public struct BlockDig : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.BlockDig; }
        }

        public int Status;
        public int X;
        public int Y;
        public int Z;
        public Face FaceType;

        public void Read(NetworkStream stream)
        {
            Status = stream.ReadByte();
            X = StreamHelper.ReadInt(stream);
            Y = stream.ReadByte();
            Z = StreamHelper.ReadInt(stream);
            FaceType = (Face)stream.ReadByte();
        }

        public void Write(NetworkStream stream)
        {
            stream.WriteByte((byte)this.Type);

            stream.WriteByte((byte)this.Status);
            StreamHelper.WriteInt(stream, this.X);
            stream.WriteByte((byte)this.Y);
            StreamHelper.WriteInt(stream, this.Z);
            stream.WriteByte((byte)this.FaceType);

            stream.Flush();
        }
    }
}
