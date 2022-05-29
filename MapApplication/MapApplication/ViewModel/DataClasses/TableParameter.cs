using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapApplication.ViewModel
{
    public class TableParameter : BaseViewModel
    {
        double longitude;
        double latitude;
        double altitude;
        double eastVelocity;
        double verticalVelocity;
        double northVelocity;
        double heading;
        double pitch;
        double roll;

        public double Longitude
        {
            get { return longitude; }
            set
            {
                longitude = value;
                OnPropertyChanged("Longitude");
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
        public double Altitude
        {
            get { return altitude; }
            set
            {
                altitude = value;
                OnPropertyChanged("Altitude");
            }
        }
        public double EastVelocity
        {
            get { return eastVelocity; }
            set
            {
                eastVelocity = value;
                OnPropertyChanged("EastVelocity");
            }
        }
        public double NorthVelocity
        {
            get { return northVelocity; }
            set
            {
                northVelocity = value;
                OnPropertyChanged("NorthVelocity");
            }
        }
        
        public double VerticalVelocity
        {
            get { return verticalVelocity; }
            set
            {
                verticalVelocity = value;
                OnPropertyChanged("VerticalVelocity");
            }
        }
        public double Heading
        {
            get { return heading; }
            set
            {
                heading = value;
                OnPropertyChanged("Heading");
            }
        }
        public double Roll
        {
            get { return roll; }
            set
            {
                roll = value;
                OnPropertyChanged("Roll");
            }
        }
        public double Pitch
        {
            get { return pitch; }
            set
            {
                pitch = value;
                OnPropertyChanged("Pitch");
            }
        }
    }
}
