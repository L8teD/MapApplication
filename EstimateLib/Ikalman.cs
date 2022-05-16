using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLib;
using CommonLib.Params;
using ModellingErrorsLib3;
using MyMatrix;

namespace EstimateLib
{
    public interface IKalman
    {
        Vector X { get; set; }
        Vector X_estimate { get; set; }
        Matrix P { get; set; }
        bool CorrectorIsGNSS { get; set; }
        void Model(Input input, Parameters parameters, MeasurementsErrors gnssMeasurments, MeasurementsErrors svsMeasurements);
    }
    public abstract class BaseKalman : IKalman
    {
        protected Vector X_error;
        public Vector X 
        { 
            get
            {
                return X_error;
            }
            set
            {
                X_error = value;
            }
        }
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

        public bool CorrectorIsGNSS { get; set; }
        private void InitX(InsErrors initErrors, Point point, AbsoluteOmega absOmega, EarthModel earthModel, Velocity velocity, Angles angles, bool Init = false)
        {
            X_error = Vector.Zero(21);

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
            X_error[1] = coordAccuracy.longitude;
            X_error[2] = coordAccuracy.latitude;
            X_error[3] = coordAccuracy.altitude;


            X_error[4] = velocityAccuracy.east + (velocity.H / earthModel.R2 + absOmega.E * Math.Tan(point.lat)) * X_error[1] + absOmega.H * X_error[3];
            X_error[5] = velocityAccuracy.north + velocity.H / earthModel.R1 * X_error[1] - absOmega.E * X_error[3];
            X_error[6] = velocityAccuracy.H;

            Matrix M = Create.MatrixM(angles.heading, angles.pitch);

            Vector orientationErrors = !M * anglesErrors;

            double alfa = orientationErrors[1] + X_dot[7] * initErrors.dt;
            double betta = orientationErrors[2] + X_dot[8] * initErrors.dt;
            double gamma = orientationErrors[3] + X_dot[9] * initErrors.dt;

            orientationAngles[1] = alfa - X_error[2] / earthModel.R1;
            orientationAngles[2] = betta + X_error[1] / earthModel.R2;
            orientationAngles[3] = gamma + X_error[1] * Math.Tan(point.lat) / earthModel.R1;

            //anglesErrors = M * orientationAngles;


            X_error[7] = orientationAngles[1];
            X_error[8] = orientationAngles[2];
            X_error[9] = orientationAngles[3];

            X_error[10] = initErrors.gyroError.first;
            X_error[11] = initErrors.gyroError.second;
            X_error[12] = initErrors.gyroError.third;

            X_error[13] = initErrors.temperatureKoef.first;
            X_error[14] = initErrors.temperatureKoef.second;
            X_error[15] = initErrors.temperatureKoef.third;

            X_error[16] = initErrors.accelerationError.first;
            X_error[17] = initErrors.accelerationError.second;
            X_error[18] = initErrors.accelerationError.third;

            X_error[19] = 9.81 * initErrors.temperatureKoef.first;
            X_error[20] = 9.81 * initErrors.temperatureKoef.second;
            X_error[21] = 9.81 * initErrors.temperatureKoef.third;
        }
        private void InitF(OmegaGyro omegaGyro, AbsoluteOmega absOmega, EarthModel earthModel, Acceleration acceleration, Matrix C, InputAirData airData)
        {
            double resultTemperature = airData.temperatureCelcius + airData.tempratureError;

            F = Matrix.Zero(21);

            F[1, 4] = 1;
            F[2, 5] = 1;
            F[3, 6] = 1;

            F[4, 1] = -Math.Pow(earthModel.shulerFrequency, 2) + Math.Pow(absOmega.N, 2) + Math.Pow(absOmega.H, 2);
            F[4, 2] = omegaGyro.Z_dot - absOmega.E * absOmega.N;
            F[4, 3] = -(omegaGyro.Y_dot + absOmega.E * absOmega.H);
            F[4, 5] = 2 * absOmega.H;
            F[4, 6] = -2 * absOmega.N;

            F[4, 8] = acceleration.H;
            F[4, 9] = -acceleration.N;

            F[4, 16] = C[1, 1] /** acceleration.X*/ * resultTemperature;
            F[4, 17] = C[1, 2] /** acceleration.Y*/ * resultTemperature;
            F[4, 18] = C[1, 3] /** acceleration.Z*/ * resultTemperature;
            F[4, 19] = C[1, 1] * acceleration.X;
            F[4, 20] = C[1, 2] * acceleration.Y;
            F[4, 21] = C[1, 3] * acceleration.Z;

            F[5, 1] = -(omegaGyro.Z_dot - absOmega.E * absOmega.N);
            F[5, 2] = -Math.Pow(earthModel.shulerFrequency, 2) + Math.Pow(absOmega.E, 2) + Math.Pow(absOmega.H, 2);
            F[5, 3] = omegaGyro.X_dot - absOmega.N * absOmega.H;
            F[5, 4] = -2 * absOmega.H;
            F[5, 6] = 2 * absOmega.E;

            //F[5, 7] = -acceleration.H;
            F[5, 7] = -acceleration.H;
            //F[5, 9] = acceleration.E;
            F[5, 9] = acceleration.E;

            F[5, 16] = C[2, 1] /** acceleration.X*/ * resultTemperature;
            F[5, 17] = C[2, 2] /** acceleration.Y*/ * resultTemperature;
            F[5, 18] = C[2, 3] /** acceleration.Z*/ * resultTemperature;
            F[5, 19] = C[2, 1] * acceleration.X;
            F[5, 20] = C[2, 2] * acceleration.Y;
            F[5, 21] = C[2, 3] * acceleration.Z;

            F[6, 1] = omegaGyro.Y_dot - absOmega.E * absOmega.H;
            F[6, 2] = -(omegaGyro.X_dot + absOmega.N * absOmega.H);
            F[6, 3] = 2 * Math.Pow(earthModel.shulerFrequency, 2) + Math.Pow(absOmega.E, 2) + Math.Pow(absOmega.N, 2);
            F[6, 4] = 2 * absOmega.N;
            F[6, 5] = -2 * absOmega.E;

            F[6, 7] = acceleration.N;
            F[6, 8] = acceleration.E;

            F[6, 15] = C[3, 1] /** acceleration.X*/ * resultTemperature;
            F[6, 16] = C[3, 2] /** acceleration.Y*/ * resultTemperature;
            F[6, 17] = C[3, 3] /** acceleration.Z*/ * resultTemperature;
            F[6, 18] = C[3, 1] * acceleration.X;
            F[6, 19] = C[3, 2] * acceleration.Y;
            F[6, 20] = C[3, 3] * acceleration.Z;

            F[7, 8] = absOmega.H;
            F[7, 9] = -absOmega.N;

            F[7, 10] = C[1, 1] /** omegaGyro.X*/ * resultTemperature;
            F[7, 11] = C[1, 2] /** omegaGyro.Y*/ * resultTemperature;
            F[7, 12] = C[1, 3] /** omegaGyro.Z*/ * resultTemperature;
            F[7, 13] = C[1, 1] * omegaGyro.X;
            F[7, 14] = C[1, 2] * omegaGyro.Y;
            F[7, 15] = C[1, 3] * omegaGyro.Z;

            F[8, 7] = -absOmega.H;
            F[8, 9] = absOmega.E;

            F[8, 10] = C[2, 1] /** omegaGyro.X */* resultTemperature;
            F[8, 11] = C[2, 2] /** omegaGyro.Y */* resultTemperature;
            F[8, 12] = C[2, 3] /** omegaGyro.Z */* resultTemperature;
            F[8, 13] = C[2, 1] * omegaGyro.X;
            F[8, 14] = C[2, 2] * omegaGyro.Y;
            F[8, 15] = C[2, 3] * omegaGyro.Z;

            F[9, 7] = absOmega.N;
            F[9, 8] = -absOmega.E;

            F[9, 10] = C[3, 1] /** omegaGyro.X*/ * resultTemperature;
            F[9, 11] = C[3, 2] /** omegaGyro.Y*/ * resultTemperature;
            F[9, 12] = C[3, 3] /** omegaGyro.Z*/ * resultTemperature;
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
        private void InitW(InsErrors insErrors)
        {
            W = Vector.Zero(6);
            W[1] = insErrors.accNoiseSKO.first * insErrors.accNoiseValue.first;
            W[2] = insErrors.accNoiseSKO.second * insErrors.accNoiseValue.second;
            W[3] = insErrors.accNoiseSKO.third * insErrors.accNoiseSKO.third;
            W[4] = insErrors.gyroNoiseSKO.first * insErrors.gyroNoiseValue.first;
            W[5] = insErrors.gyroNoiseSKO.second * insErrors.gyroNoiseValue.second;
            W[6] = insErrors.gyroNoiseSKO.third * insErrors.gyroNoiseValue.third;


            W_withoutNoise = Vector.Zero(6);
            W_withoutNoise[1] = insErrors.accNoiseSKO.first;
            W_withoutNoise[2] = insErrors.accNoiseSKO.second;
            W_withoutNoise[3] = insErrors.accNoiseSKO.third;
            W_withoutNoise[4] = insErrors.gyroNoiseSKO.first;
            W_withoutNoise[5] = insErrors.gyroNoiseSKO.second;
            W_withoutNoise[6] = insErrors.gyroNoiseSKO.third;
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
        private void InitZ(Point point, Velocity velocity, EarthModel earth, InsErrors insErrors, MeasurementsErrors gnssMeasurements)
        {
            Z = Vector.Zero(6);
            Point _point = Converter.RadiansToMeters(point, earth);
            double[] _estimatedParams = new double[]
            {
                 _point.lon,
                 _point.lat,
                 _point.alt,
                 velocity.E,
                 velocity.N,
                 velocity.H,
            };
            Vector estimatedParams = new Vector(_estimatedParams);


            Vector Z_ins = estimatedParams + H * X;

            double[] _snsErrorsSKO = new double[] {
                gnssMeasurements.constant.lon,
                gnssMeasurements.constant.lat,
                gnssMeasurements.constant.alt,
                gnssMeasurements.constant.E,
                gnssMeasurements.constant.N,
                gnssMeasurements.constant.H
            };
            Vector snsErrorsSKO = new Vector(_snsErrorsSKO);
            
            
            
            double[] _snsErrors = new double[] {
                gnssMeasurements.constant.lon * gnssMeasurements.noise.lon,
                gnssMeasurements.constant.lat * gnssMeasurements.noise.lat,
                gnssMeasurements.constant.alt * gnssMeasurements.noise.alt,
                gnssMeasurements.constant.E * gnssMeasurements.noise.E,
                gnssMeasurements.constant.N * gnssMeasurements.noise.N,
                gnssMeasurements.constant.H * gnssMeasurements.noise.H
            };
            Vector snsErrors = new Vector(_snsErrors);

            double[] _Z_sns = new double[]
            {
                _point.lon + snsErrors[1],
                _point.lat + snsErrors[2],
                _point.alt + snsErrors[3],
                velocity.E + snsErrors[4],
                velocity.N + snsErrors[5],
                velocity.H + snsErrors[6]
            };
            Vector Z_sns = new Vector(_Z_sns);

            Z = Z_ins - Z_sns;

            R = snsErrorsSKO.Diag() ^ 2 * (1.0 / insErrors.dt);


            //correctorIsGNSS = false;
        }
        private void InitZ(Point point, Velocity velocity, EarthModel earth, InsErrors insErrors, MeasurementsErrors svsMeasurements, int x = 1)
        {
            Z = Vector.Zero(6);
            Point _point = null;

            if (point.dimension != Dimension.Meters)
                _point = Converter.RadiansToMeters(point, earth);

            double[] _estimatedParams = new double[]
            {
                 _point.lon,
                 _point.lat,
                 _point.alt,
                 velocity.E,
                 velocity.N,
                 velocity.H,
            };
            Vector estimatedParams = new Vector(_estimatedParams);


            Vector Z_ins = estimatedParams + H * X;

            double[] _svsErrors = new double[]
            {
                svsMeasurements.constant.lon,
                svsMeasurements.constant.lat,
                svsMeasurements.constant.alt,
                svsMeasurements.constant.E,
                svsMeasurements.constant.N,
                svsMeasurements.constant.H
            };

            Vector svsErrors = new Vector(_svsErrors);

            double[] _Z_svs = new double[]
            {
                _point.lon + svsErrors[1],
                _point.lat + svsErrors[2],
                _point.alt + svsErrors[3],
                velocity.E + svsErrors[4],
                velocity.N + svsErrors[5],
                velocity.H + svsErrors[6]
            };
            Vector Z_svs = new Vector(_Z_svs);

            Z = Z_ins - Z_svs;

            //R = svsErrors.Diag() ^ 2 * (1.0 / insErrors.dt);
            Vector svsErrorsSKO = new Vector(new double[] { 100, 100, 100, 1, 1, 1 });
            R = svsErrorsSKO.Diag() ^ 2 * (1.0 / insErrors.dt);

            //correctorIsGNSS = true;
        }
    
        private void InitAnglesError(InsErrors initErrors)
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
        public void Model(Input input, Parameters parameters, MeasurementsErrors gnssMeasurments, MeasurementsErrors svsMeasurements)
        {
            if (X == null)
            {
                X_dot = Vector.Zero(21);
                InitAnglesError(input.INS);
                InitX(input.INS, parameters.point, parameters.absOmega, parameters.earthModel, parameters.velocity, parameters.angles, true);
            }
            else
            {
                InitX(input.INS, parameters.point, parameters.absOmega, parameters.earthModel, parameters.velocity, parameters.angles);
            }


            InitF(parameters.omegaGyro, parameters.absOmega, parameters.earthModel, parameters.acceleration, parameters.C, input.air);
            InitG(parameters.C);
            InitW(input.INS);


            X_dot = F * X + G * W;

            InitH(parameters.point, parameters.earthModel, parameters.absOmega, parameters.velocity);
            
            if(CorrectorIsGNSS)
                InitZ(parameters.point, parameters.velocity, parameters.earthModel, input.INS, gnssMeasurments);
            else
                InitZ(parameters.point, parameters.velocity, parameters.earthModel, input.INS, svsMeasurements, 1);


            Kalman(input.INS.dt);

        }
    }

}
