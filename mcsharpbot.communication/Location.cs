using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mcsharpbot.communication
{
    public class Location
    {
        public double Stance;
        public double X;
        public double Y;
        public double Z;

        public Location(double X, double Y, double Z, double Stance)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
            this.Stance = Stance;
        }
    }

 

}
