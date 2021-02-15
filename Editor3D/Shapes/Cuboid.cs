using Editor3D.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Editor3D.Shapes
{
    class Cuboid
    {
        private double a, b, c;
        private List<CuboidWall> walls = new List<CuboidWall>();
        private Matrix rotationMatrix;
        private Matrix translationMatrix;
        private readonly Color color;

        public Cuboid(double a, double b, double c, Color color)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.color = color;
            translationMatrix = Matrix.Unitary();
            rotationMatrix = Matrix.Unitary();
            InitializeMesh();
        }

        private void InitializeMesh()
        {
            Vector position = new Vector(0, 0, 0, 1);
            InitializeWall(position.CloneMoved(0 - a / 2, 0 - b / 2, 0 - c / 2),
                position.CloneMoved(a - a / 2, 0 - b / 2, 0 - c / 2),
                position.CloneMoved(a - a / 2, 0 - b / 2, c - c / 2),
                position.CloneMoved(0 - a / 2, 0 - b / 2, c - c / 2),
                new Vector(0, -1, 0, 0));
            InitializeWall(position.CloneMoved(0 - a / 2, b - b / 2, 0 - c / 2),
                position.CloneMoved(a - a / 2, b - b / 2, 0 - c / 2),
                position.CloneMoved(a - a / 2, 0 - b / 2, 0 - c / 2),
                position.CloneMoved(0 - a / 2, 0 - b / 2, 0 - c / 2),
                new Vector(0, 0, -1, 0));
            InitializeWall(position.CloneMoved(a - a / 2, b - b / 2, 0 - c / 2),
                position.CloneMoved(a - a / 2, b - b / 2, c - c / 2),
                position.CloneMoved(a - a / 2, 0 - b / 2, c - c / 2),
                position.CloneMoved(a - a / 2, 0 - b / 2, 0 - c / 2),
                new Vector(1, 0, 0, 0));
            InitializeWall(position.CloneMoved(a - a / 2, b - b / 2, c - c / 2),
                position.CloneMoved(0 - a / 2, b - b / 2, c - c / 2),
                position.CloneMoved(0 - a / 2, 0 - b / 2, c - c / 2),
                position.CloneMoved(a - a / 2, 0 - b / 2, c - c / 2),
                new Vector(0, 0, 1, 0));
            InitializeWall(position.CloneMoved(0 - a / 2, b - b / 2, c - c / 2),
                position.CloneMoved(0 - a / 2, b - b / 2, 0 - c / 2),
                position.CloneMoved(0 - a / 2, 0 - b / 2, 0 - c / 2),
                position.CloneMoved(0 - a / 2, 0 - b / 2, c - c / 2),
                new Vector(-1, 0, 0, 0));
            InitializeWall(position.CloneMoved(0 - a / 2, b - b / 2, c - c / 2),
                position.CloneMoved(a - a / 2, b - b / 2, c - c / 2),
                position.CloneMoved(a - a / 2, b - b / 2, 0 - c / 2),
                position.CloneMoved(0 - a / 2, b - b / 2, 0 - c / 2),
                new Vector(0, 1, 0, 0));
        }

        private void InitializeWall(Vector pos1, Vector pos2, Vector pos3, Vector pos4, Vector normalVector)
        {
            CuboidWall wall = new CuboidWall(pos1, pos2, pos3, pos4, normalVector);
            walls.Add(wall);
        }

        internal void Render(IDisplayer displayer, PipelineInfo info)
        {
            info.SetRotationMatrix(rotationMatrix);
            info.SetTranslationMatrix(translationMatrix);
            foreach (CuboidWall wall in walls)
            {
                if (!wall.CheckRenderFilling(displayer, info, color)) return;
            }
            foreach (CuboidWall wall in walls)
            {
                wall.RenderFilling(displayer, info, color);
            }
            if (info.ShouldRenderLines())
            {
                foreach (CuboidWall wall in walls)
                {
                    wall.RenderLines(displayer, info);
                }
            }
        }

        internal void Translate(double x, double y, double z)
        {
            translationMatrix.Translate(x, y, z);
        }

        internal void Rotate(int degrees, Axis axis)
        {
            rotationMatrix.Rotate(degrees * Math.PI / 180, axis);
        }
    }
}
