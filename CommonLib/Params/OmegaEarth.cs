using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Params
{
    public class OmegaEarth : EarthModel
    {
        public double E { get; private set; }
        public double N { get; private set; }
        public double H { get; private set; }

        public OmegaEarth(Point point) : base(point)
        {
            N = omegaEarth * Math.Cos(point.lat);
            H = omegaEarth * Math.Sin(point.lat);
            E = 0;
        }
    }
}
