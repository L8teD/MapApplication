using CommonLib;
using CommonLib.Params;
using MyMatrix;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CommonLib.Types;
using static ModellingErrorsLib3.Types;

namespace EstimateLib
{
    public class KalmanModel
    {
        public Vector X;
        public Vector X_dot;
        public Matrix F;
        public Matrix G;
        public Vector W;
        Vector W_withoutNoise;
        public Matrix H;
        public Vector Z;
        public Matrix P;
        public Vector X_estimate;
        Vector X_previous;
        Matrix eyeMatrix;
        Matrix F_discrete;
        Matrix G_discrete;
        Matrix Q;
        Matrix R;
        Matrix K;
        Matrix S;
        Vector orientationAngles;
        public Vector anglesErrors;

        private CoordAccuracy coordAccuracy;
        private VelocityAccuracy velocityAccuracy;

        private void InitX(InitErrors initErrors, Point point, OmegaGyro omegaGyro, EarthModel earthModel, Velocity velocity, Angles angles, bool Init = false)
        {
            X = Vector.Zero(21);

            if (Init)
            {
                X[1] = initErrors.coordAccuracy.longitude * Math.Cos(point.lat);
                X[2] = initErrors.coordAccuracy.latitude;
                X[3] = initErrors.coordAccuracy.altitude;

                X[4] = initErrors.velocityAccuracy.east + (velocity.H / earthModel.R2 + omegaGyro.E * Math.Tan(point.lat)) * X[1] + omegaGyro.H * X[3];
                X[5] = initErrors.velocityAccuracy.north + velocity.H / earthModel.R1 * X[1];
                X[6] = initErrors.velocityAccuracy.H;

                coordAccuracy = new CoordAccuracy();
                coordAccuracy.longitude = initErrors.coordAccuracy.longitude;
                coordAccuracy.latitude = initErrors.coordAccuracy.latitude;
                coordAccuracy.altitude = initErrors.coordAccuracy.altitude;

                velocityAccuracy = new VelocityAccuracy();
                velocityAccuracy.east = initErrors.velocityAccuracy.east;
                velocityAccuracy.north = initErrors.velocityAccuracy.north;
                velocityAccuracy.H = initErrors.velocityAccuracy.H;
            }
            else
            {
                coordAccuracy.longitude += X_dot[1];
                coordAccuracy.latitude += X_dot[2];
                coordAccuracy.altitude += X_dot[3];

                velocityAccuracy.east += X_dot[4];
                velocityAccuracy.north += X_dot[5];
                velocityAccuracy.H += X_dot[6];

                X[1] = coordAccuracy.longitude * Math.Cos(point.lat);
                X[2] = coordAccuracy.latitude;
                X[3] = coordAccuracy.altitude;


                X[4] = velocityAccuracy.east + (velocity.H / earthModel.R2 + omegaGyro.E * Math.Tan(point.lat)) * X[1] + omegaGyro.H * X[3];
                X[5] = velocityAccuracy.north + velocity.H / earthModel.R1 * X[1];
                X[6] = velocityAccuracy.H;
            }

            Matrix M = Create.MatrixM(angles.heading, angles.pitch);

            Vector orientationErrors = !M * anglesErrors;

            orientationAngles[1] = orientationErrors[1] + X_dot[7];
            orientationAngles[2] = orientationErrors[2] + X_dot[8];
            orientationAngles[3] = orientationErrors[3] + X_dot[9];

            anglesErrors = M * orientationAngles;


            X[7] = initErrors.angleAccuracy.heading;
            X[8] = initErrors.angleAccuracy.roll;
            X[9] = initErrors.angleAccuracy.pitch;

            X[10] = initErrors.gyroError.first;
            X[11] = initErrors.gyroError.second;
            X[12] = initErrors.gyroError.third;

            X[13] = 2E-5;
            X[14] = 2E-5;
            X[15] = 2E-5;

            X[16] = initErrors.accelerationError.first;
            X[17] = initErrors.accelerationError.second;
            X[18] = initErrors.accelerationError.third;

            X[19] = 9.81 * 3E-6;
            X[20] = 9.81 * 3E-6;
            X[21] = 9.81 * 3E-6;
        }
        private void InitF(OmegaGyro omegaGyro, EarthModel earthModel, Acceleration acceleration, Matrix C)
        {
            F = Matrix.Zero(21);

            F[1,3] = 1;
            F[2,4] = 1;
            F[3,5] = 1;

            F[4,1] = -Math.Pow(earthModel.shulerFrequency, 2) + Math.Pow(omegaGyro.N, 2) + Math.Pow(omegaGyro.H, 2);
            F[4,2] = omegaGyro.Z_dot - omegaGyro.E * omegaGyro.N;
            F[4,3] = -(omegaGyro.Y_dot + omegaGyro.E * omegaGyro.H);
            F[4,5] = 2 * omegaGyro.H;
            F[4,6] = -2 * omegaGyro.N;

            F[4,8] = acceleration.H;
            F[4,9] = -acceleration.N;

            F[4,16] = C[1,1];
            F[4,17] = C[1,2];
            F[4,18] = C[1,3];
            F[4,19] = C[1,1] * acceleration.X;
            F[4,20] = C[1,2] * acceleration.Y;
            F[4,21] = C[1,3] * acceleration.Z;

            F[5,1] = -(omegaGyro.Z_dot - omegaGyro.E * omegaGyro.N);
            F[5,2] = -Math.Pow(earthModel.shulerFrequency, 2) + Math.Pow(omegaGyro.E, 2) + Math.Pow(omegaGyro.H, 2);
            F[5,3] = omegaGyro.X_dot - omegaGyro.N * omegaGyro.H;
            F[5,4] = -2 * omegaGyro.H;
            F[5,6] = 2 * omegaGyro.E;

            F[5,7] = -acceleration.H;
            F[5,9] = acceleration.E;

            F[5,16] = C[2,1];
            F[5,17] = C[2,2];
            F[5,18] = C[2,3];
            F[5,19] = C[2,1] * acceleration.X;
            F[5,20] = C[2,2] * acceleration.Y;
            F[5,21] = C[2,3] * acceleration.Z;

            F[6,1] = omegaGyro.Y_dot - omegaGyro.E * omegaGyro.H;
            F[6,2] = -(omegaGyro.X_dot + omegaGyro.N * omegaGyro.H);
            F[6,3] = 2 * Math.Pow(earthModel.shulerFrequency, 2) + Math.Pow(omegaGyro.E, 2) + Math.Pow(omegaGyro.N, 2);
            F[6,4] = 2 * omegaGyro.N;
            F[6,5] = 2 * omegaGyro.E;

            F[6,7] = acceleration.N;
            F[6,8] = acceleration.E;

            F[6,15] = C[3,1];
            F[6,16] = C[3,2];
            F[6,17] = C[3,3];
            F[6,18] = C[3,1] * acceleration.X;
            F[6,19] = C[3,2] * acceleration.Y;
            F[6,20] = C[3,3] * acceleration.Z;

            F[7,7] = omegaGyro.H;
            F[7,8] = -omegaGyro.N;

            F[7,10] = C[1,1];
            F[7,11] = C[1,2];
            F[7,12] = C[1,3];
            F[7,13] = C[1,1] * omegaGyro.X;
            F[7,14] = C[1,2] * omegaGyro.Y;
            F[7,15] = C[1,3] * omegaGyro.Z;

            F[8,7] = -omegaGyro.H;
            F[8,9] = omegaGyro.E;

            F[8,10] = C[2,1];
            F[8,11] = C[2,2];
            F[8,12] = C[2,3];
            F[8,13] = C[2,1] * omegaGyro.X;
            F[8,14] = C[2,2] * omegaGyro.Y;
            F[8,15] = C[2,3] * omegaGyro.Z;

            F[9,7] = omegaGyro.N;
            F[9,8] = omegaGyro.E;

            F[9,10] = C[3,1];
            F[9,11] = C[3,2];
            F[9,12] = C[3,3];
            F[9,13] = C[3,1] * omegaGyro.X;
            F[9,14] = C[3,2] * omegaGyro.Y;
            F[9,15] = C[3,3] * omegaGyro.Z;
        }
        private void InitG(Matrix C)
        {
            G = Matrix.Zero(21, 6);
            G[4, 4] = C[1, 1];
            G[4, 5] = C[1, 2];
            G[4, 6] = C[1, 3];

            G[5, 4] = C[2, 1];
            G[5, 5] = C[2, 2];
            G[5, 6] = C[2, 3];

            G[6, 4] = C[3, 1];
            G[6, 5] = C[3, 2];
            G[6, 6] = C[3, 3];

            G[7, 1] = C[1, 1];
            G[7, 2] = C[1, 2];
            G[7, 3] = C[1, 3]; 

            G[8, 1] = C[2, 1];
            G[8, 2] = C[2, 2];
            G[8, 3] = C[2, 3];

            G[9, 1] = C[3, 1];
            G[9, 2] = C[3, 2];
            G[9, 3] = C[3, 3];
        }
        private void InitW(InitErrors initErrors)
        {
            double gyro_noise = 0.05 * initErrors.gyroError.first;
            double acc_noise = 9.78 * 0.05 * initErrors.accelerationError.first;
            Random random = new Random();

            W = Vector.Zero(6);
            W[1] = gyro_noise * random.NextDouble();
            W[2] = gyro_noise * random.NextDouble();
            W[3] = gyro_noise * random.NextDouble();
            W[4] = acc_noise * random.NextDouble();
            W[5] = acc_noise * random.NextDouble();
            W[6] = acc_noise * random.NextDouble();
            
            W_withoutNoise = Vector.Zero(6);
            W_withoutNoise[1] = gyro_noise;
            W_withoutNoise[2] = gyro_noise;
            W_withoutNoise[3] = gyro_noise;
            W_withoutNoise[4] = acc_noise;
            W_withoutNoise[5] = acc_noise;
            W_withoutNoise[6] = acc_noise;
        }
        private void InitH(Point point, EarthModel earth, OmegaGyro omegaGyro, Velocity velocity)
        {
            H = Matrix.Zero(6, 21);

            H[1,1] = 1.0 / (earth.R2 * Math.Cos(point.lat));
            H[2,2] = 1.0 / earth.R1;
            H[3,3] = 1.0;
            H[4,1] = velocity.H / earth.R2 + omegaGyro.E * Math.Tan(point.lat);
            H[4,2] = -omegaGyro.H;
            H[4,4] = 1.0;
            H[5,1] = -velocity.H / earth.R1;
            H[5,5] = 1.0;
            H[6,6] = 1.0;

        }
        private void InitZ(Point point, Velocity velocity, EarthModel earth)
        {
            Random random = new Random();
            Z = Vector.Zero(6);

            double[] _estimatedParams = new double[]
            {
                 point.lon * earth.R2,
                 point.lat * earth.R1,
                 point.alt,
                 velocity.E,
                 velocity.N,
                 velocity.H,
            };
            Vector estimatedParams = new Vector(_estimatedParams);
            Vector Z_ins = estimatedParams + H * X;
            double[] _snsErrors = new double[] {
                5,
                5,
                5,
                0.05,
                0.05,
                0.05
            };
            Vector snsErrors = new Vector(_snsErrors);
            double[] _Z_sns = new double[]
            {
                point.lon + snsErrors[1] * random.NextDouble(),
                point.lat + snsErrors[2] * random.NextDouble(),
                point.alt + snsErrors[3] * random.NextDouble(),
                velocity.E + snsErrors[4] * random.NextDouble(),
                velocity.N + snsErrors[5] * random.NextDouble(),
                velocity.H + snsErrors[6] * random.NextDouble()
            };
            Vector Z_sns = new Vector(_Z_sns);
            Z = Z_ins - Z_sns;

            R = snsErrors.Diag() ^ 2;
        }
        private void InitAnglesError(InitErrors initErrors)
        {
            orientationAngles = Vector.Zero(3);
            anglesErrors = new Vector(Converter.DegToRad(initErrors.angleAccuracy.heading),
                  Converter.DegToRad(initErrors.angleAccuracy.roll), Converter.DegToRad(initErrors.angleAccuracy.pitch));
        }
        private void IncrementX()
        {
            X[1] = MathTransformation.IncrementValue(X[1], X_dot[1]);
            X[2] = MathTransformation.IncrementValue(X[2], X_dot[2]);
            X[3] = MathTransformation.IncrementValue(X[3], X_dot[3]);
            X[4] = MathTransformation.IncrementValue(X[4], X_dot[4]);
            X[5] = MathTransformation.IncrementValue(X[5], X_dot[5]);
            X[6] = MathTransformation.IncrementValue(X[6], X_dot[6]);
        }
        private void IncrementAngle()
        {
            X[7] = MathTransformation.IncrementValue(X[7], X_dot[7]);
            X[8] = MathTransformation.IncrementValue(X[8], X_dot[8]);
            X[9] = MathTransformation.IncrementValue(X[9], X_dot[9]);
        }
        private void Kalman()
        {
            eyeMatrix = Matrix.Identity(21);
            F_discrete = eyeMatrix * F + F ^ 2;
            
            if (P == null)
            {
                P = X.Diag() ^ 2;
            }

            else
            {
                S = F_discrete * P * ~F_discrete + G_discrete * Q * ~G_discrete;
                K = S * ~H * !(H * S * ~H + R);
                P = (eyeMatrix - K * H) * S;
            }
            G_discrete = (eyeMatrix + F * 0.5) * G;

            Q = W_withoutNoise.Diag() ^ 2;
            if (X_estimate == null)
            {
                X_estimate = X.Dublicate();
            }
            else
            {
                X_estimate = F_discrete * X_previous + K * (Z - H * F_discrete * X_previous);
            }


            X_previous = X_estimate.Dublicate();
        }
        public void Model(InitErrors initErrors, Parameters parameters, Matrix C)
        {
            if (X == null)
            {
                X_dot = Vector.Zero(21);
                InitAnglesError(initErrors);
                InitX(initErrors, parameters.point, parameters.omegaGyro, parameters.earthModel, parameters.velocity, parameters.angles, true);

            }
            else
            {
                InitX(initErrors, parameters.point, parameters.omegaGyro, parameters.earthModel, parameters.velocity, parameters.angles);
            }

            
            InitF(parameters.omegaGyro, parameters.earthModel, parameters.acceleration, C);
            InitG(C);
            InitW(initErrors);


            X_dot = F * X + G * W;

            InitH(parameters.point, parameters.earthModel, parameters.omegaGyro, parameters.velocity);
            InitZ(parameters.point, parameters.velocity, parameters.earthModel);
            
            Kalman();
            //IncrementX();
            //IncrementAngle();
        }
    }
}
