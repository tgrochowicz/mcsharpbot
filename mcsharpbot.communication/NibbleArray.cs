using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mcsharpbot.communication
{
    public class NibbleArray
    {

        public byte[] data;

        public NibbleArray(int i)
        {
            data = new byte[i >> 1];
        }

        public NibbleArray(byte[] bytes)
        {
            data = bytes;
        }

        public int GetNibble(int X, int Y, int Z)
        {
            int value = X << 11 | Z << 7 | Y;
            int filled = value >> 1;
            int added = value & 1;

            if (added == 0)
            {
                return data[filled] & 0xf;
            }
            else
            {
                return data[filled] >> 4 & 0xf;
            }
        }

        public void SetNibble(int X, int Y, int Z, int Value)
        {
            int value = X << 11 | Z << 7 | Y;
            int filled = value >> 1;
            int added = value & 1;
            if (added == 0)
            {
                data[filled] = (byte)(data[filled] & 0xf0 | Value & 0xf);
            }
            else
            {
                data[filled] = (byte)(data[filled] & 0xf | (Value & 0xf) << 4);
            }
        }

    }
}
