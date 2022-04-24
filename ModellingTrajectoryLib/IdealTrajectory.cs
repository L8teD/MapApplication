using CommonLib;
using CommonLib.Params;
using ModellingErrorsLib3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EstimateLib;
using MyMatrix;

namespace ModellingTrajectoryLib
{
    public class IdealTrajectory : BaseTrajectory, ITrajectory
    {
        int wayPointsCount;
        public Action FillOutputsData { get; set; }
        private Parameters parameters;
        public OutputData outputData = new OutputData();
        public OutputData outputData2 = new OutputData();
        public PointSet OutPoints
        {
            get
            {
                return new PointSet(parameters.point, kalmanModel2.X, kalmanModel2.X_estimate, parameters.earthModel, false);
            }
            private set
            {
                OutPoints = value;
            }
        }
        public VelocitySet OutVelocities
        {
            get
            {
                return new VelocitySet(parameters.velocity, kalmanModel2.X, kalmanModel2.X_estimate, false);
            }
            private set
            {
                OutVelocities = value;
            }
        }
        public AnglesSet OutAngles
        {
            get
            {
                return new AnglesSet(parameters.angles, kalmanModel2.X, false);
            }
            private set
            {
                OutAngles = value;
            }
        }
        public AirData OutAirData
        {
            get
            {
                return default(AirData);
                //return new AnglesSet(parameters.angles, kalmanModel2.X, false);
            }
            private set
            {
                OutAirData = value;
            }
        }
        public P_out OutCovar
        {
            get
            {
                P_out p_Out = new P_out();
                p_Out.lon = kalmanModel2.P[1, 1];
                p_Out.lat = kalmanModel2.P[2, 2];
                p_Out.alt = 0;
                p_Out.ve = kalmanModel2.P[3, 3];
                p_Out.vn = kalmanModel2.P[4, 3];
                p_Out.vh = 0;
                return p_Out;
            }
            private set
            {
                OutCovar = value;
            }
        }
        public int WayPointsCount
        {
            get { return wayPointsCount; }
            private set { wayPointsCount = value; }
        }
        List<Parameters> localParams;
        KalmanModel2 kalmanModel2;

        public void Init(InputData input)
        {
            kalmanModel2 = new KalmanModel2();
            localParams = new List<Parameters>();
            parameters.point = new Point(input.latitude[0], input.longitude[0], input.altitude[0], Dimension.InRadians);
            localParams.Add(parameters);
            //InitStartedPoint(ref parameters, input);
        }
        public void ActualTrack(InitErrors initErrors, int wpNumber,double dt, ModellingFunctions functions)
        {
            InitNextPoint(ref parameters, localParams);
            AirData airData = new AirData();
            ComputeParametersData(ref parameters, ref airData, initErrors, wpNumber, dt, functions);
            localParams.Add(parameters);
            FillOutputsData?.Invoke();
        }

        protected void ComputeParametersData(ref Parameters parameters, ref AirData airData, InitErrors initErrors, int wpNumber, double dt, ModellingFunctions functions)
        {
            functions.SetAngles(ref parameters, wpNumber);

            Matrix C = functions.CreateMatrixC(parameters);

            parameters.earthModel = new EarthModel(parameters.point);
            parameters.gravAcceleration = new GravitationalAcceleration(parameters.point);
            parameters.omegaEarth = new OmegaEarth(parameters.point);

            functions.SetVelocity(ref parameters, wpNumber);

            parameters.absOmega = new AbsoluteOmega(parameters);
            parameters.acceleration = new Acceleration(parameters, C);
            parameters.omegaGyro = new OmegaGyro(parameters, C);


            //CourseAirReckoning(parameters, ref airData);
            parameters.point = Point.GetCoords(parameters, dt);


            //errorsModel.ModellingErrors(initErrors, parameters);

            //kalmanModel.Model(initErrors, parameters, C);
            kalmanModel2.Model(initErrors, parameters, C);
        }
    }
}
