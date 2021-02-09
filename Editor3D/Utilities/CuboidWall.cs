using Editor3D.Utilities;
using System;

namespace Editor3D.Utilities
{
    internal class CuboidWall
    {
        private readonly Triangle upperTriangle, lowerTriangle;

        public CuboidWall(Vector A, Vector B, Vector C, Vector D, Vector normalVector)
        {
            upperTriangle = InitializeTriangle(A, B, D, normalVector);
            lowerTriangle = InitializeTriangle(B, C, D, normalVector);
        }

        private Triangle InitializeTriangle(Vector pos1, Vector pos2, Vector pos3, Vector normalVector)
        {
            return new Triangle(pos1, pos2, pos3, normalVector);
        }

        internal void Render(IDisplayer displayer, PipelineInfo info)
        {
            upperTriangle.Render(displayer, info);
            lowerTriangle.Render(displayer, info);
        }
    }
}