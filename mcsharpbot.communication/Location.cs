using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace mcsharpbot.communication
{
    public class Location : ICloneable
    {
        public double Stance;
        public double X;
        public double Y;
        public double Z;

        public Location()
        {

        }

        public Location(double X, double Y, double Z, double Stance)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
            this.Stance = Stance;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != typeof(Location))
            {
                return base.Equals(obj);
            }
            Location l = (Location)obj;
            return (l.X == this.X) && (l.Y == this.Y) && (l.Z == this.Z) && (l.Stance == this.Stance);
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

 

}
