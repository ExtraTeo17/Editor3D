using System;
using System.Collections.Generic;
using System.Drawing;

namespace Editor3D.Utilities
{
    internal class Triangle
    {
        private const double kd = 0.3;
        private const double ks = 0.3;
        private const double ka = 0.4;
        private const double alfa = 1;
        private Color Ia = Color.White;

        private readonly Vertex v1, v2, v3;
        private readonly Vector normalVector;
        private readonly GourandInfo gourandInfo = new GourandInfo();

        public Triangle(Vector pos1, Vector pos2, Vector pos3, Vector normalVector)
        {
            v1 = new Vertex(pos1, normalVector);
            v2 = new Vertex(pos2, normalVector);
            v3 = new Vertex(pos3, normalVector);
            this.normalVector = normalVector;
        }

        public Triangle(Vector pos1, Vector pos2, Vector pos3)
        {
            v1 = new Vertex(pos1, pos1.Clone().Normalize());
            v2 = new Vertex(pos2, pos2.Clone().Normalize());
            v3 = new Vertex(pos3, pos3.Clone().Normalize());
            this.normalVector = pos2.SubstractedBy(pos1).CrossProduct(pos3.SubstractedBy(pos1));
        }

        internal void RenderFilling(IDisplayer displayer, PipelineInfo info, Color color)
        {
            if (info.GetForwardDirection().DotProduct(normalVector) > 0)
            {
                v1.Render(displayer, info);
                v2.Render(displayer, info);
                v3.Render(displayer, info);
                /*if (displayer.GetShading() == Shading.Flat)
                {
                    color = ComputeColorFlatShading();
                }
                else */
                RenderFillingScanLine(displayer, color, info.GetLights(), info.GetCameraPosition());
            }
        }

        private void PrepareGourandVertexIntensities(Color color, List<Light> lights, Vector cameraPos, List<Vertex> vertices)
        {
            foreach (Vertex vertex in vertices)
            {
                vertex.GetScreenPosition().SetColor(ComputeColorPhongModel(color, lights, vertex.GetWorldPosition(), cameraPos, vertex.GetNormalVector()));
            }
            if (DebugTriangle())
            {
                Console.WriteLine("V1 " + v1.GetScreenPosition().GetColor() + ", V2 " + v2.GetScreenPosition().GetColor() + ", V3 " + v3.GetScreenPosition().GetColor());
            }
        }

        private bool DebugTriangle()
        {
            return v1.GetScreenPosition().x == 258 && v1.GetScreenPosition().y == 251
                && v2.GetScreenPosition().x == 332 && v2.GetScreenPosition().y == 222
                && v3.GetScreenPosition().x == 260 && v3.GetScreenPosition().y == 286;
        }

        internal void RenderLines(IDisplayer displayer, PipelineInfo info)
        {
            if (info.GetForwardDirection().DotProduct(normalVector) > 0)
            {
                RenderLineBresenham(displayer, (int)v1.GetScreenPosition().x, (int)v1.GetScreenPosition().y,
                    (int)v2.GetScreenPosition().x, (int)v2.GetScreenPosition().y, v1.GetScreenPosition().z, v2.GetScreenPosition().z);
                RenderLineBresenham(displayer, (int)v1.GetScreenPosition().x, (int)v1.GetScreenPosition().y,
                    (int)v3.GetScreenPosition().x, (int)v3.GetScreenPosition().y, v1.GetScreenPosition().z, v2.GetScreenPosition().z);
                RenderLineBresenham(displayer, (int)v2.GetScreenPosition().x, (int)v2.GetScreenPosition().y,
                    (int)v3.GetScreenPosition().x, (int)v3.GetScreenPosition().y, v1.GetScreenPosition().z, v2.GetScreenPosition().z);
            }
        }

        public static int CompareByY(Vertex v1, Vertex v2)
        {
            return v1.GetScreenPosition().y.CompareTo(v2.GetScreenPosition().y);
        }

        private void RenderFillingScanLine(IDisplayer displayer, Color color, List<Light> lights, Vector cameraPos) // TODO: Fix missing Gourand cases (if, else if)
        {
            List<Vertex> vertices = new List<Vertex>() { v1, v2, v3 };
            vertices.Sort(CompareByY);
            List<Vector> positions = new List<Vector>() { vertices[0].GetScreenPosition(),
                vertices[1].GetScreenPosition(), vertices[2].GetScreenPosition() };
            if (displayer.GetShading() == Shading.Gourand)
            {
                PrepareGourandVertexIntensities(color, lights, cameraPos, vertices);
            }
            if (positions[1].y == positions[2].y)
            {
                FillBottomTriangle(displayer, positions[0], positions[1], positions[2], color, lights);
            }
            else if (positions[0].y == positions[1].y)
            {
                FillTopTriangle(displayer, positions[0], positions[1], positions[2], color, lights);
            }
            else
            {
                double v4x = positions[0].x + ((positions[1].y - positions[0].y) / (positions[2].y - positions[0].y)) * (positions[2].x - positions[0].x);
                Vector v4 = new Vector(v4x, positions[1].y, 1, 1);
                if (displayer.GetShading() == Shading.Gourand)
                {
                    v4.SetColor(InterpolateColorGourandShading(positions[2].y, positions[0].y, (int)positions[1].y,
                        positions[2].GetColor(), positions[0].GetColor()));
                }
                FillBottomTriangle(displayer, positions[0], positions[1], v4, color, lights);
                FillTopTriangle(displayer, positions[1], v4, positions[2], color, lights);
            }
        }

        private void FillTopTriangle(IDisplayer displayer, Vector v1, Vector v2, Vector v3,
            Color color, List<Light> lights)
        {
            double d1 = (v3.x - v1.x) / (v3.y - v1.y);
            double d2 = (v3.x - v2.x) / (v3.y - v2.y);
            double x1 = v3.x + 1;
            double x2 = v3.x + 1;
            for (int scanline = (int)v3.y; scanline > v1.y; --scanline)
            {
                Color leftInsensity = new Color();
                Color rightIntensity = new Color();
                if (displayer.GetShading() == Shading.Gourand)
                {
                    leftInsensity = InterpolateColorGourandShading(v3.y, v1.y, scanline,
                        v3.GetColor(), v1.GetColor());
                    rightIntensity = InterpolateColorGourandShading(v3.y, v2.y, scanline,
                        v3.GetColor(), v2.GetColor());
                }
                RenderHorizontalLine(displayer, (int)x1, (int)x2, scanline,
                    InterpolateZ(v3.z, v1.z, (double)(scanline - (int)v1.y) / (double)((int)v3.y - (int)v1.y)),
                    InterpolateZ(v3.z, v2.z, (double)(scanline - (int)v1.y) / (double)((int)v3.y - (int)v1.y)),
                    color, lights, leftInsensity, rightIntensity);
                x1 -= d1;
                x2 -= d2;
            }
        }

        private void FillBottomTriangle(IDisplayer displayer, Vector v1, Vector v2, Vector v3,
            Color color, List<Light> lights)
        {
            double d1 = (v2.x - v1.x) / (v2.y - v1.y);
            double d2 = (v3.x - v1.x) / (v3.y - v1.y);
            double x1 = v1.x + 1;
            double x2 = v1.x + 1;
            for (int scanline = (int)v1.y; scanline <= v2.y; ++scanline)
            {
                Color leftInsensity = new Color();
                Color rightIntensity = new Color();
                if (displayer.GetShading() == Shading.Gourand)
                {
                    leftInsensity = InterpolateColorGourandShading(v2.y, v1.y, scanline,
                        v2.GetColor(), v1.GetColor());
                    rightIntensity = InterpolateColorGourandShading(v3.y, v1.y, scanline,
                        v3.GetColor(), v1.GetColor());
                }
                RenderHorizontalLine(displayer, (int)x1, (int)x2, scanline,
                    InterpolateZ(v2.z, v1.z, (double)(scanline - (int)v1.y) / (double)((int)v2.y - (int)v1.y)),
                    InterpolateZ(v3.z, v1.z, (double)(scanline - (int)v1.y) / (double)((int)v2.y - (int)v1.y)),
                    color, lights, leftInsensity, rightIntensity);
                x1 += d1;
                x2 += d2;
            }
        }

        private Color InterpolateColorGourandShading(double intervalEnd, double intervalStart, int intervalPos, Color colorEnd, Color colorStart)
        {
            double q = (intervalEnd - intervalStart) == 0 ? 0 : (intervalPos - intervalStart) / (intervalEnd - intervalStart);
            Color color = ColorSummedWith(ColorMultipliedBy(colorStart, 1 - q), ColorMultipliedBy(colorEnd, q));
            return color;
        }

        private void RenderHorizontalLine(IDisplayer displayer, int x1, int x2, int y, double z0, double z1,
            Color color, List<Light> lights, Color gourandIntensity1, Color gourandIntensity2)
        {
            if (x1 > x2)
            {
                int tmp = x1;
                x1 = x2;
                x2 = tmp;
                double tmp2 = z0;
                z0 = z1;
                z1 = tmp2;
                Color tmp3 = gourandIntensity1;
                gourandIntensity1 = gourandIntensity2;
                gourandIntensity2 = tmp3;
            }
            for (int x = x1; x <= x2; ++x)
            {
                if (displayer.GetShading() == Shading.Gourand)
                {
                    color = InterpolateColorGourandShading(x2, x1, x, gourandIntensity2, gourandIntensity1);
                }
                /*else if (displayer.GetShading() == Shading.Phong)
                {
                    color = ComputeColorPhongShading();
                }*/
                displayer.Display(x, y, InterpolateZ(z0, z1, (double)(x - x1) / (double)(x2 - x1)), color); // TODO: consider changing to primitive types instead of passing Vector
            }
        }

        private Color ComputeColorPhongModel(Color color, List<Light> lights, Vector pointPos, Vector cameraPos, Vector pointNormalVector) // TODO: Consider not rounding to ints upon every multiplying
        {
            Color intensity = Color.FromArgb(0, 0, 0); // TODO: try to change to color from parameter
            foreach (Light light in lights)
            {
                Vector lightPos = light.GetPosition();
                Vector L = pointPos.DirectionTo(lightPos).Normalize();
                Vector N = pointNormalVector;
                Vector V = pointPos.DirectionTo(cameraPos).Normalize();
                Vector R = (N.MultipliedBy(2.0 * L.DotProduct(N))).SubstractedBy(L).Normalize();
                Color diffuse = ColorMultipliedBy(light.Id, Math.Abs(kd * L.DotProduct(N)));
                Color specular = ColorMultipliedBy(light.Is, ks * Math.Abs(Math.Pow(R.DotProduct(V), alfa)));
                Color diffuseSpecularSum = ColorSummedWith(diffuse, specular);
                intensity = ColorSummedWith(intensity, diffuseSpecularSum);
            }
            return ColorSummedWith(ColorMultipliedBy(color, ka), ColorMultipliedBy(intensity, ComputeIf())); // TODO: Fix ambient (Ia) to not be from shapes
        }

        private Color ColorSummedWith(Color color1, Color color2)
        {
            int red = color1.R + color2.R > 255 ? 255 : color1.R + color2.R;
            int green = color1.G + color2.G > 255 ? 255 : color1.G + color2.G;
            int blue = color1.B + color2.B > 255 ? 255 : color1.B + color2.B;
            return Color.FromArgb(red, green, blue);
        }

        private Color ColorMultipliedBy(Color color, double multiplier)
        {
            int red = color.R * multiplier > 255 ? 255 : (int)((double)color.R * multiplier);
            int green = color.G * multiplier > 255 ? 255 : (int)((double)color.G * multiplier);
            int blue = color.B * multiplier > 255 ? 255 : (int)((double)color.B * multiplier);
            return Color.FromArgb(red, green, blue);
        }

        private double ComputeIf()
        {
            return 1;
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
                displayer.Display(x, y, InterpolateZ(z0, z1, (double)(y - y0) / (double)(y1 - y0)), Color.Black);
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
                displayer.Display(x, y, InterpolateZ(z0, z1, (double)(y - y0) / (double)(y1 - y0)), Color.Black);
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
