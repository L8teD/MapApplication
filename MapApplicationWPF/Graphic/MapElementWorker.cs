using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using MapApplicationWPF.ExternalResourses;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Controls.Maps;
using static MapApplicationWPF.Helper.Types;

namespace MapApplicationWPF.Graphic
{
    class MapElementWorker
    {
        public static List<Airport> airportsData;
        public static List<MapElement> airportsMapElements;
        public static List<MapIcon> choosenAirportsIcons = new List<MapIcon>();
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
        public static void UpdateMapElementOnPointerEntered(MapElement mapElement)
        {
            if (mapElement.MapStyleSheetEntryState == MapStyleSheetEntryStates.Disabled || mapElement.MapStyleSheetEntryState == "")
            {
                mapElement.MapStyleSheetEntryState = MapStyleSheetEntryStates.Hover;
            }
        }

        public static void UpdateMapElementOnPointerExited(MapElement mapElement)
        {
            if (mapElement.MapStyleSheetEntryState == MapStyleSheetEntryStates.Hover)
            {
                mapElement.MapStyleSheetEntryState = "";
                mapElement.MapStyleSheetEntry = MapStyleSheetEntries.Forest;
            }
        }
        public static void UpdateMapElementOnClick(MapElement mapElement, InitData initData)
        {
            MapIcon myclickedIcon = mapElement as MapIcon;
            if (mapElement.MapStyleSheetEntryState == MapStyleSheetEntryStates.Selected)
            {
                mapElement.MapStyleSheetEntryState = "";
                mapElement.MapStyleSheetEntry = MapStyleSheetEntries.Forest;
                choosenAirportsIcons.Remove(myclickedIcon);
            }
            else if (mapElement.MapStyleSheetEntryState == MapStyleSheetEntryStates.Hover)
            {
                mapElement.MapStyleSheetEntryState = MapStyleSheetEntryStates.Selected;
                choosenAirportsIcons.Add(myclickedIcon);
            }
            ListViewWorker.UpdateData(initData);
        }

    }
}
