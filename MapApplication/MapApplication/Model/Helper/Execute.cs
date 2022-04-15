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
        public static void CreateTrajectory(InitData initData, ref OutputData outputData, ref OutputData outputData2)
        {
            InputData inputData = new InputData();
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
            InitErrors initErrors = SetInitErrors(initData);

            Modelling model = new Modelling(inputData, initErrors);
            outputData = model.outputData;
            outputData2 = model.outputData2;
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

            initErrors.gyroError.first = initData.sensorErrors[3].Value;
            initErrors.gyroError.second = initData.sensorErrors[4].Value;
            initErrors.gyroError.third = initData.sensorErrors[5].Value;

            return initErrors;
        }
    }
}
