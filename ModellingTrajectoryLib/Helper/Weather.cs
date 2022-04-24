using CommonLib;
using CommonLib.Params;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModellingTrajectoryLib
{
    class Weather
    {
        private static Wind lastAnswer = default(Wind);
        private static int count = -1;
        private static double prevAltitude= 0;

        public static Wind Query(Point point)
        {
            count++;
            //return lastAnswer;

            if (count % 200 != 0)
            {
                if (point.alt != prevAltitude)
                    lastAnswer.speed = Converter.SimilarityTheory(lastAnswer.speed, point.alt);
                return lastAnswer;
            }
           

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

            wind.speed = Converter.SimilarityTheory(speed, point.alt);
            prevAltitude = point.alt;
            lastAnswer = wind;
            return wind;
        }

    }
}
