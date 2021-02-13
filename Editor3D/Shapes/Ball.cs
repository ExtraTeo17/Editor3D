using Editor3D.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Editor3D.Shapes
{
    class Ball
    {
        private double radius;
        private List<CuboidWall> walls = new List<CuboidWall>();
        private List<Triangle> tops = new List<Triangle>(); // FILL CZUBKI TODO
        private Matrix modelMatrix;
        private readonly Color color;

        public Ball(double radius, int latitudeDivisions, int longitudeDivisions,
            Vector position, Color color)
        {
            this.radius = radius;
            this.color = color;
            modelMatrix = Matrix.Unitary();
            InitializeMesh(latitudeDivisions, longitudeDivisions, position);
        }

        private void InitializeMesh(double latitudes, double longitudes, Vector position)
        {
            double equator = Math.PI / 2.0;
            double northPole = 0;
            double southPole = Math.PI;
            double latDown = equator;
            for (double i = 1; i < latitudes; ++i) // NORTH
            {
                double latDelta = (Math.PI / 2.0) * (i / latitudes);
                double latUp = equator - latDelta;
                double lonLeft = 0;
                for (double j = 1; j <= longitudes; ++j)
                {
                    double lonDelta = (2.0 * Math.PI) * (j / longitudes);
                    double lonRight = lonDelta;

                    /* DODAC WALL */ /* pewnie policzyc normal vector z prostokata */

                    lonLeft = lonRight;
                }
                latDown = latUp;
            }
            for (double i = 0; i < latitudes; ++i) // SOUTH
            {

            }
            // TWO FORS FOR CZUBKI
        }

        private void InitializeWall(Vector pos1, Vector pos2, Vector pos3, Vector pos4, Vector normalVector)
        {
            CuboidWall wall = new CuboidWall(pos1, pos2, pos3, pos4, normalVector);
            walls.Add(wall);
        }

        internal void Render(IDisplayer displayer, PipelineInfo info)
        {
            
        }
    }
}
