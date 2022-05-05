using CommonLib;
using CommonLib.Params;
using ModellingErrorsLib3;
using MyMatrix;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstimateLib
{
    public class KalmanModel : IKalman
    {
        public Vector X { get; set; }
        public Vector X_dot;
        public Matrix F;
        public Matrix G;
        public Vector W;
        Vector W_withoutNoise;
        public Matrix H;
        public Vector Z;
        public Matrix P { get; set; }
        public Vector X_estimate { get; set; }
        Vector X_previous;
        Matrix eyeMatrix;
        Matrix F_discrete;
        Matrix G_discrete;
        Matrix Q;
        Matrix R;
        Matrix K;
        Matrix S;
        public Vector orientationAngles;
        public Vector anglesErrors;

        private CoordAccuracy coordAccuracy;
        private VelocityAccuracy velocityAccuracy;

        private void InitX(InitErrors initErrors, Point point, AbsoluteOmega absOmega, EarthModel earthModel, Velocity velocity, Angles angles, bool Init = false)
        {
            X = Vector.Zero(21);

            if (Init)
            {
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
                coordAccuracy.longitude += X_dot[1] * initErrors.dt;
                coordAccuracy.latitude += X_dot[2] * initErrors.dt;
                coordAccuracy.altitude += X_dot[3] * initErrors.dt;

                velocityAccuracy.east += X_dot[4] * initErrors.dt;
                velocityAccuracy.north += X_dot[5] * initErrors.dt;
                velocityAccuracy.H += X_dot[6] * initErrors.dt;
            }
            X[1] = coordAccuracy.longitude;
            X[2] = coordAccuracy.latitude;
            X[3] = coordAccuracy.altitude;


            X[4] = velocityAccuracy.east + (velocity.H / earthModel.R2 + absOmega.E * Math.Tan(point.lat)) * X[1] + absOmega.H * X[3];
            X[5] = velocityAccuracy.north + velocity.H / earthModel.R1 * X[1];
            X[6] = velocityAccuracy.H;

            Matrix M = Create.MatrixM(angles.heading, angles.pitch);

            Vector orientationErrors = !M * anglesErrors;

            double alfa = orientationErrors[1] + X_dot[7] * initErrors.dt;
            double betta = orientationErrors[2] + X_dot[8] * initErrors.dt;
            double gamma = orientationErrors[3] + X_dot[9] * initErrors.dt;

            orientationAngles[1] = alfa - X[2] / earthModel.R1;
            orientationAngles[2] = betta + X[1] / earthModel.R2;
            orientationAngles[3] = gamma + X[1] * Math.Tan(point.lat) / earthModel.R1;

            //anglesErrors = M * orientationAngles;


            X[7] = orientationAngles[1];
            X[8] = orientationAngles[2];
            X[9] = orientationAngles[3];

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
        private void InitF(OmegaGyro omegaGyro, AbsoluteOmega absOmega, EarthModel earthModel, Acceleration acceleration, Matrix C)
        {
            F = Matrix.Zero(21);

            F[1, 3] = 1;
            F[2, 4] = 1;
            F[3, 5] = 1;

            F[4, 1] = -Math.Pow(earthModel.shulerFrequency, 2) + Math.Pow(absOmega.N, 2) + Math.Pow(absOmega.H, 2);
            F[4, 2] = -omegaGyro.Z_dot - absOmega.E * absOmega.N;
            F[4, 3] = -(omegaGyro.Y_dot + absOmega.E * absOmega.H);
            F[4, 5] = 2 * absOmega.H;
            F[4, 6] = -2 * absOmega.N;

            F[4, 8] = acceleration.H;
            F[4, 9] = -acceleration.N;

            F[4, 16] = C[1, 1];
            F[4, 17] = C[1, 2];
            F[4, 18] = C[1, 3];
            F[4, 19] = C[1, 1] * acceleration.X;
            F[4, 20] = C[1, 2] * acceleration.Y;
            F[4, 21] = C[1, 3] * acceleration.Z;

            F[5, 1] = -(omegaGyro.Z_dot - absOmega.E * absOmega.N);
            F[5, 2] = -Math.Pow(earthModel.shulerFrequency, 2) + Math.Pow(absOmega.E, 2) + Math.Pow(absOmega.H, 2);
            F[5, 3] = omegaGyro.X_dot - absOmega.N * absOmega.H;
            F[5, 4] = -2 * absOmega.H;
            F[5, 6] = 2 * absOmega.E;

            F[5, 7] = -acceleration.H;
            F[5, 9] = acceleration.E;

            F[5, 16] = C[2, 1];
            F[5, 17] = C[2, 2];
            F[5, 18] = C[2, 3];
            F[5, 19] = C[2, 1] * acceleration.X;
            F[5, 20] = C[2, 2] * acceleration.Y;
            F[5, 21] = C[2, 3] * acceleration.Z;

            F[6, 1] = omegaGyro.Y_dot - absOmega.E * absOmega.H;
            F[6, 2] = -(omegaGyro.X_dot + absOmega.N * absOmega.H);
            F[6, 3] = 2 * Math.Pow(earthModel.shulerFrequency, 2) + Math.Pow(absOmega.E, 2) + Math.Pow(absOmega.N, 2);
            F[6, 4] = 2 * absOmega.N;
            F[6, 5] = 2 * absOmega.E;

            F[6, 7] = acceleration.N;
            F[6, 8] = acceleration.E;

            F[6, 15] = C[3, 1];
            F[6, 16] = C[3, 2];
            F[6, 17] = C[3, 3];
            F[6, 18] = C[3, 1] * acceleration.X;
            F[6, 19] = C[3, 2] * acceleration.Y;
            F[6, 20] = C[3, 3] * acceleration.Z;

            F[7, 7] = absOmega.H;
            F[7, 8] = -absOmega.N;

            F[7, 10] = C[1, 1];
            F[7, 11] = C[1, 2];
            F[7, 12] = C[1, 3];
            F[7, 13] = C[1, 1] * omegaGyro.X;
            F[7, 14] = C[1, 2] * omegaGyro.Y;
            F[7, 15] = C[1, 3] * omegaGyro.Z;

            F[8, 7] = -absOmega.H;
            F[8, 9] = absOmega.E;

            F[8, 10] = C[2, 1];
            F[8, 11] = C[2, 2];
            F[8, 12] = C[2, 3];
            F[8, 13] = C[2, 1] * omegaGyro.X;
            F[8, 14] = C[2, 2] * omegaGyro.Y;
            F[8, 15] = C[2, 3] * omegaGyro.Z;

            F[9, 7] = absOmega.N;
            F[9, 8] = -absOmega.E;

            F[9, 10] = C[3, 1];
            F[9, 11] = C[3, 2];
            F[9, 12] = C[3, 3];
            F[9, 13] = C[3, 1] * omegaGyro.X;
            F[9, 14] = C[3, 2] * omegaGyro.Y;
            F[9, 15] = C[3, 3] * omegaGyro.Z;
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
            double gyro_noise = initErrors.gyroNoise;
            double acc_noise = initErrors.accNoise;

            W = Vector.Zero(6);
            W[1] = gyro_noise * Import.GetRandom();
            W[2] = gyro_noise * Import.GetRandom();
            W[3] = gyro_noise * Import.GetRandom();
            W[4] = acc_noise * Import.GetRandom();
            W[5] = acc_noise * Import.GetRandom();
            W[6] = acc_noise * Import.GetRandom();

            W_withoutNoise = Vector.Zero(6);
            W_withoutNoise[1] = gyro_noise;
            W_withoutNoise[2] = gyro_noise;
            W_withoutNoise[3] = gyro_noise;
            W_withoutNoise[4] = acc_noise;
            W_withoutNoise[5] = acc_noise;
            W_withoutNoise[6] = acc_noise;
        }
        private void InitH(Point point, EarthModel earth, AbsoluteOmega absOmega, Velocity velocity)
        {
            H = Matrix.Zero(6, 21);

            H[1, 1] = 1.0;
            H[2, 2] = 1.0;
            H[3, 3] = 1.0;
            H[4, 1] = -velocity.H / earth.R2 + absOmega.E * Math.Tan(point.lat);
            H[4, 2] = -absOmega.H;
            H[4, 4] = 1.0;
            H[5, 1] = -velocity.H / earth.R1;
            H[5, 5] = 1.0;
            H[6, 6] = 1.0;

        }
        private void InitZ(Point point, Velocity velocity, EarthModel earth, InitErrors initErrors)
        {
            Z = Vector.Zero(6);

            double[] _estimatedParams = new double[]
            {
                 point.lon,
                 point.lat,
                 point.alt,
                 velocity.E,
                 velocity.N,
                 velocity.H,
            };
            Vector estimatedParams = new Vector(_estimatedParams);
            Vector Z_ins = estimatedParams + H * X;
            double[] _snsErrors = new double[] {
                5.0,
                5.0,
                5.0,
                0.05,
                0.05,
                0.05
            };
            double noise_sns = initErrors.snsNoise;
            Vector snsErrors = new Vector(_snsErrors);
            double[] _Z_sns = new double[]
            {
                point.lon + snsErrors[1] / (earth.R2 * Math.Cos(point.lat)) * noise_sns * Import.GetRandom(),
                point.lat + snsErrors[2] / (earth.R1) * noise_sns * Import.GetRandom(),
                point.alt + snsErrors[3] * noise_sns * Import.GetRandom(),
                velocity.E + snsErrors[4] * noise_sns * Import.GetRandom(),
                velocity.N + snsErrors[5] * noise_sns * Import.GetRandom(),
                velocity.H + snsErrors[6] * noise_sns * Import.GetRandom()
            };
            Vector Z_sns = new Vector(_Z_sns);
            Z = Z_ins - Z_sns;

            R = snsErrors.Diag() ^ 2 * (1.0 / initErrors.dt);
        }
        private void InitAnglesError(InitErrors initErrors)
        {
            orientationAngles = Vector.Zero(3);
            anglesErrors = new Vector(Converter.DegToRad(initErrors.angleAccuracy.heading),
                  Converter.DegToRad(initErrors.angleAccuracy.roll), Converter.DegToRad(initErrors.angleAccuracy.pitch));
        }
        private void Kalman(double dt)
        {
            eyeMatrix = Matrix.Identity(21);
            F_discrete = eyeMatrix + F * dt;// + (F*dt ^ 2) * 0.5;

            if (P == null)
            {
                P = X.Diag() ^ 2;
            }
            else
            {
                P = (eyeMatrix - K * H) * S;
            }

            G_discrete = F_discrete * G * dt; //(eyeMatrix + F*dt * 0.5 + (F*dt ^ 2) * (1.0 / 6.0)) * G*dt;

            Q = W_withoutNoise.Diag() ^ 2 * (1.0 / dt);

            S = F_discrete * P * ~F_discrete + G_discrete * Q * ~G_discrete;
            K = S * ~H * !(H * S * ~H + R);

            if (X_estimate == null)
            {
                X_previous = X.Dublicate();
            }

            X_estimate = F_discrete * X_previous + K * (Z - H * F_discrete * X_previous);

            X_previous = X_estimate.Dublicate();
        }
        public void Model(InitErrors initErrors, Parameters parameters, Matrix C, double dt)
        {
            if (X == null)
            {
                X_dot = Vector.Zero(21);
                InitAnglesError(initErrors);
                InitX(initErrors, parameters.point, parameters.absOmega, parameters.earthModel, parameters.velocity, parameters.angles, true);
            }
            else
            {
                InitX(initErrors, parameters.point, parameters.absOmega, parameters.earthModel, parameters.velocity, parameters.angles);
            }


            InitF(parameters.omegaGyro, parameters.absOmega, parameters.earthModel, parameters.acceleration, C);
            InitG(C);
            InitW(initErrors);


            X_dot = F * X + G * W;

            InitH(parameters.point, parameters.earthModel, parameters.absOmega, parameters.velocity);
            InitZ(parameters.point, parameters.velocity, parameters.earthModel, initErrors);

            Kalman(dt);

        }
    }
}
