using CommonLib;
using CommonLib.Params;
using ModellingErrorsLib3;
using ModellingTrajectoryLib.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EstimateLib;
using static CommonLib.Types;
using static ModellingErrorsLib3.Types;
using static ModellingTrajectoryLib.Types;
using MyMatrix;

namespace ModellingTrajectoryLib
{
    class TrajectoryModel
    {
        ModellingFunctions functions = new ModellingFunctions();
        ErrorsModel errorsModel = new ErrorsModel();
        KalmanModel2 kalmanModel = new KalmanModel2();
        private List<Parameters> localParams = new List<Parameters>();


        public List<PointSet> pointsList = new List<PointSet>();
        public List<VelocitySet> velocityList = new List<VelocitySet>();
        public List<AnglesSet> anglesList = new List<AnglesSet>();
        public List<DisplayedData> dDataIdeal = new List<DisplayedData>();
        public List<DisplayedData> dDataError = new List<DisplayedData>();
        public List<DisplayedData> dDataReal = new List<DisplayedData>();
        public List<DisplayedData> dDataEstimate = new List<DisplayedData>();
        public List<DisplayedData> dDataCorrect = new List<DisplayedData>();
        public List<P_out> p_Outs = new List<P_out>();
        public List<X_dot_out> x_Dot_Outs = new List<X_dot_out>();
        public List<MatlabData> matlabData = new List<MatlabData>();
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

            Matrix C = functions.CreateMatrixC(parameters);

            parameters.earthModel = new EarthModel(parameters.point);
            parameters.gravAcceleration = new GravitationalAcceleration(parameters.point);
            parameters.omegaEarth = new OmegaEarth(parameters.point);

            functions.SetVelocity(ref parameters, k);

            parameters.absOmega = new AbsoluteOmega(parameters);
            parameters.acceleration = new Acceleration(parameters, C);
            parameters.omegaGyro = new OmegaGyro(parameters, C);
            parameters.point = Point.GetCoords(parameters, dt);

            errorsModel.ModellingErrors(initErrors, parameters);
            kalmanModel.Model(initErrors, parameters, C); //ПРОВЕРИТЬ ЭЛЕМЕНТЫ F

            //pointsList.Add(new PointSet(parameters, errorsModel.X));
            pointsList.Add(new PointSet(parameters, kalmanModel.X));
            velocityList.Add(new VelocitySet(parameters.velocity, kalmanModel.X));
            anglesList.Add(new AnglesSet(parameters.angles, kalmanModel.orientationAngles));

            int index = pointsList.Count - 1;

            dDataIdeal.Add(
                new DisplayedData(pointsList[index].InDegrees,velocityList[index].Value, anglesList[index].Value));

            dDataError.Add(
                new DisplayedData(pointsList[index].ErrorInMeters, velocityList[index].Error, anglesList[index].Error));

            dDataReal.Add(
                new DisplayedData(pointsList[pointsList.Count - 1].InDegreesWithError,
                velocityList[velocityList.Count - 1].ValueWithError, anglesList[index].WithError));

            Point estPoint = new Point(kalmanModel.X_estimate[2], kalmanModel.X_estimate[1], 0.0);
            VelocityValue estVelocityValue = new VelocityValue(kalmanModel.X_estimate[3], kalmanModel.X_estimate[4], 0.0, 0.0);
            //Angles estAngles = new Angles() { heading = kalmanModel.X_estimate[6][0], roll = kalmanModel.X_estimate[7][0], pitch = kalmanModel.X_estimate[8][0] };
            Angles estAngles = new Angles() { heading = kalmanModel.orientationAngles[1], roll = kalmanModel.orientationAngles[2], pitch = kalmanModel.orientationAngles[3] };
            dDataEstimate.Add(new DisplayedData(estPoint, estVelocityValue, estAngles));

            P_out p_Out = new P_out();
            p_Out.lon = kalmanModel.P[1,1];
            p_Out.lat = kalmanModel.P[2,2];
            //p_Out.alt = kalmanModel.P[2][2];
            p_Out.ve = kalmanModel.P[3,3];
            p_Out.vn = kalmanModel.P[4,4];
            //p_Out.vh = kalmanModel.P[4][4];
            p_Outs.Add(p_Out);

            X_dot_out x_Dot_Out = new X_dot_out();
            x_Dot_Out.lon = kalmanModel.X_dot[1];
            x_Dot_Out.lat = kalmanModel.X_dot[2];
            x_Dot_Out.ve= kalmanModel.X_dot[3];
            x_Dot_Out.vn = kalmanModel.X_dot[4];
            x_Dot_Outs.Add(x_Dot_Out);

            MatlabData mData = new MatlabData();
            mData.lat = parameters.point.lat;
            mData.lon = parameters.point.lon;
            mData.heading = parameters.angles.heading;
            mData.roll = parameters.angles.roll;
            mData.Ve = parameters.velocity.E;
            mData.Vn = parameters.velocity.N;
            
            mData.R1 = parameters.earthModel.R1;
            mData.R2 = parameters.earthModel.R2;
            
            mData.aw_e = parameters.absOmega.E;
            mData.aw_n = parameters.absOmega.N;
            mData.aw_h = parameters.absOmega.H;
            mData.alfa = kalmanModel.orientationAngles[1];
            mData.betta = kalmanModel.orientationAngles[2];
            mData.gamma = kalmanModel.orientationAngles[3];
            
            mData.accE = parameters.acceleration.E;
            mData.accN = parameters.acceleration.N;
            mData.accH = parameters.acceleration.H;
            mData.w_x = parameters.omegaGyro.X;
            mData.w_y = parameters.omegaGyro.Y;
            mData.w_z = parameters.omegaGyro.Z;
            mData.n_x = parameters.acceleration.X;
            mData.n_y = parameters.acceleration.Y;
            mData.n_z = parameters.acceleration.Z;
            mData.dot_omega_h = parameters.omegaGyro.Z_dot;
            matlabData.Add(mData);

            localParams.Add(parameters);
        }
    }
}
