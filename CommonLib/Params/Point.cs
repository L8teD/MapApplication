using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CommonLib.Types;

namespace CommonLib.Params
{
    public class Point
    {
        public Dimension dimension;
        public double lat { get; private set; }
        public double lon { get; private set; }
        public double alt { get; private set; }
        public Point(double _lat, double _lon, double _alt, Dimension dim)
        {
            lat = _lat;
            lon = _lon;
            alt = _alt;
            dimension = dim;
        }
        private static double[] GetCoordsIncrement(Point point, AbsoluteOmega absOmega, Velocity velocity)
        {
            return new double[] { absOmega.E, absOmega.N / Math.Cos(point.lat), velocity.H };
        }
        public static Point GetCoords(Parameters parameters, double dt, Dimension dim)
        {
            double[] increments = GetCoordsIncrement(parameters.point, parameters.absOmega, parameters.velocity);
            double lat = parameters.point.lat + increments[0] * dt;
            double lon = parameters.point.lon + increments[1] * dt;
            double alt = parameters.point.alt + increments[2] * dt;
            return new Point(lat, lon, alt, dim);
        }

    }
}
