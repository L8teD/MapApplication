using CommonLib.Params;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib
{
    public class Types
    {
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
                Latitude = Math.Round(point.lat, 3);
                Longitude = Math.Round(point.lon, 3);
                Altitude = Math.Round(point.alt, 3);
                Velocity = Math.Round(velocity.value, 3);
                VelocityEast = Math.Round(velocity.E, 3);
                VelocityNorth = Math.Round(velocity.N, 3);
                VelocityH = Math.Round(velocity.H, 3);
                Heading = Math.Round(Converter.RadToDeg(angles.heading), 3);
                Pitch = Math.Round(Converter.RadToDeg(angles.pitch), 3);
                Roll = Math.Round(Converter.RadToDeg(angles.roll), 3);
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
            public PointSet(Parameters parameters, double[][] _error)
            {
                InDegrees = Converter.RadToDeg(parameters.point);
                InMeters = Converter.DegreesToMeters(InDegrees, parameters.point.lat, parameters.earthModel);
                ErrorInMeters = new Point(_error[2][0], _error[0][0], _error[4][0]);
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
            public VelocitySet(Velocity _velocity, double[][] _error)
            {
                Value = new VelocityValue(_velocity.E, _velocity.N, _velocity.H, _velocity.value);
                Error = new VelocityValue(_error[1][0], _error[3][0], _error[5][0], Math.Sqrt(Math.Pow(_error[1][0], 2) + Math.Pow(_error[3][0], 2) + Math.Pow(_error[3][0],2)));
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
            public AnglesSet(Angles angles, double[][] error)
            {
                Value = new Angles() { heading = angles.heading, pitch = angles.pitch, roll = angles.roll };
                Error = new Angles() { heading = error[0][0], pitch = error[1][0], roll = error[2][0] };
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
