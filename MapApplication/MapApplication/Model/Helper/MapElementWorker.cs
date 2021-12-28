using MapApplication.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Controls.Maps;

namespace MapApplication.Model
{
    public class MapElementWorker
    {
        public static List<Airport> airportsData;
        public static List<MapElement> airportsMapElements;

        public static void AddAirportsOnMAp()
        {
            airportsData = Airport.GetAirportsData();
            airportsMapElements = new List<MapElement>();
            foreach (Airport airport in airportsData)
            {
                if (airport.country == "Russian Federation" && airport.lat != 0)
                    airportsMapElements.Add(new MapIcon
                    {
                        Location = new Geopoint(new BasicGeoposition { Latitude = airport.lat, Longitude = airport.lon }),
                        Title = airport.name,
                        MapStyleSheetEntry = MapStyleSheetEntries.Forest
                    });
            }
        }
        
    }
}
