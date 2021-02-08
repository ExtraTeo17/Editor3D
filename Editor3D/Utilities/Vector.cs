using System;

namespace Editor3D.Utilities
{
    internal class Vector
    {
        public readonly double x, y, z, w;

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
            return Math.Sqrt(((pos.x - x) * (pos.x - x)) + ((pos.y - y) * (pos.y - y)) + ((pos.z - z) * (pos.z - z)));
        }

        internal Vector Clone()
        {
            return new Vector(x, y, z, w);
        }
    }
}