using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using MapApplicationWPF.ExternalResourses;
using static MapApplicationWPF.Helper.Types;

namespace MapApplicationWPF.Helper
{
    class Operations
    {
        public static void IncreaseValue(ref int value)
        {
            if (value >= int.MaxValue)
                value = 0;
            else
                value += 1;
        }
        public static double TryToParseStringToDouble(string text)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
            try
            {
                return Convert.ToDouble(text);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
                return 0;
            }
        }
        public static void ChangeEnableStates(params Button[] buttons)
        {
            foreach (Button btn in buttons)
                ChangeEnableState(btn);
        }
        public static void ChangeEnableState(Button btn)
        {
            btn.IsEnabled = (!btn.IsEnabled) ? true : false;
            btn.IsEnabled = true;
        }
        public static InitData CopyInitData(InitData original)
        {
            InitData copy = new InitData();
            copy.initErrors = new ModellingErrorsLib.Types.InitErrors();
            copy.ppmList = new System.Collections.ObjectModel.ObservableCollection<MapApplicationWPF.ExternalResourses.PPM>();

            foreach (PPM ppm in original.ppmList)
            {
                copy.ppmList.Add(ppm);
            }
            //copy.initErrors.accelerationError = original.initErrors.accelerationError;
            //copy.initErrors.gyroError = original.initErrors.gyroError;
            //copy.initErrors.coordAccuracy = original.initErrors.coordAccuracy;
            //copy.initErrors.velocityAccuracy = original.initErrors.velocityAccuracy;
            //copy.initErrors.headingAccuracy = original.initErrors.headingAccuracy;
            //copy.initErrors.pithAccuracy = original.initErrors.pithAccuracy;
            //copy.initErrors.rollAccuracy = original.initErrors.rollAccuracy;
            //copy.initErrors.sateliteErrorCoord = original.initErrors.sateliteErrorCoord;
            //copy.initErrors.sateliteErrorVelocity = original.initErrors.sateliteErrorVelocity;

            return copy;
        }
        public static List<T> CopyList<T>(List<T> original)
        {
            List<T> copy = new List<T>();

            foreach (T obj in original)
            {
                copy.Add(obj);
            }

            return copy;
        }
        public static string AccelerateTime(DateTime start, int second)
        {
            TimeSpan span = new TimeSpan(second * 10000000);
            DateTime displayTime = start  + span;
            return displayTime.ToLongTimeString();
        }
    }
}
