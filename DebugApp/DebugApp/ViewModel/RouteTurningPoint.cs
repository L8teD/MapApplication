using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DebugApp
{
    public class RouteTurningPoint : BaseViewModel
    {
        private int id;
        private string airportName;
        private double latitude;
        private double longitude;
        private double velocity;
        private double altitude;

        public int ID
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged("ID");
            }
        }
        public string AirportName
        {
            get { return airportName; }
            set
            {
                airportName = value;
                OnPropertyChanged("AirportName");
            }
        }
        public double Latitude
        {
            get { return latitude; }
            set
            {
                latitude = value;
                OnPropertyChanged("Latitude");
            }
        }
        public double Longitude
        {
            get { return longitude; }
            set
            {
                longitude = value;
                OnPropertyChanged("Longitude");
            }
        }
        public double Velocity
        {
            get { return velocity; }
            set
            {
                velocity = value;
                OnPropertyChanged("Velocity");
            }
        }
        public double Altitude
        {
            get { return altitude; }
            set
            {
                altitude = value;
                OnPropertyChanged("Velocity");
            }
        }
    }
}
