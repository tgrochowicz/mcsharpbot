using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace mcsharpbot.communication.Packets.Types
{
    public struct WindowProgress : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.WindowProgress; }
        }

        public byte WindowID;
        public short Bar;
        public short Progress;

        public void Read(NetworkStream stream)
        {
            WindowID = (byte)stream.ReadByte();
            Bar = StreamHelper.ReadShort(stream);
            Progress = StreamHelper.ReadShort(stream);
        }

        public void Write(NetworkStream stream)
        {
            stream.WriteByte((byte)this.Type);

            stream.WriteByte(this.WindowID);
            StreamHelper.WriteShort(stream, this.Bar);
            StreamHelper.WriteShort(stream, this.Progress);

            stream.Flush();
        }
    }
}
