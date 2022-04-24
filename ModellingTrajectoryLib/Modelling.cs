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
        public OutputData outputData = new OutputData();
        public OutputData outputData2 = new OutputData();

        int wayPointsCount;
        IdealTrajectory idealTrajectory;
        InitErrors initErrors;
        double dt = 1.0;

        ModellingFunctions functions;
        public void GetOutputs(ref OutputData threeChannelOutput, ref OutputData twoChannelOutput)
        {
            Model();
            threeChannelOutput = outputData;
            twoChannelOutput = outputData2;
        }
        public void Init(InputData input, InitErrors _initErrors)
        {
            idealTrajectory = new IdealTrajectory();
            input.latitude = Converter.DegToRad(input.latitude);
            input.longitude = Converter.DegToRad(input.longitude);

            wayPointsCount = input.latitude.Length;

            outputData.points = new List<PointSet>();
            outputData.velocities = new List<VelocitySet>();
            outputData.angles = new List<AnglesSet>();
            outputData.p_OutList = new List<P_out>();
            outputData.airData = new List<AirData>();

            outputData2.points = new List<PointSet>();
            outputData2.velocities = new List<VelocitySet>();
            outputData2.angles = new List<AnglesSet>();
            outputData2.p_OutList = new List<P_out>();
            outputData2.airData = new List<AirData>();

            functions = new ModellingFunctions();
            //parameters = new Parameters();
            //localParams = new List<Parameters>();

            idealTrajectory.Init(input);

            functions.InitStartedData(input.latitude, input.longitude, input.altitude, input.velocity);
            functions.InitParamsBetweenPPM();
            initErrors = _initErrors;

            idealTrajectory.FillOutputsData += AddParametersData;
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
                    idealTrajectory.ActualTrack(initErrors, wpNumber, dt, functions);

                    PPM_DisctancePrev = PPM_Distance;
                    double ortDistAngleCurrent = functions.ComputeOrtDistAngle(idealTrajectory.OutPoints.Ideal.Radians, wpNumber);
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
                        idealTrajectory.ActualTrack(initErrors, wpNumber, dt, functions);
                    }
                }
                
                //outputData2.airData.Add(idealTrajectory.OutPoints);
            }
        }
        public void AddParametersData()
        {
            outputData2.points.Add(idealTrajectory.OutPoints);
            outputData2.velocities.Add(idealTrajectory.OutVelocities);
            outputData2.angles.Add(idealTrajectory.OutAngles);
            outputData2.p_OutList.Add(idealTrajectory.OutCovar);
        }
    }
}
