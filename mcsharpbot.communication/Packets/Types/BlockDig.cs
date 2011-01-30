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

        public byte Status;
        public int X;
        public byte Y;
        public int Z;
        public Face FaceType;

        public void Read(NetworkStream stream)
        {
            Status = (byte)stream.ReadByte();
            X = StreamHelper.ReadInt(stream);
            Y = (byte)stream.ReadByte();
            Z = StreamHelper.ReadInt(stream);
            FaceType = (Face)stream.ReadByte();
        }

        public void Write(NetworkStream stream)
        {
            stream.WriteByte((byte)this.Type);

            stream.WriteByte(this.Status);
            StreamHelper.WriteInt(stream, this.X);
            stream.WriteByte(this.Y);
            StreamHelper.WriteInt(stream, this.Z);
            stream.WriteByte((byte)this.FaceType);

            stream.Flush();
        }
    }
}
