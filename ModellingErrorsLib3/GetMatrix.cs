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

            ErrorMatrix[1][0] = Math.Pow(omegaGyro.N, 2) + Math.Pow(omegaGyro.H, 2) - Math.Pow(earthModel.shulerFrequency, 2);

            ErrorMatrix[1][2] = omegaGyro.Z_dot - omegaGyro.E * omegaGyro.N;
            ErrorMatrix[1][3] = 2 * omegaGyro.H;
            ErrorMatrix[1][4] = -(omegaGyro.Y_dot + omegaGyro.E * omegaGyro.H);
            ErrorMatrix[1][5] = -2 * omegaGyro.N;

            ErrorMatrix[2][3] = 1;

            ErrorMatrix[3][0] = -(omegaGyro.E * omegaGyro.N + omegaGyro.Z_dot);
            ErrorMatrix[3][1] = -2 * omegaGyro.Z;
            ErrorMatrix[3][2] = Math.Pow(omegaGyro.E, 2) + Math.Pow(omegaGyro.H, 2) - Math.Pow(earthModel.shulerFrequency, 2);
            ErrorMatrix[3][4] = omegaGyro.X_dot - omegaGyro.N * omegaGyro.H;
            ErrorMatrix[3][5] = 2 * omegaGyro.E;

            ErrorMatrix[4][5] = 1;

            ErrorMatrix[5][0] = omegaGyro.Y_dot - omegaGyro.E * omegaGyro.H; 
            ErrorMatrix[5][1] = 2 * omegaGyro.N;
            ErrorMatrix[5][2] = -(omegaGyro.X_dot - omegaGyro.N * omegaGyro.H);
            ErrorMatrix[5][3] = 2 * omegaGyro.E;
            ErrorMatrix[5][4] = 2 * Math.Pow(earthModel.shulerFrequency, 2) + Math.Pow(omegaGyro.E, 2) + Math.Pow(omegaGyro.N, 2);

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
            MatrixOrientation[0] = new double[] { 0, omegaGyro.H, -omegaGyro.N };
            MatrixOrientation[1] = new double[] { -omegaGyro.H, 0, omegaGyro.E };
            MatrixOrientation[2] = new double[] { omegaGyro.N, -omegaGyro.E, 0 };
        }
    }
}
