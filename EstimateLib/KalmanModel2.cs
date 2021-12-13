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
    public class KalmanModel2
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
        /// <summary>
        /// heading, pitch, roll
        /// </summary>
        public Vector orientationAngles;
        /// <summary>
        /// alfa, betta, gamma
        /// </summary>
        public Vector anglesErrors;

        private CoordAccuracy coordAccuracy;
        private VelocityAccuracy velocityAccuracy;

        private void InitX(InitErrors initErrors, Point point,AbsoluteOmega absOmega, EarthModel earthModel, Velocity velocity, Angles angles, bool Init = false)
        {
            X = Vector.Zero(19);

            if (Init)
            {
                coordAccuracy = new CoordAccuracy();
                coordAccuracy.longitude = initErrors.coordAccuracy.longitude;
                coordAccuracy.latitude = initErrors.coordAccuracy.latitude;
                //coordAccuracy.altitude = initErrors.coordAccuracy.altitude;

                velocityAccuracy = new VelocityAccuracy();
                velocityAccuracy.east = initErrors.velocityAccuracy.east;
                velocityAccuracy.north = initErrors.velocityAccuracy.north;
                //velocityAccuracy.H = initErrors.velocityAccuracy.H;
            }
            else
            {
                coordAccuracy.longitude += X_dot[1];
                coordAccuracy.latitude += X_dot[2];
                //coordAccuracy.altitude += X_dot[2][0];

                velocityAccuracy.east += X_dot[3];
                velocityAccuracy.north += X_dot[4];
                //velocityAccuracy.H += X_dot[5][0];
            }
            X[1] = coordAccuracy.longitude;
            X[2] = coordAccuracy.latitude;

            X[3] = velocityAccuracy.east + (velocity.H / earthModel.R2 + absOmega.E * Math.Tan(point.lat)) * X[1] + absOmega.H * X[2];
            X[4] = velocityAccuracy.north + velocity.H / earthModel.R1 * X[1];

            Matrix M = Create.MatrixM(angles.heading, angles.pitch);

            Vector orientationErrors = !M * anglesErrors; 

            double alfa = orientationErrors[1] + X_dot[5];
            double betta = orientationErrors[2] + X_dot[6];
            double gamma = orientationErrors[3] + X_dot[7];

            orientationAngles[1] = alfa - X[2] / earthModel.R1;
            orientationAngles[2] = betta + X[1] / earthModel.R2;
            orientationAngles[3] = gamma + X[1] * Math.Tan(point.lat) / earthModel.R1;

                //anglesErrors = M * orientationAngles;
            X[5] = orientationAngles[1];
            X[6] = orientationAngles[2];
            X[7] = orientationAngles[3];

            X[8] = initErrors.gyroError.first;
            X[9] = initErrors.gyroError.second;
            X[10] = initErrors.gyroError.third;

            X[11] = 2E-5;
            X[12] = 2E-5;
            X[13] = 2E-5;

            X[14] = initErrors.accelerationError.first;
            X[15] = initErrors.accelerationError.second;
            X[16] = initErrors.accelerationError.third;

            X[17] = 9.81 * 3E-6;
            X[18] = 9.81 * 3E-6;
            X[19] = 9.81 * 3E-6;
        }
        private void InitF(OmegaGyro omegaGyro,AbsoluteOmega absOmega, EarthModel earthModel, Acceleration acceleration, Matrix C)
        {
            F = Matrix.Zero(19);

            F[1, 3] = 1;
            F[2, 4] = 1;

            //F[3, 1] = -Math.Pow(earthModel.shulerFrequency, 2) + Math.Pow(omegaGyro.N, 2) + Math.Pow(omegaGyro.H, 2);
            F[3, 1] = -Math.Pow(earthModel.shulerFrequency, 2) + Math.Pow(absOmega.N, 2) + Math.Pow(absOmega.H, 2);
            F[3, 2] = omegaGyro.Z_dot - absOmega.E * absOmega.N;
            F[3, 4] = 2 * absOmega.H;

            F[3, 6] = acceleration.H;

            F[3, 14] = C[1, 1];
            F[3, 15] = C[1, 2];
            F[3, 16] = C[1, 3];
            F[3, 17] = C[1, 1] * acceleration.X;
            F[3, 18] = C[1, 2] * acceleration.Y;
            F[3, 19] = C[1, 3] * acceleration.Z;

            F[4, 1] = -(omegaGyro.Z_dot - absOmega.E * absOmega.N);
            F[4, 2] = -Math.Pow(earthModel.shulerFrequency, 2) + Math.Pow(omegaGyro.E, 2) + Math.Pow(omegaGyro.H, 2);
            F[4, 3] = -2 * omegaGyro.H;

            F[4, 5] = -acceleration.H;
            F[4, 7] = acceleration.E;

            F[4, 14] = C[2, 1];
            F[4, 15] = C[2, 2];
            F[4, 16] = C[2, 3];
            F[4, 17] = C[2, 1] * acceleration.X;
            F[4, 18] = C[2, 2] * acceleration.Y;
            F[4, 19] = C[2, 3] * acceleration.Z;

            F[5, 6] = absOmega.H;
            F[5, 7] = -absOmega.N;

            F[5, 8] = C[1, 1];
            F[5, 9] = C[1, 2];
            F[5, 10] = C[1, 3];
            F[5, 11] = C[1, 1] * omegaGyro.X;
            F[5, 12] = C[1, 2] * omegaGyro.Y;
            F[5, 13] = C[1, 3] * omegaGyro.Z;

            F[6, 5] = -absOmega.H;
            F[6, 7] = absOmega.E;

            F[6, 8] = C[2, 1];
            F[6, 9] = C[2, 2];
            F[6, 10] = C[2, 3];
            F[6, 11] = C[2, 1] * omegaGyro.X;
            F[6, 12] = C[2, 2] * omegaGyro.Y;
            F[6, 13] = C[2, 3] * omegaGyro.Z;

            F[7, 5] = absOmega.N;
            F[7, 6] = -absOmega.E;

            F[7, 8] = C[3, 1];
            F[7, 9] = C[3, 2];
            F[7, 10] = C[3, 3];
            F[7, 11] = C[3, 1] * omegaGyro.X;
            F[7, 12] = C[3, 2] * omegaGyro.Y;
            F[7, 13] = C[3, 3] * omegaGyro.Z;
        }
        private void InitG(Matrix C)
        {
            G = Matrix.Zero(19, 19);

            G[3, 17] = C[1, 1];
            G[3, 18] = C[1, 2];
            G[3, 19] = C[1, 3];

            G[4, 17] = C[2, 1];
            G[4, 18] = C[2, 2];
            G[4, 19] = C[2, 3];

            G[5, 14] = C[1, 1];
            G[5, 15] = C[1, 2];
            G[5, 16] = C[1, 3];

            G[6, 14] = C[2, 1];
            G[6, 15] = C[2, 2];
            G[6, 16] = C[2, 3];

            G[7, 14] = C[3, 1];
            G[7, 15] = C[3, 2];
            G[7, 16] = C[3, 3];
        }
        private void InitW(InitErrors initErrors)
        {
            double gyro_noise = 0.05 * initErrors.gyroError.first; //default = 0.05
            double acc_noise = 9.78 * 0.05 * initErrors.accelerationError.first;//default = 0.05
            Random random = new Random();

            W = Vector.Zero(19);
            W[14] = gyro_noise * random.NextDouble();
            W[15] = gyro_noise * random.NextDouble();
            W[16] = gyro_noise * random.NextDouble();
            W[17] = acc_noise * random.NextDouble();
            W[18] = acc_noise * random.NextDouble();
            W[19] = acc_noise * random.NextDouble();

            W_withoutNoise = Vector.Zero(19);
            W_withoutNoise[14] = gyro_noise;
            W_withoutNoise[15] = gyro_noise;
            W_withoutNoise[16] = gyro_noise;
            W_withoutNoise[17] = acc_noise;
            W_withoutNoise[18] = acc_noise;
            W_withoutNoise[19] = acc_noise;
        }
        private void InitH(Point point, EarthModel earth, AbsoluteOmega absOmega, Velocity velocity)
        {
            H = Matrix.Zero(4, 19);

            H[1,1] = 1.0 / (earth.R2 * Math.Cos(point.lat));
            H[2,2] = 1.0 / earth.R1;
            H[3,1] = -velocity.H / earth.R2 + absOmega.E * Math.Tan(point.lat);
            H[3,2] = -absOmega.H;
            H[3,3] = 1.0;
            H[4,1] = -velocity.H / earth.R1;
            H[4,4] = 1.0;
        }
        private void InitZ(Point point, Velocity velocity, EarthModel earth)
        {
            Random random = new Random();
            Z = Vector.Zero(4);

            double[] _estimatedParams = new double[]
                       {
                 point.lon,
                 point.lat,
                 velocity.E,
                 velocity.N
                       };
            Vector estimatedParams = new Vector(_estimatedParams);
            Vector Z_ins = estimatedParams + H * X;
            double[] _snsErrors = new double[] {
                5.0 / (earth.R2*Math.Cos(point.lat)),
                5.0 / earth.R1,
                0.05,
                0.05
            };
            Vector snsErrors = new Vector(_snsErrors);
            double[] _Z_sns = new double[]
            {
                point.lon + snsErrors[1] / (earth.R2 * Math.Cos(point.lat)) * 10.0 * random.NextDouble(),
                point.lat + snsErrors[2] / (earth.R1) * 10.0 * random.NextDouble(),
                velocity.E + snsErrors[3] * 10.0 * random.NextDouble(),
                velocity.N + snsErrors[4] * 10.0 * random.NextDouble()
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
            X[7] = MathTransformation.IncrementValue(X[5], X_dot[5]);
            X[8] = MathTransformation.IncrementValue(X[6], X_dot[6]);
            X[9] = MathTransformation.IncrementValue(X[7], X_dot[7]);
        }
        private void Kalman()
        {
            eyeMatrix = Matrix.Identity(19);

            F_discrete = eyeMatrix + F + (F ^ 2) * 0.5;

            if (P == null)
            {
                P = X.Diag() ^ 2;
            }

            G_discrete = (eyeMatrix + F * 0.5 + (F ^ 2) * (1.0 / 6.0)) * G;
            Q = W_withoutNoise.Diag() ^ 2;
            S = F_discrete * P * ~F_discrete + G_discrete * Q * ~G_discrete;
            K = S * ~H * !(H * S * ~H + R);

            P = (eyeMatrix - K * H) * S;


            if (X_estimate == null)
            {
                X_previous = X.Dublicate();
            }
            
            
            X_estimate = F_discrete * X_previous + K * (Z - H * F_discrete * X_previous);
            


            X_previous = X_estimate.Dublicate();
        }
        public void Model(InitErrors initErrors, Parameters parameters, Matrix C)
        {
            if (X == null)
            {
                X_dot = Vector.Zero(19);
                InitAnglesError(initErrors);
                InitX(initErrors, parameters.point, parameters.absOmega, parameters.earthModel, parameters.velocity, parameters.angles, true);

            }
            else
            {
                InitX(initErrors, parameters.point,  parameters.absOmega, parameters.earthModel, parameters.velocity, parameters.angles);
            }


            InitF(parameters.omegaGyro,parameters.absOmega, parameters.earthModel, parameters.acceleration, C);
            InitG(C);
            InitW(initErrors);



            //X_dot = MatrixOperations.Sum(MatrixOperations.Product(F, X), MatrixOperations.Product(G, W));

            X_dot = F * X + G * W; // шумы повторяются

            InitH(parameters.point, parameters.earthModel, parameters.absOmega, parameters.velocity);
            InitZ(parameters.point, parameters.velocity, parameters.earthModel);

            Kalman();
            //IncrementX();
            //IncrementAngle();
        }
    }
}
