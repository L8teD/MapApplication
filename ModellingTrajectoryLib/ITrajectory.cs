using CommonLib;
using CommonLib.Params;
using EstimateLib;
using ModellingErrorsLib3;
using MyMatrix;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModellingTrajectoryLib
{
    public interface ITrajectory
    {
        void Init(Input input, InputAirData air, InputWindData wind, InsErrors ins);
        void Track(int wpNumber, ModellingFunctions functions, DrydenInput drydenInput, ref MeasurementsErrors measurements, bool turnIsHappened);
        Action<IKalman> FillOutputsData { get; set; }
        PointSet OutPoints { get; set; }
        VelocitySet OutVelocities { get; set; }
        AnglesSet OutAngles { get; set; }
        P_out OutCovar { get; set; }

        PointSet GnssPoints { get; set; }
        PointSet KvsPoints { get; set; }
        VelocitySet GnssVelocities { get; set; }
        VelocitySet KvsVelocities { get; set; }
    }
    public abstract class BaseTrajectory : ITrajectory
    {
        
        public Action<IKalman> FillOutputsData { get; set; }
        protected Parameters parameters;
        public IKalman kalmanModel;
        public PointSet OutPoints
        {
            get
            {
                return new PointSet(parameters.point, kalmanModel.X, kalmanModel.X_estimate, parameters.earthModel);
            }
            set
            {
                OutPoints = value;
            }
        }
        public PointSet gnssPoints;
        public PointSet GnssPoints
        {
            get { return gnssPoints; }
            set { gnssPoints = value; }
        }
        public PointSet kvsPoints;
        public PointSet KvsPoints
        {
            get { return kvsPoints; }
            set { kvsPoints = value; }
        }
        public VelocitySet OutVelocities
        {
            get
            {
                return new VelocitySet(parameters.velocity, kalmanModel.X, kalmanModel.X_estimate);
            }
            set
            {
                OutVelocities = value;
            }
        }
        public VelocitySet gnssVelocities;
        public VelocitySet GnssVelocities
        {
            get { return gnssVelocities; }
            set { gnssVelocities = value; }
        }
        public VelocitySet kvsVelocities;
        public VelocitySet KvsVelocities
        {
            get { return kvsVelocities; }
            set { KvsVelocities = value; }
        }
        public AnglesSet OutAngles
        {
            get
            {
                return new AnglesSet(parameters.angles, kalmanModel.X);
            }
            set
            {
                OutAngles = value;
            }
        }
       
        public P_out OutCovar
        {
            get
            {
                P_out p_Out = new P_out();
                p_Out.lon = Math.Sqrt(kalmanModel.P[1, 1]);
                p_Out.lat = Math.Sqrt(kalmanModel.P[2, 2]);
                p_Out.alt = Math.Sqrt(kalmanModel.P[3, 3]);
                p_Out.ve = Math.Sqrt(kalmanModel.P[4, 4]);
                p_Out.vn = Math.Sqrt(kalmanModel.P[5, 5]);
                p_Out.vh = Math.Sqrt(kalmanModel.P[6, 6]);

                return p_Out;
            }
            set
            {
                OutCovar = value;
            }
        }

        int wayPointsCount;

        Input input;
        List<Parameters> localParams;

        CourseAirReckoning courseAir;
        public int WayPointsCount
        {
            get { return wayPointsCount; }
            private set { wayPointsCount = value; }
        }
        public void Init(Input _input, InputAirData air, InputWindData wind, InsErrors INS)
        {
            localParams = new List<Parameters>();
            parameters.point = new Point(_input.trajectory.latitude[0], _input.trajectory.longitude[0], _input.trajectory.altitude[0], Dimension.Radians);
            localParams.Add(parameters);

            input = _input;
            input.air = air;
            input.wind = wind;
            input.INS = INS;
            courseAir = new CourseAirReckoning();
            courseAir.Init(parameters.point, input.trajectory.altitude, air);
        }
        public void Track(int wpNumber, ModellingFunctions functions, DrydenInput drydenInput, ref MeasurementsErrors measurements, bool turnIsHappened)
        {
            parameters.dt = input.INS.dt;

            InitNextPoint(ref parameters, localParams);

            ComputeParametersData(wpNumber, functions);

            courseAir.Model(parameters, input.wind, ref input.air, drydenInput, ref kvsPoints, ref kvsVelocities, ref measurements, turnIsHappened);

            localParams.Add(parameters);
        }

        protected void ComputeParametersData(int wpNumber, ModellingFunctions functions)
        {
            functions.SetAngles(ref parameters, wpNumber);

            parameters.C = functions.CreateMatrixC(parameters);

            parameters.earthModel = new EarthModel(parameters.point);
            parameters.gravAcceleration = new GravitationalAcceleration(parameters.point);
            parameters.omegaEarth = new OmegaEarth(parameters.point);

            functions.SetVelocity(ref parameters, wpNumber);

            parameters.absOmega = new AbsoluteOmega(parameters);
            parameters.acceleration = new Acceleration(parameters);
            parameters.omegaGyro = new OmegaGyro(parameters);

            parameters.point = Point.GetCoords(parameters);
        }
        
        public void Estimation(IKalman kalman, bool correctorIsGNSS, MeasurementsErrors gnssMeasurments, MeasurementsErrors svsMeasurements, InsErrors insErrors)
        {
            gnssPoints = new PointSet();
            gnssVelocities = new VelocitySet();

            Point _point = Converter.RadiansToMeters(parameters.point, parameters.earthModel);

            gnssPoints.Error = new PointValue(
              new Point(
                  gnssMeasurments.SKO.lon * gnssMeasurments.noise.lon,
                  gnssMeasurments.SKO.lat * gnssMeasurments.noise.lat,
                  gnssMeasurments.SKO.alt * gnssMeasurments.noise.alt,
                  Dimension.Meters),
              parameters.earthModel,
              parameters.point);

            gnssPoints.Real = new PointValue(
                new Point(parameters.point.lat + gnssPoints.Error.Value.Meters.lat, 
                          parameters.point.lon + gnssPoints.Error.Value.Meters.lon,
                          parameters.point.alt + gnssPoints.Error.Value.Meters.alt, 
                          Dimension.Meters),
                parameters.earthModel,
                parameters.point);


            gnssVelocities.Error = new VelocityValue(
                gnssMeasurments.SKO.E * gnssMeasurments.noise.E, 
                gnssMeasurments.SKO.N * gnssMeasurments.noise.N,
                gnssMeasurments.SKO.H * gnssMeasurments.noise.H);

            gnssVelocities.Real = new VelocityValue(
                parameters.velocity.E + gnssVelocities.Error.Value.E,
                parameters.velocity.N + gnssVelocities.Error.Value.N, 
                parameters.velocity.H + gnssVelocities.Error.Value.H);

            svsMeasurements.constant.lat = kvsPoints.Error.Value.Meters.lat;
            svsMeasurements.constant.lon = kvsPoints.Error.Value.Meters.lon;
            svsMeasurements.constant.alt = kvsPoints.Error.Value.Meters.alt;
            svsMeasurements.constant.E = KvsVelocities.Error.Value.E;
            svsMeasurements.constant.N = KvsVelocities.Error.Value.N;
            svsMeasurements.constant.H = KvsVelocities.Error.Value.H;


            input.INS.gyroNoiseValue = insErrors.gyroNoiseValue;
            input.INS.accNoiseValue = insErrors.accNoiseValue;
            kalmanModel = kalman;
            kalman.CorrectorIsGNSS = correctorIsGNSS;
            kalmanModel.Model(input, parameters, gnssMeasurments, svsMeasurements);
            FillOutputsData?.Invoke(kalman);
        }
        protected void InitStartedPoint(ref Parameters parameters, TrajectoryInput input)
        {
            parameters.point = new Point(input.latitude[0], input.longitude[0], input.altitude[0], Dimension.Radians);
        }
        protected void InitNextPoint(ref Parameters parameters, List<Parameters> localParams)
        {
            
            parameters.point = localParams[localParams.Count - 1].point;
        }

    }
}
