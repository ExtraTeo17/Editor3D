using System;
using System.Collections.Generic;
using System.Text;

namespace Editor3D.Utilities
{
    class Vertex
    {
        private readonly Vector position;
        private readonly Vector normalVector;

        public Vertex(Vector position, Vector normalVector)
        {
            this.position = position;
            this.normalVector = normalVector;
        }

        internal void Render(IDisplayer displayer, PipelineInfo info)
        {
            position.TransformAndRender(displayer, info);
        }
    }
}
