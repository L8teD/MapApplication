using MyMatrix;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Params
{
    public class Acceleration
    {
        public double X { get; private set; }
        public double Y { get; private set; }
        public double Z { get; private set; }
        public double E { get; private set; }
        public double N { get; private set; }
        public double H { get; private set; }
        public Acceleration(Parameters parameters, Matrix C)
        {
            GetProjectionNZSK(parameters.absOmega, parameters.velocity, parameters.gravAcceleration, parameters.omegaEarth);
            GetProjectionSSK(C);
        }
        public void GetProjectionNZSK(AbsoluteOmega absOmega, Velocity velocity, GravitationalAcceleration gravitationalAcceleration, OmegaEarth omegaEarth)
        {
            E = velocity.E_dot - (absOmega.H + 2 * omegaEarth.H) * velocity.N + (absOmega.N + 2 * omegaEarth.N) * velocity.H;
            N = velocity.N_dot + (absOmega.H + 2 * omegaEarth.H) * velocity.E + absOmega.E * velocity.H - gravitationalAcceleration.Y;
            H = velocity.H_dot - (absOmega.N + 2 * omegaEarth.N) * velocity.E + absOmega.E * velocity.N - gravitationalAcceleration.Z;
        }
        public void GetProjectionSSK(Matrix C)
        {
            Vector acceleration_XYZ = !C * new Vector(E, N, H);
            X = acceleration_XYZ[1];
            Y = acceleration_XYZ[2];
            Z = acceleration_XYZ[3];
        }
    }


}
