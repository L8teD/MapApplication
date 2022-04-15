using ModellingTrajectoryLib.Helper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ModellingTrajectoryLib
{
    public struct Wind
    {
        public double angle { get; set; }
        public double speed { get; set; }
        public double gust { get; set; }
    }
    public struct Altitude
    {
        public double geometric;
        public double geopotential;
    }
    public struct Temperature
    {
        public double kelvin;
        public double celcius;
    }
    public struct Pressure
    {
        public double pascal;
        public double mmOfMercure;
    }
}
