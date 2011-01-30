using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mcsharpbot.communication.Packets
{
    public class ItemStack
    {
        public int StackSize = 0;
        public int ItemID;
        public int ItemDamage;

        public ItemStack(int id, int stackSize, int damage)
        {
            ItemID = id;
            StackSize = stackSize;
            ItemDamage = damage;
        }
    }
}
