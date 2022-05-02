using CommonLib.Params;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib
{
    public class Converter
    {
        private const double pascalToKgMConst = 0.1019716212978;

        public static double DegToRad(double valueDeg)
        {
            return valueDeg * Math.PI / 180.0;
        }
        public static double[] DegToRad(double[] valuesDeg)
        {
            double[] valuesRad = new double[valuesDeg.Length];
            for (int i = 0; i < valuesDeg.Length; i++)
                valuesRad[i] = DegToRad(valuesDeg[i]);
            return valuesRad;
        }
        public static double[][] DegToRad(double[][] valuesDeg)
        {
            double[][] valuesRad = new double[valuesDeg.Length][];
            for (int i = 0; i < valuesDeg.Length; i++)
            {
                valuesRad[i] = new double[] { DegToRad(valuesDeg[i][0]) };
            }

            return valuesRad;
        }

        public static double RadToDeg(double valueRad)
        {
            return valueRad * 180.0 / Math.PI;
        }
        public static double[] RadToDeg(double[] valuesRad)
        {
            double[] valuesDeg = new double[valuesRad.Length];
            for (int i = 0; i < valuesRad.Length; i++)
                valuesDeg[i] = RadToDeg(valuesRad[i]);
            return valuesDeg;
        }
        public static Point DegToRad(Point pointInDegrees)
        {
            return new Point(

                DegToRad(pointInDegrees.lat),
                DegToRad(pointInDegrees.lon),
                pointInDegrees.alt,
                Dimension.InRadians
            );

        }
        public static Point RadToDeg(Point pointInRadians)
        {
            return new Point(

                RadToDeg(pointInRadians.lat),
                RadToDeg(pointInRadians.lon),
                pointInRadians.alt,
                Dimension.InDegrees
            );
        }
       
        public static (double[], double[], double[]) ListInPointsToDoubleCoords(List<Point> points)
        {
            double[] latArray = new double[points.Count];
            double[] lonArray = new double[points.Count];
            double[] altArray = new double[points.Count];
            for (int i = 0; i < points.Count; i++)
            {
                latArray[i] = points[i].lat;
                lonArray[i] = points[i].lon;
                altArray[i] = points[i].alt;
            }
            return (latArray, lonArray, altArray);
        }
        public static double[][] MetersToRadians(double[][] input, double latitude, EarthModel earthModel)
        {
            double[][] output = new double[input.Length][];

            output[0] = new double[] { input[0][0] / (earthModel.R2 * Math.Cos(latitude)) };
            output[1] = new double[1];
            output[2] = new double[] { input[2][0] / earthModel.R1 };
            output[3] = new double[1];
            return output;
        }
        public static Point MetersToDegrees(Point inputPointInMeters, double latitude, EarthModel earthModel)
        {
            return RadToDeg(MetersToRadians(inputPointInMeters, latitude, earthModel));
        }
        public static Point MetersToRadians(Point inputPoint, double latitude, EarthModel earthModel)
        {
            Point outPoint = new Point(
                inputPoint.lat / earthModel.R2,
                inputPoint.lon / (earthModel.R1 * Math.Cos(latitude)),
                inputPoint.alt,
                Dimension.InRadians);
            return outPoint;
        }
        public static Point DegreesToMeters(Point inputPointInDegrees, EarthModel earthModel)
        {
            return RadiansToMeters(DegToRad(inputPointInDegrees), earthModel);
        }
        public static Point RadiansToMeters(Point inputPointInRadians, EarthModel earthModel)
        {
            return new Point(
                inputPointInRadians.lat * earthModel.R2,
                inputPointInRadians.lon * earthModel.R1 * Math.Cos(inputPointInRadians.lat),
                inputPointInRadians.alt,
                Dimension.InMeters);
        }
        public static double KmPerHourToMeterPerSec(double kmPerHour)
        {
            return kmPerHour / 3.6;
        }
        public static double MeterPerSecToKmPerHour(double meterPerSec)
        {
            return meterPerSec * 3.6;
        }
        public static double DateTimeToUnix(DateTime dateTime)
        {
            TimeSpan timeSpan = dateTime - new DateTime(1970, 1, 1, 0, 0, 0);
            return (long)timeSpan.TotalSeconds;
        }
        public static DateTime UnixToDateTime(double unixSeconds)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(unixSeconds);
            return new DateTime(1970, 1, 1, 0, 0, 0).Add(timeSpan);
        }
        public static double SimilarityTheory(double value, double altitde)
        {
            double similarityConst = 0.143;
            double zeroAltitude = 10.0;
            return value * Math.Pow(altitde / zeroAltitude, similarityConst);
        }
        public static double PascalToKgM(double pascal)
        {
            return pascal * pascalToKgMConst;
        }
        public static double KgMToPascal(double kgM)
        {
            return kgM / pascalToKgMConst;
        }
    }
}
