using Editor3D.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Editor3D.Shapes
{
    class Cuboid
    {
        private double a, b, c;
        private List<TriangleMesh> meshes;

        public Cuboid(double a, double b, double c)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            InitializeMesh();
        }

        private void InitializeMesh()
        {
            
        }

        internal void Render()
        {
            
        }
    }
}
