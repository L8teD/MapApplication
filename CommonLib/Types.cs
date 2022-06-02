using CommonLib.Params;
using MyMatrix;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib
{
    #region Input 
    public struct Input
    {
        public TrajectoryInput trajectory;
        public InputWindData wind;
        public InputAirData air;
        public InsErrors INS;
        public GnssErrors GNSS;
    }
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
    }
    public struct InputAirData
    {
        public double pressureIndicatorError;
        public double relativeAltitude;
        public double pressureError;
        public double tempratureError;
        public double temperatureCelcius;
        public double coordSKO;
        public double velSKO;
        public double isCompensation;
    }
    #region InitErrors Struct
    public struct InsErrors
    {
        public AngleAccuracy angleAccuracy;
        public CoordAccuracy coordAccuracy;
        public VelocityAccuracy velocityAccuracy;

        public SensorError gyroError;
        public SensorError accelerationError;
        public SensorError accNoiseSKO;
        public SensorError gyroNoiseSKO;
        public SensorError accNoiseValue;
        public SensorError gyroNoiseValue;
        public SensorError accTemperatureKoef;
        public SensorError gyroTemperatureKoef;

        public double dt; //вынести отсюда
    }

    public struct GnssErrors
    {
        public double noise;

        public double coord;
        public double velocity;
    }
    public struct AngleAccuracy
    {
        public double heading { get; set; }
        public double pitch { get; set; }
        public double roll { get; set; }
    }
    public struct CoordAccuracy
    {
        public double latitude { get; set; }
        public double longitude { get; set; }
        public double altitude { get; set; }
    }
    public struct VelocityAccuracy
    {
        public double east { get; set; }
        public double north { get; set; }
        public double H { get; set; }
    }
    public struct SensorError
    {
        public double first { get; set; }
        public double second { get; set; }
        public double third { get; set; }
    }
   
    #endregion

    #endregion

    public struct MeasurementsParams
    {
        public double lat;
        public double lon;
        public double alt;
        public double E;
        public double N;
        public double H;
    }
    public struct MeasurementsErrors
    {
        public MeasurementsParams constant;
        public MeasurementsParams SKO;
        public MeasurementsParams noise;
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

    public struct TrackData
    {
        public OutputData INS;
        public OutputData GNSS;
        public OutputData KVS;
    }

    public struct T_Output
    {
        public TrackData DesiredTrack;
        public TrackData ActualTrack;
        public TrackData AdditionalTrack;
    }
    public struct T_OutputFull
    {
        public T_Output Feedback;
        public T_Output Default;
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
        public PointValue(Point point, EarthModel earth, Point pointInRad)
        {
            if (pointInRad.dimension != Dimension.Radians && point.dimension == Dimension.Meters)
                throw new Exception("Dimension should be in radians for convert meters to degrees");
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

                    Degrees = Converter.MetersToDegrees(point, pointInRad.lat, earth);
                    Radians = Converter.MetersToRadians(point, pointInRad.lat, earth);
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
        public PointValue? Ideal;
        public PointValue? Error;
        public PointValue? Real;
        public PointValue? Estimate;
        public PointValue? CorrectError;
        public PointValue? CorrectTrajectory;
        public PointSet(Point idP, Vector error, Vector estimate, EarthModel earth, bool ISChannel3 = true)
        {
            if (error == null)
                error = Vector.Zero(3);
            if (estimate == null)
                estimate = Vector.Zero(3);
            if (ISChannel3)
            {
                Ideal = new PointValue(idP, earth, idP);
                Point errP = new Point(error[2], error[1], error[3], Dimension.Meters);
                Error = new PointValue(errP, earth, idP);
                Point realP = new Point(
                    Ideal.Value.Meters.lat + Error.Value.Meters.lat,
                    Ideal.Value.Meters.lon + Error.Value.Meters.lon,
                    Ideal.Value.Meters.alt + Error.Value.Meters.alt,
                    Dimension.Meters);
                Real = new PointValue(realP, earth, idP);
                Point estP = new Point(estimate[2], estimate[1], estimate[3], Dimension.Meters);
                Estimate = new PointValue(estP, earth, idP);
                Point corErrP = new Point(
                    Error.Value.Meters.lat - Estimate.Value.Meters.lat,
                    Error.Value.Meters.lon - Estimate.Value.Meters.lon,
                    Error.Value.Meters.alt - Estimate.Value.Meters.alt,
                    Dimension.Meters);
                CorrectError = new PointValue(corErrP, earth, idP);
                Point corTrajP = new Point(
                    Ideal.Value.Meters.lat + CorrectError.Value.Meters.lat,
                    Ideal.Value.Meters.lon + CorrectError.Value.Meters.lon,
                    Ideal.Value.Meters.alt + CorrectError.Value.Meters.alt,
                    Dimension.Meters);
                CorrectTrajectory = new PointValue(corTrajP, earth, idP);
            }
            else
            {
                Ideal = new PointValue(idP, earth, idP);
                Point errP = new Point(error[2], error[1], 0, Dimension.Meters);
                Error = new PointValue(errP, earth, idP);
                Point realP = new Point(
                    Ideal.Value.Meters.lat + Error.Value.Meters.lat,
                    Ideal.Value.Meters.lon + Error.Value.Meters.lon,
                    Ideal.Value.Meters.alt + Error.Value.Meters.alt,
                    Dimension.Meters);
                Real = new PointValue(realP, earth, idP);
                Point estP = new Point(estimate[2], estimate[1], 0, Dimension.Meters);
                Estimate = new PointValue(estP, earth, idP);
                Point corErrP = new Point(
                    Error.Value.Meters.lat - Estimate.Value.Meters.lat,
                    Error.Value.Meters.lon - Estimate.Value.Meters.lon,
                    Error.Value.Meters.alt - Estimate.Value.Meters.alt,
                    Dimension.Meters);
                CorrectError = new PointValue(corErrP, earth, idP);
                Point corTrajP = new Point(
                    Ideal.Value.Meters.lat + CorrectError.Value.Meters.lat,
                    Ideal.Value.Meters.lon + CorrectError.Value.Meters.lon,
                    Ideal.Value.Meters.alt + CorrectError.Value.Meters.alt,
                    Dimension.Meters);
                CorrectTrajectory = new PointValue(corTrajP, earth, idP);
            }

        }
    }
    public struct VelocitySet
    {
        public VelocityValue? Ideal;
        public VelocityValue? Error;
        public VelocityValue? Real;
        public VelocityValue? Estimate;
        public VelocityValue? CorrectError;
        public VelocityValue? CorrectTrajectory;
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
                    Ideal.Value.E + Error.Value.E,
                    Ideal.Value.N + Error.Value.N,
                    Ideal.Value.H + Error.Value.H);
                Estimate = new VelocityValue(estimate[4], estimate[5], estimate[6]);
                CorrectError = new VelocityValue(
                    Error.Value.E - Estimate.Value.E,
                    Error.Value.N - Estimate.Value.N,
                    Error.Value.H - Estimate.Value.H);
                CorrectTrajectory = new VelocityValue(
                    Ideal.Value.E + CorrectError.Value.E,
                    Ideal.Value.N + CorrectError.Value.N,
                    Ideal.Value.H + CorrectError.Value.H);
            }
            else
            {
                Ideal = new VelocityValue(velocity.E, velocity.N, 0);
                Error = new VelocityValue(error[3], error[4], 0);
                Real = new VelocityValue(
                    Ideal.Value.E + Error.Value.E,
                    Ideal.Value.N + Error.Value.N,
                    Ideal.Value.H + Error.Value.H);
                Estimate = new VelocityValue(estimate[3], estimate[4], 0);
                CorrectError = new VelocityValue(
                    Error.Value.E - Estimate.Value.E,
                    Error.Value.N - Estimate.Value.N,
                    Error.Value.H - Estimate.Value.H);
                CorrectTrajectory = new VelocityValue(
                    Ideal.Value.E + CorrectError.Value.E,
                    Ideal.Value.N + CorrectError.Value.N,
                    Ideal.Value.H + CorrectError.Value.H);
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
        public AngleValue? Ideal;
        public AngleValue? Error;
        public AngleValue? Real;
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
                    Ideal.Value.Radians.heading + Error.Value.Radians.heading,
                    Ideal.Value.Radians.roll + Error.Value.Radians.roll,
                    Ideal.Value.Radians.pitch + Error.Value.Radians.pitch,
                    Dimension.Radians);
                Real = new AngleValue(realAngles);
            }
            else
            {
                Ideal = new AngleValue(angles);
                Angles errAngles = new Angles(error[5], error[6], error[7], Dimension.Radians);
                Error = new AngleValue(errAngles);
                Angles realAngles = new Angles(
                    Ideal.Value.Radians.heading + Error.Value.Radians.heading,
                    Ideal.Value.Radians.roll + Error.Value.Radians.roll,
                    Ideal.Value.Radians.pitch + Error.Value.Radians.pitch,
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
    public struct TrajectoryInput
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
