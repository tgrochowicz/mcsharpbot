using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mcsharpbot.communication.Entities
{
    public class PickupEntityType : EntityType
    {
        public short ItemID;
        public byte Count;
        public short Secondary;
        public byte Rotation;
        public byte Roll;
    }
}
