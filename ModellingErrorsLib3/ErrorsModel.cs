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
    public class ErrorsModel
    {
        Vector orientationAngles = Vector.Zero(3);

        Vector angles_Dot = Vector.Zero(3);
        Vector X_Dot = Vector.Zero(4);

        public Vector anglesErrors;
        public Vector X;

        private void Model(InitErrors initErrors, Acceleration acceleration, OmegaGyro omegaGyro, EarthModel earthModel, Angles angles)
        {
            Matrix M = Create.MatrixM(angles.heading, angles.pitch);
            Vector orientationErrors = !M * anglesErrors;

            orientationAngles[1] = orientationErrors[1] + angles_Dot[1];
            orientationAngles[2] = orientationErrors[2] + angles_Dot[2];
            orientationAngles[3] = orientationErrors[3] + angles_Dot[3];

            //anglesErrors = M * orientationAngles;

            Vector accelerationArray = Vector.Zero(6);
            accelerationArray[1] = acceleration.X;
            accelerationArray[2] = acceleration.Y;
            accelerationArray[3] = acceleration.Z;

            Vector accelerationIncrementArray = Vector.Zero(6);
            accelerationIncrementArray[2] = initErrors.accelerationError.first;
            accelerationIncrementArray[4] = initErrors.accelerationError.second;
            accelerationIncrementArray[6] = initErrors.accelerationError.third;

            Vector gyroIncrementArray = new Vector(initErrors.gyroError.first, initErrors.gyroError.second, initErrors.gyroError.third);


            Matrix ErrorMatrix = GetMatrix.CreateErrorMatrix(omegaGyro, earthModel);
            Matrix AngleMatrix = GetMatrix.CreateAnglesMatrix(orientationAngles[1], orientationAngles[2], orientationAngles[3]);
            Matrix MatrixOrientation = GetMatrix.CreateMatrixOrientation(omegaGyro);

            angles_Dot = (MatrixOrientation * orientationAngles) + gyroIncrementArray;

            X_Dot = ErrorMatrix * X + AngleMatrix * accelerationArray + accelerationIncrementArray;
        }
        public void ModellingErrors(InitErrors initErrors, Parameters parameters)
        {
            if (X == null || anglesErrors == null)
            {
                InitX(initErrors, parameters.point, parameters.omegaGyro, parameters.earthModel, parameters.velocity);
                InitAnglesError(initErrors);
            }

            Model(initErrors, parameters.acceleration, parameters.omegaGyro, parameters.earthModel, parameters.angles);
            IncrementX();
            IncrementAngle();
        }
        private void IncrementX()
        {
            
            X[1] = MathTransformation.IncrementValue(X[1], X_Dot[1]);
            X[2] = MathTransformation.IncrementValue(X[2], X_Dot[2]);
            X[3] = MathTransformation.IncrementValue(X[3], X_Dot[3]);
            X[4] = MathTransformation.IncrementValue(X[4], X_Dot[4]);
            X[5] = MathTransformation.IncrementValue(X[5], X_Dot[5]);
            X[6] = MathTransformation.IncrementValue(X[6], X_Dot[6]);
        }
        private void IncrementAngle()
        {
            //MathTransformation.IncrementValue(ref anglesErrors[0][0], angles_Dot[0][0]);
            //MathTransformation.IncrementValue(ref anglesErrors[1][0], angles_Dot[1][0]);
            //MathTransformation.IncrementValue(ref anglesErrors[2][0], angles_Dot[2][0]);
        }
        private void InitX(InitErrors initErrors, Point point, OmegaGyro omegaGyro, EarthModel earthModel, Velocity velocity)
        {
            X = Vector.Zero(6);

            X[1] = initErrors.coordAccuracy.longitude * Math.Cos(point.lat);
            X[3] = initErrors.coordAccuracy.latitude;
            X[5] = initErrors.coordAccuracy.altitude;


            X[2] = initErrors.velocityAccuracy.east + (velocity.H / earthModel.R2 + omegaGyro.E * Math.Tan(point.lat)) * X[1] + omegaGyro.H * X[3];
            X[4] = initErrors.velocityAccuracy.north + velocity.H / earthModel.R1 *X[1];
            X[6] = initErrors.velocityAccuracy.H;
        }
        private void InitAnglesError(InitErrors initErrors)
        {
            anglesErrors = new Vector(Converter.DegToRad(initErrors.angleAccuracy.heading),
                Converter.DegToRad(initErrors.angleAccuracy.roll), Converter.DegToRad(initErrors.angleAccuracy.pitch));
        }
    }


}
