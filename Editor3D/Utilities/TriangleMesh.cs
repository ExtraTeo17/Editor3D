using System;
using System.Collections.Generic;
using System.Text;

namespace Editor3D.Utilities
{
    class TriangleMesh
    {
        private int horizontalDivisionCount = 1, verticalDivisionCount = 1; // TODO: Make customizable
        private TrianglePair[,] triangles;

        public TriangleMesh(Vector pos1, Vector pos2, Vector pos3, Vector pos4)
        {
            triangles = new TrianglePair[horizontalDivisionCount, verticalDivisionCount];
            FillTriangles(pos1, pos2, pos3, pos4);
        }

        private void FillTriangles(Vector pos1, Vector pos2, Vector pos3, Vector pos4)
        {
            Vector[,] positionsInBetween = new Vector[horizontalDivisionCount + 1, verticalDivisionCount + 1];
            for (int i = 0; i < verticalDivisionCount; ++i)
            {
                
            }
        }
    }
}
