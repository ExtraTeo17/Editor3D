using Editor3D.Shapes;
using Editor3D.Utilities;
using System;
using System.Drawing;

namespace Editor3D
{
    internal class Light
    {
        internal readonly Color Id;
        internal readonly Color Is;
        private Vector lightPosition;
        private readonly bool isSpotlight;
        private Vector spotlightDirection;
        private readonly double spotlightRange;
        private readonly double spotlightExponent;
        internal bool on = true;
        internal Ball lightBall;

        public Light(Color Id, Color Is, Vector position, bool isSpotlight, Ball lightBall)
        {
            this.Id = Id;
            this.Is = Is;
            this.lightPosition = position;
            this.isSpotlight = isSpotlight;
            this.lightBall = lightBall;
        }

        public Light(Color Id, Color Is, Vector position, bool isSpotlight, Ball lightBall,
            Vector spotlightDirection, double spotlightRange, double spotlightExponent)
        {
            this.Id = Id;
            this.Is = Is;
            this.lightPosition = position;
            this.isSpotlight = isSpotlight;
            this.spotlightDirection = spotlightDirection;
            this.spotlightRange = spotlightRange;
            this.spotlightExponent = spotlightExponent;
            this.lightBall = lightBall;
        }

        internal void RotateSpotlight(double degrees, Axis axis)
        {
            spotlightDirection = spotlightDirection.Rotate(Matrix.Rotation(Math.PI * degrees / 180, axis));
        }

        internal Vector GetPosition()
        {
            return lightPosition;
        }

        internal bool IsSpotlight()
        {
            return isSpotlight;
        }

        internal Vector GetSpotlightDirection()
        {
            return spotlightDirection;
        }

        internal double GetSpotlightRange()
        {
            return spotlightRange;
        }

        internal double GetSpotlightExponent()
        {
            return spotlightExponent;
        }

        internal void Translate(double x, double y, double z)
        {
            Vector translation = new Vector(x, y, z, 1);
            lightPosition = lightPosition.Translate(translation);
            lightBall.Translate(x, y, z);
        }
    }
}
