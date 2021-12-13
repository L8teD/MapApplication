using ModellingTrajectoryLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CommonLib.Types;
using static DebugApp.Model.Types;
using static ModellingErrorsLib3.Types;

namespace DebugApp.Model
{
    class Execute
    {
        public static void CreateTrajectory(InitData initData, ref OutputData outputData, ref List<P_out> p_Outs,
            ref List<X_dot_out> x_Dot_Outs, ref List<MatlabData> matlabData) 
        {
            p_Outs = new List<P_out>();

            InputData inputData = new InputData();
            inputData.latitude = new double[initData.rtpList.Count];
            inputData.longitude = new double[initData.rtpList.Count];
            inputData.altitude = new double[initData.rtpList.Count];
            inputData.velocity = new double[initData.rtpList.Count];
            for (int i = 0; i < initData.rtpList.Count; i++)
            {
                inputData.latitude[i] = initData.rtpList[i].Latitude;
                inputData.longitude[i] = initData.rtpList[i].Longitude;
                inputData.altitude[i] = initData.rtpList[i].Altitude;
                inputData.velocity[i] = initData.rtpList[i].Velocity;
            }
            InitErrors initErrors = SetInitErrors(initData);

            Modelling model = new Modelling(inputData, initErrors);
            outputData.Points = model.points;
            outputData.Velocities = model.velocities;
            outputData.Angles = model.angles;

            FullDisplayedData fullDisplayedData = new FullDisplayedData();
            fullDisplayedData.ideal = model.dDataIdeal;
            fullDisplayedData.error= model.dDataError;
            fullDisplayedData.real= model.dDataReal;
            fullDisplayedData.estimated= model.dDataEstimate;
           
            outputData.FullDisplayedData = fullDisplayedData;

            p_Outs = model.p_Outs;
            x_Dot_Outs = model.x_Dot_Outs;
            matlabData = model.matlabData;
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
