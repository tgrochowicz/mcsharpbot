using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace mcsharpbot.communication.Packets.Types
{
    public struct Note : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.Note; }
        }

        public int X;
        public short Y;
        public int Z;
        public InstrumentType Instrument;
        public byte Pitch;

        public void Read(NetworkStream stream)
        {
            X = StreamHelper.ReadInt(stream);
            Y = StreamHelper.ReadShort(stream);
            Z = StreamHelper.ReadInt(stream);
            Instrument = (InstrumentType)stream.ReadByte();
            Pitch = (byte)stream.ReadByte();
        }

        public void Write(NetworkStream stream)
        {
            stream.WriteByte((byte)this.Type);

            StreamHelper.WriteInt(stream, this.X);
            StreamHelper.WriteShort(stream, this.Y);
            StreamHelper.WriteInt(stream, this.Z);
            stream.WriteByte((byte)Instrument);
            stream.WriteByte(this.Pitch);

            stream.Flush();
        }
    }
}
