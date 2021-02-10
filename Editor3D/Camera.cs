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
            PrepareViewMatrix(cameraPosition, observedPosition);
            PrepareProjectionMatrix(nearPlane, farPlane, fieldOfView, aspect);
        }

        private void PrepareViewMatrix(Vector cameraPosition, Vector observedPosition)
        {
            Vector upWorldDirection = new Vector(0, 1, 0, 0);
            Vector fromCameraToObservedPoint = cameraPosition.DirectionTo(observedPosition);
            Vector rightDirection = upWorldDirection.CrossProduct(fromCameraToObservedPoint);
            Vector upDirection = fromCameraToObservedPoint.CrossProduct(rightDirection);
        }

        private void PrepareProjectionMatrix(double nearPlane, double farPlane, double fieldOfView,
            double aspect)
        {
            throw new NotImplementedException();
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
