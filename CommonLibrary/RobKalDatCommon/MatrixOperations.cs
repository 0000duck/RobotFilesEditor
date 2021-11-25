using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.RobKalDatCommon
{
    public static class MatrixOperations
    {
        public static double[,] MultiplyMatrix(double[,] m1, double[,] m2) // mnozenie macierzy 3x3 z 1x3
        {
            double[,] result = new double[,] { { m1[0,0] * m2[0,0] + m1[0,1] * m2[1,0] + m1[0,2] * m2[2,0]}, { m1[1, 0] * m2[0, 0] + m1[1, 1] * m2[1, 0] + m1[1, 2] * m2[2, 0] }, { m1[2, 0] * m2[0, 0] + m1[2, 1] * m2[1, 0] + m1[2, 2] * m2[2, 0] } };
            return result;
        }

        public static double[,] MultiplyMatrix3x3(double[,] m1, double[,] m2) // mnozenie macierze 3x3 z 3x3
        {
            double[,] result = new double[,]
            {
                { (m1[0,0] * m2[0,0] + m1[0,1] * m2[1,0] + m1[0,2] * m2[2,0]) , (m1[0,0] * m2[0,1] + m1[0,1] * m2[1,1] + m1[0,2] * m2[2,1]), (m1[0,0] * m2[0,2] + m1[0,1] * m2[1,2] + m1[0,2] * m2[2,2])},
                { (m1[1,0] * m2[0,0] + m1[1,1] * m2[1,0] + m1[1,2] * m2[2,0]) , (m1[1,0] * m2[0,1] + m1[1,1] * m2[1,1] + m1[1,2] * m2[2,1]), (m1[1,0] * m2[0,2] + m1[1,1] * m2[1,2] + m1[1,2] * m2[2,2])},
                { (m1[2,0] * m2[0,0] + m1[2,1] * m2[1,0] + m1[2,2] * m2[2,0]) , (m1[2,0] * m2[0,1] + m1[2,1] * m2[1,1] + m1[2,2] * m2[2,1]), (m1[2,0] * m2[0,2] + m1[2,1] * m2[1,2] + m1[2,2] * m2[2,2])},
            };
            
            return result;
        }
    }
}
