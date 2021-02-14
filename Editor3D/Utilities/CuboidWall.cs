using Editor3D.Utilities;
using System;
using System.Drawing;

namespace Editor3D.Utilities
{
    internal class CuboidWall
    {
        private readonly Triangle upperTriangle, lowerTriangle;

        public CuboidWall(Vector A, Vector B, Vector C, Vector D, Vector normalVector)
        {
            upperTriangle = new Triangle(A, B, D, normalVector);
            lowerTriangle = new Triangle(B, C, D, normalVector);
        }

        public CuboidWall(Vector A, Vector B, Vector C, Vector D)
        {
            upperTriangle = new Triangle(A, B, D);
            lowerTriangle = new Triangle(B, C, D);
        }

        internal void RenderFilling(IDisplayer displayer, PipelineInfo info, Color color)
        {
            upperTriangle.RenderFilling(displayer, info, color);
            lowerTriangle.RenderFilling(displayer, info, color);
        }

        internal void RenderLines(IDisplayer displayer, PipelineInfo info)
        {
            upperTriangle.RenderLines(displayer, info);
            lowerTriangle.RenderLines(displayer, info);
        }
    }
}
