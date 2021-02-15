using Editor3D.Utilities;
using System;

namespace Editor3D
{
    internal class Camera
    {
        private Vector cameraPosition;
        private Vector observedPosition;
        private double nearPlane;
        private double farPlane;
        private double fieldOfView;
        private double aspect;

        private Matrix cameraRotationMatrix = Matrix.Unitary();

        internal Camera(Vector cameraPosition, Vector observedPosition, double nearPlane,
            double farPlane, double fieldOfView, double aspect)
        {
            this.cameraPosition = cameraPosition;
            this.observedPosition = observedPosition;
            this.nearPlane = nearPlane;
            this.farPlane = farPlane;
            this.fieldOfView = fieldOfView;
            this.aspect = aspect;
        }

        private Matrix GenerateViewMatrix()
        {
            Vector upWorldDirection = new Vector(0, 1, 0, 0);
            Vector forwardDirection = GetForwardDirection();
            Vector rightDirection = upWorldDirection.CrossProduct(forwardDirection).Normalize();
            Vector upDirection = forwardDirection.CrossProduct(rightDirection).Normalize();
            Matrix rotationMatrix = Matrix.Rotation(rightDirection, upDirection, forwardDirection);
            Matrix transformationMatrix = Matrix.Translation(cameraPosition.NegatedWithoutW());
            return rotationMatrix.MultipliedBy(transformationMatrix);
        }

        internal Matrix GetViewMatrix()
        {
            return GenerateViewMatrix();
        }

        internal Matrix GetProjectionMatrix()
        {
            return Matrix.Projection(nearPlane, farPlane, fieldOfView, aspect);
        }

        internal Vector GetForwardDirection()
        {
            return GetObservedPoint().DirectionTo(cameraPosition).Normalize();
        }

        private Vector GetObservedPoint()
        {
            return cameraRotationMatrix.MultipliedBy(observedPosition);
        }

        internal Vector GetPosition()
        {
            return cameraPosition;//cameraRotationMatrix.MultipliedBy(position);
        }

        internal void Rotate(int degrees, Axis axis)
        {
            cameraRotationMatrix.Rotate(degrees * Math.PI / 180, axis);
        }
    }
}
