using CommonLib.Params;
using MyMatrix;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib
{
    public struct InputWindData
    {
        public double speed;
        public double wind_n;
        public double wind_e;
        public double wind_d;
        public double L_u;
        public double L_v;
        public double L_w;
        public double sigma_u;
        public double sigma_v;
        public double sigma_w;

        public double angle;
    }
    public struct InputAirData
    {
        public double relativeAltitude;
        public double pressureError;
        public double tempratureError;
    }
    public struct MatlabOutData
    {
        public double baro_h;
        public double baro_Vh;
        public double air_Ve;
        public double air_Vn;
        public double air_lat;
        public double air_lon;
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
        public AirData airData;
        public Matrix C;
        public double dt;
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
    public struct AirData
    {
        public Point point;
        public Velocity airSpeed;
        public Velocity windSpeed;
        public Angles angles;
        public double windAngle;
    }
    public struct T_OutputFull
    {
        public T_Output DesiredTrack;
        public T_Output ActualTrack;
    }
    public struct T_Output
    {
        public OutputData Feedback;
        public OutputData Default;
    }
    public struct OutputData
    {
        public List<PointSet> points;
        public List<VelocitySet> velocities;
        public List<AnglesSet> angles;
        public List<P_out> p_OutList;
        public List<AirData> airData;
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
                case Dimension.Degrees:
                    Degrees = new Point(point.lat, point.lon, point.alt, Dimension.Degrees);
                    Radians = Converter.DegToRad(point);
                    Meters = Converter.DegreesToMeters(point, earth);
                    break;

                case Dimension.Radians:
                    Degrees = Converter.RadToDeg(point);
                    Radians = new Point(point.lat, point.lon, point.alt, Dimension.Radians);
                    Meters = Converter.RadiansToMeters(point, earth);
                    break;

                case Dimension.Meters:

                    Degrees = Converter.MetersToDegrees(point, latitude, earth);
                    Radians = Converter.MetersToRadians(point, latitude, earth);
                    Meters = new Point(point.lat, point.lon, point.alt, Dimension.Meters);
                    break;

                default:
                    Degrees = new Point(point.lat, point.lon, point.alt, Dimension.Degrees);
                    Radians = new Point(point.lat, point.lon, point.alt, Dimension.Degrees);
                    Meters = new Point(point.lat, point.lon, point.alt, Dimension.Degrees);
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
                Point errP = new Point(error[2], error[1], error[3], Dimension.Meters);
                Error = new PointValue(errP, earth, idP.lat);
                Point realP = new Point(
                    Ideal.Meters.lat + Error.Meters.lat,
                    Ideal.Meters.lon + Error.Meters.lon,
                    Ideal.Meters.alt + Error.Meters.alt,
                    Dimension.Meters);
                Real = new PointValue(realP, earth, idP.lat);
                Point estP = new Point(estimate[2], estimate[1], estimate[3], Dimension.Meters);
                Estimate = new PointValue(estP, earth, idP.lat);
                Point corErrP = new Point(
                    Error.Meters.lat - Estimate.Meters.lat,
                    Error.Meters.lon - Estimate.Meters.lon,
                    Error.Meters.alt - Estimate.Meters.alt,
                    Dimension.Meters);
                CorrectError = new PointValue(corErrP, earth, idP.lat);
                Point corTrajP = new Point(
                    Ideal.Meters.lat + CorrectError.Meters.lat,
                    Ideal.Meters.lon + CorrectError.Meters.lon,
                    Ideal.Meters.alt + CorrectError.Meters.alt,
                    Dimension.Meters);
                CorrectTrajectory = new PointValue(corTrajP, earth, idP.lat);
            }
            else
            {
                Ideal = new PointValue(idP, earth, idP.lat);
                Point errP = new Point(error[2], error[1], 0, Dimension.Meters);
                Error = new PointValue(errP, earth, idP.lat);
                Point realP = new Point(
                    Ideal.Meters.lat + Error.Meters.lat,
                    Ideal.Meters.lon + Error.Meters.lon,
                    Ideal.Meters.alt + Error.Meters.alt,
                    Dimension.Meters);
                Real = new PointValue(realP, earth, idP.lat);
                Point estP = new Point(estimate[2], estimate[1], 0, Dimension.Meters);
                Estimate = new PointValue(estP, earth, idP.lat);
                Point corErrP = new Point(
                    Error.Meters.lat - Estimate.Meters.lat,
                    Error.Meters.lon - Estimate.Meters.lon,
                    Error.Meters.alt - Estimate.Meters.alt,
                    Dimension.Meters);
                CorrectError = new PointValue(corErrP, earth, idP.lat);
                Point corTrajP = new Point(
                    Ideal.Meters.lat + CorrectError.Meters.lat,
                    Ideal.Meters.lon + CorrectError.Meters.lon,
                    Ideal.Meters.alt + CorrectError.Meters.alt,
                    Dimension.Meters);
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
                Angles errAngles = new Angles(error[7], error[8], error[9], Dimension.Radians);
                Error = new AngleValue(errAngles);
                Angles realAngles = new Angles(
                    Ideal.Radians.heading + Error.Radians.heading,
                    Ideal.Radians.roll + Error.Radians.roll,
                    Ideal.Radians.pitch + Error.Radians.pitch,
                    Dimension.Radians);
                Real = new AngleValue(realAngles);
            }
            else
            {
                Ideal = new AngleValue(angles);
                Angles errAngles = new Angles(error[5], error[6], error[7], Dimension.Radians);
                Error = new AngleValue(errAngles);
                Angles realAngles = new Angles(
                    Ideal.Radians.heading + Error.Radians.heading,
                    Ideal.Radians.roll + Error.Radians.roll,
                    Ideal.Radians.pitch + Error.Radians.pitch,
                    Dimension.Radians);
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
                case Dimension.Degrees:
                    Degrees = new Angles(angles.heading, angles.roll, angles.pitch, Dimension.Degrees);
                    Radians = new Angles(
                        Converter.DegToRad(angles.heading),
                        Converter.DegToRad(angles.roll),
                        Converter.DegToRad(angles.pitch),
                        Dimension.Radians);
                    break;
                case Dimension.Radians:
                    Radians = new Angles(angles.heading, angles.roll, angles.pitch, Dimension.Radians);
                    Degrees = new Angles(
                        Converter.RadToDeg(angles.heading),
                        Converter.RadToDeg(angles.roll),
                        Converter.RadToDeg(angles.pitch),
                        Dimension.Degrees);
                    break;
                default:
                    Radians = new Angles(angles.heading, angles.roll, angles.pitch, Dimension.Radians);
                    Degrees = new Angles(angles.heading, angles.roll, angles.pitch, Dimension.Degrees);
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
        Degrees,
        Radians,
        Meters
    }
}
