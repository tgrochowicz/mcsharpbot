using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace mcsharpbot.communication.Packets.Types
{
    public struct PlayerPosition : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.PlayerPosition; }
        }

        public double X;
        public double Y;
        public double Stance;
        public double Z;
        public bool OnGround;

        public void Read(NetworkStream stream)
        {
            X = StreamHelper.ReadDouble(stream);
            Y = StreamHelper.ReadDouble(stream);
            Stance = StreamHelper.ReadDouble(stream);
            Z = StreamHelper.ReadDouble(stream);
            OnGround = stream.ReadByte() != 0;
        }

        public void Write(NetworkStream stream)
        {
            stream.WriteByte((byte)this.Type);

            StreamHelper.WriteDouble(stream, this.X);
            StreamHelper.WriteDouble(stream, this.Y);
            StreamHelper.WriteDouble(stream, this.Stance);
            StreamHelper.WriteDouble(stream, this.Z);
            stream.WriteByte((OnGround ? (byte)1 : (byte)0));

            stream.Flush();
        }
    }
}
