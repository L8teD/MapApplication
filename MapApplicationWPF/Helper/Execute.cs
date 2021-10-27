using ModellingTrajectoryLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModellingTrajectoryLib.Helper;
using static MapApplicationWPF.Helper.Types;
using static CommonLib.Types;

namespace MapApplicationWPF.Helper
{
    class Execute
    {
        public static void CreateTrajectory(InitData initData, ref OutputData outputData)
        {
            InputData inputData = new InputData();
            inputData.latitude = new double[initData.ppmList.Count];
            inputData.longitude = new double[initData.ppmList.Count];
            inputData.altitude = new double[initData.ppmList.Count];
            inputData.velocity = new double[initData.ppmList.Count];
            for (int i = 0; i < initData.ppmList.Count; i++)
            {
                inputData.latitude[i] = initData.ppmList[i].Latitude;
                inputData.longitude[i] = initData.ppmList[i].Longitude;
                inputData.altitude[i] = initData.ppmList[i].Altitude;
                inputData.velocity[i] = initData.ppmList[i].Velocity;
            }
            Modelling model = new Modelling(inputData, initData.initErrors);
            outputData.Points = model.points;
            outputData.Velocities = model.velocities;
            FullDisplayedData fullDisplayedData = new FullDisplayedData();
            fullDisplayedData.DisplayedDatasIdeal = model.displayedDatasIdeal;
            fullDisplayedData.DisplayedDatasError = model.displayedDatasError;
            outputData.FullDisplayedData = fullDisplayedData;
        }
    }
}
