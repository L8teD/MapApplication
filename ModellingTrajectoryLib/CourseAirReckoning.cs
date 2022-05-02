﻿using CommonLib;
using CommonLib.Params;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModellingTrajectoryLib
{
    public class CourseAirReckoning
    {
        static double adiabaticKoef = 1.4; //k
        static double gaseConst = 29.27; //R


        static double prevBaroAlt;
        static Point airPoint;
        static List<Atmosphere> atmosphereData;

        public static void Init(Point _point)
        {
            atmosphereData = Atmosphere.Read();
            airPoint = new Point(_point.lat, _point.lon, _point.alt, _point.dimension);
        }
        public static void Model(ref Parameters parameters, double dt)
        {
            Point _point = parameters.point;
            Atmosphere atmosphere = atmosphereData.Find(atm => CompareAltitude(atm.altitude.geometric, _point.alt));

            Wind wind = default(Wind);//Weather.Query(parameters.point);
            Velocity windSpeed = new Velocity(wind.speed, wind.angle, default(double));
            Velocity modellingAirSpeed = new Velocity(
                parameters.velocity.E - windSpeed.E,
                parameters.velocity.N - windSpeed.N,
                parameters.velocity.H);

            double Pd = GetDynamicPressure(atmosphere, modellingAirSpeed);
            double M = GetM(Pd, atmosphere);
            double _airSpeed = GetAirSpeed(atmosphere, M);

            
            Velocity airSpeed = new Velocity(_airSpeed, parameters.angles, dt);

            double baroAltitude = GetBaroAltitude(parameters.point.alt, atmosphere, M);

          
            double verticalSpeed;
            if (prevBaroAlt == default(double))
                verticalSpeed = 0.0;
            else
                verticalSpeed = baroAltitude - prevBaroAlt;

            AbsoluteOmega absOmega = new AbsoluteOmega(airSpeed, parameters.earthModel, airPoint);

            airPoint = Point.GetCoords(airPoint, absOmega, airSpeed, dt);

            parameters.airData.point = Converter.RadToDeg(airPoint);
            parameters.airData.airSpeed = airSpeed;
            parameters.airData.windSpeed = windSpeed;
            parameters.airData.angles = parameters.angles;            

            prevBaroAlt = baroAltitude;        
        }
        private static double GetDynamicPressure(Atmosphere atm, Velocity vel)
        {
            double Ps_meters = Converter.PascalToKgM(atm.pressure.pascal);
            double massDensity = atm.density / atm.gravitationalAcceleration;
            double speed = Converter.MeterPerSecToKmPerHour(vel.value);

            double Pd_meters = Ps_meters * (Math.Pow(1.0 + ((adiabaticKoef - 1.0) * massDensity * Math.Pow(speed, 2)
                        / (25.92 * adiabaticKoef * Ps_meters)), adiabaticKoef / (adiabaticKoef - 1.0)) - 1.0);

            return Converter.KgMToPascal(Pd_meters);
        }
        private static double GetM(double Pd, Atmosphere atm)
        {
            double Ps = atm.pressure.pascal;

            return Math.Sqrt(2.0 / (adiabaticKoef - 1.0) * (Math.Pow(Pd / Ps + 1, (adiabaticKoef - 1.0) / adiabaticKoef) - 1.0));
        }
        private static double GetAirSpeed(Atmosphere atm, double M)
        {
            double ksi = 1.0;
            double c = Math.Sqrt(adiabaticKoef * atm.gravitationalAcceleration * gaseConst);
            double machFunction = c * M / Math.Sqrt(1 + 0.2 * ksi * Math.Pow(M, 2));
            return machFunction * Math.Sqrt(atm.temperature.kelvin);
        }
        private static double GetBaroAltitude(double m_Altitude, Atmosphere atm, double M)
        {
            double middleTemp;
            double ksi = 1.0;
            double machFunction = 1 + 0.2 * ksi * Math.Pow(M, 2);
            double Tn = atm.temperature.kelvin / machFunction;
            Atmosphere atmZero = atmosphereData.Find(item => item.altitude.geometric == 0);
            //if (m_Altitude < 11000)
            middleTemp = (atmZero.temperature.kelvin + Tn) / 2.0;
            return gaseConst * middleTemp * Math.Log(atmZero.pressure.pascal / atm.pressure.pascal);

        }
        private static bool CompareAltitude(double alt1, double alt2)
        {
            /*altitude-step in csv-file = 50m*/
            return (Math.Abs(alt1 - alt2) < 25.0);
        }
    }
}
