using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLib;
using CommonLib.Matrix;
using CommonLib.Params;
using ModellingErrorsLib.Matrix;
using static CommonLib.Types;
using static ModellingErrorsLib.Types;

namespace ModellingErrorsLib
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

            double[][] accelerationArray = new double[][]
            {
                new double[] { acceleration.X },
                new double[] { acceleration.Y },
                new double[] { acceleration.Z },
                new double[] { 0 }
            };
            double[][] accelerationIncrementArray = new double[][]
            {
                new double[] { 0 },
                new double[] { initErrors.accelerationError.first },
                new double[] { 0 },
                new double[] { initErrors.accelerationError.second }
            };
            double[][] gyroIncrementArray = new double[][]
            {
                new double[] {initErrors.gyroError.first},
                new double[] {initErrors.gyroError.second},
                new double[] {initErrors.gyroError.third}
            };

            double[][] vectorStateInVerticalChannel = new double[][]
            {
                new double[] { initErrors.sateliteErrorCoord },
                new double[] { initErrors.sateliteErrorVelocity },
                new double[] { 0 },
                new double[] { 0 }
            };
            CreateErrorMatrixies(omegaGyro, earthModel);
            angles_Dot = MatrixOperations.Sum(MatrixOperations.Product(GetMatrix.MatrixOrientation, orientationAngles), gyroIncrementArray);

            double[][] mat1_X = MatrixOperations.Product(GetMatrix.Matrix1, X);
            double[][] mat2_ACC = MatrixOperations.Product(GetMatrix.Matrix2, accelerationArray);
            double[][] mat3_X_Vert = MatrixOperations.Product(GetMatrix.Matrix3, vectorStateInVerticalChannel);
            X_Dot = MatrixOperations.Sum(MatrixOperations.Sum(MatrixOperations.Sum(mat1_X,
                mat2_ACC), accelerationIncrementArray), mat3_X_Vert);
        }
        private void CreateErrorMatrixies(OmegaGyro omegaGyro, EarthModel earthModel)
        {
            GetMatrix.CreateMatrix1(omegaGyro, earthModel);
            GetMatrix.CreateMatrix2(orientationAngles[0][0], orientationAngles[1][0], orientationAngles[2][0]);
            GetMatrix.CreateMatrix3(omegaGyro);
            GetMatrix.CreateMatrixOrientation(omegaGyro);

        }
        public void ModellingErrors(InitErrors initErrors, Parameters parameters)
        {
            if (X == null || anglesErrors == null)
            {
                InitX(initErrors, parameters.point, parameters.omegaGyro);
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
        }
        private void IcrementAngle()
        {
            MathTransformation.IncrementValue(ref anglesErrors[0][0], angles_Dot[0][0]);
            MathTransformation.IncrementValue(ref anglesErrors[1][0], angles_Dot[1][0]);
            MathTransformation.IncrementValue(ref anglesErrors[2][0], angles_Dot[2][0]);
        }
        private void InitX(InitErrors initErrors, Point point, OmegaGyro omegaGyro)
        {
            X = MatrixOperations.Zeros(4, 1);

            X[0][0] = initErrors.coordAccuracy.longitude * Math.Cos(point.lat);
            X[2][0] = initErrors.coordAccuracy.latitude;
            X[1][0] = initErrors.velocityAccuracy.east + omegaGyro.E * Math.Tan(point.lat) * X[0][0] + omegaGyro.H * X[2][0];
            X[3][0] = initErrors.velocityAccuracy.north;
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
