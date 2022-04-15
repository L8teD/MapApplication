using CommonLib;
using CommonLib.Params;
using ModellingErrorsLib3;
using ModellingTrajectoryLib.Helper;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EstimateLib;
using CommonLib;
using ModellingErrorsLib3;
using ModellingTrajectoryLib;
using MyMatrix;

namespace ModellingTrajectoryLib
{
    class TrajectoryModel
    {
        ModellingFunctions functions = new ModellingFunctions();
        ErrorsModel errorsModel = new ErrorsModel();
        KalmanModel kalmanModel = new KalmanModel();
        KalmanModel2 kalmanModel2 = new KalmanModel2();
        private List<Parameters> localParams = new List<Parameters>();


        public OutputData outputData = new OutputData();
        public OutputData outputData2 = new OutputData();
        
        public void Model(double[] latArray, double[] lonArray, double[] altArray, double[] velocity, InitErrors initErrors)
        {
            outputData.points = new List<PointSet>();
            outputData.velocities = new List<VelocitySet>();
            outputData.angles = new List<AnglesSet>();
            outputData.p_OutList = new List<P_out>();

            outputData2.points = new List<PointSet>();
            outputData2.velocities = new List<VelocitySet>();
            outputData2.angles = new List<AnglesSet>();
            outputData2.p_OutList = new List<P_out>();

            int inputPointsCount = latArray.Length;

            functions.InitStartedData(latArray, lonArray, altArray, velocity);
            functions.InitParamsBetweenPPM();

            double dt = 1;
            bool Init = true;
            
            for (int wpNumber = 0; wpNumber < inputPointsCount - 1; wpNumber++)
            {
                functions.CheckParamsBetweenPPM(wpNumber);

                double LUR_Distance = functions.GetLUR(wpNumber, inputPointsCount - 2);
                double PPM_Distance = functions.GetPPM(wpNumber);
                double PPM_DisctancePrev;
                int countOfIncreasePPM = 0;
                while (LUR_Distance < PPM_Distance )
                {
                    Parameters parameters = new Parameters();

                    CheckOfInitalizationStartedPoint(ref parameters, ref Init, latArray, lonArray, altArray, wpNumber);
                    //functions.RecountWind(parameters, k);

                    ComputeParametersData(ref parameters, initErrors, wpNumber, dt);


                    PPM_DisctancePrev = PPM_Distance;
                    double ortDistAngleCurrent = functions.ComputeOrtDistAngle(parameters, wpNumber);
                    PPM_Distance = functions.GetPPM(ortDistAngleCurrent);

                    if (PPM_DisctancePrev < PPM_Distance)
                        countOfIncreasePPM++;
                    if (countOfIncreasePPM > 1)
                        break;

                }
                if (functions.TurnIsAvailable(wpNumber, inputPointsCount - 2))
                {
                    Parameters parameters = new Parameters();

                    CheckOfInitalizationStartedPoint(ref parameters, ref Init, latArray, lonArray, altArray, wpNumber);

                    functions.InitTurnVariables(wpNumber, dt);

                    for (double j = 0; functions.TurnIsNotEnded(j); j += dt)
                    {
                        //functions.RecountWind(parameters, k);
                        functions.SetTurnAngles(wpNumber, dt);
                        ComputeParametersData(ref parameters, initErrors, wpNumber, dt);
                    }
                    //Init = true;
                }

            }  

        }
        private void ComputeParametersData(ref Parameters parameters, InitErrors initErrors, int wpNumber, double dt)
        {
            //Trace.WriteLine(k.ToString() + "   '''Compute trajectory'''    " + DateTime.Now.ToShortTimeString());
            functions.SetAngles(ref parameters, wpNumber);

            Matrix C = functions.CreateMatrixC(parameters);

            parameters.earthModel = new EarthModel(parameters.point);
            parameters.gravAcceleration = new GravitationalAcceleration(new Point(10,10,10,Dimension.InDegrees));
            parameters.omegaEarth = new OmegaEarth(parameters.point);

            functions.SetVelocity(ref parameters, wpNumber);

            parameters.absOmega = new AbsoluteOmega(parameters);
            parameters.acceleration = new Acceleration(parameters, C);
            parameters.omegaGyro = new OmegaGyro(parameters, C);
            parameters.point = Point.GetCoords(parameters, dt, Dimension.InRadians);

            //errorsModel.ModellingErrors(initErrors, parameters);

            kalmanModel.Model(initErrors, parameters, C);
            kalmanModel2.Model(initErrors, parameters, C);

            outputData.points.Add(new PointSet(
                parameters.point, errorsModel.X, kalmanModel.X_estimate, parameters.earthModel));
            //outputData.points.Add(new PointSet(
                //parameters.point, errorsModel.X, kalmanModel.X_estimate, parameters.earthModel));

            outputData.velocities.Add(new VelocitySet(parameters.velocity, kalmanModel.X, kalmanModel.X_estimate));
            //outputData.velocities.Add(new VelocitySet(parameters.velocity, errorsModel.X, kalmanModel.X_estimate));

            outputData.angles.Add(new AnglesSet(parameters.angles, kalmanModel.X));

            outputData2.points.Add(new PointSet(
                parameters.point, kalmanModel2.X, kalmanModel2.X_estimate, parameters.earthModel, false));
            //outputData2.points.Add(new PointSet(
                //parameters.point, errorsModel.X, kalmanModel2.X_estimate, parameters.earthModel, false));

            outputData2.velocities.Add(new VelocitySet(parameters.velocity, kalmanModel2.X, kalmanModel2.X_estimate, false));
            //outputData2.velocities.Add(new VelocitySet(parameters.velocity, errorsModel.X, kalmanModel2.X_estimate, false));

            outputData2.angles.Add(new AnglesSet(parameters.angles, kalmanModel2.X, false));


            P_out p_Out = new P_out();
            p_Out.lon = kalmanModel.P[1, 1];
            p_Out.lat = kalmanModel.P[2, 2];
            p_Out.alt = kalmanModel.P[3, 3];
            p_Out.ve = kalmanModel.P[4, 4];
            p_Out.vn = kalmanModel.P[5, 5];
            p_Out.vh = kalmanModel.P[6, 6];
            outputData.p_OutList.Add(p_Out);


            p_Out = new P_out();
            p_Out.lon = kalmanModel2.P[1, 1];
            p_Out.lat = kalmanModel2.P[2, 2];
            p_Out.alt = 0;
            p_Out.ve = kalmanModel2.P[3, 3];
            p_Out.vn = kalmanModel2.P[4, 3];
            p_Out.vh = 0;
            outputData2.p_OutList.Add(p_Out);


            localParams.Add(parameters);

        }
        private void CheckOfInitalizationStartedPoint(ref Parameters parameters, ref bool Init, double[] latArray, double[] lonArray, double[] altArray, int k)
        {
            if (Init)
            {
                parameters.point = new Point(latArray[k], lonArray[k], altArray[k], Dimension.InRadians);
                Init = false;
            }
            else
            {
                parameters.point = localParams[localParams.Count - 1].point;
            }
        }
    }
}
