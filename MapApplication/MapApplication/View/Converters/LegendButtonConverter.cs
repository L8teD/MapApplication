using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MapApplication.View
{
    public class LegendButtonConverter : IMultiValueConverter
    {
        public class LegendButtonValue
        {
            public string seriesText { get; set; }
            public bool btnIsChecked { get; set; }
        }
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new LegendButtonValue()
            {
                btnIsChecked = (bool)values[0],
                seriesText = values[1].ToString()
            };
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
