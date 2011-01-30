using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace mcsharpbot.communication.Packets.Types
{
    public struct PlayerLookMove : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.PlayerLookMove; }
        }

        public double X;
        public double Y;
        public double Stance;
        public double Z;
        public float Yaw;
        public float Pitch;
        public bool OnGround;

        public void Read(NetworkStream stream)
        {
            X = StreamHelper.ReadDouble(stream);
            Y = StreamHelper.ReadDouble(stream);
            Stance = StreamHelper.ReadDouble(stream);
            Z = StreamHelper.ReadDouble(stream);
            Yaw = StreamHelper.ReadFloat(stream);
            Pitch = StreamHelper.ReadFloat(stream);
            OnGround = stream.ReadByte() != 0;
        }

        public void Write(NetworkStream stream)
        {
            stream.WriteByte((byte)this.Type);

            StreamHelper.WriteDouble(stream, this.X);
            StreamHelper.WriteDouble(stream, this.Y);
            StreamHelper.WriteDouble(stream, this.Stance);
            StreamHelper.WriteDouble(stream, this.Z);
            StreamHelper.WriteFloat(stream, this.Yaw);
            StreamHelper.WriteFloat(stream, this.Pitch);
            stream.WriteByte((OnGround ? (byte)1 : (byte)0));

            stream.Flush();
        }
    }
}
