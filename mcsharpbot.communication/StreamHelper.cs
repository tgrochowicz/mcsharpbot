using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace mcsharpbot.communication
{
    class StreamHelper
    {
        public static int ReadInt(Stream s)
        {
            return IPAddress.HostToNetworkOrder((int)Read(s, 4));
        }

        public static short ReadShort(Stream s)
        {
            return IPAddress.HostToNetworkOrder((short)Read(s, 2));
        }

        public static long ReadLong(Stream s)
        {
            return IPAddress.HostToNetworkOrder((long)Read(s, 8));
        }

        public static double ReadDouble(Stream s)
        {
            return new BinaryReader(s).ReadDouble();
        }

        public static float ReadFloat(Stream s)
        {
            return new BinaryReader(s).ReadSingle();
        }

        public static Boolean ReadBoolean(Stream s)
        {
            return new BinaryReader(s).ReadBoolean();
        }

        public static byte[] ReadBytes(Stream s, int count)
        {
            return new BinaryReader(s).ReadBytes(count);
        }

        public static String ReadString(Stream s)
        {
            short len;
            byte[] a = new byte[2];
            a[0] = (byte)s.ReadByte();
            a[1] = (byte)s.ReadByte();
            len = IPAddress.HostToNetworkOrder(BitConverter.ToInt16(a, 0));
            if (len > 100) len = 100; //Shouldn't even be this high in the first place.
            if (len < 0) len = 0; //What the hell even happened?

            byte[] b = new byte[len];
            for (int i = 0; i < len; i++)
            {
                b[i] = (byte)s.ReadByte();
            }
            return Encoding.ASCII.GetString(b);
        }

        public static void WriteString(Stream s, String msg)
        {

            short len = IPAddress.HostToNetworkOrder((short)msg.Length);
            byte[] a = BitConverter.GetBytes(len);
            byte[] b = Encoding.Default.GetBytes(msg);
            byte[] c = a.Concat(b).ToArray();
            s.Write(c, 0, c.Length);
        }

        public static void WriteInt(Stream s, int i)
        {
            byte[] a = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(i));
            s.Write(a, 0, a.Length);
        }

        public static void WriteLong(Stream s, long i)
        {
            byte[] a = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(i));
            s.Write(a, 0, a.Length);
        }

        public static void WriteShort(Stream s, short i)
        {
            byte[] a = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(i));
            s.Write(a, 0, a.Length);
        }

        public static void WriteDouble(Stream s, double d)
        {
            new BinaryWriter(s).Write(d);
        }

        public static void WriteFloat(Stream s, float f)
        {
            new BinaryWriter(s).Write(f);
        }

        public static void ReadBoolean(Stream s, Boolean b)
        {
            new BinaryWriter(s).Write(b);
        }

        public static void WriteBytes(Stream s, byte[] b)
        {
            new BinaryWriter(s).Write(b);
        }

        public static Object Read(Stream s, int num)
        {
            byte[] b = new byte[num];
            for (int i = 0; i < b.Length; i++)
            {
                b[i] = (byte)s.ReadByte();
            }
            switch (num)
            {
                case 4:
                    return BitConverter.ToInt32(b, 0);
                case 8:
                    return BitConverter.ToInt64(b, 0);
                case 2:
                    return BitConverter.ToInt16(b, 0);
                default:
                    return 0;
            }
        }
    }
}