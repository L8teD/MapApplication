using MyMatrix;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Params
{
    public class OmegaGyro
    {
        public double X { get; private set; }
        public double Y { get; private set; }
        public double Z { get; private set; }
        public double E { get; private set; }
        public double N { get; private set; }
        public double H { get; private set; }
        public double X_dot { get; private set; }
        public double Y_dot { get; private set; }
        public double Z_dot { get; private set; }

        private double V_Eprev = 0;
        private double V_Nprev = 0;
        private double V_Hprev = 0;
        public OmegaGyro(Parameters parameters)
        {
            GetProjectionsNZSK(parameters.absOmega, parameters.omegaEarth);
            GetProjectionSSK(parameters.C);
            GetDot(parameters.point, parameters.velocity, parameters.acceleration, parameters.earthModel, parameters.omegaEarth, parameters.dt);
        }

        private void GetProjectionsNZSK(AbsoluteOmega absOmega, OmegaEarth omegaEarth)
        {
            E = absOmega.E;
            N = absOmega.N + omegaEarth.N;
            H = absOmega.H + omegaEarth.H;
        }
        private void GetProjectionSSK(Matrix C)
        {
            Vector omega_XYZ = !C * new Vector(E, N, H);
            X = omega_XYZ[1];
            Y = omega_XYZ[2];
            Z = omega_XYZ[3];
        }
        private void GetDot(Point point, Velocity velocity, Acceleration acceleration, EarthModel earth, OmegaEarth omegaEarth, double dt)
        {
            if (V_Eprev == 0 && V_Hprev == 0 && V_Nprev == 0)
            {
                V_Eprev = velocity.E;
                V_Nprev = velocity.N;
                V_Hprev = velocity.H;
            }

            //X_dot = -(velocity.N_dot - velocity.H * velocity.N / earth.R1) / earth.R1;
            //Y_dot = (velocity.E_dot - omegaEarth.H * velocity.N - velocity.H * velocity.E / earth.R2) / earth.R2;
            //Z_dot = (omegaEarth.N * velocity.N + acceleration.E * Math.Tan(point.lat) + velocity.H * this.H +
            //    velocity.E * velocity.N / (earth.R2 * Math.Pow(Math.Cos(point.lat), 2))) / earth.R2 / 100.0;

            //X_dot = -((velocity.E - V_Eprev) / dt - velocity.H * velocity.N / earth.R1) / earth.R1;
            //Y_dot = ((velocity.N - V_Nprev) / dt - omegaEarth.H * velocity.N - velocity.H * velocity.E / earth.R2) / earth.R2;
            //Z_dot = ((velocity.H - V_Hprev) / dt * velocity.N + acceleration.E * Math.Tan(point.lat) + velocity.H * this.H +
            //    velocity.E * velocity.N / (earth.R2 * Math.Pow(Math.Cos(point.lat), 2))) / earth.R2 / 100.0;

            X_dot = -((velocity.N - V_Nprev) / dt - velocity.H * velocity.N / earth.R1) / earth.R1;
            Y_dot = ((velocity.E - V_Eprev) / dt - omegaEarth.H * Math.Sin(point.lat) * velocity.N - velocity.H * velocity.E / earth.R2) / earth.R2;
            Z_dot = ((velocity.H - V_Hprev) / dt * velocity.N + acceleration.E * Math.Tan(point.lat) + velocity.H * this.H +
                velocity.E * velocity.N / (earth.R2 * Math.Pow(Math.Cos(point.lat), 2))) / earth.R2;// / 100.0;
        }
    }
}
