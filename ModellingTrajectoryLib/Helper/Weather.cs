using CommonLib;
using CommonLib.Params;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModellingTrajectoryLib.Helper
{
    class Weather
    {
        public static Wind Query(Point point)
        {
            Wind wind = new Wind();
            string apiKey = "69199cd1ac4f09270d7954f270d947d3";
            string part = "current";
            JObject jsonData;
            using (var client = new System.Net.WebClient())
                jsonData = JObject.Parse(client.DownloadString($"http://api.openweathermap.org/data/2.5/weather?lat={point.lat}&lon={point.lon}&exclude={part}&appid={apiKey}"));
            JToken windAnswer = jsonData.SelectToken("wind");

            wind.angle = Converter.DegToRad(Convert.ToDouble(windAnswer.SelectToken("deg").ToString()));
            wind.gust = Convert.ToDouble(windAnswer.SelectToken("gust").ToString());

            double speed = Convert.ToDouble(windAnswer.SelectToken("speed").ToString());

            wind.speed = speed * Math.Pow(point.alt / 10, 0.143);
            return wind;
        }
    }
}
