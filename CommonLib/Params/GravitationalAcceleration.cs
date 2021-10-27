using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Params
{
    public class GravitationalAcceleration : EarthModel
    {
        public double X { get; private set; }
        public double Y { get; private set; }
        public double Z { get; private set; }
        public GravitationalAcceleration(Point point) : base(point)
        {
            Get_G_Projections(point);
        }
        private void Get_G_Projections(Point point)
        {
            double g0 = g_e * (1 + betta_g * Math.Pow(Math.Sin(point.lat), 2) + betta_g1 * Math.Pow(Math.Sin(2 * point.lat), 2));
            Y = g0 * Math.Sin(2 * point.lat) * point.alt / a * (Math.Pow(e, 2) / a - 2 * q);
            Z = g0 + point.alt / a * (3 * point.alt / a - 2 * q * g_e * Math.Pow(Math.Cos(point.lat), 2) + Math.Pow(e, 2) * (3 * Math.Pow(Math.Sin(point.lat), 2) - 1) - q * (1 + 6 * Math.Pow(Math.Sin(point.lat), 2)));
        }
    }
}
