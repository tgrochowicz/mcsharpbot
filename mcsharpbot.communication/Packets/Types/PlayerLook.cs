using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace mcsharpbot.communication.Packets.Types
{
    public struct PlayerLook : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.PlayerLook; }
        }

        public float Yaw;
        public float Pitch;
        public bool OnGround;

        public void Read(NetworkStream stream)
        {
            Yaw = StreamHelper.ReadFloat(stream);
            Pitch = StreamHelper.ReadFloat(stream);
            OnGround = stream.ReadByte() != 0;
        }

        public void Write(NetworkStream stream)
        {
            stream.WriteByte((byte)this.Type);

            StreamHelper.WriteFloat(stream, this.Yaw);
            StreamHelper.WriteFloat(stream, this.Pitch);
            stream.WriteByte((OnGround ? (byte)1 : (byte)0));

            stream.Flush();
        }
    }
}
