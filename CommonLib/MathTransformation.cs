using CommonLib.Params;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib
{
    public class MathTransformation
    {
        public static double ReverseAngle(double value)
        {
            return Converter.DegToRad(360) - value;
        }
        public static void IncrementValue(ref double value, double increment)
        {
            value += increment;
        }
        public static void IncrementValue(ref double value)
        {
            value += 1;
        } 
        public static void IncrementValue(ref int value)
        {
            value += 1;
        }
        public static double IncrementValue(double value, double increment)
        {
            return value + increment;
        }
        public static Point SumCoordsAndErrors(Point coord, Point error)
        {
            if (coord.dimension != error.dimension)
                throw new Exception("Points have different dimensions");
            return new Point(
                coord.lat + error.lat,
                coord.lon + error.lon,
                coord.alt + error.alt,
                coord.dimension);
        }
    }
}
