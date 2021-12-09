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

namespace EstimateLib
{
    public class KalmanModel2
    {
        public double[][] X;
        public double[][] X_dot;
        public double[][] F;
        public double[][] G;
        public double[][] W;
        double[][] W_withoutNoise;
        public double[][] H;
        public double[][] Z;
        public double[][] P;
        public double[][] X_estimate;
        double[][] X_previous;
        double[][] eyeMatrix;
        double[][] F_discrete;
        double[][] G_discrete;
        double[][] Q;
        double[][] R;
        double[][] K;
        double[][] S;
        double[][] orientationAngles;
        public double[][] anglesErrors;

        private CoordAccuracy coordAccuracy;
        private VelocityAccuracy velocityAccuracy;

        private void InitX(InitErrors initErrors, Point point, OmegaGyro omegaGyro, EarthModel earthModel, Velocity velocity, Angles angles, bool Init = false)
        {
            X = MatrixOperations.Zeros(19, 1);

            if (Init)
            {
                X[0][0] = initErrors.coordAccuracy.longitude; //* Math.Cos(point.lat);
                X[1][0] = initErrors.coordAccuracy.latitude;

                X[2][0] = initErrors.velocityAccuracy.east + (velocity.H / earthModel.R2 + omegaGyro.E * Math.Tan(point.lat)) * X[0][0] + omegaGyro.H * X[2][0];
                X[3][0] = initErrors.velocityAccuracy.north + velocity.H / earthModel.R1 * X[0][0];

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
                coordAccuracy.longitude += X_dot[0][0];
                coordAccuracy.latitude += X_dot[1][0];
                //coordAccuracy.altitude += X_dot[2][0];

                velocityAccuracy.east += X_dot[3][0];
                velocityAccuracy.north += X_dot[4][0];
                //velocityAccuracy.H += X_dot[5][0];

                X[0][0] = coordAccuracy.longitude * Math.Cos(point.lat);
                X[1][0] = coordAccuracy.latitude;
                //X[2][0] = coordAccuracy.altitude;


                X[2][0] = velocityAccuracy.east + (velocity.H / earthModel.R2 + omegaGyro.E * Math.Tan(point.lat)) * X[0][0] + omegaGyro.H * X[2][0];
                X[3][0] = velocityAccuracy.north + velocity.H / earthModel.R1 * X[0][0];
                //X[5][0] = velocityAccuracy.H;
            }

            double[][] M = Create.MatrixM(angles.heading, angles.pitch);

            double[][] orientationErrors = MatrixOperations.Product(MatrixOperations.Inverted(M), anglesErrors);

            orientationAngles[0][0] = orientationErrors[0][0] + X_dot[6][0];
            orientationAngles[1][0] = orientationErrors[1][0] + X_dot[7][0];
            orientationAngles[2][0] = orientationErrors[2][0] + X_dot[8][0];

            anglesErrors = MatrixOperations.Product(M, orientationAngles);


            X[4][0] = initErrors.angleAccuracy.heading;
            X[5][0] = initErrors.angleAccuracy.roll;
            X[6][0] = initErrors.angleAccuracy.pitch;

            X[7][0] = initErrors.gyroError.first;
            X[8][0] = initErrors.gyroError.second;
            X[9][0] = initErrors.gyroError.third;

            X[10][0] = 2E-5;
            X[11][0] = 2E-5;
            X[12][0] = 2E-5;

            X[13][0] = initErrors.accelerationError.first;
            X[14][0] = initErrors.accelerationError.second;
            X[15][0] = initErrors.accelerationError.third;

            X[16][0] = 9.81 * 3E-6;
            X[17][0] = 9.81 * 3E-6;
            X[18][0] = 9.81 * 3E-6;
        }
        private void InitF(OmegaGyro omegaGyro, EarthModel earthModel, Acceleration acceleration, double[][] C)
        {
            F = MatrixOperations.Zeros(19, 19);

            F[0][2] = 1;
            F[1][3] = 1;
            

            F[2][0] = -Math.Pow(earthModel.shulerFrequency, 2) + Math.Pow(omegaGyro.N, 2) + Math.Pow(omegaGyro.H, 2);
            F[2][1] = omegaGyro.Z_dot - omegaGyro.E * omegaGyro.N;
            F[2][3] = 2 * omegaGyro.H;

            F[2][5] = acceleration.H;

            F[2][13] = C[0][0];
            F[2][14] = C[0][1];
            F[2][15] = C[0][2];
            F[2][16] = C[0][0] * acceleration.X;
            F[2][17] = C[0][1] * acceleration.Y;
            F[2][18] = C[0][2] * acceleration.Z;

            F[3][0] = -(omegaGyro.Z_dot - omegaGyro.E * omegaGyro.N);
            F[3][1] = -Math.Pow(earthModel.shulerFrequency, 2) + Math.Pow(omegaGyro.E, 2) + Math.Pow(omegaGyro.H, 2);
            F[3][2] = -2 * omegaGyro.H;

            F[3][4] = -acceleration.H;
            F[3][6] = acceleration.E;

            F[3][13] = C[1][0];
            F[3][14] = C[1][1];
            F[3][15] = C[1][2];
            F[3][16] = C[1][0] * acceleration.X;
            F[3][17] = C[1][1] * acceleration.Y;
            F[3][18] = C[1][2] * acceleration.Z;

            F[4][5] = omegaGyro.H;
            F[4][6] = -omegaGyro.N;
              
            F[4][7] = C[0][0];
            F[4][8] = C[0][1];
            F[4][9] = C[0][2];
            F[4][10] = C[0][0] * omegaGyro.X;
            F[4][11] = C[0][1] * omegaGyro.Y;
            F[4][12] = C[0][2] * omegaGyro.Z;

            F[5][4] = -omegaGyro.H;
            F[5][6] = omegaGyro.E;
              
            F[5][7] = C[1][0];
            F[5][8]  = C[1][1];
            F[5][9]  = C[1][2];
            F[5][10] = C[1][0] * omegaGyro.X;
            F[5][11] = C[1][1] * omegaGyro.Y;
            F[5][12] = C[1][2] * omegaGyro.Z;

            F[6][4] = omegaGyro.N;
            F[6][5] = omegaGyro.E;

            F[6][7] = C[2][0];
            F[6][8]  = C[2][1];
            F[6][9]  = C[2][2];
            F[6][10] = C[2][0] * omegaGyro.X;
            F[6][11] = C[2][1] * omegaGyro.Y;
            F[6][12] = C[2][2] * omegaGyro.Z;
        }
        private void InitG(double[][] C)
        {
            G = MatrixOperations.Zeros(19, 6);

            G[2] = new double[] { 0, 0, 0, C[0][0], C[0][1], C[0][2] };
            G[3] = new double[] { 0, 0, 0, C[1][0], C[1][1], C[1][2] };
            //G[5] = new double[] { 0, 0, 0, C[2][0], C[2][1], C[2][2] };
            G[4] = new double[] { C[0][0], C[0][1], C[0][2], 0, 0, 0 };
            G[5] = new double[] { C[1][0], C[1][1], C[1][2], 0, 0, 0 };
            G[6] = new double[] { C[2][0], C[2][1], C[2][2], 0, 0, 0 };
        }
        private void InitW(InitErrors initErrors)
        {
            W = MatrixOperations.Zeros(6, 1);
            double gyro_noise = 0.05 * initErrors.gyroError.first;
            double acc_noise = 9.78 * 0.05 * initErrors.accelerationError.first;
            Random random = new Random();
            W = new double[][]
            {
                new double[] {gyro_noise * random.NextDouble() },
                new double[] {gyro_noise * random.NextDouble() },
                new double[] {gyro_noise * random.NextDouble() },
                new double[] {acc_noise * random.NextDouble() },
                new double[] {acc_noise * random.NextDouble() },
                new double[] {acc_noise * random.NextDouble() }
            };

            W_withoutNoise = new double[][]
            {
                new double[] {Converter.DegToRad(gyro_noise) },
                new double[] {Converter.DegToRad(gyro_noise) },
                new double[] {Converter.DegToRad(gyro_noise) },
                new double[] {Converter.DegToRad(acc_noise ) },
                new double[] {Converter.DegToRad(acc_noise ) },
                new double[] {Converter.DegToRad(acc_noise ) }
            };
        }
        private void InitH(Point point, EarthModel earth, OmegaGyro omegaGyro, Velocity velocity)
        {
            H = MatrixOperations.Zeros(4, 19);

            H[0][0] = 1.0 / (earth.R2 * Math.Cos(point.lat));
            H[1][1] = 1.0 / earth.R1;
            H[2][1] = velocity.H / earth.R2 + omegaGyro.E * Math.Tan(point.lat);
            H[2][2] = -omegaGyro.H;
            H[2][3] = 1.0;
            H[3][1] = -velocity.H / earth.R1;
            H[3][4] = 1.0;


        }
        private void InitZ(Point point, Velocity velocity, EarthModel earth)
        {
            Random random = new Random();
            Z = MatrixOperations.Zeros(4, 1);

            double[][] estimatedParams = new double[][]
            {
                new double[] {point.lon * earth.R2},
                new double[] {point.lat * earth.R1},
                new double[] {velocity.E},
                new double[] {velocity.N},
            };
            double[][] Z_ins = MatrixOperations.Sum(estimatedParams, MatrixOperations.Product(H, X));
            double[][] snsErrors = new double[][] {
                new double[] { 5 },
                new double[] { 5 },
                new double[] { 0.05 },
                new double[] { 0.05 }
            };
            double[][] Z_sns = new double[][]
            {
                new double[] { point.lon + snsErrors[0][0] * random.NextDouble()},
                new double[] { point.lat + snsErrors[1][0] * random.NextDouble()},
                new double[] { velocity.E + snsErrors[2][0] * random.NextDouble()},
                new double[] { velocity.N + snsErrors[3][0] * random.NextDouble()},
            };
            Z = new double[][]
            {
                new double[] {Z_ins[0][0] - Z_sns[0][0] },
                new double[] {Z_ins[1][0] - Z_sns[1][0] },
                new double[] {Z_ins[2][0] - Z_sns[2][0] },
                new double[] {Z_ins[3][0] - Z_sns[3][0] },
            };
            R = MatrixOperations.Pow(MatrixOperations.DiagMatrix(snsErrors), 2);

        }
        private void InitAnglesError(InitErrors initErrors)
        {
            orientationAngles = new double[3][]
            {
                new double[1],
                new double[1],
                new double[1]
            };
            anglesErrors = new double[][] {
                new double[] { Converter.DegToRad(initErrors.angleAccuracy.heading) },
                new double[] { Converter.DegToRad(initErrors.angleAccuracy.roll) },
                new double[] { Converter.DegToRad(initErrors.angleAccuracy.pitch) } };
        }
        private void IncrementX()
        {
            MathTransformation.IncrementValue(ref X[0][0], X_dot[0][0]);
            MathTransformation.IncrementValue(ref X[1][0], X_dot[1][0]);
            MathTransformation.IncrementValue(ref X[2][0], X_dot[2][0]);
            MathTransformation.IncrementValue(ref X[3][0], X_dot[3][0]);
            MathTransformation.IncrementValue(ref X[4][0], X_dot[4][0]);
            MathTransformation.IncrementValue(ref X[5][0], X_dot[5][0]);
        }
        private void IncrementAngle()
        {
            MathTransformation.IncrementValue(ref X[6][0], X_dot[6][0]);
            MathTransformation.IncrementValue(ref X[7][0], X_dot[7][0]);
            MathTransformation.IncrementValue(ref X[8][0], X_dot[8][0]);
        }
        private void Kalman()
        {
            eyeMatrix = MatrixOperations.Eye(19);
            F_discrete = MatrixOperations.Sum(MatrixOperations.Sum(eyeMatrix, F), MatrixOperations.Pow(F, 2));

            if (P == null)
            {
                P = MatrixOperations.Pow(MatrixOperations.DiagMatrix(X), 2);
            }
            else
            {
                S = MatrixOperations.Sum(
                    MatrixOperations.Product(MatrixOperations.Product(F_discrete, P), MatrixOperations.Transporation(F_discrete)),
                    MatrixOperations.Product(MatrixOperations.Product(G_discrete, Q), MatrixOperations.Transporation(G_discrete))
                    );
                K = MatrixOperations.Product(MatrixOperations.Product(S, MatrixOperations.Transporation(H)),
                    MatrixOperations.Inverted(MatrixOperations.Sum(MatrixOperations.Product(
                        MatrixOperations.Product(H, S), MatrixOperations.Transporation(H)), R)));
                P = MatrixOperations.Product(MatrixOperations.Difference(eyeMatrix, MatrixOperations.Product(K, H)), S);
            }
            G_discrete = MatrixOperations.Product(MatrixOperations.Sum(eyeMatrix, MatrixOperations.Multiplication(F, 0.5)), G);

            Q = MatrixOperations.Pow(MatrixOperations.DiagMatrix(W_withoutNoise), 2);
            if (X_estimate == null)
            {
                X_estimate = X;
            }
            else
            {
                X_estimate = MatrixOperations.Sum(MatrixOperations.Product(
                F_discrete, X_previous), MatrixOperations.Product(K, MatrixOperations.Difference(Z,
                    MatrixOperations.Product(MatrixOperations.Product(H, F_discrete), X_previous))));
            }


            X_previous = X_estimate;
        }
        public void Model(InitErrors initErrors, Parameters parameters, double[][] C)
        {
            if (X == null)
            {
                X_dot = MatrixOperations.Zeros(19, 1);
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



            //X_dot = MatrixOperations.Sum(MatrixOperations.Product(F, X), MatrixOperations.Product(G, W));
            X_dot = MatrixOperations.Product(F, X);

            InitH(parameters.point, parameters.earthModel, parameters.omegaGyro, parameters.velocity);
            InitZ(parameters.point, parameters.velocity, parameters.earthModel);

            Kalman();
            //IncrementX();
            //IncrementAngle();
        }
    }
}
