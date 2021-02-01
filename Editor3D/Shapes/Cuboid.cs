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

        public Cuboid(double a, double b, double c, Vector position)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            InitializeMesh(position);
        }

        private void InitializeMesh(Vector position)
        {
            InitializeWall(position, position.CloneMoved(a, 0, 0),
                position.CloneMoved(a, 0, c),
                position.CloneMoved(0, 0, c));
        }

        private void InitializeWall(Vector pos1, Vector pos2, Vector pos3, Vector pos4)
        {
            TriangleMesh mesh = new TriangleMesh(pos1, pos2, pos3, pos4);
            meshes.Add(mesh);
        }

        internal void Render()
        {
            
        }
    }
}
