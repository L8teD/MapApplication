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

            Input input = new Input();

            SetInputs(initData, ref input);
            trajectoryModelling.Init(input);
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
        private static void SetInputs(InitData initData, ref Input input)
        {
            input.trajectory = SetTrajectoryInput(initData);
            input.air = SetAirData(initData);
            input.INS = SetInsErrors(initData);
            input.wind = SetWindData(initData);
            input.GNSS = SetGnssErrors(initData);
        }
        private static TrajectoryInput SetTrajectoryInput(InitData initData)
        {
            TrajectoryInput inputData = new TrajectoryInput();
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
            return inputData;
        }
        private static InputAirData SetAirData(InitData initData)
        {
            InputAirData airData = new InputAirData();

            airData.relativeAltitude = initData.airInfo[0].Value;
            airData.pressureError = initData.windInfo[3].Value;
            airData.tempratureError = initData.windInfo[4].Value;

            return airData;
        }
        private static InputWindData SetWindData(InitData initData)
        {
            InputWindData windData = new InputWindData();

            windData.wind_e = initData.windInfo[0].Value;
            windData.wind_n = initData.windInfo[1].Value;
            windData.wind_d = initData.windInfo[2].Value;

            windData.sigma_u = initData.windInfoDryden[0].Value;
            windData.sigma_v = initData.windInfoDryden[1].Value;
            windData.sigma_w = initData.windInfoDryden[2].Value;
            windData.L_u = initData.windInfoDryden[3].Value;
            windData.L_v = initData.windInfoDryden[4].Value;
            windData.L_w = initData.windInfoDryden[5].Value;

            return windData;
        }
        private static InsErrors SetInsErrors(InitData initData)
        {
            InsErrors insErrors = new InsErrors();

            insErrors.angleAccuracy.heading = initData.insErrors[0].Value;
            insErrors.angleAccuracy.pitch = initData.insErrors[1].Value;
            insErrors.angleAccuracy.roll = initData.insErrors[2].Value;

            insErrors.coordAccuracy.latitude = initData.insErrors[3].Value;
            insErrors.coordAccuracy.longitude = initData.insErrors[4].Value;
            insErrors.coordAccuracy.altitude = initData.insErrors[5].Value;

            insErrors.velocityAccuracy.east = initData.insErrors[6].Value;
            insErrors.velocityAccuracy.north = initData.insErrors[7].Value;
            insErrors.velocityAccuracy.H = initData.insErrors[8].Value;

            insErrors.dt = initData.insErrors[9].Value;

            insErrors.accelerationError.first = initData.sensorErrors[0].Value;
            insErrors.accelerationError.second = initData.sensorErrors[1].Value;
            insErrors.accelerationError.third = initData.sensorErrors[2].Value;

            insErrors.gyroError.first = Converter.DegToRad(initData.sensorErrors[3].Value);
            insErrors.gyroError.second = Converter.DegToRad(initData.sensorErrors[4].Value);
            insErrors.gyroError.third = Converter.DegToRad(initData.sensorErrors[5].Value);

            insErrors.accNoise = initData.sensorErrors[6].Value;
            insErrors.gyroNoise = Converter.DegToRad(initData.sensorErrors[7].Value);

            insErrors.temperatureCoef = initData.sensorErrors[8].Value;

            return insErrors;
        }
        private static GnssErrors SetGnssErrors(InitData initData)
        {
            GnssErrors gnss = new GnssErrors();

            gnss.sateliteErrorCoord = initData.gnssErrors[0].Value;
            gnss.sateliteErrorVelocity = initData.gnssErrors[1].Value;
            gnss.noise = initData.gnssErrors[2].Value;

            return gnss;
        }
    }
}
