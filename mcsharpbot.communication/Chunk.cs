using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mcsharpbot.communication
{
    public class Chunk
    {
        public static int ChunkSize = 10;
        public static int Height = 128;

        public byte[] Blocks;
        public NibbleArray Data;
        public NibbleArray SkylightMap;
        public NibbleArray BlocklightMap;
        public byte[] HeightMap;

        public int X;
        public int Y;

        public Chunk(byte[] buffer, int X, int Y)
        {
            HeightMap = new byte[256];
            Blocks = buffer;
            Data = new NibbleArray(buffer.Length);
            SkylightMap = new NibbleArray(buffer.Length);
            BlocklightMap = new NibbleArray(buffer.Length);
            this.X = X;
            this.Y = Y;
        }

        public int LoadFromChunk(byte[] Chunk, int row, int yCopy, int zRow, int nextrow, int yEnd, int zNextrow, int Previous)
        {
            for (int i = row; i < nextrow; i++)
            {
                for (int k = zRow; k < zNextrow; k++)
                {
                    int value = ((i << 11) | k << 7) | yCopy;
                    int end = yEnd - yCopy;
                    Array.Copy(Chunk, Previous, Blocks, value, end);
                    Previous += end;
                }
            }

            for (int i = row; i < nextrow; i++)
            {
                for (int k = zRow; k < zNextrow; k++)
                {
                    int value = (((i << 11) | k << 7) | yCopy) >> 1;
                    int middle = (yEnd - yCopy) / 2;
                    Array.Copy(Chunk, Previous, Data.data, value, middle);
                    Previous += middle;
                }
            }

            for (int i = row; i < nextrow; i++)
            {
                for (int k = zRow; k < zNextrow; k++)
                {
                    int value = (((i << 11) | k << 7) | yCopy) >> 1;
                    int middle = (yEnd - yCopy) / 2;
                    Array.Copy(Chunk, Previous, BlocklightMap.data, value, middle);
                    Previous += middle;
                }
            }

            for (int i = row; i < nextrow; i++)
            {
                for (int k = zRow; k < zNextrow; k++)
                {
                    int value = (((i << 11) | k << 7) | yCopy) >> 1;
                    int middle = (yEnd - yCopy) / 2;
                    Array.Copy(Chunk, Previous, SkylightMap.data, value, middle);
                    Previous += middle;
                }
            }

            return Previous;
        }

        public Blocks GetBlockID(int X, int Y, int Z)
        {
            return (Blocks)this.Blocks[X << 11 | Y << 7 | Z];
        }
    }
}
