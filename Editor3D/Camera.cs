using Editor3D.Utilities;
using System;

namespace Editor3D
{
    internal class Camera
    {
        private Matrix viewMatrix;
        private Matrix projectionMatrix;

        internal Camera(Vector cameraPosition, Vector observedPosition, double nearPlane,
            double farPlane, double fieldOfView, double aspect)
        {
            viewMatrix = GenerateViewMatrix(cameraPosition, observedPosition);
            projectionMatrix = Matrix.Projection(nearPlane, farPlane, fieldOfView, aspect);
        }

        private Matrix GenerateViewMatrix(Vector cameraPosition, Vector observedPosition)
        {
            Vector upWorldDirection = new Vector(0, 1, 0, 0);
            Vector forwardDirection = cameraPosition.DirectionTo(observedPosition).Normalize();
            Vector rightDirection = upWorldDirection.CrossProduct(forwardDirection).Normalize();
            Vector upDirection = forwardDirection.CrossProduct(rightDirection).Normalize();
            Matrix rotationMatrix = Matrix.Rotation(rightDirection, upDirection, forwardDirection);
            Matrix transformationMatrix = Matrix.Translation(cameraPosition.Negated());
            return rotationMatrix.MultipliedBy(transformationMatrix);
        }

        internal Matrix GetViewMatrix()
        {
            return viewMatrix;
        }

        internal Matrix GetProjectionMatrix()
        {
            return projectionMatrix;
        }
    }
}
