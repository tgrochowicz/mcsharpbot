using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mcsharpbot.communication
{
    public class ChunkCoordinates
    {
        public int X;
        public int Y;

        public ChunkCoordinates(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }
    }
    public struct BlockCoordinates
    {
        public int X;
        public int Y;
        public int Z;

        public BlockCoordinates(int X, int Y, int Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }
    }
}
