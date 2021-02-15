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
        private Vector normalVector;

        public Vertex(Vector position, Vector normalVector)
        {
            this.startPosition = position;
            this.normalVector = normalVector;
        }

        internal void MakeModel(PipelineInfo info)
        {
            worldPosition = startPosition.Rotate(info).Translate(info);
            normalVector = worldPosition.Clone().Normalize();
        }

        internal bool Render(IDisplayer displayer, PipelineInfo info)
        {
            Vector isInCameraView = worldPosition.Render(displayer, info);
            if (isInCameraView == null) return false;
            screenPosition = isInCameraView;
            return true;
        }

        public Vector GetScreenPosition()
        {
            return screenPosition;
        }

        public Vector GetWorldPosition()
        {
            return worldPosition;
        }

        internal Vector GetNormalVector()
        {
            return normalVector;
        }
    }
}
