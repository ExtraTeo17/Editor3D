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
            double equatorLat = Math.PI / 2.0;
            double latUp, lonLeft;

            double latDown = equatorLat;
            for (double i = 1; i < latitudes; ++i) // NORTH
            {
                double latDelta = (Math.PI / 2.0) * (i / latitudes);
                latUp = equatorLat - latDelta;
                lonLeft = 0;
                for (double j = 1; j <= longitudes; ++j)
                {
                    double lonDelta = (2.0 * Math.PI) * (j / longitudes);
                    double lonRight = lonDelta;
                    InitializeWall(latUp, latDown, lonLeft, lonRight);
                    lonLeft = lonRight;
                }
                latDown = latUp;
            }

            lonLeft = 0;
            for (double j = 1; j <= longitudes; ++j) // TOP CAP
            {
                double lonDelta = (2.0 * Math.PI) * (j / longitudes);
                double lonRight = lonDelta;
                InitializeNorthTriangle(latDown, lonLeft, lonRight);
                lonLeft = lonRight;
            }

            latUp = equatorLat;
            for (double i = 1; i < latitudes; ++i) // SOUTH
            {
                double latDelta = (Math.PI / 2.0) * (i / latitudes);
                latDown = equatorLat + latDelta;
                lonLeft = 0;
                for (double j = 1; j <= longitudes; ++j)
                {
                    double lonDelta = (2.0 * Math.PI) * (j / longitudes);
                    double lonRight = lonDelta;
                    InitializeWall(latUp, latDown, lonLeft, lonRight);
                    lonLeft = lonRight;
                }
                latUp = latDown;
            }

            lonLeft = 0;
            for (double j = 1; j <= longitudes; ++j) // BOTTOM CAP
            {
                double lonDelta = (2.0 * Math.PI) * (j / longitudes);
                double lonRight = lonDelta;
                InitializeSouthTriangle(latUp, lonLeft, lonRight);
                lonLeft = lonRight;
            }
        }

        private void InitializeNorthTriangle(double latDown, double lonLeft, double lonRight)
        {
            Vector pos1 = new Vector(0, radius, 0, 1);
            Vector pos2 = GeographicToScene(latDown, lonRight);
            Vector pos3 = GeographicToScene(latDown, lonLeft);
            Vector normalVector = pos2.SubstractedBy(pos1).CrossProduct(pos3.SubstractedBy(pos1));
            InitializeTriangle(pos1, pos2, pos3, normalVector);
        }

        private void InitializeTriangle(Vector pos1, Vector pos2, Vector pos3, Vector normalVector)
        {
            Triangle triangle = new Triangle(pos1, pos2, pos3, normalVector);
            tops.Add(triangle);
        }

        private void InitializeSouthTriangle(double latUp, double lonLeft, double lonRight)
        {
            Vector pos1 = new Vector(0, -radius, 0, 1);
            Vector pos3 = GeographicToScene(latUp, lonLeft);
            Vector pos2 = GeographicToScene(latUp, lonRight);
            Vector normalVector = pos2.SubstractedBy(pos1).CrossProduct(pos3.SubstractedBy(pos1));
            InitializeTriangle(pos1, pos2, pos3, normalVector);
        }

        private void InitializeWall(double latUp, double latDown, double lonLeft, double lonRight)
        {
            Vector pos1 = GeographicToScene(latUp, lonLeft);
            Vector pos2 = GeographicToScene(latUp, lonRight);
            Vector pos3 = GeographicToScene(latDown, lonRight);
            Vector pos4 = GeographicToScene(latDown, lonLeft);
            Vector normalVector = pos2.SubstractedBy(pos1).CrossProduct(pos3.SubstractedBy(pos1));
            InitializeWall(pos1, pos2, pos3, pos4, normalVector);
        }

        private Vector GeographicToScene(double lat, double lon)
        {
            double x = radius * Math.Sin(lat) * Math.Cos(lon);
            double y = radius * Math.Cos(lat);
            double z = radius * Math.Sin(lat) * Math.Sin(lon);
            return new Vector(x, y, z, 1);
        }

        private void InitializeWall(Vector pos1, Vector pos2, Vector pos3, Vector pos4, Vector normalVector)
        {
            CuboidWall wall = new CuboidWall(pos1, pos2, pos3, pos4, normalVector);
            walls.Add(wall);
        }

        internal void Render(IDisplayer displayer, PipelineInfo info)
        {
            info.SetModelMatrix(modelMatrix);
            foreach (CuboidWall wall in walls)
            {
                wall.RenderFilling(displayer, info, color);
            }
            foreach (Triangle top in tops)
            {
                top.RenderFilling(displayer, info, color);
            }
            if (info.ShouldRenderLines())
            {
                foreach (CuboidWall wall in walls)
                {
                    wall.RenderLines(displayer, info);
                }
                foreach (Triangle top in tops)
                {
                    top.RenderLines(displayer, info);
                }
            }
        }
    }
}
