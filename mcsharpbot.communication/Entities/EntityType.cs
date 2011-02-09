using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mcsharpbot.communication.Entities
{
    public class EntityType
    {
        public int EntityID;
        public double X;
        public double Y;
        public double Z;
        public int ServerX;
        public int ServerY;
        public int ServerZ;
        public float Yaw;
        public float Pitch;

        public void SetPosition(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public void SetRotation(float yaw, float pitch)
        {
            Yaw = yaw;
            Pitch = pitch;
        }
    }
}
