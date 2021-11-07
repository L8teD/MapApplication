using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DebugApp.Types;

namespace DebugApp
{
    public class ListViewWorker
    {
        public static void UpdateData(ObservableCollection<RouteTurningPoint> rtpList, RouteTurningPoint RTP)
        {
            RouteTurningPoint tempRTP = new RouteTurningPoint();
            tempRTP.Latitude = RTP.Latitude;
            tempRTP.Longitude = RTP.Longitude;
            tempRTP.Altitude = RTP.Altitude;
            tempRTP.Velocity = RTP.Velocity;
            tempRTP.ID = rtpList.Count() + 1;
            rtpList.Add(tempRTP);
            
        }
        public static void RemoveElement(ObservableCollection<RouteTurningPoint> rtpList, int id)
        {
            rtpList.RemoveAt(id - 1);
            for (int i = 0; i < rtpList.Count; i++)
            {
                rtpList[i].ID = i + 1;
            }
        }

    }
}
