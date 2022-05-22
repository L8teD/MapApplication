using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Accord.IO;

namespace ModellingTrajectoryLib
{
    public class Atmosphere
    {
        public Altitude altitude;
        public Temperature temperature;
        public Pressure pressure;
        public double density;
        public double gravitationalAcceleration;

        public static List<Atmosphere> ReadMAT(double minLimit, double maxLimit, double relativeAltitude)
        {
            var atmosphereList = ReadCSV();
            MatReader matReader = new MatReader(@"..\..\..\..\ModellingTrajectoryLib\AnotherFiles\atmosphere.mat");

            double[,] H = matReader.Read<double[,]>("H");
            double[,] P = matReader.Read<double[,]>("P");
            double[,] ro = matReader.Read<double[,]>("RO");
            double[,] T = matReader.Read<double[,]>("T");
            for (int i = 0; i < H.GetLength(1); i++)
            {
                if (H[0, i] == relativeAltitude)
                    atmosphereList.Add(GetAltomsphereFromMatData(H[0, i], P[0, i], T[0, i], ro[0, i]));

                else if (H[0, i] < minLimit)
                    continue;
                else if (H[0, i] > maxLimit)
                    break;

                atmosphereList.Add(GetAltomsphereFromMatData(H[0, i], P[0, i], T[0, i], ro[0, i]));


            }

            return atmosphereList;
        }
        private static Atmosphere GetAltomsphereFromMatData(double H, double P, double T, double ro)
        {
            Atmosphere atmosphere = new Atmosphere();
            atmosphere.altitude = new Altitude()
            {
                geometric = H,
                geopotential = default(double)
            };
            atmosphere.temperature = new Temperature()
            {
                kelvin = T,
                celcius = T - 273.15
            };
            atmosphere.pressure = new Pressure()
            {
                pascal = P,
                mmOfMercure = default(double)
            };
            atmosphere.density = ro;
            atmosphere.gravitationalAcceleration = default(double);

            return atmosphere;
        }
        public static List<Atmosphere> ReadCSV()
        {
            List<Atmosphere> atmosphereList = new List<Atmosphere>();
            try
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("ru-RU");
                using (StreamReader sr = new StreamReader(@"..\..\..\..\ModellingTrajectoryLib\AnotherFiles\atmosphere.csv"))
                {
                    string[] array;
                    //sr.ReadLine();// skip header
                    //sr.ReadLine();// skip header
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        try
                        {
                            array = line.Split(';');
                            Atmosphere atmosphere = new Atmosphere();
                            atmosphere.altitude = new Altitude()
                            {
                                geometric = Convert.ToDouble(array[0].Replace(" ", "")),
                                geopotential = Convert.ToDouble(array[1].Replace(" ", ""))
                            };
                            atmosphere.temperature = new Temperature()
                            {
                                kelvin = Convert.ToDouble(array[3]),
                                celcius = Convert.ToDouble(array[5])
                            };
                            atmosphere.pressure = new Pressure()
                            {
                                pascal = Convert.ToDouble(array[6].Replace(" ", "")),
                                mmOfMercure = Convert.ToDouble(array[8])
                            };
                            atmosphere.density = Convert.ToDouble(array[9]);
                            atmosphere.gravitationalAcceleration = Convert.ToDouble(array[10]);
                            atmosphereList.Add(atmosphere);
                            if (Convert.ToDouble(array[0].Replace(" ", "")) == 0.0)
                            {

                            }
                        }
                        catch (FormatException ex)
                        {
                            continue;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
            }
            return atmosphereList;
        }
    }

}
