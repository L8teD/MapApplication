using ModellingTrajectoryLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLib;
using ModellingErrorsLib3;

namespace MapApplication.Model.Helper
{
    class Execute
    {
        static Modelling trajectoryModelling;
        private static void Init(InitData initData)
        {
            if (trajectoryModelling == null)
                trajectoryModelling = new Modelling();

            InputData input;
            InitErrors initErrors = new InitErrors();
            InputWindData inputWindData = new InputWindData();
            InputAirData inputAirData = new InputAirData();
            SetInputs(initData, ref input, ref initErrors, ref inputWindData, ref inputAirData);
            trajectoryModelling.Init(input, initErrors, inputWindData, inputAirData);
        }
        //private static void GetOutputs(ref OutputData threeChannelOutput, ref OutputData twoChannelOutput, ref OutputData feedbackOutput3, ref OutputData feedbackOutput2)
        //{
        //    trajectoryModelling.GetOutputs(ref threeChannelOutput, ref twoChannelOutput, ref feedbackOutput3, ref feedbackOutput2);
        //}
        public static void CreateTrajectory(InitData initData, ref T_OutputFull Output)
        {
            Init(initData);
            trajectoryModelling.GetOutputs(ref Output);
        }
        private static void SetInputs(InitData initData, ref InputData inputData, ref InitErrors initErrors, ref InputWindData windData, ref InputAirData inputAirData)
        {
            inputData = new InputData();
            inputData.latitude = new double[initData.wayPointList.Count];
            inputData.longitude = new double[initData.wayPointList.Count];
            inputData.altitude = new double[initData.wayPointList.Count];
            inputData.velocity = new double[initData.wayPointList.Count];
            for (int i = 0; i < initData.wayPointList.Count; i++)
            {
                inputData.latitude[i] = initData.wayPointList[i].Latitude;
                inputData.longitude[i] = initData.wayPointList[i].Longitude;
                inputData.altitude[i] = initData.wayPointList[i].Altitude;
                inputData.velocity[i] = initData.wayPointList[i].Velocity;
            }
            inputAirData = SetAirData(initData);
            initErrors = SetInitErrors(initData);
            windData = SetWindData(initData);
        }
        private static InputAirData SetAirData(InitData initData)
        {
            InputAirData airData = new InputAirData();

            airData.relativeAltitude = initData.airInfo[0].Value;
            airData.pressureError = initData.windInfo[4].Value;
            airData.tempratureError = initData.windInfo[5].Value;

            return airData;
        }
        private static InputWindData SetWindData(InitData initData)
        {
            InputWindData windData = new InputWindData();

            windData.angle = Converter.DegToRad(initData.windInfo[0].Value);
            windData.wind_e = initData.windInfo[1].Value;
            windData.wind_n = initData.windInfo[2].Value;
            windData.wind_d = initData.windInfo[3].Value;

            windData.sigma_u = initData.windInfoDryden[0].Value;
            windData.sigma_v = initData.windInfoDryden[1].Value;
            windData.sigma_w = initData.windInfoDryden[2].Value;
            windData.L_u = initData.windInfoDryden[3].Value;
            windData.L_v = initData.windInfoDryden[4].Value;
            windData.L_w = initData.windInfoDryden[5].Value;

            return windData;
        }
        private static InitErrors SetInitErrors(InitData initData)
        {
            InitErrors initErrors = new InitErrors();

            initErrors.angleAccuracy.heading = initData.insErrors[0].Value;
            initErrors.angleAccuracy.pitch = initData.insErrors[1].Value;
            initErrors.angleAccuracy.roll = initData.insErrors[2].Value;

            initErrors.coordAccuracy.latitude = initData.insErrors[3].Value;
            initErrors.coordAccuracy.longitude = initData.insErrors[4].Value;
            initErrors.coordAccuracy.altitude = initData.insErrors[5].Value;

            initErrors.velocityAccuracy.east = initData.insErrors[6].Value;
            initErrors.velocityAccuracy.north = initData.insErrors[7].Value;
            initErrors.velocityAccuracy.H = initData.insErrors[8].Value;

            initErrors.accelerationError.first = initData.sensorErrors[0].Value;
            initErrors.accelerationError.second = initData.sensorErrors[1].Value;
            initErrors.accelerationError.third = initData.sensorErrors[2].Value;

            initErrors.gyroError.first = Converter.DegToRad(initData.sensorErrors[3].Value);
            initErrors.gyroError.second = Converter.DegToRad(initData.sensorErrors[4].Value);
            initErrors.gyroError.third = Converter.DegToRad(initData.sensorErrors[5].Value);

            initErrors.accNoise = initData.sensorErrors[6].Value;
            initErrors.gyroNoise = Converter.DegToRad(initData.sensorErrors[7].Value);
            initErrors.snsNoise = initData.sensorErrors[8].Value;

            initErrors.dt = initData.sensorErrors[9].Value;

            return initErrors;
        }
    }
}
