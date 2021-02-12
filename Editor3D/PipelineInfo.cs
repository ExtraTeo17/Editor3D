using Editor3D.Utilities;
using System;

namespace Editor3D
{
    internal class PipelineInfo
    {
        private Matrix modelMatrix;
        private readonly Matrix viewMatrix;
        private readonly Matrix projectionMatrix;
        private readonly int screenWidth;
        private readonly int screenHeight;
        private readonly Vector forwardDirection;

        internal PipelineInfo(Matrix viewMatrix, Matrix projectionMatrix,
            int screenWidth, int screenHeight, Vector forwardDirection)
        {
            this.viewMatrix = viewMatrix;
            this.projectionMatrix = projectionMatrix;
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            this.forwardDirection = forwardDirection;
        }

        internal Matrix GetModelMatrix()
        {
            return modelMatrix;
        }

        internal Matrix GetViewMatrix()
        {
            return viewMatrix;
        }

        internal Matrix GetProjectionMatrix()
        {
            return projectionMatrix;
        }

        internal int GetScreenWidth()
        {
            return screenWidth;
        }

        internal int GetScreenHeight()
        {
            return screenHeight;
        }

        internal Vector GetForwardDirection()
        {
            return forwardDirection;
        }

        internal void SetModelMatrix(Matrix modelMatrix)
        {
            this.modelMatrix = modelMatrix;
        }
    }
}