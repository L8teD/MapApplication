using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Params
{
    public class Velocity
    {
        double Eprev;
        double Nprev;
        double Hprev;

        public double E { get; set; }
        public double N { get; set; }
        public double H { get; set; }
        public double E_dot { get; private set; }
        public double N_dot { get; private set; }
        public double H_dot { get; private set; }

        public double module { get; private set; }

        public Velocity(double _value, Angles angles, double dt)
        {
            Eprev = E;
            Nprev = N;
            Hprev = H;
            module = _value;
            GetProjectionsNZSK(angles.heading, angles.pitch);
            SetDerivatives(dt);
        }
        private void SetDerivatives(double dt)
        {
            E_dot = (E - Eprev) / dt;
            N_dot = (N - Nprev) / dt;
            H_dot = (H - Hprev) / dt;
        }
        public Velocity(double vel_E, double vel_N, double vel_H)
        {
            Eprev = E;
            Nprev = N;
            Hprev = H;
            E = vel_E;
            N = vel_N;
            H = vel_H;
            module = Math.Sqrt(Math.Pow(E, 2) + Math.Pow(N, 2) + Math.Pow(H, 2));
        }

        public void GetProjectionsNZSK(double directAngle, double pitch)
        {
            H = module * Math.Sin(pitch);
            double horizontalVelocity = Math.Sqrt(Math.Pow(module, 2) - Math.Pow(H, 2));
            E = horizontalVelocity * Math.Sin(directAngle);
            N = horizontalVelocity * Math.Cos(directAngle);
        }
    }
}
