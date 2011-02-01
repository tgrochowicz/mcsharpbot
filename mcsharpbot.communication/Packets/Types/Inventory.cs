using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace mcsharpbot.communication.Packets.Types
{
    public struct Inventory : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.Inventory; }
        }


        public byte Name;
        public short Length;
        public ItemStack[] Items;

        public void Read(NetworkStream stream)
        {
            Name = (byte)stream.ReadByte();
            Length = StreamHelper.ReadShort(stream);
            Items = new ItemStack[Length];
            for (int i = 0; i < Length; i++)
            {
                short primary = StreamHelper.ReadShort(stream);
                if (primary >= 0)
                {
                    byte count = (byte)stream.ReadByte();
                    short secondary = StreamHelper.ReadShort(stream);
                    Items[i] = new ItemStack(primary, count, secondary);
                }
            }
        }

        public void Write(NetworkStream stream)
        {
            stream.WriteByte((byte)this.Type);

            stream.WriteByte(this.Name);
            StreamHelper.WriteShort(stream, (short)this.Items.Length);
            for (int i = 0; i < this.Items.Length; i++)
            {
                if (this.Items[i] == null)
                {
                    StreamHelper.WriteShort(stream, -1);
                }
                else
                {
                    StreamHelper.WriteShort(stream, (short)this.Items[i].ItemID);
                    stream.WriteByte((byte)this.Items[i].StackSize);
                    StreamHelper.WriteShort(stream, (short)this.Items[i].ItemDamage);
                }
            }

            stream.Flush();
        }
    }
}
