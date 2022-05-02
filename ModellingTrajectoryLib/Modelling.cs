using CommonLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModellingErrorsLib3;
using EstimateLib;

namespace ModellingTrajectoryLib
{
    public class Modelling
    {
        public OutputData outputData3 = new OutputData();
        public OutputData outputData2 = new OutputData();
        public OutputData feedbackOutput3 = new OutputData();
        public OutputData feedbackOutput2 = new OutputData();
        private KalmanFeedbackModel kalmanFeedbackModel3;
        private KalmanModel kalmanModel3;
        private KalmanModel2 kalmanModel2;

        int wayPointsCount;
        DesiredTrack desiredTrack;
        InitErrors initErrors;
        double dt;

        ModellingFunctions functions;
        List<Atmosphere> atmosphereData;
        public void GetOutputs(ref OutputData threeChannelOutput, ref OutputData twoChannelOutput, ref OutputData feedbackOut3, ref OutputData feedbackOut2)
        {
            Model();
            threeChannelOutput = outputData3;
            twoChannelOutput = outputData2;
            feedbackOut3 = feedbackOutput3;
            feedbackOut2 = feedbackOutput2;
        }
        private void CreateOutputDataLists(ref OutputData data)
        {
            data.points = new List<PointSet>();
            data.velocities = new List<VelocitySet>();
            data.angles = new List<AnglesSet>();
            data.p_OutList = new List<P_out>();
            data.airData = new List<AirData>();
        }
        public void Init(InputData input, InitErrors _initErrors)
        {
            dt = _initErrors.dt;
            desiredTrack = new DesiredTrack();
            input.latitude = Converter.DegToRad(input.latitude);
            input.longitude = Converter.DegToRad(input.longitude);

            wayPointsCount = input.latitude.Length;

            CreateOutputDataLists(ref outputData3);
            CreateOutputDataLists(ref outputData2);
            CreateOutputDataLists(ref feedbackOutput3);
            CreateOutputDataLists(ref feedbackOutput2);


            functions = new ModellingFunctions();
            kalmanModel2 = new KalmanModel2();
            kalmanFeedbackModel3 = new KalmanFeedbackModel();
            kalmanModel3 = new KalmanModel();
            desiredTrack.Init(input);

            functions.InitStartedData(input.latitude, input.longitude, input.altitude, input.velocity);
            functions.InitParamsBetweenPPM();
            initErrors = _initErrors;

            desiredTrack.FillOutputsData += AddParametersData;
            atmosphereData = Atmosphere.Read();
        }
        public void Model()
        {
            for (int wpNumber = 0; wpNumber < wayPointsCount - 1; wpNumber++)
            {
                functions.CheckParamsBetweenPPM(wpNumber);

                double LUR_Distance = functions.GetLUR(wpNumber, wayPointsCount - 2);
                double PPM_Distance = functions.GetPPM(wpNumber);
                double PPM_DisctancePrev;
                int countOfIncreasePPM = 0;
                while (LUR_Distance < PPM_Distance)
                {
                    desiredTrack.Track(wpNumber, dt, functions);
                    Kalman(dt);

                    PPM_DisctancePrev = PPM_Distance;
                    double ortDistAngleCurrent = functions.ComputeOrtDistAngle(desiredTrack.OutPoints.Ideal.Radians, wpNumber);
                    PPM_Distance = functions.GetPPM(ortDistAngleCurrent);

                    if (PPM_DisctancePrev < PPM_Distance)
                        countOfIncreasePPM++;
                    if (countOfIncreasePPM > 1)
                        break;
                }
                if (functions.TurnIsAvailable(wpNumber, wayPointsCount - 2))
                {
                    functions.InitTurnVariables(wpNumber, dt);

                    for (double j = 0; functions.TurnIsNotEnded(j); j += dt)
                    {
                        functions.SetTurnAngles(wpNumber, dt);
                        desiredTrack.Track(wpNumber, dt, functions);
                        Kalman(dt);
                    }
                }
                
                //outputData2.airData.Add(idealTrajectory.OutPoints);
            }
        }
        private void Kalman(double dt)
        {
            desiredTrack.Kalman(kalmanModel2, initErrors, functions, dt);
            //desiredTrack.Kalman(kalmanModel3, initErrors, functions, dt);
            //desiredTrack.Kalman(kalmanFeedbackModel3, initErrors, functions, dt);
        }
        public void AddParametersData(IKalman kalman)
        {
            if (counterAddPlotData % (1.0/dt) == 0)
            {
                if (kalman is KalmanFeedbackModel)
                    FillOutputData(feedbackOutput3);
                else if (kalman is KalmanModel2)
                    FillOutputData(outputData2);
                else if (kalman is KalmanModel)
                    FillOutputData(outputData3);
            }
            if (kalman is KalmanModel2)
                counterAddPlotData++;

        }
        private int counterAddPlotData = 0;
        private void FillOutputData(OutputData output)
        {
            output.points.Add(desiredTrack.OutPoints);
            output.velocities.Add(desiredTrack.OutVelocities);
            output.angles.Add(desiredTrack.OutAngles);
            if (output.p_OutList.Count < 400)
            {
                output.p_OutList.Add(desiredTrack.OutCovar);
            }
        }
    }
}
