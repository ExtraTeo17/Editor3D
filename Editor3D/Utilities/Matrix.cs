using System;

namespace Editor3D.Utilities
{
    internal class Matrix
    {
        private double[,] matrix = new double[4, 4];

        internal static Matrix Translation(Vector translation)
        {
            Matrix translationMatrix = Unitary();
            translationMatrix.FillColumn(3, translation);
            return translationMatrix;
        }

        internal static Matrix Rotation(Vector rightDirection, Vector upDirection, Vector forwardDirection)
        {
            Matrix rotationMatrix = new Matrix();
            rotationMatrix.FillRow(0, rightDirection);
            rotationMatrix.FillRow(1, upDirection);
            rotationMatrix.FillRow(2, forwardDirection);
            rotationMatrix.matrix[3, 3] = 1;
            return rotationMatrix;
        }

        private void FillRow(int height, Vector vector)
        {
            matrix[0, height] = vector.x;
            matrix[1, height] = vector.y;
            matrix[2, height] = vector.z;
            matrix[3, height] = vector.w;
        }

        private void FillColumn(int width, Vector vector)
        {
            matrix[width, 0] = vector.x;
            matrix[width, 1] = vector.y;
            matrix[width, 2] = vector.z;
            matrix[width, 3] = vector.w;
        }

        internal static Matrix Projection(double nearPlane, double farPlane, double fieldOfView, double aspect)
        {
            Matrix projectionMatrix = new Matrix();
            double ctg = 1 / Math.Tan(fieldOfView / 2);
            projectionMatrix.matrix[0, 0] = ctg / aspect;
            projectionMatrix.matrix[1, 1] = ctg;
            projectionMatrix.matrix[2, 2] = (farPlane + nearPlane) / (farPlane - nearPlane);
            projectionMatrix.matrix[3, 2] = ((-2) * farPlane * nearPlane) / (farPlane - nearPlane);
            projectionMatrix.matrix[2, 3] = 1;
            return projectionMatrix;
        }

        internal static Matrix Unitary()
        {
            Matrix unitaryMatrix = new Matrix();
            unitaryMatrix.FillDiagonal(1, 1, 1, 1);
            return unitaryMatrix;
        }

        private void FillDiagonal(double v1, double v2, double v3, double v4)
        {
            matrix[0, 0] = v1;
            matrix[1, 1] = v2;
            matrix[2, 2] = v3;
            matrix[3, 3] = v4;
        }

        internal Matrix MultipliedBy(Matrix matrix)
        {
            Matrix multiplied = new Matrix();
            for (int i = 0; i < 4; ++i)
            {
                for (int j = 0; j < 4; ++j)
                {
                    multiplied.matrix[i, j] = (this.matrix[0, j] * matrix.matrix[i, 0]) +
                        (this.matrix[1, j] * matrix.matrix[i, 1]) +
                        (this.matrix[2, j] * matrix.matrix[i, 2]) +
                        (this.matrix[3, j] * matrix.matrix[i, 3]);
                }
            }
            return multiplied;
        }
    }
}
