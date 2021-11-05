using CommonLib;
using CommonLib.Matrix;
using CommonLib.Params;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CommonLib.Types;
using static ModellingErrorsLib3.Types;

namespace ModellingErrorsLib3
{
    public class ErrorsModel
    {
        double[][] orientationAngles = new double[3][] { new double[1], new double[1], new double[1] };

        private double[][] angles_Dot = MatrixOperations.Zeros(3, 1);
        private double[][] X_Dot = MatrixOperations.Zeros(4, 1);

        public double[][] anglesErrors;
        public double[][] X;

        private void Model(InitErrors initErrors, Acceleration acceleration, OmegaGyro omegaGyro, EarthModel earthModel, Angles angles)
        {
            double[][] M = GetMatrix.CreateM(angles.heading, angles.pitch);
            double[][] orientationErrors = MatrixOperations.Product(MatrixOperations.Inverted(M), Converter.DegToRad(anglesErrors));

            orientationAngles[0][0] = orientationErrors[0][0] + angles_Dot[0][0];
            orientationAngles[1][0] = orientationErrors[1][0] + angles_Dot[1][0];
            orientationAngles[2][0] = orientationErrors[2][0] + angles_Dot[2][0];

            double[][] accelerationArray = MatrixOperations.Zeros(6, 1);
            accelerationArray[0][0] = acceleration.X;
            accelerationArray[1][0] = acceleration.Y;
            accelerationArray[2][0] = acceleration.Z;

            double[][] accelerationIncrementArray = MatrixOperations.Zeros(6, 1);
            accelerationIncrementArray[1][0] = initErrors.accelerationError.first;
            accelerationIncrementArray[3][0] = initErrors.accelerationError.second;
            accelerationIncrementArray[5][0] = initErrors.accelerationError.third;

            double[][] gyroIncrementArray = new double[][]
            {
                new double[] {initErrors.gyroError.first},
                new double[] {initErrors.gyroError.second},
                new double[] {initErrors.gyroError.third}
            };

            GetMatrix.CreateErrorMatrix(omegaGyro, earthModel);
            GetMatrix.CreateAnglesMatrix(orientationAngles[0][0], orientationAngles[1][0], orientationAngles[2][0]);
            GetMatrix.CreateMatrixOrientation(omegaGyro);

            angles_Dot = MatrixOperations.Sum(MatrixOperations.Product(GetMatrix.MatrixOrientation, orientationAngles), gyroIncrementArray);
            X_Dot = MatrixOperations.Sum(
                MatrixOperations.Sum(MatrixOperations.Product(GetMatrix.ErrorMatrix, X), MatrixOperations.Product(GetMatrix.AngleMatrix, accelerationArray)),
                accelerationIncrementArray);
        }
        public void ModellingErrors(InitErrors initErrors, Parameters parameters)
        {
            if (X == null || anglesErrors == null)
            {
                InitX(initErrors, parameters.point, parameters.omegaGyro, parameters.earthModel, parameters.velocity);
                InintAnglesError(initErrors);
            }

            Model(initErrors, parameters.acceleration, parameters.omegaGyro, parameters.earthModel, parameters.angles);
            IncrementX();
            IcrementAngle();
        }
        private void IncrementX()
        {
            MathTransformation.IncrementValue(ref X[0][0], X_Dot[0][0]);
            MathTransformation.IncrementValue(ref X[1][0], X_Dot[1][0]);
            MathTransformation.IncrementValue(ref X[2][0], X_Dot[2][0]);
            MathTransformation.IncrementValue(ref X[3][0], X_Dot[3][0]);
            MathTransformation.IncrementValue(ref X[4][0], X_Dot[4][0]);
            MathTransformation.IncrementValue(ref X[5][0], X_Dot[5][0]);
        }
        private void IcrementAngle()
        {
            MathTransformation.IncrementValue(ref anglesErrors[0][0], angles_Dot[0][0]);
            MathTransformation.IncrementValue(ref anglesErrors[1][0], angles_Dot[1][0]);
            MathTransformation.IncrementValue(ref anglesErrors[2][0], angles_Dot[2][0]);
        }
        private void InitX(InitErrors initErrors, Point point, OmegaGyro omegaGyro, EarthModel earthModel, Velocity velocity)
        {
            X = MatrixOperations.Zeros(6, 1);

            X[0][0] = initErrors.coordAccuracy.longitude * Math.Cos(point.lat);
            X[2][0] = initErrors.coordAccuracy.latitude;
            X[4][0] = initErrors.coordAccuracy.altitude;


            X[1][0] = initErrors.velocityAccuracy.east + (velocity.H / earthModel.R2 + omegaGyro.E * Math.Tan(point.lat)) * X[0][0] + omegaGyro.H * X[2][0];
            X[3][0] = initErrors.velocityAccuracy.north + velocity.H / earthModel.R1 *X[0][0];
            X[5][0] = initErrors.velocityAccuracy.H;
        }
        private void InintAnglesError(InitErrors initErrors)
        {
            anglesErrors = new double[][] {
                new double[] { Converter.DegToRad(initErrors.angleAccuracy.heading) },
                new double[] { Converter.DegToRad(initErrors.angleAccuracy.roll) },
                new double[] { Converter.DegToRad(initErrors.angleAccuracy.pitch) } };
        }
    }


}
