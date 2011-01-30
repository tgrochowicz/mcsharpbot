using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace mcsharpbot.communication.Packets.Types
{
    public struct Place : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.Place; }
        }

        public int ID;
        public byte X;
        public int Y;
        public byte Z;
        public ItemStack Direction;

        public void Read(NetworkStream stream)
        {
            ID = StreamHelper.ReadInt(stream);
            X = (byte)stream.ReadByte();
            Y = StreamHelper.ReadInt(stream);
            Z = (byte)stream.ReadByte();
            short word0 = StreamHelper.ReadShort(stream);
            if (word0 >= 0)
            {
                byte byte0 = (byte)stream.ReadByte();
                short word1 = StreamHelper.ReadShort(stream);
                Direction = new ItemStack((int)word0, (int)byte0, (int)word1);
            }
            else
            {
                Direction = null;
            }
        }

        public void Write(NetworkStream stream)
        {
            stream.WriteByte((byte)this.Type);

            StreamHelper.WriteInt(stream, this.ID);
            stream.WriteByte(this.X);
            StreamHelper.WriteInt(stream, this.Y);
            stream.WriteByte(this.Z);
            if (Direction == null)
            {
                StreamHelper.WriteShort(stream, -1);
            }
            else
            {
                StreamHelper.WriteShort(stream, (short)this.Direction.ItemID);
                stream.WriteByte((byte)this.Direction.StackSize);
                StreamHelper.WriteShort(stream, (short)this.Direction.ItemDamage);
            }

            stream.Flush();
        }
    }
}
