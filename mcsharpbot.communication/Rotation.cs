using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace mcsharpbot.communication
{
    public class Rotation : ICloneable
    {
        public float Pitch;
        public float Yaw;

        public Rotation()
        {

        }

        public Rotation(float Pitch, float Yaw)
        {
            this.Pitch = Pitch;
            this.Yaw = Yaw;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != typeof(Rotation)) 
            {
                return base.Equals(obj);
            }
            Rotation r = (Rotation)obj;
            return (r.Pitch == this.Pitch) && (r.Yaw == this.Yaw);
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

    }

 

}
