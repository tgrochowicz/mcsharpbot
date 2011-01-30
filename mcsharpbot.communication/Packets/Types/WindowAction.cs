using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace mcsharpbot.communication.Packets.Types
{
    public struct WindowAction : IPacket
    {
        public PacketType Type
        {
            get { return PacketType.WindowAction; }
        }

        public byte WindowID;
        public short Slot;
        public byte Button;
        public short Token;
        public ItemStack Items;

        public void Read(NetworkStream stream)
        {
            WindowID = (byte)stream.ReadByte();
            Slot = StreamHelper.ReadShort(stream);
            Button = (byte)stream.ReadByte();
            Token = StreamHelper.ReadShort(stream);
            short word0 = StreamHelper.ReadShort(stream);
            if (word0 >= 0)
            {
                byte byte0 = (byte)stream.ReadByte();
                short word1 = StreamHelper.ReadShort(stream);
                Items = new ItemStack(word0, byte0, word1);
            }
            else
            {
                Items = null;
            }
        }

        public void Write(NetworkStream stream)
        {
            stream.WriteByte((byte)this.Type);

            stream.WriteByte(this.WindowID);
            StreamHelper.WriteShort(stream, this.Slot);
            stream.WriteByte(this.Button);
            StreamHelper.WriteShort(stream, this.Token);
            if (Items == null)
            {
                StreamHelper.WriteShort(stream, -1);
            }
            else
            {
                StreamHelper.WriteShort(stream, (short)this.Items.ItemID);
                stream.WriteByte((byte)this.Items.StackSize);
                StreamHelper.WriteShort(stream, (short)this.Items.ItemDamage);

            }

            stream.Flush();
        }
    }
}
