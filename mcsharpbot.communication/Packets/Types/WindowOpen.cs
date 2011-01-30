using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace mcsharpbot.communication.Packets.Types
{
    public struct WindowOpen : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.WindowOpen; }
        }

        public byte WindowId;
        public WindowTypes WindowType;
        public string Title;
        public byte Slots;

        public void Read(NetworkStream stream)
        {
            WindowId = (byte)stream.ReadByte();
            WindowType = (WindowTypes)stream.ReadByte();
            Title = StreamHelper.ReadString(stream);
            Slots = (byte)stream.ReadByte();
        }

        public void Write(NetworkStream stream)
        {
            stream.WriteByte((byte)this.Type);

            stream.WriteByte(this.WindowId);
            stream.WriteByte((byte)this.WindowType);
            StreamHelper.WriteString(stream, this.Title);
            stream.WriteByte(this.Slots);

            stream.Flush();
        }
    }
}
