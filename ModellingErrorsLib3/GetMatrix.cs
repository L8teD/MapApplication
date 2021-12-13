using CommonLib;
using CommonLib.Params;
using MyMatrix;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModellingErrorsLib3
{
    class GetMatrix
    {
        //public static double[][] ErrorMatrix { get; private set; }
        //public static double[][] AngleMatrix { get; private set; }
        //public static double[][] MatrixOrientation { get; private set; }

        public static Matrix CreateErrorMatrix(OmegaGyro omegaGyro, EarthModel earthModel)
        {
            Matrix ErrorMatrix = Matrix.Zero(6);
            ErrorMatrix[1,2] = 1;
                         
            ErrorMatrix[2,1] = Math.Pow(omegaGyro.N, 2) + Math.Pow(omegaGyro.H, 2) - Math.Pow(earthModel.shulerFrequency, 2);
                         
            ErrorMatrix[2,3] = omegaGyro.Z_dot - omegaGyro.E * omegaGyro.N;
            ErrorMatrix[2,4] = 2 * omegaGyro.H;
            ErrorMatrix[2,5] = -(omegaGyro.Y_dot + omegaGyro.E * omegaGyro.H);
            ErrorMatrix[2,6] = -2 * omegaGyro.N;
                         
            ErrorMatrix[3,4] = 1;
                         
            ErrorMatrix[4,1] = -(omegaGyro.E * omegaGyro.N + omegaGyro.Z_dot);
            ErrorMatrix[4,2] = -2 * omegaGyro.Z;
            ErrorMatrix[4,3] = Math.Pow(omegaGyro.E, 2) + Math.Pow(omegaGyro.H, 2) - Math.Pow(earthModel.shulerFrequency, 2);
            ErrorMatrix[4,5] = omegaGyro.X_dot - omegaGyro.N * omegaGyro.H;
            ErrorMatrix[4,6] = 2 * omegaGyro.E;
                         
            ErrorMatrix[5,6] = 1;
                         
            ErrorMatrix[6,1] = omegaGyro.Y_dot - omegaGyro.E * omegaGyro.H; 
            ErrorMatrix[6,2] = 2 * omegaGyro.N;
            ErrorMatrix[6,3] = -(omegaGyro.X_dot - omegaGyro.N * omegaGyro.H);
            ErrorMatrix[6,4] = 2 * omegaGyro.E;
            ErrorMatrix[6,5] = 2 * Math.Pow(earthModel.shulerFrequency, 2) + Math.Pow(omegaGyro.E, 2) + Math.Pow(omegaGyro.N, 2);

            return ErrorMatrix;

        }
        public static Matrix CreateAnglesMatrix(double alfa, double betta, double gamma)
        {
            Matrix AngleMatrix = Matrix.Zero(6);

            AngleMatrix[2,2] = gamma;
            AngleMatrix[2,3] = -betta;
            AngleMatrix[4,1] = -gamma;
            AngleMatrix[4,3] = alfa;
            AngleMatrix[6,1] = betta;
            AngleMatrix[6,2] = alfa;

            return AngleMatrix;
        }
        public static Matrix CreateMatrixOrientation(OmegaGyro omegaGyro)
        {
            Matrix MatrixOrientation = Matrix.Zero(3);
            MatrixOrientation[1, 2] = omegaGyro.H;
            MatrixOrientation[1, 3] = omegaGyro.N;
            MatrixOrientation[2, 1] = -omegaGyro.H;
            MatrixOrientation[2, 3] = omegaGyro.E;
            MatrixOrientation[3, 1] = omegaGyro.N;
            MatrixOrientation[3, 2] = -omegaGyro.E;

            return MatrixOrientation;
        }
    }
}
