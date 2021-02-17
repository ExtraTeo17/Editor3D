using System;
using System.Collections.Generic;
using System.Drawing;

namespace Editor3D.Utilities
{
    internal class Triangle
    {
        private const double kd = 0.3;
        private const double ks = 0.4;
        private const double ka = 0.3;
        private const double alfa = 1;

        private readonly Vertex v1, v2, v3;
        private Vector normalVector;

        private bool shouldBeDisplayed = false;
        private Color lineColor = Color.Black;

        public Triangle(Vector pos1, Vector pos2, Vector pos3, Vector normalVector)
        {
            v1 = new Vertex(pos1, normalVector, false);
            v2 = new Vertex(pos2, normalVector, false);
            v3 = new Vertex(pos3, normalVector, false);
            this.normalVector = normalVector;
        }

        public Triangle(Vector pos1, Vector pos2, Vector pos3)
        {
            v1 = new Vertex(pos1, pos1.Clone().Normalize(), true);
            v2 = new Vertex(pos2, pos2.Clone().Normalize(), true);
            v3 = new Vertex(pos3, pos3.Clone().Normalize(), true);
            this.normalVector = pos2.SubstractedBy(pos1).CrossProduct(pos3.SubstractedBy(pos1)).Normalize();
        }

        internal bool CheckRenderFilling(IDisplayer displayer, PipelineInfo info, Color color)
        {
            v1.MakeModel(info);
            v2.MakeModel(info);
            v3.MakeModel(info);
            UpdateNormalVector();
            if (!v1.IsSmooth())
            {
                UpdateSharpEdges();
            }
            if (ShouldBeDisplayed(v1, info))
            {
                this.shouldBeDisplayed = true;
                if (!v1.Render(displayer, info)) return false;
                if (!v2.Render(displayer, info)) return false;
                if (!v3.Render(displayer, info)) return false;
                /*if (displayer.GetShading() == Shading.Flat)
                {
                    color = ComputeColorFlatShading();
                }
                else */
            }
            else
            {
                this.shouldBeDisplayed = false;
            }
            return true;
        }

        private void UpdateSharpEdges()
        {
            v1.SetNormalVector(normalVector.Clone());
            v2.SetNormalVector(normalVector.Clone());
            v3.SetNormalVector(normalVector.Clone());
        }

        internal void RenderFilling(IDisplayer displayer, PipelineInfo info, Color color)
        {
            if (shouldBeDisplayed)
                RenderFillingScanLine(displayer, color, info.GetLights(), info.GetCameraPosition());
        }

        private void UpdateNormalVector()
        {
            this.normalVector = v2.GetWorldPosition().SubstractedBy(v1.GetWorldPosition())
                .CrossProduct(v3.GetWorldPosition().SubstractedBy(v1.GetWorldPosition())).Normalize();
        }

        private bool ShouldBeDisplayed(Vertex vertex, PipelineInfo info)
        {
            return vertex.GetWorldPosition().DirectionTo(info.GetCameraPosition()).DotProduct(normalVector) > 0;
        }

        private void PrepareGourandVertexIntensities(Color color, List<Light> lights, Vector cameraPos, List<Vertex> vertices, bool isFog)
        {
            foreach (Vertex vertex in vertices)
            {
                vertex.GetScreenPosition().SetColor(ComputeColorPhongModel(color, lights, vertex.GetWorldPosition(), cameraPos, vertex.GetNormalVector(), isFog));
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
            if (ShouldBeDisplayed(v1, info))
            {
                if (displayer.IsFog())
                    lineColor = ComputeFog(Color.Black, v1.GetWorldPosition(), info.GetCameraPosition());
                else
                    lineColor = Color.Black;
                RenderLineBresenham(displayer,(int)v1.GetScreenPosition().x, (int)v1.GetScreenPosition().y,
                    (int)v2.GetScreenPosition().x, (int)v2.GetScreenPosition().y, v1.GetScreenPosition().z, v2.GetScreenPosition().z);
                RenderLineBresenham(displayer,(int)v1.GetScreenPosition().x, (int)v1.GetScreenPosition().y,
                    (int)v3.GetScreenPosition().x, (int)v3.GetScreenPosition().y, v1.GetScreenPosition().z, v3.GetScreenPosition().z);
                RenderLineBresenham(displayer, (int)v2.GetScreenPosition().x, (int)v2.GetScreenPosition().y,
                    (int)v3.GetScreenPosition().x, (int)v3.GetScreenPosition().y, v2.GetScreenPosition().z, v3.GetScreenPosition().z);
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
            int x0 = (int)vertices[0].GetScreenPosition().x;
            int y0 = (int)vertices[0].GetScreenPosition().y;
            int x1 = (int)vertices[1].GetScreenPosition().x;
            int y1 = (int)vertices[1].GetScreenPosition().y;
            int x2 = (int)vertices[2].GetScreenPosition().x;
            int y2 = (int)vertices[2].GetScreenPosition().y;
            double z0 = vertices[0].GetScreenPosition().z;
            double z1 = vertices[1].GetScreenPosition().z;
            double z2 = vertices[2].GetScreenPosition().z;

            if (displayer.GetShading() == Shading.Gourand)
            {
                PrepareGourandVertexIntensities(color, lights, cameraPos, vertices, displayer.IsFog());
            }
            else if (displayer.GetShading() == Shading.Flat)
            {
                color = ComputeColorPhongModel(color, lights, GetWorldMiddle(), cameraPos, normalVector, displayer.IsFog());
            }
            Color c0 = vertices[0].GetScreenPosition().GetColor();
            Color c1 = vertices[1].GetScreenPosition().GetColor();
            Color c2 = vertices[2].GetScreenPosition().GetColor();
            Vector n0 = null, n1 = null, n2 = null;
            Vector w0 = null, w1 = null, w2 = null;
            if (displayer.GetShading() == Shading.Phong)
            {
                n0 = vertices[0].GetNormalVector();
                n1 = vertices[1].GetNormalVector();
                n2 = vertices[2].GetNormalVector();
                w0 = vertices[0].GetWorldPosition();
                w1 = vertices[1].GetWorldPosition();
                w2 = vertices[2].GetWorldPosition();
            }

            if (y1 == y2)
            {
                FillBottomTriangle(displayer, x0, y0, z0, c0, n0, w0, x1, y1, z1, c1, n1, w1, x2, y2, z2, c2, n2, w2, color, lights, cameraPos);
            }
            else if (y0 == y1)
            {
                FillTopTriangle(displayer, x0, y0, z0, c0, n0, w0, x1, y1, z1, c1, n1, w1, x2, y2, z2, c2, n2, w2, color, lights, cameraPos);
            }
            else
            {
                int x3 = x0 + (int)(((double)(y1 - y0) / (double)(y2 - y0)) * (x2 - x0));
                int y3 = y1;
                double z3 = InterpolateZ(z0, z2, (double)(y1 - y0) / (double)(y2 - y0));
                Color c3 = new Color();
                Vector n3 = null, w3 = null;
                if (displayer.GetShading() == Shading.Gourand)
                {
                    c3 = InterpolateColorGourandShading(y2, y0, y1, c2, c0);
                }
                else if (displayer.GetShading() == Shading.Phong)
                {
                    n3 = InterpolateNormalPhongShading(y2, y0, y1, n2, n0);
                    w3 = InterpolateWorldPhongShading(y2, y0, y1, w2, w0);
                }
                FillBottomTriangle(displayer, x0, y0, z0, c0, n0, w0, x1, y1, z1, c1, n1, w1, x3, y3, z3, c3, n3, w3, color, lights, cameraPos);
                FillTopTriangle(displayer, x1, y1, z1, c1, n1, w1, x3, y3, z3, c3, n3, w3, x2, y2, z2, c2, n2, w2, color, lights, cameraPos);
            }
        }

        private Vector GetWorldMiddle()
        {
            Vector w1 = v1.GetWorldPosition();
            Vector w2 = v2.GetWorldPosition();
            Vector w3 = v3.GetWorldPosition();
            double a = w2.SubstractedBy(w3).Magnitude();
            double b = w3.SubstractedBy(w1).Magnitude();
            double c = w1.SubstractedBy(w2).Magnitude();
            Vector A = w1.MultipliedBy(a / (a + b + c));
            Vector B = w2.MultipliedBy(b / (a + b + c));
            Vector C = w3.MultipliedBy(c / (a + b + c));
            return A.SummedWith(B).SummedWith(C);
        }

        private void FillTopTriangle(IDisplayer displayer,
            int x0, int y0, double z0, Color c0, Vector n0, Vector w0,
            int x1, int y1, double z1, Color c1, Vector n1, Vector w1,
            int x2, int y2, double z2, Color c2, Vector n2, Vector w2,
            Color color, List<Light> lights, Vector cameraPos)
        {
            double d1 = (double)(x2 - x0) / (double)(y2 - y0 + 1);
            double d2 = (double)(x2 - x1) / (double)(y2 - y1 + 1);
            double xa = x2;
            double xb = x2;
            for (int sc = y2; sc >= y0; --sc)
            {
                Color leftInsensity = new Color();
                Color rightIntensity = new Color();
                Vector leftNormal = null, rightNormal = null, leftWorld = null, rightWorld = null;
                if (displayer.GetShading() == Shading.Gourand)
                {
                    leftInsensity = InterpolateColorGourandShading(y2, y0, sc, c2, c0);
                    rightIntensity = InterpolateColorGourandShading(y2, y1, sc, c2, c1);
                }
                else if (displayer.GetShading() == Shading.Phong)
                {
                    leftNormal = InterpolateNormalPhongShading(y2, y0, sc, n2, n0);
                    rightNormal = InterpolateNormalPhongShading(y2, y1, sc, n2, n1);
                    leftWorld = InterpolateWorldPhongShading(y2, y0, sc, w2, w0);
                    rightWorld = InterpolateWorldPhongShading(y2, y1, sc, w2, w1);
                }
                xa -= d1;
                xb -= d2;
                RenderHorizontalLine(displayer, (int)xa, (int)xb, sc,
                    InterpolateZ(z0, z2, (double)(sc - y0) / (double)(y2 - y0)),
                    InterpolateZ(z1, z2, (double)(sc - y0) / (double)(y2 - y0)),
                    color, lights, leftInsensity, rightIntensity, leftNormal, rightNormal, cameraPos, leftWorld, rightWorld);
            }
        }

        private void FillBottomTriangle(IDisplayer displayer,
            int x0, int y0, double z0, Color c0, Vector n0, Vector w0,
            int x1, int y1, double z1, Color c1, Vector n1, Vector w1,
            int x2, int y2, double z2, Color c2, Vector n2, Vector w2,
            Color color, List<Light> lights, Vector cameraPos)
        {
            double d1 = (double)(x1 - x0) / (double)(y1 - y0 + 1);
            double d2 = (double)(x2 - x0) / (double)(y2 - y0 + 1);
            double xa = x0;
            double xb = x0;
            for (int sc = y0; sc <= y1; ++sc)
            {
                Color leftInsensity = new Color();
                Color rightIntensity = new Color();
                Vector leftNormal = null, rightNormal = null, leftWorld = null, rightWorld = null;
                if (displayer.GetShading() == Shading.Gourand)
                {
                    leftInsensity = InterpolateColorGourandShading(y1, y0, sc, c1, c0);
                    rightIntensity = InterpolateColorGourandShading(y2, y0, sc, c2, c0);
                }
                else if (displayer.GetShading() == Shading.Phong)
                {
                    leftNormal = InterpolateNormalPhongShading(y1, y0, sc, n1, n0);
                    rightNormal = InterpolateNormalPhongShading(y2, y0, sc, n2, n0);
                    leftWorld = InterpolateWorldPhongShading(y1, y0, sc, w1, w0);
                    rightWorld = InterpolateWorldPhongShading(y2, y0, sc, w2, w0);
                }
                xa += d1;
                xb += d2;
                RenderHorizontalLine(displayer, (int)xa, (int)xb, sc,
                    InterpolateZ(z0, z1, (double)(sc - y0) / (double)(y1 - y0)),
                    InterpolateZ(z0, z2, (double)(sc - y0) / (double)(y1 - y0)),
                    color, lights, leftInsensity, rightIntensity, leftNormal, rightNormal, cameraPos, leftWorld, rightWorld);
            }
        }

        private Color InterpolateColorGourandShading(double intervalEnd, double intervalStart, int intervalPos, Color colorEnd, Color colorStart)
        {
            double q = (intervalEnd - intervalStart) == 0 ? 0 : (intervalPos - intervalStart) / (intervalEnd - intervalStart);
            Color color = ColorSummedWith(ColorMultipliedBy(colorStart, 1 - q), ColorMultipliedBy(colorEnd, q));
            return color;
        }

        private Vector InterpolateNormalPhongShading(double intervalEnd, double intervalStart, int intervalPos, Vector normalEnd, Vector normalStart)
        {
            double q = (intervalEnd - intervalStart) == 0 ? 0 : (intervalPos - intervalStart) / (intervalEnd - intervalStart);
            Vector vector = normalStart.MultipliedBy(1 - q).SummedWith(normalEnd.MultipliedBy(q));
            return vector.Normalize();
        }
        private Vector InterpolateWorldPhongShading(double intervalEnd, double intervalStart, int intervalPos, Vector worldEnd, Vector worldStart)
        {
            double q = (intervalEnd - intervalStart) == 0 ? 0 : (intervalPos - intervalStart) / (intervalEnd - intervalStart);
            Vector vector = worldStart.MultipliedBy(1 - q).SummedWith(worldEnd.MultipliedBy(q));
            return vector;
        }

        private void RenderHorizontalLine(IDisplayer displayer, int x1, int x2, int y, double z1, double z2,
            Color color, List<Light> lights, Color gourandIntensity1, Color gourandIntensity2, Vector normal1, Vector normal2,
            Vector cameraPos, Vector world1, Vector world2)
        {
            double z = 0;
            if (x1 > x2)
            {
                int tmp = x1;
                x1 = x2;
                x2 = tmp;
                double tmp2 = z1;
                z1 = z2;
                z2 = tmp2;
                Color tmp3 = gourandIntensity1;
                gourandIntensity1 = gourandIntensity2;
                gourandIntensity2 = tmp3;
                Vector nTmp = null;
                if (normal1 != null)
                {
                    nTmp = normal1.Clone();
                    normal1 = normal2.Clone();
                    normal2 = nTmp;
                }
                Vector wTmp = null;
                if (world1 != null)
                {
                    wTmp = world1.Clone();
                    world1 = world2.Clone();
                    world2 = wTmp;
                }
            }
            for (int x = x1; x <= x2; ++x)
            {
                Color usedColor = new Color();
                if (displayer.GetShading() == Shading.Gourand)
                {
                    usedColor = InterpolateColorGourandShading(x2, x1, x, gourandIntensity2, gourandIntensity1);
                }
                else if (displayer.GetShading() == Shading.Phong)
                {
                    Vector normal = InterpolateNormalPhongShading(x2, x1, x, normal2, normal1);
                    Vector world = InterpolateWorldPhongShading(x2, x1, x, world2, world1);
                    usedColor = ComputeColorPhongModel(color, lights, world, cameraPos, normal, displayer.IsFog());
                }
                else
                {
                    usedColor = color;
                }
                double q = x2 == x1 ? 0 : (x - x1) / (x2 - x1);
                z = InterpolateZ(z1, z2, q);
                displayer.Display(x, y, z, usedColor); // TODO: consider changing to primitive types instead of passing Vector
            }
        }

        private Color ComputeColorPhongModel(Color color, List<Light> lights, Vector pointPos, Vector cameraPos, Vector pointNormalVector, bool isFog) // TODO: Consider not rounding to ints upon every multiplying
        {
            Color intensity = Color.FromArgb(0, 0, 0); // TODO: try to change to color from parameter
            foreach (Light light in lights)
            {
                if (!light.on)
                    continue;
                Vector lightPos = light.GetPosition();
                Vector L = pointPos.DirectionTo(lightPos).Normalize();
                Vector N = pointNormalVector;
                Vector V = pointPos.DirectionTo(cameraPos).Normalize();
                Vector R = (N.MultipliedBy(2.0 * L.DotProduct(N))).SubstractedBy(L).Normalize();
                Color diffuse = ColorMultipliedBy(light.Id, kd * L.DotProduct(N));
                Color specular = ColorMultipliedBy(light.Is, ks * Math.Pow(R.DotProduct(V), alfa));
                double spotlightFactor = 1;
                bool isSpotlightInRange = false;
                if (light.IsSpotlight())
                {
                    Vector minusSpotlightDirection = light.GetSpotlightDirection().NegatedWithoutW();
                    double spotlightCosine = minusSpotlightDirection.DotProduct(L);
                    if (spotlightCosine >= light.GetSpotlightRange())
                    {
                        spotlightFactor = Math.Pow(spotlightCosine, light.GetSpotlightExponent());
                        isSpotlightInRange = true;
                    }
                    else
                    {
                        continue;
                    }
                }
                Color diffuseSpecularSum = ColorSummedWith(diffuse, specular);
                intensity = ColorSummedWith(intensity, diffuseSpecularSum);
                if (isSpotlightInRange)
                {
                    intensity = ColorMultipliedBy(intensity, spotlightFactor);
                }
            }
            Color final = ColorSummedWith(ColorMultipliedBy(color, ka), ColorMultipliedBy(intensity, ComputeIf()));
            if (isFog)
                final = ComputeFog(final, pointPos, cameraPos);
            return final;
        }

        private Color ComputeFog(Color color, Vector pointPos, Vector cameraPos)
        {
            int R = color.R;
            int G = color.G;
            int B = color.B;
            int diffR = SystemColors.Control.R - R;
            int diffG = SystemColors.Control.G - G;
            int diffB = SystemColors.Control.B - B;
            double distance = cameraPos.DistanceTo(pointPos) - EditorForm.NEAR_PLANE;
            double fogFactor = distance / (EditorForm.FAR_PLANE - EditorForm.NEAR_PLANE);
            diffR = (int)(diffR * fogFactor);
            diffG = (int)(diffG * fogFactor);
            diffB = (int)(diffB * fogFactor);
            R = R + diffR > SystemColors.Control.R ? SystemColors.Control.R : R + diffR;
            G = G + diffG > SystemColors.Control.G ? SystemColors.Control.G : G + diffG;
            B = B + diffB > SystemColors.Control.B ? SystemColors.Control.B : B + diffB;
            return Color.FromArgb(R < 0 ? 0 : R, G < 0 ? 0 : G, B < 0 ? 0 : B);
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
            return Color.FromArgb(red < 0 ? 0 : red, green < 0 ? 0 : green, blue < 0 ? 0 : blue);
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
                double tmpZ = InterpolateZ(z0, z1, (double)(x - x0) / (double)(x1 - x0));
                displayer.Display(x, y, tmpZ, lineColor);
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
            double x = (z0 * (1.0 - q)) + (z1 * q);
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

            double tmpZ = 0;
            for (int y = y0; y < y1; ++y)
            {
                tmpZ = InterpolateZ(z0, z1, (double)(y - y0) / (double)(y1 - y0));
                displayer.Display(x, y, tmpZ, lineColor);
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
