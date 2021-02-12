using System;
using System.Collections.Generic;
using System.Drawing;

namespace Editor3D.Utilities
{
    internal class Triangle
    {
        private readonly Vertex v1, v2, v3;
        private readonly Vector normalVector;

        public Triangle(Vector pos1, Vector pos2, Vector pos3, Vector normalVector)
        {
            v1 = new Vertex(pos1, normalVector);
            v2 = new Vertex(pos2, normalVector);
            v3 = new Vertex(pos3, normalVector);
            this.normalVector = normalVector;
        }

        internal void RenderFilling(IDisplayer displayer, PipelineInfo info)
        {
            if (info.GetForwardDirection().DotProduct(normalVector) > 0)
            {
                displayer.SetColor(Color.Cyan);
                v1.Render(displayer, info);
                v2.Render(displayer, info);
                v3.Render(displayer, info);
                RenderFillingScanLine(displayer, v1.GetScreenPosition(), v2.GetScreenPosition(), v3.GetScreenPosition());
            }
        }

        internal void RenderLines(IDisplayer displayer, PipelineInfo info)
        {
            if (info.GetForwardDirection().DotProduct(normalVector) > 0)
            {
                displayer.SetColor(Color.Black);
                RenderLineBresenham(displayer, (int)v1.GetScreenPosition().x, (int)v1.GetScreenPosition().y,
                    (int)v2.GetScreenPosition().x, (int)v2.GetScreenPosition().y, v1.GetScreenPosition().z, v2.GetScreenPosition().z);
                RenderLineBresenham(displayer, (int)v1.GetScreenPosition().x, (int)v1.GetScreenPosition().y,
                    (int)v3.GetScreenPosition().x, (int)v3.GetScreenPosition().y, v1.GetScreenPosition().z, v2.GetScreenPosition().z);
                RenderLineBresenham(displayer, (int)v2.GetScreenPosition().x, (int)v2.GetScreenPosition().y,
                    (int)v3.GetScreenPosition().x, (int)v3.GetScreenPosition().y, v1.GetScreenPosition().z, v2.GetScreenPosition().z);
            }
        }

        public static int CompareByY(Vector v1, Vector v2)
        {
            return v1.y.CompareTo(v2.y);
        }

        private void RenderFillingScanLine(IDisplayer displayer, Vector v1, Vector v2, Vector v3)
        {
            List<Vector> vertices = new List<Vector>() { v1, v2, v3 };
            vertices.Sort(CompareByY);
            if (vertices[1].y == vertices[2].y)
            {
                FillBottomTriangle(displayer, vertices[0], vertices[1], vertices[2]);
            }
            else if (vertices[0].y == vertices[1].y)
            {
                FillTopTriangle(displayer, vertices[0], vertices[1], vertices[2]);
            }
            else
            {
                double v4x = vertices[0].x + ((vertices[1].y - vertices[0].y) / (vertices[2].y - vertices[0].y)) * (vertices[2].x - vertices[0].x);
                Vector v4 = new Vector(v4x, vertices[1].y, 1, 1);
                FillBottomTriangle(displayer, vertices[0], vertices[1], v4);
                FillTopTriangle(displayer, vertices[1], v4, vertices[2]);
            }
        }

        private void FillTopTriangle(IDisplayer displayer, Vector v1, Vector v2, Vector v3)
        {
            double d1 = (v3.x - v1.x) / (v3.y - v1.y);
            double d2 = (v3.x - v2.x) / (v3.y - v2.y);
            double x1 = v3.x + 1;
            double x2 = v3.x + 1;
            for (int scanline = (int)v3.y; scanline > v1.y; --scanline)
            {
                RenderHorizontalLine(displayer, (int)x1, (int)x2, scanline,
                    InterpolateZ(v3.z, v1.z, ((double)(scanline) - v1.y) / (double)((int)v3.y - v1.y)),
                    InterpolateZ(v3.z, v2.z, ((double)(scanline) - v1.y) / (double)((int)v3.y - v1.y)));
                x1 -= d1;
                x2 -= d2;
            }
        }

        private void FillBottomTriangle(IDisplayer displayer, Vector v1, Vector v2, Vector v3)
        {
            double d1 = (v2.x - v1.x) / (v2.y - v1.y);
            double d2 = (v3.x - v1.x) / (v3.y - v1.y);
            double x1 = v1.x + 1;
            double x2 = v1.x + 1;
            for (int scanline = (int)v1.y; scanline <= v2.y; ++scanline)
            {
                RenderHorizontalLine(displayer, (int)x1, (int)x2, scanline,
                    InterpolateZ(v2.z, v1.z, ((double)(scanline) - v1.y) / (double)((int)v2.y - v1.y)),
                    InterpolateZ(v3.z, v1.z, ((double)(scanline) - v1.y) / (double)((int)v2.y - v1.y)));
                x1 += d1;
                x2 += d2;
            }
        }

        private void RenderHorizontalLine(IDisplayer displayer, int x1, int x2, int y, double z0, double z1)
        {
            if (x1 > x2)
            {
                int tmp = x1;
                x1 = x2;
                x2 = tmp;
                double tmp2 = z0;
                z0 = z1;
                z1 = tmp2;
            }
            for (int x = x1; x <= x2; ++x)
            {
                displayer.Display(x, y, InterpolateZ(z0, z1, ((double)(x) - (double)x1) / (double)(x2 - x1)));
            }
        }

        private void RenderLineBresenham(IDisplayer displayer, int x0, int y0, int x1, int y1, double z0, double z1)
        {
            if (Math.Abs(y1 - y0) < Math.Abs(x1 - x0))
            {
                if (x0 > x1)
                {
                    RenderLineBresenhamLow(displayer, x1, y1, x0, y0, z1, z0);
                }
                else
                {
                    RenderLineBresenhamLow(displayer, x0, y0, x1, y1, z0, z1);
                }
            }
            else
            {
                if (y0 > y1)
                {
                    RenderLineBresenhamHigh(displayer, x1, y1, x0, y0, z1, z0);
                }
                else
                {
                    RenderLineBresenhamHigh(displayer, x0, y0, x1, y1, z0, z1);
                }
            }
        }

        private void RenderLineBresenhamLow(IDisplayer displayer, int x0, int y0, int x1, int y1, double z0, double z1)
        {
            int dx = x1 - x0;
            int dy = y1 - y0;
            int yi = 1;
            if (dy < 0)
            {
                yi = -1;
                dy = -dy;
            }
            int d = (2 * dy) - dx;
            int incrV = 2 * dy;
            int incrDiag = 2 * (dy - dx);
            int y = y0;
            for (int x = x0; x < x1; ++x)
            {
                displayer.Display(x, y, InterpolateZ(z0, z1, ((double)(x) - (double)x0) / (double)(x1 - x0)));
                if (d > 0)
                {
                    y += yi;
                    d += incrDiag;
                }
                else
                {
                    d += incrV;
                }
            }
        }

        private double InterpolateZ(double z0, double z1, double q)
        {
            double x = (z0 * (1 - q)) + (z1 * q);
            return x;
        }

        private void RenderLineBresenhamHigh(IDisplayer displayer, int x0, int y0, int x1, int y1, double z0, double z1)
        {
            int dx = x1 - x0;
            int dy = y1 - y0;
            int xi = 1;
            if (dx < 0)
            {
                xi = -1;
                dx = -dx;
            }
            int d = (2 * dx) - dy;
            int incrH = 2 * dx;
            int incrDiag = 2 * (dx - dy);
            int x = x0;
            for (int y = y0; y < y1; ++y)
            {
                displayer.Display(x, y, InterpolateZ(z0, z1, (double)(y) / (double)(y1 - y0)));
                if (d > 0)
                {
                    x += xi;
                    d += incrDiag;
                }
                else
                {
                    d += incrH;
                }
            }
        }
    }
}
