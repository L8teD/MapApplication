using CommonLib;
using CommonLib.Matrix;
using CommonLib.Params;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModellingErrorsLib3
{
    class GetMatrix
    {
        public static double[][] ErrorMatrix { get; private set; }
        public static double[][] AngleMatrix { get; private set; }
        public static double[][] MatrixOrientation { get; private set; }

        public static void CreateErrorMatrix(OmegaGyro omegaGyro, EarthModel earthModel)
        {
            ErrorMatrix = MatrixOperations.Zeros(6, 6);
            ErrorMatrix[0][1] = 1;

            ErrorMatrix[1][0] = Math.Pow(omegaGyro.Y, 2) + Math.Pow(omegaGyro.Z, 2) - Math.Pow(earthModel.shulerFrequency, 2);

            ErrorMatrix[1][2] = omegaGyro.Z_dot - omegaGyro.X * omegaGyro.Y;
            ErrorMatrix[1][3] = 2 * omegaGyro.Z;
            ErrorMatrix[1][4] = -(omegaGyro.Y_dot + omegaGyro.X * omegaGyro.Z);
            ErrorMatrix[1][5] = -2 * omegaGyro.Y;

            ErrorMatrix[2][3] = 1;

            ErrorMatrix[3][0] = -(omegaGyro.X * omegaGyro.Y + omegaGyro.Z_dot);
            ErrorMatrix[3][1] = -2 * omegaGyro.Z;
            ErrorMatrix[3][2] = Math.Pow(omegaGyro.X, 2) + Math.Pow(omegaGyro.Z, 2) - Math.Pow(earthModel.shulerFrequency, 2);
            ErrorMatrix[3][4] = omegaGyro.X_dot - omegaGyro.Y * omegaGyro.Z;
            ErrorMatrix[3][5] = 2 * omegaGyro.X;

            ErrorMatrix[4][5] = 1;

            ErrorMatrix[5][0] = omegaGyro.Y_dot - omegaGyro.X * omegaGyro.Z; 
            ErrorMatrix[5][1] = 2 * omegaGyro.Y;
            ErrorMatrix[5][2] = -(omegaGyro.X_dot - omegaGyro.Y * omegaGyro.Z);
            ErrorMatrix[5][3] = 2 * omegaGyro.X;
            ErrorMatrix[5][4] = 2 * Math.Pow(earthModel.shulerFrequency, 2) + Math.Pow(omegaGyro.X, 2) + Math.Pow(omegaGyro.Y, 2);

        }
        public static void CreateAnglesMatrix(double alfa, double betta, double gamma)
        {
            AngleMatrix = MatrixOperations.Zeros(6, 6);

            AngleMatrix[1][1] = gamma;
            AngleMatrix[1][2] = -betta;
            AngleMatrix[3][0] = -gamma;
            AngleMatrix[3][2] = alfa;
            AngleMatrix[5][0] = betta;
            AngleMatrix[5][1] = alfa;
        }
        public static void CreateMatrixOrientation(OmegaGyro omegaGyro)
        {
            MatrixOrientation = MatrixOperations.Create(3, 3);
            MatrixOrientation[0] = new double[] { 0, omegaGyro.Z, -omegaGyro.Y };
            MatrixOrientation[1] = new double[] { -omegaGyro.Z, 0, omegaGyro.X };
            MatrixOrientation[2] = new double[] { omegaGyro.Y, -omegaGyro.X, 0 };
        }
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

    }
}
