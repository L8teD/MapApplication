using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Params
{
    public class Velocity
    {
        double X;
        double Y;
        double Z;

        public double E { get; private set; }
        public double N { get; private set; }
        public double H { get; private set; }
        public double E_dot { get; private set; }
        public double N_dot { get; private set; }
        public double H_dot { get; private set; }

        public double value { get; private set; }
        public Velocity(double _value, Parameters parameters)
        {
            value = _value;
            GetProjectionsNZSK(parameters.angles.heading, parameters.angles.pitch);
        }
        public Velocity(double _value, Angles angles)
        {
            value = _value;
            GetProjectionsNZSK(angles.heading, angles.pitch);
        }
        public Velocity(double vel_E, double vel_N, double vel_H)
        {
            E = vel_E;
            N = vel_N;
            H = vel_H;
            value = Math.Sqrt(Math.Pow(E,2) + Math.Pow(N,2) + Math.Pow(H,2));
        }

        public void GetProjectionsNZSK(double directAngle, double pitch)
        {
            H = value * Math.Sin(pitch);
            double horizontalVelocity = Math.Sqrt(Math.Pow(value, 2) - Math.Pow(H, 2));
            E = horizontalVelocity * Math.Sin(directAngle);
            N = horizontalVelocity * Math.Cos(directAngle);
        }
    }
}
