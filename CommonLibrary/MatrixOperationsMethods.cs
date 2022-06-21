using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace CommonLibrary
{
    public static class MatrixOperationsMethods
    {
        public static Matrix<double> BuildHTMMatrix(IRobotPoint frame)
        {
            Matrix<double> frameRotMatZ = BuildRotationMatrix("Z", (frame as PointXYZABC).A);
            Matrix<double> frameRotMatY = BuildRotationMatrix("Y", (frame as PointXYZABC).B);
            Matrix<double> frameRotMatX = BuildRotationMatrix("X", (frame as PointXYZABC).C);
            Matrix<double> frameRotMatZYX = frameRotMatZ.Multiply(frameRotMatY).Multiply(frameRotMatX);

            Matrix<double> resultMatrix = Matrix<double>.Build.Dense(4, 4);
            resultMatrix[0, 0] = frameRotMatZYX[0, 0];
            resultMatrix[0, 1] = frameRotMatZYX[0, 1];
            resultMatrix[0, 2] = frameRotMatZYX[0, 2];
            resultMatrix[1, 0] = frameRotMatZYX[1, 0];
            resultMatrix[1, 1] = frameRotMatZYX[1, 1];
            resultMatrix[1, 2] = frameRotMatZYX[1, 2];
            resultMatrix[2, 0] = frameRotMatZYX[2, 0];
            resultMatrix[2, 1] = frameRotMatZYX[2, 1];
            resultMatrix[2, 2] = frameRotMatZYX[2, 2];

            resultMatrix[0, 3] = frame.X;
            resultMatrix[1, 3] = frame.Y;
            resultMatrix[2, 3] = frame.Z;
            resultMatrix[3, 0] = 0;
            resultMatrix[3, 1] = 0;
            resultMatrix[3, 2] = 0;
            resultMatrix[3, 3] = 1;

            return resultMatrix;

        }

        private static Matrix<double> BuildRotationMatrix(string axis, double rotationValueInDegress)
        {
            double rotationValueInRad = CommonLibrary.CommonMethods.ConvertToRadians(rotationValueInDegress);
            Matrix<double> resultMatrix = Matrix<double>.Build.Dense(3, 3);
            switch (axis)
            {
                case "X":
                    resultMatrix[0, 0] = 1;
                    resultMatrix[0, 1] = 0;
                    resultMatrix[0, 2] = 0;
                    resultMatrix[1, 0] = 0;
                    resultMatrix[1, 1] = Math.Cos(rotationValueInRad);
                    resultMatrix[1, 2] = -Math.Sin(rotationValueInRad);
                    resultMatrix[2, 0] = 0;
                    resultMatrix[2, 1] = Math.Sin(rotationValueInRad);
                    resultMatrix[2, 2] = Math.Cos(rotationValueInRad);
                    break;
                case "Y":
                    resultMatrix[0, 0] = Math.Cos(rotationValueInRad);
                    resultMatrix[0, 1] = 0;
                    resultMatrix[0, 2] = Math.Sin(rotationValueInRad);
                    resultMatrix[1, 0] = 0;
                    resultMatrix[1, 1] = 1;
                    resultMatrix[1, 2] = 0;
                    resultMatrix[2, 0] = -Math.Sin(rotationValueInRad);
                    resultMatrix[2, 1] = 0;
                    resultMatrix[2, 2] = Math.Cos(rotationValueInRad);
                    break;
                case "Z":
                    resultMatrix[0, 0] = Math.Cos(rotationValueInRad);
                    resultMatrix[0, 1] = -Math.Sin(rotationValueInRad);
                    resultMatrix[0, 2] = 0;
                    resultMatrix[1, 0] = Math.Sin(rotationValueInRad);
                    resultMatrix[1, 1] = Math.Cos(rotationValueInRad);
                    resultMatrix[1, 2] = 0;
                    resultMatrix[2, 0] = 0;
                    resultMatrix[2, 1] = 0;
                    resultMatrix[2, 2] = 1;
                    break;
            }
            return resultMatrix;
        }
    }
}
