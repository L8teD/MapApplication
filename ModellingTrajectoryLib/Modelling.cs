using CommonLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModellingErrorsLib3;
using EstimateLib;
using CommonLib.Params;

namespace ModellingTrajectoryLib
{
    public class Modelling
    {
        public T_OutputFull Output = new T_OutputFull();
        private IKalman ActualFeedbackKalman;
        private IKalman ActualKalman;
        private IKalman DesiredFeedbackKalman;
        private IKalman DesiredKalman;

        int wayPointsCount;
        BaseTrajectory desiredTrack;
        BaseTrajectory actualTrack;
        InitErrors initErrors;
        InputWindData inputWindData;
        InputAirData inputAirData;
        double dt;
        ModellingFunctions functions;
        public void GetOutputs(ref T_OutputFull output)
        {
            Model();
            output = Output;
        }
        private void CreateOutputDataLists(ref OutputData data)
        {
            data = new OutputData();
            data.points = new List<PointSet>();
            data.velocities = new List<VelocitySet>();
            data.angles = new List<AnglesSet>();
            data.p_OutList = new List<P_out>();
            data.airData = new List<AirData>();
        }
        public void Init(InputData input, InitErrors _initErrors, InputWindData _inputWindData, InputAirData _inputAirData)
        {
            dt = _initErrors.dt;
            desiredTrack = new DesiredTrack();
            actualTrack = new ActualTrack();
            input.latitude = Converter.DegToRad(input.latitude);
            input.longitude = Converter.DegToRad(input.longitude);

            wayPointsCount = input.latitude.Length;

            CreateOutputDataLists(ref Output.DesiredTrack.Feedback);
            CreateOutputDataLists(ref Output.DesiredTrack.Default);
            CreateOutputDataLists(ref Output.ActualTrack.Feedback);
            CreateOutputDataLists(ref Output.ActualTrack.Default);


            functions = new ModellingFunctions();

            DesiredFeedbackKalman = new KalmanFeedbackModel();
            DesiredKalman = new KalmanModel();

            ActualFeedbackKalman = new KalmanFeedbackModel();
            ActualKalman = new KalmanModel();

            desiredTrack.Init(input, inputAirData, inputWindData);
            actualTrack.Init(input, inputAirData, inputWindData);

            functions.InitStartedData(input.latitude, input.longitude, input.altitude, input.velocity);
            functions.InitParamsBetweenPPM();

            initErrors = _initErrors;
            inputWindData = _inputWindData;
            inputAirData = _inputAirData;
            desiredTrack.FillOutputsData += FillDesiredTrackOutput;
            actualTrack.FillOutputsData += FillActualTrackOutput;
        }
        public void Model()
        {
            for (int wpNumber = 0; wpNumber < wayPointsCount - 1; wpNumber++)
            {
                double LUR_Distance = functions.GetLUR(wpNumber, wayPointsCount - 2);
                double PPM_Distance = functions.GetPPM(wpNumber);
                double PPM_DisctancePrev;
                while (LUR_Distance < PPM_Distance)
                {
                    desiredTrack.Track(wpNumber, dt, functions);
                    actualTrack.Track(wpNumber, dt, functions);
                    Kalman(dt);

                    PPM_DisctancePrev = PPM_Distance;
                    double ortDistAngleCurrent = functions.ComputeOrtDistAngle(desiredTrack.OutPoints.Ideal.Radians, wpNumber);
                    PPM_Distance = functions.GetPPM(ortDistAngleCurrent);

                    functions.CheckParamsBetweenPPM(wpNumber, desiredTrack.OutPoints.Ideal.Radians,
                        new Velocity(desiredTrack.OutVelocities.Ideal.E, desiredTrack.OutVelocities.Ideal.N, desiredTrack.OutVelocities.Ideal.H).module);

                    LUR_Distance = functions.GetLUR(wpNumber, wayPointsCount - 2);
                    if (PPM_DisctancePrev < PPM_Distance)
                        break;

                }
                if (functions.TurnIsAvailable(wpNumber, wayPointsCount - 2))
                {
                    functions.InitTurnVariables(wpNumber, dt);

                    for (double j = 0; functions.TurnIsNotEnded(j); j += dt)
                    {
                        functions.SetTurnAngles(wpNumber, dt, desiredTrack.OutPoints.Ideal.Radians.alt);
                        desiredTrack.Track(wpNumber, dt, functions);
                        actualTrack.Track(wpNumber, dt, functions);
                        Kalman(dt);
                    }
                }
            }
        }
        private void Kalman(double dt)
        {
            InputAirData desiredAirData = inputAirData;
            desiredAirData.tempratureError = 0;
            desiredTrack.Estimation(DesiredKalman, initErrors, desiredAirData, functions, dt);
            desiredTrack.Estimation(DesiredFeedbackKalman, initErrors, desiredAirData, functions, dt);

            actualTrack.Estimation(ActualKalman, initErrors, inputAirData, functions, dt);
            actualTrack.Estimation(ActualFeedbackKalman, initErrors, inputAirData, functions, dt);
        }
        private int counterAddPlotDataDes = 0;
        private int counterAddPlotDataAct = 0;
        public void FillDesiredTrackOutput(IKalman kalman)
        {
            if (counterAddPlotDataDes % (1.0 / dt) == 0)
            {
                if (kalman is KalmanFeedbackModel)
                    FillOutputData(Output.DesiredTrack.Feedback, desiredTrack);
                else if (kalman is KalmanModel)
                    FillOutputData(Output.DesiredTrack.Default, desiredTrack);
            }
            if (kalman is KalmanModel)
                counterAddPlotDataDes++;

        }
        public void FillActualTrackOutput(IKalman kalman)
        {
            if (counterAddPlotDataAct % (1.0 / dt) == 0)
            {
                if (kalman is KalmanFeedbackModel)
                    FillOutputData(Output.ActualTrack.Feedback, actualTrack);
                else if (kalman is KalmanModel)
                    FillOutputData(Output.ActualTrack.Default, actualTrack);
            }
            if (kalman is KalmanModel)
                counterAddPlotDataAct++;
        }

        private void FillOutputData(OutputData output, ITrajectory trajectory)
        {
            output.points.Add(trajectory.OutPoints);
            output.velocities.Add(trajectory.OutVelocities);
            output.angles.Add(trajectory.OutAngles);
            output.airData.Add(trajectory.OutAirData);
            output.p_OutList.Add(trajectory.OutCovar);
        }
    }
}
