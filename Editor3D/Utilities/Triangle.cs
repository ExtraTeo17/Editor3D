using System;

namespace Editor3D.Utilities
{
    internal class Triangle
    {
        private readonly Vertex v1, v2, v3;

        public Triangle(Vector pos1, Vector pos2, Vector pos3, Vector normalVector)
        {
            v1 = new Vertex(pos1, normalVector);
            v2 = new Vertex(pos2, normalVector);
            v3 = new Vertex(pos3, normalVector);
        }

        internal void Render()
        {
            v1.Render();
            v2.Render();
            v3.Render();
        }
    }
}