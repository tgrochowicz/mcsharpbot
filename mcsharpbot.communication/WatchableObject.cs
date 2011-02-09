using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mcsharpbot.communication
{
    public class WatchableObject
    {

        public int Type;
        public int Lower;
        public object Value;
        public bool Init;

        public WatchableObject(int i, int j, object value)
        {
            Type = i;
            Lower = j;
            Value = value;
            Init = true;
        }

    }
}
