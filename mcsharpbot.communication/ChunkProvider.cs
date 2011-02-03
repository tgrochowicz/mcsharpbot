using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace mcsharpbot.communication
{
    public class ChunkProvider
    {
        public Hashtable Chunks;

        public ChunkProvider()
        {
            Chunks = new Hashtable();
        }

        public Chunk AllocateChunk(int X, int Y)
        {
            ChunkCoordinates chunkCoords = new ChunkCoordinates(X, Y);
            byte[] blocks = new byte[32768];
            Chunk chunk = new Chunk(blocks, X, Y);
            for (int i = 0; i < chunk.SkylightMap.data.Length; i++)
            {
                chunk.SkylightMap.data[i] = 255;
            }
            Chunks.Add(chunkCoords, chunk);
            return chunk;
        }

        public Chunk GetFromCoordinates(int X, int Y)
        {
            Chunk c = (Chunk)Chunks[new ChunkCoordinates(X, Y)];
            if(c == null) 
            {
                return new Chunk(new byte[32768], 0, 0);
            } 
            else 
            {
                return c;
            }
        }

        public Blocks GetBlock(int X, int Y, int Z)
        {
            if (Y < 0)
            {
                return Blocks.Bedrock;
            }
            if (Y >= Chunk.Height)
            {
                return Blocks.Air;
            }
            int ChunkX;
            int ChunkY;

            ChunkFromCoordinate(X, Y, out ChunkX, out ChunkY);
            Chunk chunk = GetFromCoordinates(ChunkX, ChunkY);

            int CX = chunk.X;
            int CY = chunk.Y;

            if (X < 0)
            {
                X = X - (CX * Chunk.ChunkSize);
            }
            if (Z < 0)
            {
                Z = Z - (CY * Chunk.ChunkSize);
            }

            return (Blocks)chunk.Blocks[X + (Y * Chunk.Height + Z) * Chunk.ChunkSize];

        }

        public void ChunkFromCoordinate(int X, int Y, out int ChunkX, out int ChunkY)
        {
            ChunkX = X / Chunk.ChunkSize;
            ChunkY = Y / Chunk.ChunkSize;
            if (X < 0 && X % Chunk.ChunkSize != 0)
            {
                ChunkX--;
            }
            if (Y < 0 && Y % Chunk.ChunkSize != 0)
            {
                ChunkY--;
            }
        }

    }
}
