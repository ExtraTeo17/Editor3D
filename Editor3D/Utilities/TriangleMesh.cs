using System;
using System.Collections.Generic;
using System.Text;

namespace Editor3D.Utilities
{
    class TriangleMesh
    {
        private int horizontalCount = 3, verticalCount = 2; // TODO: Make customizable
        private TrianglePair[,] triangles;

        public TriangleMesh(Vector pos1, Vector pos2, Vector pos3, Vector pos4)
        {
            triangles = new TrianglePair[horizontalCount, verticalCount];
            FillTriangles(pos1, pos2, pos3, pos4);
        }

        private void FillTriangles(Vector pos1, Vector pos2, Vector pos3, Vector pos4)
        {
            for (int i = 0; i < horizontalCount; ++i)
            {
                for (int j = 0; j < verticalCount; ++j)
                {
                    triangles[i, j] = CreateTrianglePair(pos1, pos2, pos3, pos4, (double)(i + 1) / (double)horizontalCount, (double)(j + 1) / (double)verticalCount);
                }
            }
        }

        private TrianglePair CreateTrianglePair(Vector pos1, Vector pos2, Vector pos3, Vector pos4, double pairHpos, double pairVpos)
        {
            TrianglePair pair = new TrianglePair();

            // TODO: Implement

            return pair;
        }
    }
}
