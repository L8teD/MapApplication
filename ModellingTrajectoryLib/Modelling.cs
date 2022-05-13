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

        Input input;

        ModellingFunctions functions;
        public void GetOutputs(ref T_OutputFull output)
        {
            Model();
            output = Output;
        }

        public void Init(Input _input)
        {
            desiredTrack = new DesiredTrack();
            actualTrack = new ActualTrack();
            input = _input;
            input.trajectory.latitude = Converter.DegToRad(input.trajectory.latitude);
            input.trajectory.longitude = Converter.DegToRad(input.trajectory.longitude);

            wayPointsCount = input.trajectory.latitude.Length;

            CreateOutputData(ref Output.Default);
            CreateOutputData(ref Output.Feedback);


            functions = new ModellingFunctions();

            DesiredFeedbackKalman = new KalmanFeedbackModel();
            DesiredKalman = new KalmanModel();

            ActualFeedbackKalman = new KalmanFeedbackModel();
            ActualKalman = new KalmanModel();

            desiredTrack.Init(input);
            actualTrack.Init(input);

            functions.InitStartedData(input.trajectory.latitude, input.trajectory.longitude, input.trajectory.altitude, input.trajectory.velocity);
            functions.InitParamsBetweenPPM();


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
                    desiredTrack.Track(wpNumber, functions);
                    actualTrack.Track(wpNumber, functions);
                    Kalman();

                    PPM_DisctancePrev = PPM_Distance;
                    double ortDistAngleCurrent = functions.ComputeOrtDistAngle(desiredTrack.OutPoints.Ideal.GetValueOrDefault().Radians, wpNumber);
                    PPM_Distance = functions.GetPPM(ortDistAngleCurrent);

                    functions.CheckParamsBetweenPPM(wpNumber, desiredTrack.OutPoints.Ideal.GetValueOrDefault().Radians,
                        new Velocity(desiredTrack.OutVelocities.Ideal.GetValueOrDefault().E,
                                    desiredTrack.OutVelocities.Ideal.GetValueOrDefault().N,
                                    desiredTrack.OutVelocities.Ideal.GetValueOrDefault().H).module);

                    LUR_Distance = functions.GetLUR(wpNumber, wayPointsCount - 2);
                    if (PPM_DisctancePrev < PPM_Distance)
                        break;

                }
                if (functions.TurnIsAvailable(wpNumber, wayPointsCount - 2))
                {
                    functions.InitTurnVariables(wpNumber, input.INS.dt);

                    for (double j = 0; functions.TurnIsNotEnded(j); j += input.INS.dt)
                    {
                        functions.SetTurnAngles(wpNumber, input.INS.dt, desiredTrack.OutPoints.Ideal.GetValueOrDefault().Radians.alt);
                        desiredTrack.Track(wpNumber, functions);
                        actualTrack.Track(wpNumber, functions);
                        Kalman();
                    }
                }
            }
        }
        private void Kalman()
        {
            InputAirData desiredAirData = input.air;
            desiredAirData.tempratureError = 0;
            desiredTrack.Estimation(DesiredKalman, input.INS, desiredAirData, functions);
            //desiredTrack.Estimation(DesiredFeedbackKalman, initErrors, desiredAirData, functions, dt);

            actualTrack.Estimation(ActualKalman, input.INS, input.air, functions);
            //actualTrack.Estimation(ActualFeedbackKalman, initErrors, inputAirData, functions, dt);
        }
        private int counterAddPlotDataDes = 0;
        private int counterAddPlotDataAct = 0;
        public void FillDesiredTrackOutput(IKalman kalman)
        {
            if (counterAddPlotDataDes % (1.0 / input.INS.dt) == 0)
            {
                if (kalman is KalmanFeedbackModel)
                    FillOutputData(Output.Feedback.DesiredTrack, desiredTrack);
                else if (kalman is KalmanModel)
                    FillOutputData(Output.Default.DesiredTrack, desiredTrack);
            }
            if (kalman is KalmanModel)
                counterAddPlotDataDes++;

        }
        public void FillActualTrackOutput(IKalman kalman)
        {
            if (counterAddPlotDataAct % (1.0 / input.INS.dt) == 0)
            {
                if (kalman is KalmanFeedbackModel)
                    FillOutputData(Output.Feedback.ActualTrack, actualTrack);
                else if (kalman is KalmanModel)
                    FillOutputData(Output.Default.ActualTrack, actualTrack);
            }
            if (kalman is KalmanModel)
                counterAddPlotDataAct++;
        }

        private void FillOutputData(TrackData trackData, ITrajectory trajectory)
        {
            trackData.INS.points.Add(trajectory.OutPoints);
            trackData.INS.velocities.Add(trajectory.OutVelocities);
            trackData.INS.angles.Add(trajectory.OutAngles);
            trackData.INS.p_OutList.Add(trajectory.OutCovar);

            trackData.GNSS.points.Add(trajectory.GnssPoints);
            trackData.GNSS.velocities.Add(trajectory.GnssVelocities);

            trackData.KVS.points.Add(trajectory.KvsPoints);
            trackData.KVS.velocities.Add(trajectory.KvsVelocities);
        }
        private void CreateOutputData(ref OutputData data)
        {
            data = new OutputData();
            data.points = new List<PointSet>();
            data.velocities = new List<VelocitySet>();
            data.angles = new List<AnglesSet>();
            data.p_OutList = new List<P_out>();
        }
        private void CreateOutputData(ref TrackData data)
        {
            data = new TrackData();
            CreateOutputData(ref data.INS);
            CreateOutputData(ref data.GNSS);
            CreateOutputData(ref data.KVS);
        }
        private void CreateOutputData(ref T_Output data)
        {
            data = new T_Output();
            CreateOutputData(ref data.DesiredTrack);
            CreateOutputData(ref data.ActualTrack);
        }

    }
}
