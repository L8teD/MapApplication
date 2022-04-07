using MapApplication.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapApplication.Model
{
    public class ListViewWorker
    {
        public static void UpdateData(ObservableCollection<WayPoint> wayPointList, WayPoint RTP)
        {
            WayPoint temp = new WayPoint();
            temp.AirportName = RTP.AirportName;
            temp.Latitude = RTP.Latitude;
            temp.Longitude = RTP.Longitude;
            temp.Altitude = RTP.Altitude;
            temp.Velocity = RTP.Velocity;
            temp.ID = wayPointList.Count() + 1;
            wayPointList.Add(temp);
            
        }
        public static void RemoveElement(ObservableCollection<WayPoint> wayPointList, int id)
        {
            wayPointList.RemoveAt(id - 1);
            for (int i = 0; i < wayPointList.Count; i++)
            {
                wayPointList[i].ID = i + 1;
            }
        }
    }
}
