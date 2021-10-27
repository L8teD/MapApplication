using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Controls;
using MapApplicationWPF.ExternalResourses;
using MapApplicationWPF.Helper;
using static MapApplicationWPF.Helper.Types;


namespace MapApplicationWPF.Graphic
{
    class ListViewWorker
    {
        public static void UpdateData(InitData initData)
        {
            initData.ppmList.Clear();
            for (int i = 0; i < MapElementWorker.choosenAirportsIcons.Count(); i++)
            {
                PPM ppm = new PPM();
                ppm.ID = i + 1;
                ppm.AirportName = MapElementWorker.choosenAirportsIcons[i].Title;
                
                ppm.Latitude = MapElementWorker.choosenAirportsIcons[i].Location.Position.Latitude;
                ppm.Longitude = MapElementWorker.choosenAirportsIcons[i].Location.Position.Longitude;

                ppm.Velocity = 850;
                ppm.Altitude = 1500;
                
                //ppm.TargetArrivalTime = DateTime.Now.ToLongTimeString();

                initData.ppmList.Add(ppm);
            }
        }
        public static void SaveInitDataHandler(ref InitData initData, ObservableCollection<InputError> inputErrorsList)
        {
            initData.initErrors.angleAccuracy.heading = inputErrorsList[0].Value;
            initData.initErrors.angleAccuracy.roll = inputErrorsList[1].Value;
            initData.initErrors.angleAccuracy.pitch = inputErrorsList[2].Value;

            initData.initErrors.coordAccuracy.latitude = inputErrorsList[3].Value;
            initData.initErrors.coordAccuracy.longitude = inputErrorsList[4].Value;
            initData.initErrors.coordAccuracy.altitude = inputErrorsList[5].Value;

            initData.initErrors.velocityAccuracy.east = inputErrorsList[6].Value;
            initData.initErrors.velocityAccuracy.north = inputErrorsList[7].Value;
            initData.initErrors.velocityAccuracy.H = inputErrorsList[8].Value;

            initData.initErrors.accelerationError.first = inputErrorsList[9].Value;
            initData.initErrors.accelerationError.second = inputErrorsList[10].Value;
            initData.initErrors.accelerationError.third = inputErrorsList[11].Value;

            initData.initErrors.gyroError.first = inputErrorsList[12].Value;
            initData.initErrors.gyroError.second = inputErrorsList[13].Value;
            initData.initErrors.gyroError.third = inputErrorsList[14].Value;
        }
        public static void RefreshTimes(InitData initData)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("ru-RU");
            foreach (PPM ppm in initData.ppmList)
            {
                //ppm.TargetArrivalTime = DateTime.Now.ToLongTimeString();
            }
        }
    }
}
