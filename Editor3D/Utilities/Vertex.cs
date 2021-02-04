using System;
using System.Collections.Generic;
using System.Text;

namespace Editor3D.Utilities
{
    class Vertex
    {
        private Vector position;
        private Vector normalVector;

        public Vertex(Vector position, Vector normalVector)
        {
            this.position = position;
            this.normalVector = normalVector;
        }
    }
}
