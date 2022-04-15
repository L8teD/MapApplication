using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ModellingTrajectoryLib
{
    public class Atmosphere
    {
        public Altitude altitude;
        public Temperature temperature;
        public Pressure pressure;
        public double density;
        public double gravitationalAcceleration;

        public static void Read()
        {
            List<Atmosphere> atmosphereList = new List<Atmosphere>();
            try
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("ru-RU");
                using (StreamReader sr = new StreamReader(@"B:\Ucheba\Диплом\Soft_from_work\ModellingTrajectoryLib\AnotherFiles\atmosphere.csv"))
                {
                    string[] array;
                    sr.ReadLine();// skip header
                    sr.ReadLine();// skip header
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
        }
    }
    
}
