using CommonLib.Params;
using MyMatrix;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib
{
    public class Types
    {
        public struct MatlabData
        {
            public double lat { get; set; }
            public double lon { get; set; }
            public double heading { get; set; }
            public double roll { get; set; }
            public double Ve { get; set; }
            public double Vn { get; set; }
            public double R1 { get; set; }
            public double R2 { get; set; }
            public double aw_e { get; set; }
            public double aw_n { get; set; }
            public double aw_h { get; set; }
            public double alfa { get; set; }
            public double betta { get; set; }
            public double gamma { get; set; }
            public double accE { get; set; }
            public double accN { get; set; }
            public double accH { get; set; }
            public double w_x { get; set; }
            public double w_y { get; set; }
            public double w_z { get; set; }
            public double n_x { get; set; }
            public double n_y { get; set; }
            public double n_z { get; set; }
            public double dot_omega_h { get; set; }
        }
        public struct P_out
        {
            public double lon { get; set; }
            public double lat { get; set; }
            public double alt { get; set; }
            public double ve { get; set; }
            public double vn { get; set; }
            public double vh { get; set; }
        }
        public struct X_dot_out
        {
            public double lon { get; set; }
            public double lat { get; set; }
            public double alt { get; set; }
            public double ve { get; set; }
            public double vn { get; set; }
            public double vh { get; set; }
        }
        public struct Parameters
        {
            public AbsoluteOmega absOmega;
            public Acceleration acceleration;
            public EarthModel earthModel;
            public GravitationalAcceleration gravAcceleration;
            public OmegaEarth omegaEarth;
            public OmegaGyro omegaGyro;
            public Point point;
            public Velocity velocity;
            public Angles angles;
        }
        public struct Angles
        {
            public double heading;
            public double roll;
            public double pitch;
        }
        public struct DisplayedData
        {
            public double Latitude { get; private set; }
            public double Longitude { get; private set; }
            public double Altitude { get; private set; }
            public double Velocity { get; private set; }
            public double VelocityEast { get; private set; }
            public double VelocityNorth { get; private set; }
            public double VelocityH { get; private set; }
            public double Heading { get; private set; }
            public double Pitch { get; private set; }
            public double Roll { get; private set; }
            public DisplayedData(Point point, VelocityValue velocity, Angles angles)
            {
                Latitude = Math.Round(point.lat, 8);
                Longitude = Math.Round(point.lon, 8);
                Altitude = Math.Round(point.alt, 8);
                Velocity = Math.Round(velocity.value, 8);
                VelocityEast = Math.Round(velocity.E, 8);
                VelocityNorth = Math.Round(velocity.N, 8);
                VelocityH = Math.Round(velocity.H, 8);
                Heading = Math.Round(Converter.RadToDeg(angles.heading), 8);
                Pitch = Math.Round(Converter.RadToDeg(angles.pitch), 8);
                Roll = Math.Round(Converter.RadToDeg(angles.roll), 8);
            }
        }
        public struct PointSet
        {
            public Point InDegrees { get; private set; }
            public Point InMeters { get; private set; }
            public Point ErrorInDegrees { get; private set; }
            public Point ErrorInMeters { get; private set; }
            public Point InDegreesWithError { get; private set; }
            public Point InMetersWithError { get; private set; }
            public PointSet(Parameters parameters, Vector _error)
            {
                InDegrees = Converter.RadToDeg(parameters.point);
                InMeters = Converter.DegreesToMeters(InDegrees, parameters.point.lat, parameters.earthModel);
                //ErrorInMeters = new Point(_error[3], _error[1], _error[5]);
                ErrorInMeters = new Point(_error[2], _error[1], 0);
                ErrorInDegrees = Converter.MetersToDegrees(ErrorInMeters, parameters.point.lat, parameters.earthModel);
                InDegreesWithError = MathTransformation.SumCoordsAndErrors(InDegrees, ErrorInDegrees);
                InMetersWithError = MathTransformation.SumCoordsAndErrors(InMeters, ErrorInMeters);
            }
        }
        public struct VelocitySet
        {
            public VelocityValue Value { get; private set; }
            public VelocityValue Error { get; private set; }
            public VelocityValue ValueWithError { get; private set; }
            public VelocitySet(Velocity _velocity, Vector _error)
            {
                Value = new VelocityValue(_velocity.E, _velocity.N, _velocity.H, _velocity.value);
                //Error = new VelocityValue(_error[2], _error[4], _error[6], Math.Sqrt(Math.Pow(_error[2], 2) + Math.Pow(_error[4], 2) + Math.Pow(_error[6],2)));
                Error = new VelocityValue(_error[3], _error[4], 0, Math.Sqrt(Math.Pow(_error[3], 2) + Math.Pow(_error[4], 2) +0));
                ValueWithError = new VelocityValue(Value.E + Error.E, Value.N + Error.N, Value.H + Error.H, Value.value + Error.value);
            }
        }
        public struct VelocityValue
        {
            public double E { get; private set; }
            public double N { get; private set; }
            public double H { get; private set; }
            public double value { get; private set; }
            public VelocityValue(double _E, double _N, double _H, double _value)
            {
                E = _E;
                N = _N;
                H = _H;
                value = _value;
            }
        }
        public struct AnglesSet
        {
            public Angles Value;
            public Angles Error;
            public Angles WithError;
            public AnglesSet(Angles angles, Vector error)
            {
                Value = new Angles() { heading = angles.heading, pitch = angles.pitch, roll = angles.roll };
                Error = new Angles() { heading = error[1], pitch = error[2], roll = error[3] };
                WithError = new Angles()
                {
                    heading = Value.heading + Error.heading,
                    pitch = Value.pitch + Error.pitch,
                    roll = Value.roll + Error.roll
                };
            }
        }
        public struct InputData
        {
            public double[] latitude { get; set; }
            public double[] longitude { get; set; }
            public double[] altitude { get; set; }
            public double[] velocity { get; set; }
        }
    }
}
