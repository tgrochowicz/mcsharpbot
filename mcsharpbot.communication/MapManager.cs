using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mcsharpbot.communication
{
    public class MapManager
    {
        private ChunkProvider Chunks;
        public MapManager(ChunkProvider chunks)
        {
            this.Chunks = chunks;
        }

        /// <summary>
        /// Returns a Dictionary of coordinate/blocktype 
        /// </summary>
        /// <param name="coordinate"></param>
        /// <param name="Distance"></param>
        /// <returns></returns>
        public Dictionary<BlockCoordinates, Blocks> GetNeighboringBlocks(BlockCoordinates coordinate, int Distance)
        {
            Dictionary<BlockCoordinates, Blocks> returnable = new Dictionary<BlockCoordinates, Blocks>();
            if (Distance <= 0)
            {
                Blocks current = (Blocks)Enum.Parse(typeof(Blocks), Chunks.GetFromCoordinates(coordinate.X, coordinate.Y).Blocks[coordinate.Z].ToString());
            }
            else
            {
                for (int x = -1; x < 2; x++)
                {
                    for (int y = -1; y < 2; y++)
                    {
                        for (int z = -1; z < 2; z++)
                        {
                            if (x != 0 && y != 0 && z != 0)
                            {
                                var neighbor = GetNeighboringBlocks(new BlockCoordinates(x, y, z), Distance - 1);
                                foreach (var key in neighbor.Keys)
                                {
                                    if(!returnable.ContainsKey(key))
                                        returnable.Add(key, neighbor[key]);
                                }
                            }
                        }
                    }
                }
            }
            return returnable;
        }
    }
}
