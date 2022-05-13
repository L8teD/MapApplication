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
        void Init(Input input);
        void Track(int wpNumber, ModellingFunctions functions);
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
        protected IKalman kalmanModel;
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

        Randomize randomize;
        Input input;
        List<Parameters> localParams;

        CourseAirReckoning courseAir;
        public int WayPointsCount
        {
            get { return wayPointsCount; }
            private set { wayPointsCount = value; }
        }
        protected virtual int RandomSeed { get; set; }

        public void Init(Input _input)
        {
            randomize = new Randomize();
            randomize.Init(RandomSeed);
            localParams = new List<Parameters>();
            parameters.point = new Point(_input.trajectory.latitude[0], _input.trajectory.longitude[0], _input.trajectory.altitude[0], Dimension.Radians);
            localParams.Add(parameters);

            input = _input;

            courseAir = new CourseAirReckoning();
            courseAir.Init(parameters.point);
        }
        public void Track(int wpNumber, ModellingFunctions functions)
        {
            parameters.dt = input.INS.dt;

            InitNextPoint(ref parameters, localParams);

            ComputeParametersData(wpNumber, functions);

            courseAir.Model(ref parameters, input.wind, input.air, randomize, ref kvsPoints, ref kvsVelocities);

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
        
        public void Estimation(IKalman kalman, InsErrors initErrors,InputAirData airData, ModellingFunctions functions)
        {
            Matrix C = functions.CreateMatrixC(parameters);
            kalmanModel = kalman;
            kalmanModel.Model(input, parameters,C, randomize, ref gnssPoints, ref gnssVelocities);
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
