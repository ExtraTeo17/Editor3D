using System;
using System.Collections.Generic;
using System.Text;

namespace Editor3D.Utilities
{
    class Vertex
    {
        private Vector startPosition;
        private Vector worldPosition;
        private Vector screenPosition;
        private readonly Vector normalVector;

        public Vertex(Vector position, Vector normalVector)
        {
            this.startPosition = position;
            this.normalVector = normalVector;
        }

        internal void Render(IDisplayer displayer, PipelineInfo info)
        {
            (worldPosition, screenPosition) = startPosition.Render(displayer, info);
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
