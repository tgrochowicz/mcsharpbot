using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace mcsharpbot.communication.Packets.Types
{
    public struct WindowToken : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.WindowToken; }
        }

        public byte WindowID;
        public short Token;
        public bool Acknowledged;

        public void Read(NetworkStream stream)
        {
            WindowID = (byte)stream.ReadByte();
            Token = StreamHelper.ReadShort(stream);
            Acknowledged = stream.ReadByte() != 0;
        }

        public void Write(NetworkStream stream)
        {
            stream.WriteByte((byte)this.Type);

            stream.WriteByte(this.WindowID);
            StreamHelper.WriteShort(stream, this.Token);
            stream.WriteByte((byte)(this.Acknowledged ? 1 : 0));

            stream.Flush();
        }
    }
}
