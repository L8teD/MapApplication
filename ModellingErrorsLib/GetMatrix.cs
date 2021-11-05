using CommonLib;
using CommonLib.Matrix;
using CommonLib.Params;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModellingErrorsLib.Matrix
{
    class GetMatrix
    {

        public static double[][] Matrix1 { get; private set; }
        public static double[][] Matrix2 { get; private set; }
        public static double[][] Matrix3 { get; private set; }
        public static double[][] MatrixOrientation { get; private set; }

        public static double[][] CreateM(double heading, double pitch)
        {
            double[][] M = MatrixOperations.Create(3, 3);
            double headingReverse = MathTransformation.ReverseAngle(heading);

            M[0][0] = Math.Sin(headingReverse) * Math.Tan(pitch);
            M[0][1] = Math.Cos(headingReverse) * Math.Tan(pitch);
            M[0][2] = -1;
            M[1][0] = Math.Cos(headingReverse);
            M[1][1] = -Math.Sin(headingReverse);
            M[1][2] = 0;
            M[2][0] = Math.Sin(headingReverse) / Math.Cos(pitch);
            M[2][1] = Math.Cos(-headingReverse) / Math.Cos(pitch);
            M[2][2] = 0;

            return M;
        }
        public static void CreateMatrix1(OmegaGyro omegaGyro, EarthModel earthModel)
        {
            Matrix1 = MatrixOperations.Create(4, 4);
            Matrix1[0] = new double[] { 0, 1, 0, 0 };

            Matrix1[1][0] = Math.Pow(omegaGyro.Y, 2) + Math.Pow(omegaGyro.Z, 2) - Math.Pow(earthModel.shulerFrequency, 2);
            Matrix1[1][1] = 0;
            Matrix1[1][2] = omegaGyro.Z_dot - omegaGyro.X * omegaGyro.Y;
            Matrix1[1][3] = 2 * omegaGyro.Z;

            Matrix1[2] = new double[] { 0, 0, 0, 1 };

            Matrix1[3][0] = -(omegaGyro.X * omegaGyro.Y + omegaGyro.Z_dot);
            Matrix1[3][1] = -2 * omegaGyro.Z;
            Matrix1[3][2] = Math.Pow(omegaGyro.X, 2) + Math.Pow(omegaGyro.Z, 2) - Math.Pow(earthModel.shulerFrequency, 2);
            Matrix1[3][3] = 0;
        }
        public static void CreateMatrix2(double alfa, double betta, double gamma)
        {
            Matrix2 = MatrixOperations.Zeros(4, 4);
            Matrix2[1] = new double[] { 0, gamma, -betta, 0 };
            Matrix2[3] = new double[] { -gamma, 0, alfa, 0 };
        }
        public static void CreateMatrix3(OmegaGyro omegaGyro)
        {
            Matrix3 = MatrixOperations.Zeros(4, 4);

            Matrix3[1][0] = -(omegaGyro.X * omegaGyro.Z + omegaGyro.Y_dot);
            Matrix3[1][1] = -2 * omegaGyro.Y;

            Matrix3[3][0] = omegaGyro.X_dot - omegaGyro.Y * omegaGyro.Z;
            Matrix3[3][1] = 2 * omegaGyro.X;
        }
        public static void CreateMatrixOrientation(OmegaGyro omegaGyro)
        {
            MatrixOrientation = MatrixOperations.Create(3, 3);
            MatrixOrientation[0] = new double[] { 0, omegaGyro.Z, -omegaGyro.Y };
            MatrixOrientation[1] = new double[] { -omegaGyro.Z, 0, omegaGyro.X };
            MatrixOrientation[2] = new double[] { omegaGyro.Y, -omegaGyro.X, 0 };
        }

    }
}
