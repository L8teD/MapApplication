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
    public class ErrorsModel
    {
        public double[][] X;
        public double[][] X_dot;
        public double[][] F;
        public double[][] G;
        public double[][] W;
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

        private void InitX(InitErrors initErrors, Point point, OmegaGyro omegaGyro, EarthModel earthModel, Velocity velocity)
        {
            X = MatrixOperations.Zeros(21, 1);

            X[0][0] = initErrors.coordAccuracy.longitude * Math.Cos(point.lat);
            X[2][0] = initErrors.coordAccuracy.latitude;
            X[4][0] = initErrors.coordAccuracy.altitude;


            X[1][0] = initErrors.velocityAccuracy.east + (velocity.H / earthModel.R2 + omegaGyro.E * Math.Tan(point.lat)) * X[0][0] + omegaGyro.H * X[2][0];
            X[3][0] = initErrors.velocityAccuracy.north + velocity.H / earthModel.R1 * X[0][0];
            X[5][0] = initErrors.velocityAccuracy.H;

            X[6][0] = initErrors.angleAccuracy.heading;
            X[7][0] = initErrors.angleAccuracy.roll;
            X[8][0] = initErrors.angleAccuracy.pitch;

            X[9][0] = initErrors.gyroError.first;
            X[10][0] = initErrors.gyroError.second;
            X[11][0] = initErrors.gyroError.third;

            X[12][0] = 2E-5;
            X[13][0] = 2E-5;
            X[14][0] = 2E-5;

            X[15][0] = initErrors.accelerationError.first;
            X[16][0] = initErrors.accelerationError.second;
            X[17][0] = initErrors.accelerationError.third;

            X[18][0] = 9.81 * 3E-6;
            X[19][0] = 9.81 * 3E-6;
            X[20][0] = 9.81 * 3E-6;
        }
        private void InitF(OmegaGyro omegaGyro, EarthModel earthModel, Acceleration acceleration, double[][] C)
        {
            F = MatrixOperations.Zeros(21, 21);

            F[0][2] = 1;
            F[1][3] = 1;
            F[2][4] = 1;

            F[3][0] = -Math.Pow(earthModel.shulerFrequency, 2) + Math.Pow(omegaGyro.N, 2) + Math.Pow(omegaGyro.H, 2);
            F[3][1] = omegaGyro.Z_dot - omegaGyro.E * omegaGyro.N;
            F[3][2] = -(omegaGyro.Y_dot + omegaGyro.E * omegaGyro.H);
            F[3][4] = 2 * omegaGyro.H;
            F[3][5] = -2 * omegaGyro.N;

            F[3][7] = acceleration.H;
            F[3][8] = -acceleration.N;

            F[3][15] = C[0][0];
            F[3][16] = C[0][1];
            F[3][17] = C[0][2];
            F[3][18] = C[0][0] * acceleration.X;
            F[3][19] = C[0][1] * acceleration.Y;
            F[3][20] = C[0][2] * acceleration.Z;

            F[4][0] = -(omegaGyro.Z_dot - omegaGyro.E * omegaGyro.N);
            F[4][1] = -Math.Pow(earthModel.shulerFrequency, 2) + Math.Pow(omegaGyro.E, 2) + Math.Pow(omegaGyro.H, 2);
            F[4][2] = omegaGyro.X_dot - omegaGyro.N * omegaGyro.H;
            F[4][3] = -2 * omegaGyro.H;
            F[4][5] = 2 * omegaGyro.E;

            F[4][6] = -acceleration.H;
            F[4][8] = acceleration.E;

            F[4][15] = C[1][0];
            F[4][16] = C[1][1];
            F[4][17] = C[1][2];
            F[4][18] = C[1][0] * acceleration.X;
            F[4][19] = C[1][1] * acceleration.Y;
            F[4][20] = C[1][2] * acceleration.Z;

            F[5][0] = omegaGyro.Y_dot - omegaGyro.E * omegaGyro.H;
            F[5][1] = -(omegaGyro.X_dot + omegaGyro.N * omegaGyro.H);
            F[5][2] = 2 * Math.Pow(earthModel.shulerFrequency, 2) + Math.Pow(omegaGyro.E, 2) + Math.Pow(omegaGyro.N, 2);
            F[5][3] = 2 * omegaGyro.N;
            F[5][4] = 2 * omegaGyro.E;

            F[5][6] = acceleration.N;
            F[5][7] = acceleration.E;

            F[5][15] = C[2][0];
            F[5][16] = C[2][1];
            F[5][17] = C[2][2];
            F[5][18] = C[2][0] * acceleration.X;
            F[5][19] = C[2][1] * acceleration.Y;
            F[5][20] = C[2][2] * acceleration.Z;

            F[6][7] = omegaGyro.H;
            F[6][8] = -omegaGyro.N;

            F[6][9] = C[0][0];
            F[6][10] = C[0][1];
            F[6][11] = C[0][2];
            F[6][12] = C[0][0] * omegaGyro.X;
            F[6][13] = C[0][1] * omegaGyro.Y;
            F[6][14] = C[0][2] * omegaGyro.Z;

            F[7][6] = -omegaGyro.H;
            F[7][8] = omegaGyro.E;

            F[7][9] = C[1][0];
            F[7][10] = C[1][1];
            F[7][11] = C[1][2];
            F[7][12] = C[1][0] * omegaGyro.X;
            F[7][13] = C[1][1] * omegaGyro.Y;
            F[7][14] = C[1][2] * omegaGyro.Z;

            F[8][6] = omegaGyro.N;
            F[8][7] = omegaGyro.E;

            F[8][9] = C[2][0];
            F[8][10] = C[2][1];
            F[8][11] = C[2][2];
            F[8][12] = C[2][0] * omegaGyro.X;
            F[8][13] = C[2][1] * omegaGyro.Y;
            F[8][14] = C[2][2] * omegaGyro.Z;
        }
        private void InitG(double[][] C)
        {
            G = MatrixOperations.Zeros(21, 6);

            G[3] = new double[] { 0, 0, 0, C[0][0], C[0][1], C[0][2] };
            G[4] = new double[] { 0, 0, 0, C[1][0], C[1][1], C[1][2] };
            G[5] = new double[] { 0, 0, 0, C[2][0], C[2][1], C[2][2] };
            G[6] = new double[] { C[0][0], C[0][1], C[0][2], 0, 0, 0 };
            G[7] = new double[] { C[1][0], C[1][1], C[1][2], 0, 0, 0 };
            G[8] = new double[] { C[2][0], C[2][1], C[2][2], 0, 0, 0 };
        }
        private void InitW(InitErrors initErrors)
        {
            W = MatrixOperations.Zeros(6, 1);
            double gyro_noise = 0.05 * initErrors.gyroError.first;
            double acc_noise = 9.78 * 0.05 * initErrors.accelerationError.first;
            Random random = new Random();
            W = new double[][]
            {
                new double[] {Converter.DegToRad(gyro_noise * random.Next(0,1)) },
                new double[] {Converter.DegToRad(gyro_noise * random.Next(0,1)) },
                new double[] {Converter.DegToRad(gyro_noise * random.Next(0,1)) },
                new double[] {Converter.DegToRad(acc_noise * random.Next(0,1)) },
                new double[] {Converter.DegToRad(acc_noise * random.Next(0,1)) },
                new double[] {Converter.DegToRad(acc_noise * random.Next(0,1)) }
            };
        }
        private void InitH(Point point, EarthModel earth, OmegaGyro omegaGyro, Velocity velocity)
        {
            H = MatrixOperations.Zeros(6, 21);

            H[0][0] = 1.0 / (earth.R2 * Math.Cos(point.lat));
            H[1][1] = 1.0 / earth.R1;
            H[2][2] = 1;
            H[3][1] = velocity.H / earth.R2 + omegaGyro.E * Math.Tan(point.lat);
            H[3][2] = -omegaGyro.H;
            H[3][3] = 1;
            H[4][1] = -velocity.H / earth.R1;
            H[4][4] = 1;
            H[5][5] = 1;

        }
        private void InitZ(Point point, Velocity velocity, EarthModel earth)
        {
            Random random = new Random();
            Z = MatrixOperations.Zeros(6, 1);

            double[][] estimatedParams = new double[][]
            {
                new double[] {point.lon * earth.R2},
                new double[] {point.lat * earth.R1},
                new double[] {point.alt},
                new double[] {velocity.E},
                new double[] {velocity.N},
                new double[] {velocity.H},
            };
            double[][] Z_ins = MatrixOperations.Sum(estimatedParams, MatrixOperations.Product(H, X));
            double[][] snsErrors = new double[][] {
                new double[] { 5 },
                new double[] { 5 },
                new double[] { 5 },
                new double[] { 0.05 },
                new double[] { 0.05 },
                new double[] { 0.05 }
            };
            double[][] Z_sns = new double[][]
            {
                new double[] { point.lon * earth.R2  + snsErrors[0][0] * random.Next(0,1)},
                new double[] { point.lat * earth.R1 + snsErrors[1][0] * random.Next(0,1)},
                new double[] { point.alt + snsErrors[2][0] * random.Next(0,1)},
                new double[] { velocity.E + snsErrors[3][1] * random.Next(0,1)},
                new double[] { velocity.N + snsErrors[4][1] * random.Next(0,1)},
                new double[] { velocity.H + snsErrors[5][1] * random.Next(0,1)}
            };
            Z = new double[][] 
            {
                new double[] {Z_ins[0][0] - Z_sns[0][0] },
                new double[] {Z_ins[1][0] - Z_sns[1][0] },
                new double[] {Z_ins[2][0] - Z_sns[2][0] },
                new double[] {Z_ins[3][0] - Z_sns[3][0] },
            };
            R = MatrixOperations.DiagMatrix(snsErrors);

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
                    MatrixOperations.Product(MatrixOperations.Product(G_discrete, Q), MatrixOperations.Transporation(F_discrete))
                    );
                K = MatrixOperations.Product(
                    MatrixOperations.Product(S, MatrixOperations.Transporation(H)),
                    MatrixOperations.Inverted(MatrixOperations.Sum(MatrixOperations.Product(
                        MatrixOperations.Product(H, S), MatrixOperations.Transporation(H)), R))
                    );
                P = MatrixOperations.Product(MatrixOperations.Difference(eyeMatrix, MatrixOperations.Product(K, H)), S);
            }
            G_discrete = MatrixOperations.Product(MatrixOperations.Sum(eyeMatrix,
                 MatrixOperations.Multiplication(F, 0.5)), G);
            Q = MatrixOperations.Pow(MatrixOperations.DiagMatrix(W),2);
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
        public void Model(InitErrors initErrors, Point point, OmegaGyro omegaGyro, EarthModel earthModel, Velocity velocity, 
            Acceleration acceleration, double[][] C)
        {
            if (X == null)
                InitX(initErrors, point, omegaGyro, earthModel, velocity);

            InitF(omegaGyro, earthModel, acceleration, C);
            InitG(C);
            InitW(initErrors);

            if (X_dot == null)
                X_dot = MatrixOperations.Zeros(21, 1);

            X_dot = MatrixOperations.Sum(MatrixOperations.Product(F, X), MatrixOperations.Product(G, W));

            InitH(point, earthModel, omegaGyro, velocity);
            InitZ(point, velocity, earthModel);
            
            Kalman();
            IncrementX();
            IncrementAngle();
        }
    }
}
