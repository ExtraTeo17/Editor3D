using System;

namespace Editor3D.Utilities
{
    internal class Triangle
    {
        private readonly Vertex v1, v2, v3;

        public Triangle(Vector pos1, Vector pos2, Vector pos3, Vector normalVector)
        {
            v1 = new Vertex(pos1, normalVector);
            v2 = new Vertex(pos2, normalVector);
            v3 = new Vertex(pos3, normalVector);
        }

        internal void Render(IDisplayer displayer, PipelineInfo info)
        {
            v1.Render(displayer, info);
            v2.Render(displayer, info);
            v3.Render(displayer, info);
            RenderLineBresenham(displayer, (int)v1.GetScreenPosition().x, (int)v1.GetScreenPosition().y,
                (int)v2.GetScreenPosition().x, (int)v2.GetScreenPosition().y);
            RenderLineBresenham(displayer, (int)v1.GetScreenPosition().x, (int)v1.GetScreenPosition().y,
                (int)v3.GetScreenPosition().x, (int)v3.GetScreenPosition().y);
            RenderLineBresenham(displayer, (int)v2.GetScreenPosition().x, (int)v2.GetScreenPosition().y,
                (int)v3.GetScreenPosition().x, (int)v3.GetScreenPosition().y);
        }

        private void RenderLineBresenham(IDisplayer displayer, int x0, int y0, int x1, int y1)
        {
            if (Math.Abs(y1 - y0) < Math.Abs(x1 - x0))
            {
                if (x0 > x1)
                {
                    RenderLineBresenhamLow(displayer, x1, y1, x0, y0);
                }
                else
                {
                    RenderLineBresenhamLow(displayer, x0, y0, x1, y1);
                }
            }
            else
            {
                if (y0 > y1)
                {
                    RenderLineBresenhamHigh(displayer, x1, y1, x0, y0);
                }
                else
                {
                    RenderLineBresenhamHigh(displayer, x0, y0, x1, y1);
                }
            }
        }

        private void RenderLineBresenhamLow(IDisplayer displayer, int x0, int y0, int x1, int y1)
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
                displayer.Display(x, y);
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

        private void RenderLineBresenhamHigh(IDisplayer displayer, int x0, int y0, int x1, int y1)
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
                displayer.Display(x, y);
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
