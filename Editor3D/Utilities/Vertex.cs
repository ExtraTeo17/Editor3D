using System;
using System.Collections.Generic;
using System.Text;

namespace Editor3D.Utilities
{
    class Vertex
    {
        private readonly Vector position;
        private readonly Vector normalVector;
        private Vector worldPosition;
        private Vector screenPosition;

        public Vertex(Vector position, Vector normalVector)
        {
            this.position = position;
            this.normalVector = normalVector;
        }

        internal void Render(IDisplayer displayer, PipelineInfo info)
        {
            (worldPosition, screenPosition) = position.Render(displayer, info);
        }

        public Vector GetScreenPosition()
        {
            return screenPosition;
        }

        public Vector GetWorldPosition()
        {
            return worldPosition;
        }
    }
}
