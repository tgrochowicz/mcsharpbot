using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Net.Sockets;
using mcsharpbot.communication.Packets;

namespace mcsharpbot.communication
{
    public class DataWatcher
    {

        public DataWatcher()
        {

        }

        public static ArrayList Read(NetworkStream stream)
        {
            ArrayList list = new ArrayList();
            for (byte b = (byte)stream.ReadByte(); b != 127; b = (byte)stream.ReadByte())
            {
                int i = (b & 0xe0) >> 5;
                int j = b & 0x1f;
                WatchableObject obj = null;
                switch (i)
                {
                    case 0: //Byte
                        obj = new WatchableObject(i, j, (byte)stream.ReadByte());
                        break;
                    case 1: //Short
                        obj = new WatchableObject(i, j, StreamHelper.ReadShort(stream));
                        break;
                    case 2: //Int
                        obj = new WatchableObject(i, j, StreamHelper.ReadInt(stream));
                        break;
                    case 3: //Float
                        obj = new WatchableObject(i, j, StreamHelper.ReadFloat(stream));
                        break;
                    case 4: //String
                        obj = new WatchableObject(i, j, StreamHelper.ReadString(stream));
                        break;
                    case 5: //Item stack
                        obj = new WatchableObject(i, j, new ItemStack(StreamHelper.ReadShort(stream), stream.ReadByte(), StreamHelper.ReadShort(stream)));
                        break;
                }
                list.Add(obj);
            }
            return list;
        }

        public static void Write(ArrayList list, NetworkStream stream)
        {
            list.ToArray().ToList().ForEach(x =>
            {
                WatchableObject obj = (WatchableObject)x;
                int joined = ((obj.Type << 5) | (obj.Lower & 0x1f)) & 0xff;
                stream.WriteByte((byte)joined);
                switch (obj.Type)
                {
                    case 0:
                        stream.WriteByte((byte)obj.Value);
                        break;
                    case 1:
                        StreamHelper.WriteShort(stream, (short)obj.Value);
                        break;
                    case 2:
                        StreamHelper.WriteInt(stream, (int)obj.Value);
                        break;
                    case 3:
                        StreamHelper.WriteFloat(stream, (float)obj.Value);
                        break;
                    case 4:
                        StreamHelper.WriteString(stream, (string)obj.Value);
                        break;
                    case 5:
                        ItemStack stack = (ItemStack)obj.Value;
                        StreamHelper.WriteShort(stream, (short)stack.ItemID);
                        stream.WriteByte((byte)stack.StackSize);
                        StreamHelper.WriteShort(stream, (short)stack.ItemDamage);
                        break;
                }
            });
            stream.WriteByte(127);
        }

    }
}
