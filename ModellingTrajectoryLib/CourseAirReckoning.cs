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
        List<double> velocitiesE;
        List<double> velocitiesN;

        public void Init(Point _point, double[] altitudeArray, InputAirData airData)
        {
            atmosphereData = Atmosphere.ReadMAT(AltitudeMin(altitudeArray) - 0.1 * AltitudeMin(altitudeArray), 
                AltitudeMax(altitudeArray) + 0.1 * AltitudeMax(altitudeArray), airData.relativeAltitude);

            airPoint = new Point(_point.lat, _point.lon, _point.alt, _point.dimension);
            prevBaroAlt = airPoint.alt;
            dryden = new Dryden();
            dryden.Init();

            foreach (Atmosphere atm in atmosphereData)
            {
                atm.pressure.pascal += airData.pressureError;
                atm.temperature.kelvin += airData.tempratureError;
                atm.temperature.celcius += airData.tempratureError;
            }

            velocitiesE = new List<double>();
            velocitiesN = new List<double>();
        }
        public Vector GetDrydenComponent(InputWindData windData, Velocity modellingAirspeed, Matrix C, DrydenInput drydenInput)
        {
            DrydenOutput _randWind = dryden.Model(windData, modellingAirspeed.module, drydenInput);

            Vector randWind = !C * new Vector(_randWind.windRand1, _randWind.windRand2, _randWind.windRand3);

            return randWind;
        }
        public void Model(Parameters parameters, InputWindData windData, ref InputAirData airData, DrydenInput drydenInput,
                            ref PointSet kvsPoints, ref VelocitySet kvsVelocities, ref MeasurementsErrors measurements)
        {
            Atmosphere atmosphere = atmosphereData.Find(atm => CompareAltitude(atm.altitude.geometric, parameters.point.alt));          
            double relativeAltitude = airData.relativeAltitude;
            Atmosphere atmZero = atmosphereData.Find(item => item.altitude.geometric == relativeAltitude);

            airData.temperatureCelcius = atmosphere.temperature.celcius;
            Velocity windSpeed = new Velocity(windData.wind_e, windData.wind_n, windData.wind_d);

            Velocity modellingAirSpeed = new Velocity(
                                            parameters.velocity.E - windSpeed.E,
                                            parameters.velocity.N - windSpeed.N,
                                            parameters.velocity.H - windSpeed.H);

            Vector randWind = Vector.Zero(3); //GetDrydenComponent(windData, modellingAirSpeed, parameters.C, drydenInput);

            modellingAirSpeed.E += randWind[2];
            modellingAirSpeed.N += randWind[1];
            modellingAirSpeed.H += randWind[3];

            double Pd = GetDynamicPressure(atmosphere, modellingAirSpeed, parameters.gravAcceleration);
           
            double M = GetM(Pd, atmosphere);

            double error_MachConstant = GetMachError(atmosphere, airData, Pd, M);

            double error_TemperatureConstant = GetTemperatureError(airData, M, error_MachConstant);

            double baroAltError = GetBaroAltitudeError(atmZero, airData, error_TemperatureConstant) + measurements.SKO.alt * measurements.noise.alt;
            double error_AirspeedConstantValue = GetAirspeedError(atmosphere, airData, M, error_MachConstant);

            double baroAltitudeWithoutError = GetBaroAltitude(atmosphere, atmZero, airData);
            double baroAltitude = baroAltitudeWithoutError + baroAltError;

            Velocity error_AirspeedConstant = new Velocity(error_AirspeedConstantValue, parameters.angles, parameters.dt);
            modellingAirSpeed.E += error_AirspeedConstant.E + measurements.SKO.E * measurements.noise.E;
            modellingAirSpeed.N += error_AirspeedConstant.N + measurements.SKO.N * measurements.noise.N;
            modellingAirSpeed.H += error_AirspeedConstant.H + measurements.SKO.H * measurements.noise.H;

            Velocity recountSpeed = new Velocity(modellingAirSpeed.E + windSpeed.E,
                                                modellingAirSpeed.N + windSpeed.N,
                                                modellingAirSpeed.H + windSpeed.H);

            //double errorE = recountSpeed.E - parameters.velocity.E;
            //double errorN = recountSpeed.N - parameters.velocity.N;
            //double errorH = recountSpeed.H - parameters.velocity.H;

            

            kvsVelocities = new VelocitySet();
            kvsVelocities.Real = new VelocityValue(recountSpeed.E, recountSpeed.N, recountSpeed.H);
            kvsVelocities.Error = new VelocityValue(
                recountSpeed.E - parameters.velocity.E,
                recountSpeed.N - parameters.velocity.N,
                recountSpeed.H - parameters.velocity.H);


            velocitiesE.Add(kvsVelocities.Error.Value.E);
            velocitiesN.Add(kvsVelocities.Error.Value.N);

            //velocitiesH.Add(kvsVelocities.Error.Value.H);
            if (/*airData.isCompensation == 1 ||*/ airData.isCompensation == 0)
            {
                if (velocitiesE.Count > 5)
                    kvsVelocities.Estimate = new VelocityValue(Common.Aproximate(velocitiesE, 3)[velocitiesE.Count],
                                                               Common.Aproximate(velocitiesN, 3)[velocitiesN.Count],
                                                               0);
                else
                    kvsVelocities.Estimate = new VelocityValue(0, 0, 0);
            }
            else
                kvsVelocities.Estimate = new VelocityValue(0, 0, 0);


            Velocity aproximatedVelocity = new Velocity
                (
                    recountSpeed.E - kvsVelocities.Estimate.Value.E,
                    recountSpeed.N - kvsVelocities.Estimate.Value.N,
                    recountSpeed.H
                );
            AbsoluteOmega absOmega = new AbsoluteOmega(aproximatedVelocity, parameters.earthModel, airPoint);
            airPoint = Point.GetCoords(airPoint, absOmega, aproximatedVelocity, parameters.dt);
            airPoint.alt = baroAltitude;

            kvsPoints = new PointSet();
            
            kvsPoints.Real = new PointValue(airPoint, parameters.earthModel, airPoint);
            kvsPoints.Error = new PointValue(
                new Point(parameters.point.lat - airPoint.lat, parameters.point.lon - airPoint.lon, baroAltError, Dimension.Radians),
                parameters.earthModel, parameters.point);

           


            kvsPoints.Ideal = new PointValue(new Point(0, 0, baroAltitudeWithoutError, Dimension.Radians), parameters.earthModel, parameters.point);
            kvsVelocities.Ideal = new VelocityValue(parameters.velocity.E - error_AirspeedConstant.E - measurements.SKO.E * measurements.noise.E, 
                                                    parameters.velocity.N - error_AirspeedConstant.N - measurements.SKO.N * measurements.noise.N,
                                                    parameters.velocity.H - error_AirspeedConstant.H - measurements.SKO.H * measurements.noise.H);

            measurements.constant.lat = kvsPoints.Error.Value.Meters.lat + measurements.SKO.lat * measurements.noise.lat;
            measurements.constant.lon = kvsPoints.Error.Value.Meters.lon + measurements.SKO.lon * measurements.noise.lon;
            measurements.constant.alt = kvsPoints.Error.Value.Meters.alt;

            measurements.constant.E = error_AirspeedConstant.E;
            measurements.constant.N = error_AirspeedConstant.N;
            measurements.constant.H = error_AirspeedConstant.H;
        }
       
        private double GetBaroAltitudeError(Atmosphere atmZero, InputAirData airData, double error_TemperatureConst)
        {
            double middleTempError = (airData.tempratureError + error_TemperatureConst) / 2.0;
            return -gaseConst * middleTempError * airData.pressureError / atmZero.pressure.pascal;
        }
        private double GetBaroAltitude(Atmosphere atm, Atmosphere atmZero, InputAirData airData)
        {
            double middleTemp = (atmZero.temperature.kelvin + atm.temperature.kelvin) / 2.0;
            double baroAlt = gaseConst * middleTemp * Math.Log(atmZero.pressure.pascal / atm.pressure.pascal);

            return baroAlt + airData.relativeAltitude;
        }
        private double GetTemperatureError(InputAirData airData, double M, double machErr)
        {
            double ksi = 1.0;
            double mErrComponent = 2.0 / 5.0 * ksi * M * machErr / (1.0 + ksi * M * M / 5.0);
            return airData.pressureIndicatorError + airData.tempratureError / (airData.temperatureCelcius + 273.15) - mErrComponent;
        }
        private double GetAirspeedError(Atmosphere atm, InputAirData airData, double M, double machErr)
        {
            double ksi = 1.0;

            double tComponent = airData.pressureIndicatorError / 2.0 + airData.tempratureError / atm.temperature.kelvin;
            double mComponent = machErr / M;
            double mErrComponent = ksi * M * machErr / (1.0 - ksi * M * M / 5.0);

            return tComponent + mComponent - mErrComponent;
        }

        private double GetMachError(Atmosphere atm, InputAirData airData, double Pd, double M)
        {
            double chisl = Math.Sqrt(2 / (adiabaticKoef - 1)) * Pd / atm.pressure.pascal;
            double znamen = adiabaticKoef * M * Math.Pow(Pd / atm.pressure.pascal + 1.0, 1.0 / adiabaticKoef);
            double mnoj = airData.pressureIndicatorError + airData.tempratureError / atm.temperature.kelvin - airData.pressureError / atm.pressure.pascal;

            return mnoj * chisl / znamen;
        }
        private double GetDynamicPressure(Atmosphere atm, Velocity vel, GravitationalAcceleration gravAcc)
        {
            double Ps_meters = Converter.PascalToKgM(atm.pressure.pascal);
            double massDensity = atm.density / Math.Abs(gravAcc.Z);
            double speed = Converter.MeterPerSecToKmPerHour(vel.module);

            double Pd_meters = Ps_meters * (Math.Pow(1.0 + ((adiabaticKoef - 1.0) * massDensity * Math.Pow(speed, 2)
                        / (25.92 * adiabaticKoef * Ps_meters)), adiabaticKoef / (adiabaticKoef - 1.0)) - 1.0);

            return Converter.KgMToPascal(Pd_meters);
        }
        private double GetM(double Pd, Atmosphere atm)
        {
            return Math.Sqrt(2.0 / (adiabaticKoef - 1.0) * (Math.Pow(Pd / atm.pressure.pascal + 1, (adiabaticKoef - 1.0) / adiabaticKoef) - 1.0));
        }
        private double GetAirSpeed(Atmosphere atm, double M, GravitationalAcceleration gravAcc)
        {
            double ksi = 1.0;
            double c = Math.Sqrt(adiabaticKoef * Math.Abs(gravAcc.Z) * gaseConst);
            double machFunction = c * M / Math.Sqrt(1 + 0.2 * ksi * Math.Pow(M, 2));
            return machFunction * Math.Sqrt(atm.temperature.kelvin);
        }
        private double GetBaroAltitude(Atmosphere atm, double M, InputAirData airData)
        {
            double middleTemp;
            double ksi = 1.0;
            double machFunction = 1 + 0.2 * ksi * Math.Pow(M, 2);
            double Tn = (atm.temperature.kelvin + airData.tempratureError) / machFunction;
            Atmosphere atmZero = atmosphereData.Find(item => item.altitude.geometric == airData.relativeAltitude);
            //if (m_Altitude < 11000)
            middleTemp = ((atmZero.temperature.kelvin + airData.tempratureError) + Tn) / 2.0;
            double baroAlt = gaseConst * middleTemp * Math.Log((atmZero.pressure.pascal) / atm.pressure.pascal);
            double baroAltError = -gaseConst * atmZero.temperature.kelvin * airData.pressureError / atmZero.pressure.pascal;
            return baroAlt + baroAltError + airData.relativeAltitude;

        }
        private bool CompareAltitude(double alt1, double alt2)
        {
            /*altitude-step in csv-file = 50m*/
            return (Math.Abs(alt1 - alt2) < 0.05);
        }
        private double AltitudeMax(double[] array)
        {
            double max = array[0];
            for (int i = 1; i < array.Length; i++)
            {
                max = Math.Max(max, array[i]);
            }
            return max;
        }
        private double AltitudeMin(double[] array)
        {
            double min = array[0];
            for (int i = 1; i < array.Length; i++)
            {
                min = Math.Min(min, array[i]);
            }
            return min;
        }
        //private Atmosphere GetAtmosphere100()
        //{
        //    Atmosphere atm = new Atmosphere();
        //    atm.altitude = new Altitude() { geometric = 100, geopotential = default(double) };
        //    atm.temperature = new Temperature() { kelvin = 287.5, celcius = 14.35 };
        //    atm.pressure = new Pressure() { mmOfMercure = default(double), pascal = 100129 };
        //    atm.density = 751.033;
        //    atm.gravitationalAcceleration = default(double);
        //    return atm;
        //}
        //private Atmosphere GetAtmosphere0()
        //{
        //    Atmosphere atm = new Atmosphere();
        //    atm.altitude = new Altitude() { geometric = 0, geopotential = default(double) };
        //    atm.temperature = new Temperature() { kelvin = 288.15, celcius = 15 };
        //    atm.pressure = new Pressure() { mmOfMercure = default(double), pascal = 101325 };
        //    atm.density = 760;
        //    atm.gravitationalAcceleration = default(double);
        //    return atm;
        //}
    }
}
