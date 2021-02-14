using System;
using System.Drawing;

namespace Editor3D.Utilities
{
    public class Vector
    {
        public double x, y, z, w;
        private Color colorIntensity;

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

        internal (Vector, Vector) Render(IDisplayer displayer, PipelineInfo info)
        {
            Vector worldVector = info.GetModelMatrix().MultipliedBy(this);
            Vector viewVector = info.GetViewMatrix().MultipliedBy(worldVector);
            Vector ndcVector = info.GetProjectionMatrix().MultipliedBy(viewVector).DivideByW();
            Vector screenVector = ndcVector.InScreenSpace(info.GetScreenWidth(), info.GetScreenHeight());
            if (info.ShouldRenderLines())
            {
                displayer.Display((int)screenVector.x, (int)screenVector.y, screenVector.z, Color.Black);
            }
            return (worldVector, screenVector);
        }

        private Vector InScreenSpace(double width, double height)
        {
            double screenX = Math.Round(((x + 1) * width) / 2);
            double screenY = Math.Round(((y + 1) * height) / 2);
            double screenZ = (z + 1) / 2;
            return new Vector(screenX, screenY, screenZ, 1);
        }

        private Vector DivideByW()
        {
            x /= w;
            y /= w;
            z /= w;
            w = 1;
            return this;
        }

        internal double DotProduct(Vector vector)
        {
            return (x * vector.x) + (y * vector.y) + (z * vector.z);
        }

        internal void SetColor(Color color)
        {
            this.colorIntensity = color;
        }

        internal Color GetColor()
        {
            return colorIntensity;
        }

        internal Vector CrossProduct(Vector vector)
        {
            double resultX = (y * vector.z) - (z * vector.y);
            double resultY = (z * vector.x) - (x * vector.z);
            double resultZ = (x * vector.y) - (y * vector.x);
            return new Vector(resultX, resultY, resultZ, 0);
        }

        internal Vector DirectionTo(Vector vector)
        {
            return new Vector(vector.x - x, vector.y - y, vector.z - z, 0);
        }

        internal Vector Normalize()
        {
            double magnitude = Magnitude();
            if (magnitude == 0)
            {
                return this;
            }
            x /= magnitude;
            y /= magnitude;
            z /= magnitude;
            return this;
        }

        internal Vector NegatedWithoutW()
        {
            return new Vector(-x, -y, -z, w);
        }

        internal double DistanceTo(Vector pos)
        {
            return Math.Sqrt(((pos.x - x) * (pos.x - x)) + ((pos.y - y) * (pos.y - y)) + ((pos.z - z) * (pos.z - z)));
        }

        internal double Magnitude()
        {
            return Math.Sqrt((x * x) + (y * y) + (z * z));
        }

        internal Vector Clone()
        {
            return new Vector(x, y, z, w);
        }

        internal Vector MultipliedBy(double v)
        {
            return new Vector(v * x, v * y, v * z, 0);
        }

        internal Vector SubstractedBy(Vector vector)
        {
            return new Vector(x - vector.x, y - vector.y, z - vector.z, 0);
        }

        public override string ToString()
        {
            return "Vector [" + x + ", " + y + ", " + z + ", " + w + "]";
        }
    }
}
