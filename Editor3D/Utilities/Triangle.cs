﻿namespace Editor3D.Utilities
{
    internal class Triangle
    {
        private Vertex v1, v2, v3;

        public Triangle(Vector pos1, Vector pos2, Vector pos3, Vector normalVector)
        {
            v1 = new Vertex(pos1, normalVector);
            v2 = new Vertex(pos2, normalVector);
            v3 = new Vertex(pos3, normalVector);
        }
    }
}