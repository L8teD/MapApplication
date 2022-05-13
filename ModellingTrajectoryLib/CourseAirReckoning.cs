using CommonLib;
using CommonLib.Params;
using MyMatrix;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModellingTrajectoryLib
{
    public class CourseAirReckoning
    {
        double adiabaticKoef = 1.4; //k
        double gaseConst = 29.27125; //R


        double prevBaroAlt;
        Point airPoint;
        List<Atmosphere> atmosphereData;

        Dryden dryden;

        public void Init(Point _point)
        {
            atmosphereData = Atmosphere.Read();
            airPoint = new Point(_point.lat, _point.lon, _point.alt, _point.dimension);
            prevBaroAlt = default(double);
            dryden = new Dryden();
            dryden.Init();
        }
        private Vector GetDrydenComponent(Randomize randomize, InputWindData windData, Velocity modellingAirspeed, Matrix C)
        {
            DrydenOutput _randWind = dryden.Model(windData, modellingAirspeed.module, randomize);

            Vector randWind = !C * new Vector(_randWind.windRand1, _randWind.windRand2, _randWind.windRand3);

            return randWind;
        }
        public void Model(ref Parameters parameters, InputWindData windData, InputAirData airData, Randomize randomize, ref PointSet kvsPoints, ref VelocitySet kvsVelocities)
        {
            Point _point = parameters.point;
            Atmosphere atmosphere = atmosphereData.Find(atm => CompareAltitude(atm.altitude.geometric, _point.alt));

            Velocity windSpeed = new Velocity(windData.wind_e, windData.wind_n, windData.wind_d);

            Velocity modellingAirSpeed = new Velocity(
                parameters.velocity.E - windSpeed.E,
                parameters.velocity.N - windSpeed.N,
                parameters.velocity.H);

            Vector randWind = GetDrydenComponent(randomize, windData, modellingAirSpeed, parameters.C);
            //Vector randWind = Vector.Zero(3);

            modellingAirSpeed.E -= randWind[2];
            modellingAirSpeed.N -= randWind[1];
            modellingAirSpeed.H -= randWind[3];


            double Pd = GetDynamicPressure(atmosphere, modellingAirSpeed, airData.pressureError);
            double M = GetM(Pd, atmosphere);
            double _airSpeed = GetAirSpeed(atmosphere, M);


            Velocity airSpeed = new Velocity(_airSpeed, parameters.angles, parameters.dt);

            double baroAltitude = GetBaroAltitude(parameters.point.alt, atmosphere, M, airData);


            double verticalSpeed;
            if (prevBaroAlt == default(double))
                verticalSpeed = 0.0;
            else
            {
                verticalSpeed = baroAltitude - prevBaroAlt;
                airSpeed.H = verticalSpeed;
            }


            Velocity recountSpeed = new Velocity(airSpeed.E + randWind[2] /*+ windSpeed.E*/,
                                                airSpeed.N + randWind[1] /*+ windSpeed.N*/,
                                                airSpeed.H + randWind[3] /*+ windSpeed.H*/);

            AbsoluteOmega absOmega = new AbsoluteOmega(recountSpeed, parameters.earthModel, airPoint);

            airPoint = Point.GetCoords(airPoint, absOmega, recountSpeed, parameters.dt);
            airPoint.alt = baroAltitude;

            kvsPoints = new PointSet();

            kvsPoints.Real = new PointValue(airPoint, parameters.earthModel, airPoint.lat);
            kvsPoints.Error = new PointValue(
                new Point(airPoint.lat - parameters.point.lat, airPoint.lon - parameters.point.lon, airPoint.alt - parameters.point.alt, Dimension.Radians),
                parameters.earthModel, airPoint.lat);

            kvsVelocities = new VelocitySet();
            kvsVelocities.Real = new VelocityValue(recountSpeed.E, recountSpeed.N, recountSpeed.H);
            kvsVelocities.Error = new VelocityValue(
                recountSpeed.E - parameters.velocity.E, 
                recountSpeed.N - parameters.velocity.N, 
                recountSpeed.H - parameters.velocity.H);

            prevBaroAlt = baroAltitude;
        }
        private double GetDynamicPressure(Atmosphere atm, Velocity vel, double dP)
        {
            double Ps_meters = Converter.PascalToKgM(atm.pressure.pascal) + dP;
            double massDensity = atm.density / atm.gravitationalAcceleration;
            double speed = Converter.MeterPerSecToKmPerHour(vel.module);

            double Pd_meters = Ps_meters * (Math.Pow(1.0 + ((adiabaticKoef - 1.0) * massDensity * Math.Pow(speed, 2)
                        / (25.92 * adiabaticKoef * Ps_meters)), adiabaticKoef / (adiabaticKoef - 1.0)) - 1.0);

            return Converter.KgMToPascal(Pd_meters);
        }
        private double GetM(double Pd, Atmosphere atm)
        {
            double Ps = atm.pressure.pascal;

            return Math.Sqrt(2.0 / (adiabaticKoef - 1.0) * (Math.Pow(Pd / Ps + 1, (adiabaticKoef - 1.0) / adiabaticKoef) - 1.0));
        }
        private double GetAirSpeed(Atmosphere atm, double M)
        {
            double ksi = 1.0;
            double c = Math.Sqrt(adiabaticKoef * atm.gravitationalAcceleration * gaseConst);
            double machFunction = c * M / Math.Sqrt(1 + 0.2 * ksi * Math.Pow(M, 2));
            return machFunction * Math.Sqrt(atm.temperature.kelvin);
        }
        private double GetBaroAltitude(double m_Altitude, Atmosphere atm, double M, InputAirData airData)
        {
            double middleTemp;
            double ksi = 1.0;
            double machFunction = 1 + 0.2 * ksi * Math.Pow(M, 2);
            double Tn = (atm.temperature.kelvin + airData.tempratureError) / machFunction;
            Atmosphere atmZero = atmosphereData.Find(item => item.altitude.geometric == airData.relativeAltitude);
            //if (m_Altitude < 11000)
            middleTemp = ((atmZero.temperature.kelvin + airData.tempratureError) + Tn) / 2.0;
            double baroAlt = gaseConst * middleTemp * Math.Log((atmZero.pressure.pascal + airData.pressureError) / atm.pressure.pascal);
            return baroAlt + airData.relativeAltitude;

        }
        private bool CompareAltitude(double alt1, double alt2)
        {
            /*altitude-step in csv-file = 50m*/
            return (Math.Abs(alt1 - alt2) < 25.0);
        }
    }
}
