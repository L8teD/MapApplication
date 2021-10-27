using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Resources;
using Windows.Storage;

namespace MapApplicationWPF.ExternalResourses
{
    public class Airport
    {
        public string name;
        public string city;
        public string country;
        public double lat;
        public double lon;
        public double runElevation;

        private static List<Airport> airports;
        public Airport(string name, string city, string country, double lat, double lon)
        {
            this.name = name;
            this.city = city;
            this.country = country;
            this.lat = lat;
            this.lon = lon;
        }
        public static List<Airport> GetAirportsData()
        {
            if (airports == null)
                FillAirportsData();
            return airports;
        }
        private static void FillAirportsData()
        {
            try
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");

                airports = new List<Airport>();
                
                Uri uri = new Uri("pack://application:,,,/AnotherFiles/ports_info.csv");
                //StorageFile sampleFile = await StorageFile.GetFileFromApplicationUriAsync(uri);
                StreamResourceInfo fileInfo = Application.GetResourceStream(uri);
                //using (StreamReader sr = new StreamReader(sampleFile.Path))
                using (StreamReader sr = new StreamReader(fileInfo.Stream))
                {
                    string[] array;
                    sr.ReadLine();// skip header
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        try
                        {
                            array = line.Split('|');
                            Airport airport = new Airport(array[1], array[3], array[5], double.Parse(array[7], CultureInfo.InvariantCulture),
                            double.Parse(array[9], CultureInfo.InvariantCulture));
                            airports.Add(airport);
                        }
                        catch (Exception ex)
                        {

                        }
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }
    }
}
