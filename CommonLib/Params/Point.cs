using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public static Point GetCoords(Parameters parameters, double dt)
        {
            double[] increments = GetCoordsIncrement(parameters.point, parameters.absOmega, parameters.velocity);
            return IncrementCoords(parameters.point, increments, dt);
        }
        public static Point GetCoords(Point point, AbsoluteOmega absOmega, Velocity velocity, double dt)
        {
            double[] increments = GetCoordsIncrement(point, absOmega, velocity);
            return IncrementCoords(point, increments, dt);
        }
        private static Point IncrementCoords(Point point, double[] increments, double dt)
        {
            double lat = point.lat + increments[0] * dt;
            double lon = point.lon + increments[1] * dt;
            double alt = point.alt + increments[2] * dt;
            return new Point(lat, lon, alt, point.dimension);
        }
    }
}
