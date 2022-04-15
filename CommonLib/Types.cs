using CommonLib.Params;
using MyMatrix;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib
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
        public Dimension dimension;
        public Angles(double _heading, double _roll, double _pitch, Dimension dim)
        {
            heading = _heading;
            roll = _roll;
            pitch = _pitch;
            dimension = dim;
        }
    }
    public struct OutputData
    {
        public List<PointSet> points;
        public List<VelocitySet> velocities;
        public List<AnglesSet> angles;
        public List<P_out> p_OutList;
    }
    public struct PointValue
    {
        public Point Degrees;
        public Point Radians;
        public Point Meters;
        public PointValue(Point point, EarthModel earth, double latitude)
        {
            switch (point.dimension)
            {
                case Dimension.InDegrees:
                    Degrees = new Point(point.lat, point.lon, point.alt, Dimension.InDegrees);
                    Radians = Converter.DegToRad(point);
                    Meters = Converter.DegreesToMeters(point, earth);
                    break;

                case Dimension.InRadians:
                    Degrees = Converter.RadToDeg(point);
                    Radians = new Point(point.lat, point.lon, point.alt, Dimension.InRadians);
                    Meters = Converter.RadiansToMeters(point, earth);
                    break;

                case Dimension.InMeters:

                    Degrees = Converter.MetersToDegrees(point, latitude, earth);
                    Radians = Converter.MetersToRadians(point, latitude, earth);
                    Meters = new Point(point.lat, point.lon, point.alt, Dimension.InMeters);
                    break;

                default:
                    Degrees = new Point(point.lat, point.lon, point.alt, Dimension.InDegrees);
                    Radians = new Point(point.lat, point.lon, point.alt, Dimension.InDegrees);
                    Meters = new Point(point.lat, point.lon, point.alt, Dimension.InDegrees);
                    break;

            }
        }
    }
    public struct PointSet
    {
        public PointValue Ideal;
        public PointValue Error;
        public PointValue Real;
        public PointValue Estimate;
        public PointValue CorrectError;
        public PointValue CorrectTrajectory;
        public PointSet(Point idP, Vector error, Vector estimate, EarthModel earth, bool ISChannel3 = true)
        {
            if (error == null)
                error = Vector.Zero(3);
            if (estimate == null)
                estimate = Vector.Zero(3);
            if (ISChannel3)
            {
                Ideal = new PointValue(idP, earth, idP.lat);
                Point errP = new Point(error[2], error[1], error[3], Dimension.InMeters);
                Error = new PointValue(errP, earth, idP.lat);
                Point realP = new Point(
                    Ideal.Meters.lat + Error.Meters.lat,
                    Ideal.Meters.lon + Error.Meters.lon,
                    Ideal.Meters.alt + Error.Meters.alt,
                    Dimension.InMeters);
                Real = new PointValue(realP, earth, idP.lat);
                Point estP = new Point(estimate[2], estimate[1], estimate[3], Dimension.InMeters);
                Estimate = new PointValue(estP, earth, idP.lat);
                Point corErrP = new Point(
                    Error.Meters.lat - Estimate.Meters.lat,
                    Error.Meters.lon - Estimate.Meters.lon,
                    Error.Meters.alt - Estimate.Meters.alt,
                    Dimension.InMeters);
                CorrectError = new PointValue(corErrP, earth, idP.lat);
                Point corTrajP = new Point(
                    Ideal.Meters.lat + CorrectError.Meters.lat,
                    Ideal.Meters.lon + CorrectError.Meters.lon,
                    Ideal.Meters.alt + CorrectError.Meters.alt,
                    Dimension.InMeters);
                CorrectTrajectory = new PointValue(corTrajP, earth, idP.lat);
            }
            else
            {
                Ideal = new PointValue(idP, earth, idP.lat);
                Point errP = new Point(error[2], error[1], 0, Dimension.InMeters);
                Error = new PointValue(errP, earth, idP.lat);
                Point realP = new Point(
                    Ideal.Meters.lat + Error.Meters.lat,
                    Ideal.Meters.lon + Error.Meters.lon,
                    Ideal.Meters.alt + Error.Meters.alt,
                    Dimension.InMeters);
                Real = new PointValue(realP, earth, idP.lat);
                Point estP = new Point(estimate[2], estimate[1], 0, Dimension.InMeters);
                Estimate = new PointValue(estP, earth, idP.lat);
                Point corErrP = new Point(
                    Error.Meters.lat - Estimate.Meters.lat,
                    Error.Meters.lon - Estimate.Meters.lon,
                    Error.Meters.alt - Estimate.Meters.alt,
                    Dimension.InMeters);
                CorrectError = new PointValue(corErrP, earth, idP.lat);
                Point corTrajP = new Point(
                    Ideal.Meters.lat + CorrectError.Meters.lat,
                    Ideal.Meters.lon + CorrectError.Meters.lon,
                    Ideal.Meters.alt + CorrectError.Meters.alt,
                    Dimension.InMeters);
                CorrectTrajectory = new PointValue(corTrajP, earth, idP.lat);
            }

        }
    }
    public struct VelocitySet
    {
        public VelocityValue Ideal;
        public VelocityValue Error;
        public VelocityValue Real;
        public VelocityValue Estimate;
        public VelocityValue CorrectError;
        public VelocityValue CorrectTrajectory;
        public VelocitySet(Velocity velocity, Vector error, Vector estimate, bool ISChannel3 = true)
        {
            if (error == null)
                error = Vector.Zero(6);
            if (estimate == null)
                estimate = Vector.Zero(6);
            if (ISChannel3)
            {
                Ideal = new VelocityValue(velocity.E, velocity.N, velocity.H);
                Error = new VelocityValue(error[4], error[5], error[6]);
                Real = new VelocityValue(
                    Ideal.E + Error.E,
                    Ideal.N + Error.N,
                    Ideal.H + Error.H);
                Estimate = new VelocityValue(estimate[4], estimate[5], estimate[6]);
                CorrectError = new VelocityValue(
                    Error.E - Estimate.E,
                    Error.N - Estimate.N,
                    Error.H - Estimate.H);
                CorrectTrajectory = new VelocityValue(
                    Ideal.E + CorrectError.E,
                    Ideal.N + CorrectError.N,
                    Ideal.H + CorrectError.H);
            }
            else
            {
                Ideal = new VelocityValue(velocity.E, velocity.N, 0);
                Error = new VelocityValue(error[3], error[4], 0);
                Real = new VelocityValue(
                    Ideal.E + Error.E,
                    Ideal.N + Error.N,
                    Ideal.H + Error.H);
                Estimate = new VelocityValue(estimate[3], estimate[4], 0);
                CorrectError = new VelocityValue(
                    Error.E - Estimate.E,
                    Error.N - Estimate.N,
                    Error.H - Estimate.H);
                CorrectTrajectory = new VelocityValue(
                    Ideal.E + CorrectError.E,
                    Ideal.N + CorrectError.N,
                    Ideal.H + CorrectError.H);
            }


        }
    }
    public struct VelocityValue
    {
        public double E { get; private set; }
        public double N { get; private set; }
        public double H { get; private set; }
        public VelocityValue(double _E, double _N, double _H)
        {
            E = _E;
            N = _N;
            H = _H;
        }
    }
    public struct AnglesSet
    {
        public AngleValue Ideal;
        public AngleValue Error;
        public AngleValue Real;
        public AnglesSet(Angles angles, Vector error, bool IsChannel3 = true)
        {
            if (error == null)
                error = Vector.Zero(9);
            if (IsChannel3)
            {
                Ideal = new AngleValue(angles);
                Angles errAngles = new Angles(error[7], error[8], error[9], Dimension.InRadians);
                Error = new AngleValue(errAngles);
                Angles realAngles = new Angles(
                    Ideal.Radians.heading + Error.Radians.heading,
                    Ideal.Radians.roll + Error.Radians.roll,
                    Ideal.Radians.pitch + Error.Radians.pitch,
                    Dimension.InRadians);
                Real = new AngleValue(realAngles);
            }
            else
            {
                Ideal = new AngleValue(angles);
                Angles errAngles = new Angles(error[5], error[6], error[7], Dimension.InRadians);
                Error = new AngleValue(errAngles);
                Angles realAngles = new Angles(
                    Ideal.Radians.heading + Error.Radians.heading,
                    Ideal.Radians.roll + Error.Radians.roll,
                    Ideal.Radians.pitch + Error.Radians.pitch,
                    Dimension.InRadians);
                Real = new AngleValue(realAngles);
            }
        }
    }
    public struct AngleValue
    {
        public Angles Radians;
        public Angles Degrees;
        public AngleValue(Angles angles)
        {
            switch (angles.dimension)
            {
                case Dimension.InDegrees:
                    Degrees = new Angles(angles.heading, angles.roll, angles.pitch, Dimension.InDegrees);
                    Radians = new Angles(
                        Converter.DegToRad(angles.heading),
                        Converter.DegToRad(angles.roll),
                        Converter.DegToRad(angles.pitch),
                        Dimension.InRadians);
                    break;
                case Dimension.InRadians:
                    Radians = new Angles(angles.heading, angles.roll, angles.pitch, Dimension.InRadians);
                    Degrees = new Angles(
                        Converter.RadToDeg(angles.heading),
                        Converter.RadToDeg(angles.roll),
                        Converter.RadToDeg(angles.pitch),
                        Dimension.InDegrees);
                    break;
                default:
                    Radians = new Angles(angles.heading, angles.roll, angles.pitch, Dimension.InRadians);
                    Degrees = new Angles(angles.heading, angles.roll, angles.pitch, Dimension.InDegrees);
                    break;
            }
        }
    }
    public struct InputData
    {
        public double[] latitude { get; set; }
        public double[] longitude { get; set; }
        public double[] altitude { get; set; }
        public double[] velocity { get; set; }
    }
    public enum Dimension
    {
        InDegrees,
        InRadians,
        InMeters
    }
}
