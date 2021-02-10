using System;

namespace Editor3D.Utilities
{
    internal class Matrix
    {
        internal static Matrix Translation(object v)
        {
            throw new NotImplementedException();
        }

        internal static Matrix Rotation(Vector rightDirection, Vector upDirection, Vector forwardDirection)
        {
            throw new NotImplementedException();
        }

        internal static Matrix Projection(double nearPlane, double farPlane, double fieldOfView, double aspect)
        {
            throw new NotImplementedException();
        }

        internal Matrix MultipliedBy(Matrix transformationMatrix)
        {
            throw new NotImplementedException();
        }
    }
}