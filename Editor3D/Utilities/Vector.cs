using System;

namespace Editor3D.Utilities
{
    internal class Vector
    {
        public double x, y, z, w;

        public Vector(double x, double y, double z, double w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        internal Vector CloneMoved(double a, double b, double c)
        {
            return new Vector(x + a, y + b, z + c, w);
        }

        internal double DistanceTo(Vector pos)
        {
            // TODO: Implement
            throw new NotImplementedException();
        }
    }
}