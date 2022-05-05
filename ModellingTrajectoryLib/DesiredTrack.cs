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
    public class DesiredTrack : BaseTrajectory, ITrajectory
    {
        int wayPointsCount;
        public Action<IKalman> FillOutputsData { get; set; }
        private Parameters parameters;
        List<Parameters> localParams;
        IKalman kalmanModel;

        public PointSet OutPoints
        {
            get
            {
                if (kalmanModel is KalmanModel2)
                    return new PointSet(parameters.point, kalmanModel.X, kalmanModel.X_estimate, parameters.earthModel, false);

                return new PointSet(parameters.point, kalmanModel.X, kalmanModel.X_estimate, parameters.earthModel);
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
                if (kalmanModel is KalmanModel2)
                    return new VelocitySet(parameters.velocity, kalmanModel.X, kalmanModel.X_estimate, false);
                return new VelocitySet(parameters.velocity, kalmanModel.X, kalmanModel.X_estimate);
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
                if (kalmanModel is KalmanModel2)
                    return new AnglesSet(parameters.angles, kalmanModel.X, false);
                return new AnglesSet(parameters.angles, kalmanModel.X);
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
                return parameters.airData;
                //return new AnglesSet(parameters.angles, kalmanModel.X, false);
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
                if (kalmanModel is KalmanModel2)
                {
                    p_Out.lon = Math.Sqrt(kalmanModel.P[1, 1]);
                    p_Out.lat = Math.Sqrt(kalmanModel.P[2, 2]);
                    p_Out.ve = Math.Sqrt(kalmanModel.P[3, 3]);
                    p_Out.vn = Math.Sqrt(kalmanModel.P[4, 4]);
                }
                else
                {
                    p_Out.lon = Math.Sqrt(kalmanModel.P[1, 1]);
                    p_Out.lat = Math.Sqrt(kalmanModel.P[2, 2]);
                    p_Out.alt = Math.Sqrt(kalmanModel.P[3, 3]);
                    p_Out.ve = Math.Sqrt(kalmanModel.P[4, 4]);
                    p_Out.vn = Math.Sqrt(kalmanModel.P[5, 5]);
                    p_Out.vh = Math.Sqrt(kalmanModel.P[6, 6]);
                }

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


        public void Init(InputData input)
        {
            Import.Init();
            localParams = new List<Parameters>();
            parameters.point = new Point(input.latitude[0], input.longitude[0], input.altitude[0], Dimension.Radians);
            localParams.Add(parameters);

            CourseAirReckoning.Init(parameters.point);
        }
        public void Track(int wpNumber, double dt, ModellingFunctions functions, InputWindData inputWindData, InputAirData inputAirData)
        {
            parameters.dt = dt;

            InitNextPoint(ref parameters, localParams);

            ComputeParametersData(wpNumber, functions);
            CourseAirReckoning.Model(ref parameters, inputWindData, inputAirData);

            //functions.CheckParamsBetweenPPM(wpNumber, parameters.point);

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
        public void Kalman(IKalman kalman, InitErrors initErrors, ModellingFunctions functions, double dt)
        {
            Matrix C = functions.CreateMatrixC(parameters);
            kalmanModel = kalman;
            kalmanModel.Model(initErrors, parameters, C, dt);
            FillOutputsData?.Invoke(kalman);
        }
    }
}
