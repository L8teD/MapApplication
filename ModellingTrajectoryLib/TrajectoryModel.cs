using CommonLib;
using CommonLib.Params;
using ModellingErrorsLib;
using ModellingTrajectoryLib.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CommonLib.Types;
using static ModellingErrorsLib.Types;
using static ModellingTrajectoryLib.Types;

namespace ModellingTrajectoryLib
{
    class TrajectoryModel
    {
        ModellingFunctions functions = new ModellingFunctions();
        ErrorsModel errorsModel = new ErrorsModel();
        private List<Parameters> localParams = new List<Parameters>();


        public List<PointSet> outputPointsList = new List<PointSet>();
        public List<VelocitySet> outputVelocityList = new List<VelocitySet>();
        public List<DisplayedData> outputDisplayedDataIdeal = new List<DisplayedData>();
        public List<DisplayedData> outputDisplayedDataError = new List<DisplayedData>();
        public void Model(double[] latArray, double[] lonArray, double[] altArray, double[] velocity, InitErrors initErrors)
        {
            int inputPointsCount = latArray.Length;

            functions.InitStartedData(latArray, lonArray, altArray, velocity);
            functions.InitParamsBetweenPPM();

            double dt = 1;
            bool Init = true;
            
            for (int k = 0; k < inputPointsCount - 1; k++)
            {
                functions.CheckParamsBetweenPPM(k);

                double LUR_Distance = functions.GetLUR(k, inputPointsCount - 2);
                double PPM_Distance = functions.GetPPM(k);
                double PPM_DisctancePrev;
                int countOfIncreasePPM = 0;
                while (LUR_Distance < PPM_Distance )
                {
                    Parameters parameters = new Parameters();

                    if (Init)
                    {
                        parameters.point = new Point(latArray[0], lonArray[0], altArray[0]);
                        Init = false;
                    }
                    else
                    {
                        parameters.point = localParams[localParams.Count - 1].point;
                    }
                    //functions.RecountWind(parameters, k);

                    ComputeParametersData(ref parameters, initErrors, k, dt);


                    PPM_DisctancePrev = PPM_Distance;
                    double ortDistAngleCurrent = functions.ComputeOrtDistAngle(parameters, k);
                    PPM_Distance = functions.GetPPM(ortDistAngleCurrent);

                    if (PPM_DisctancePrev < PPM_Distance)
                        countOfIncreasePPM++;
                    if (countOfIncreasePPM > 1)
                        break;

                }
                if (functions.TurnIsAvailable(k, inputPointsCount - 2))
                {
                    Parameters parameters = new Parameters();
                    parameters.point = localParams[localParams.Count - 1].point;

                    functions.InitTurnVariables(k, dt);

                    for (double j = 0; functions.TurnIsNotEnded(j); j += dt)
                    {
                        //functions.RecountWind(parameters, k);
                        functions.SetTurnAngles(k, dt);
                        ComputeParametersData(ref parameters, initErrors, k, dt);
                    }
                }

            }
        }
        private void ComputeParametersData(ref Parameters parameters, InitErrors initErrors, int k, double dt)
        {
            functions.SetAngles(ref parameters, k);

            double[][] C = functions.CreateMatrixC(parameters);

            parameters.earthModel = new EarthModel(parameters.point);
            parameters.gravAcceleration = new GravitationalAcceleration(parameters.point);
            parameters.omegaEarth = new OmegaEarth(parameters.point);

            functions.SetVelocity(ref parameters, k);

            parameters.absOmega = new AbsoluteOmega(parameters);
            parameters.acceleration = new Acceleration(parameters, C);
            parameters.omegaGyro = new OmegaGyro(parameters, C);
            parameters.point = Point.GetCoords(parameters, dt);

            errorsModel.ModellingErrors(initErrors, parameters);

            outputPointsList.Add(new PointSet(parameters, errorsModel.X));
            outputVelocityList.Add(new VelocitySet(parameters.velocity, errorsModel.X));



            outputDisplayedDataIdeal.Add(new DisplayedData(outputPointsList[outputPointsList.Count - 1].InDegrees,
                outputVelocityList[outputVelocityList.Count - 1].Value, parameters.angles));
            outputDisplayedDataError.Add(new DisplayedData(outputPointsList[outputPointsList.Count - 1].ErrorInDegrees,
                outputVelocityList[outputVelocityList.Count - 1].Error,
                new Angles() { heading = errorsModel.anglesErrors[0][0], pitch = errorsModel.anglesErrors[1][0], roll = errorsModel.anglesErrors[2][0] }));

            localParams.Add(parameters);
        }
    }
}
