using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Params
{
    public class AbsoluteOmega
    {
        double X;
        double Y;
        double Z;
        public double E { get; private set; }
        public double N { get; private set; }
        public double H { get; private set; }
        public AbsoluteOmega(Parameters parameters)
        {
            GetProjectionsNZSK(parameters.velocity, parameters.earthModel, parameters.point);
        }
        public AbsoluteOmega(Velocity velocity, EarthModel earthModel, Point point)
        {
            GetProjectionsNZSK(velocity, earthModel, point);
        }
        public void GetProjectionsNZSK(Velocity velocity, EarthModel earthModel, Point point)
        {
            E = velocity.N / (earthModel.R1 + point.alt);
            N = velocity.E / (earthModel.R2 + point.alt);
            H = velocity.E / earthModel.R2 * Math.Tan(point.lat);
        }
    }
}
