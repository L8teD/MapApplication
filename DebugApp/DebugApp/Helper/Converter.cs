using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebugApp.Model
{
    class Converter
    {
        public static double DateTimeToUnix(DateTime dateTime)
        {
            TimeSpan timeSpan = dateTime - new DateTime(1970, 1, 1, 0, 0, 0);
            return (long)timeSpan.TotalSeconds;
        }
        public static DateTime UnixToDateTime(double unixSeconds)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(unixSeconds);
            return new DateTime(1970, 1, 1, 0, 0, 0).Add(timeSpan);
        }
    }
}
