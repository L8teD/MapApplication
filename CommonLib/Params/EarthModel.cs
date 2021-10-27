using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Params
{
    public class EarthModel
    {
        public static double g_e = 9.78049;
        public static double q = 0.00346775;
        public static double betta_g = 0.0053171;
        public static double betta_g1 = 71e-7;

        public static double a = 6378245;
        public static double b = 6356863;
        public static double e = Math.Sqrt(Math.Pow(a, 2) - Math.Pow(b, 2)) / a;

        public double omegaEarth = 7.29e-5;

        public double shulerFrequency = 1.25e-3;

        public double R1 { get; set; }
        public double R2 { get; set; }
        public EarthModel(Point point)
        {
            ComputeRadiusInCurrentPoint(point);
        }
        private void ComputeRadiusInCurrentPoint(Point point)
        {
            R1 = a * (1 - Math.Pow(e, 2)) / Math.Sqrt(Math.Pow(1 - Math.Pow(e, 2) * Math.Sin(point.lat), 3)) + point.alt;
            R2 = a / Math.Sqrt(1 - Math.Pow(e, 2) * Math.Sin(point.lat)) + point.alt;
        }
    }
}
